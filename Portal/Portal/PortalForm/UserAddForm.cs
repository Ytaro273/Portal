using Npgsql;
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
    /// ユーザー追加フォーム
    /// </summary>
    public partial class UserAddForm : Form
    {
        UserInfo UserInfo; //ログインユーザーのユーザー情報

        ClosingType ClosingStatus = ClosingType.DoNothing; //FormClosingイベントが発生した際の挙動の種類


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="userInfo">ログインユーザーのユーザー情報</param>
        /// <param name="callerLocation">呼び出し元の座標</param>
        public UserAddForm(UserInfo userInfo, Point callerLocation)
        {
            InitializeComponent();

            this.UserInfo = userInfo;

            this.Location = callerLocation;
        }


        /// <summary>
        /// 保存ボタンクリックイベント
        /// 入力したユーザー情報を登録する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            List<UserInfo> userInfoList = new List<UserInfo>(); //登録するユーザー情報
            int authorityID; //権限ID

            var result = MessageBox.Show("入力したデータを追加します。よろしいですか？", "お知らせ", MessageBoxButtons.OKCancel);
            if (result == DialogResult.No) return;

            for (int i = 0; i < dataGridView1.Rows.Count -1; i++)
            {
                if (dataGridView1.Rows[i].Cells[0].Value == null)
                {
                    MessageBox.Show("ユーザーIDを入力してください。", "お知らせ");
                    return;
                }

                if (dataGridView1.Rows[i].Cells[1].Value == null)
                {
                    MessageBox.Show("ユーザー名を入力してください。", "お知らせ");
                    return;
                }

                if (dataGridView1.Rows[i].Cells[2].Value == null)
                {
                    MessageBox.Show("権限名を選択してください。", "お知らせ");
                    return;
                }

                if (dataGridView1.Rows[i].Cells[3].Value == null)
                {
                    MessageBox.Show("パスワードを入力してください。", "お知らせ");
                    return;
                }

                if ((string)dataGridView1.Rows[i].Cells[2].Value == "一般") authorityID = 0;
                else authorityID = 1;

                userInfoList.Add(new UserInfo()
                {
                    UserID = (string)dataGridView1.Rows[i].Cells[0].Value,
                    UserName = (string)dataGridView1.Rows[i].Cells[1].Value,
                    Pass = BCrypt.Net.BCrypt.HashPassword((string)dataGridView1.Rows[i].Cells[3].Value),
                    AuthorityID = authorityID
                });
            }

            try
            {
                new UserManageLogic().InsertUser(userInfoList);
            }
            catch (NpgsqlException)
            {
                MessageBox.Show("ユーザーの登録に失敗しました。", "お知らせ");
                return;
            }

            MessageBox.Show("ユーザーの追加に成功しました。", "お知らせ");

            this.Hide();
            new UserManageForm(this.UserInfo, this.Location).Show();
            this.Close();
        }


        /// <summary>
        /// キャンセルボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("ユーザー管理メニューに戻ります。よろしいですか？", "お知らせ", MessageBoxButtons.OKCancel);
            if (result == DialogResult.OK)
            {
                this.Hide();
                new UserManageForm(this.UserInfo, this.Location).Show();
                this.Close();
            }
        }


        /// <summary>
        /// フォーム終了時イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserAddForm_FormClosing(object sender, FormClosingEventArgs e)
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
