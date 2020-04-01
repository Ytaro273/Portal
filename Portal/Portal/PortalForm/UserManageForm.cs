using BCrypt.Net;
using Npgsql;
using Portal.DBAccess;
using Portal.Enum;
using Portal.Info;
using Portal.Logic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Portal.PortalForm
{
    /// <summary>
    /// ユーザー管理フォーム
    /// </summary>
    public partial class UserManageForm : Form
    {
        UserInfo UserInfo; //ログインユーザーの情報

        List<UserInfo> GeneralUserInfoList; //一般ユーザー情報リスト

        //FormClosingイベントが発生した際の挙動の種類
        ClosingType ClosingStatus = ClosingType.DoNothing;


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="userInfo">ログインユーザーのユーザー情報</param>
        /// <param name="callerLocation">呼び出し元の座標</param>
        public UserManageForm(UserInfo userInfo, Point callerLocation)
        {
            InitializeComponent();

            this.GeneralUserInfoList = new List<UserInfo>();

            this.UserInfo = userInfo;

            this.Location = callerLocation;

            IndicateGeneralUsers(); //一般ユーザー一覧表示
        }


        /// <summary>
        /// 一般ユーザー一覧表示
        /// </summary>
        private void IndicateGeneralUsers()
        {
            dataGridView1.Rows.Clear();

            try
            {
                //一般ユーザー情報リスト取得
                this.GeneralUserInfoList = new UserManageLogic().GetGeneralUserInfo();
            }
            catch (NpgsqlException)
            {
                MessageBox.Show("ユーザー情報の取得に失敗しました。", "お知らせ");
                return;
            }

            //ユーザー情報を表に表示
            foreach (var generalUserInfo in GeneralUserInfoList)
            {
                dataGridView1.Rows.Add(
                    generalUserInfo.UserID,
                    generalUserInfo.UserName,
                    generalUserInfo.AuthorityName);
            }

            //編集不可の列(ユーザーID)を灰色にする
            dataGridView1.Columns[0].DefaultCellStyle.BackColor = Color.LightGray;
        }


        /// <summary>
        /// ユーザー追加ボタンクリックイベント
        /// ユーザー追加フォームを表示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            new UserAddForm(this.UserInfo, this.Location).Show();
            this.Close();
        }


        /// <summary>
        /// ユーザー削除ボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("選択したユーザーを削除しますか？", "質問", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    new UserManageLogic().DeleteUser((string)dataGridView1.CurrentRow.Cells[0].Value, 1);
                }
                catch (NpgsqlException)
                {
                    MessageBox.Show("削除に失敗しました。", "お知らせ");
                    return;
                }
            }

            MessageBox.Show("ユーザーの削除が完了しました。", "お知らせ");

            IndicateGeneralUsers();
        }


        /// <summary>
        /// 保存ボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            List<UserInfo> userInfoList = new List<UserInfo>(); //保存するユーザー情報リスト
            int authorityID; //権限ID
            string pass; //パスワード

            var result = MessageBox.Show("入力したデータを保存します。よろしいですか？", "お知らせ", MessageBoxButtons.OKCancel);
            if (result == DialogResult.No) return;

            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (dataGridView1.Rows[i].Cells[1].Value == null)
                {
                    MessageBox.Show("ユーザー名が空白です。", "お知らせ");
                    return;
                }

                if ((string)dataGridView1.Rows[i].Cells[2].Value == "一般") authorityID = 0;
                else authorityID = 1;

                //新しいパスワードが入力されている場合
                if ((string)dataGridView1.Rows[i].Cells[3].Value != null)
                {
                    pass = BCrypt.Net.BCrypt.HashPassword((string)dataGridView1.Rows[i].Cells[3].Value);
                }
                //パスワードが入力されていない場合
                else
                {
                    pass = "";
                }

                userInfoList.Add(new UserInfo()
                {
                    UserID = (string)dataGridView1.Rows[i].Cells[0].Value,
                    UserName = (string)dataGridView1.Rows[i].Cells[1].Value,
                    Pass = pass,
                    AuthorityID = authorityID
                });
            }

            try
            {
                new UserManageLogic().UpdateUserInfo(userInfoList);
            }
            catch (NpgsqlException)
            {
                MessageBox.Show("メール情報の取得に失敗しました。", "お知らせ");
                return;
            }

            MessageBox.Show("保存に成功しました。", "お知らせ");

            IndicateGeneralUsers();
        }


        /// <summary>
        /// 終了ボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("メインメニューに戻ります。よろしいですか？", "お知らせ", MessageBoxButtons.OKCancel);
            if (result == DialogResult.OK)
            {
                this.Hide();
                new MenuForm(this.UserInfo, this.Location).Show();
                this.Close();
            }
        }


        /// <summary>
        /// フォーム終了時イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserManageForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //アプリケーションを終了するかどうか判定
            this.ClosingStatus = new CommonLogic().CheckWhetherToExit(this.Visible, this.ClosingStatus);

            switch (this.ClosingStatus)
            {
                //アプリケーションを終了
                case ClosingType.Exit:
                    Application.Exit();
                    break;

                //終了をキャンセル
                case ClosingType.Cancel:
                    this.ClosingStatus = ClosingType.DoNothing;
                    e.Cancel = true;
                    break;

                //何もしない
                case ClosingType.DoNothing:
                    break;
            }
        }
    }
}
