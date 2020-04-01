using Npgsql;
using Portal.Enum;
using Portal.Info;
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
    /// メール宛先フォーム
    /// </summary>
    public partial class MailDestinationForm : Form
    {
        UserInfo UserInfo; //ログインユーザーのユーザー情報

        public List<MailInfo> MailInfoList; //送信先の情報
        
        ClosingType ClosingStatus = ClosingType.DoNothing; //FormClosingイベントが発生した際の挙動の種類


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="userInfo">ログインユーザーのユーザー情報</param>
        /// <param name="callerLocation">呼び出し元の座標</param>
        public MailDestinationForm(UserInfo userInfo, Point callerLocation)
        {
            InitializeComponent();

            var addresInfoList = new List<MailInfo>(); //アドレス情報

            this.MailInfoList = new List<MailInfo>();

            this.UserInfo = userInfo;

            this.Location = callerLocation;

            //アドレス情報取得
            try
            {
                addresInfoList = new MailLogic().GetAddresInfo(this.UserInfo.UserID);
            }
            catch (NpgsqlException)
            {
                MessageBox.Show("アドレス情報の取得に失敗しました。", "お知らせ");
                return;
            }

            var mailLogic = new MailLogic();

            //画面に宛先一覧を表示し、各行にその宛先のアドレス情報を設定
            for (int i = 0; i < addresInfoList.Count; i++)
            {
                dataGridView1.Rows.Add(mailLogic.GenerateAddressName(addresInfoList[i]));
                dataGridView1.Rows[i].Cells[1].Value = false; //チェックボックスの初期値をfalseに設定
                dataGridView1.Rows[i].Cells[0].Tag = addresInfoList[i]; 
            }
        }


        /// <summary>
        /// OKボタンクリックイベント
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            //チェックボックスがtrueの行のアドレス情報を、宛先フォームのフィールドに設定
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if ((bool)dataGridView1.Rows[i].Cells[1].Value == true)
                {
                    this.MailInfoList.Add((MailInfo)dataGridView1.Rows[i].Cells[0].Tag);
                }
            }

            this.Hide();
        }


        /// <summary>
        /// チェックボックス変更時イベント
        /// 値の変更をすぐに適用させる
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentCellAddress.X == 1 && dataGridView1.IsCurrentCellDirty)
            {
                dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }


        /// <summary>
        /// フォーム終了時イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MailDestinationForm_FormClosing(object sender, FormClosingEventArgs e)
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
