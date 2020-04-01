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
    /// メールフォーム
    /// </summary>
    public partial class MailForm : Form
    {
        UserInfo UserInfo; //ログインユーザーのユーザー情報

        List<MailInfo> MailInfoList; //表示するメールの情報リスト

        bool IsShowingIncomingMails; //現在表示しているのが受信したメールの一覧なのかどうか
        
        ClosingType ClosingStatus = ClosingType.DoNothing; //FormClosingイベントが発生した際の挙動の種類


        /// <summary>
        /// コンストラクタ
        /// フォーム起動時にすべき画面表示を行う
        /// </summary>
        /// <param name="userInfo">ログインユーザーのユーザー情報</param>
        /// <param name="callerLocation">呼び出し元の座標</param>
        public MailForm(UserInfo userInfo, Point callerLocation)
        {
            InitializeComponent();

            this.MailInfoList = new List<MailInfo>();

            this.UserInfo = userInfo;

            this.Location = callerLocation;

            label1.Text = "ログインユーザー：" + this.UserInfo.UserName;

            IndicateMail(UserInfo.UserID,SendOrReceive.Receiver);

            this.IsShowingIncomingMails = true;
        }


        /// <summary>
        /// 受信ボックスボタンクリックイベント
        /// ログインユーザーが受信したメールを表示する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            IndicateMail(this.UserInfo.UserID, SendOrReceive.Receiver);

            this.IsShowingIncomingMails = true;
        }


        /// <summary>
        /// 送信ボックスボタンクリックイベント
        /// ログインユーザーが送信したメールを表示する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            IndicateMail(this.UserInfo.UserID, SendOrReceive.Sender);

            this.IsShowingIncomingMails = false;

            button4.Enabled = false; //返信ボタンを非活性にする
        }


        /// <summary>
        /// 新規作成ボタンクリックイベント
        /// 新しく宛先を指定してメールを作成する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();
            new MailCreateForm(this.UserInfo, this.Location).Show();
            this.Close();
        }


        /// <summary>
        /// 返信ボタンクリックイベント
        /// 選択したメールに対して返信する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        { 
            this.Hide();               

            //メール作成フォームに、ログインユーザーの情報、選択したメールの情報、メールフォームの現在の座標を渡す
            new MailCreateForm(this.UserInfo, (MailInfo)dataGridView1.CurrentRow.Cells[0].Tag, this.Location).Show();

            this.Close();
        }


        /// <summary>
        /// 終了ボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
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
        /// 表の行選択時イベント
        /// 選択された行のメール内容を表示する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Rows[e.RowIndex].Cells[0].Tag == null) return; //その行のメール情報がなければreturn

            //Tagに設定されているメール内容を表示
            var SelectedRowMailInfo = (MailInfo)dataGridView1.Rows[e.RowIndex].Cells[0].Tag;
            
            textBox1.Text = (string)SelectedRowMailInfo.Message;

            //受信したメール一覧を表示中なら、返信ボタンを活性にする
            if (this.IsShowingIncomingMails) button4.Enabled = true;
        }


        /// <summary>
        /// メールを取得し、画面に表示する
        /// </summary>
        /// <param name="userID">ユーザーID</param>
        /// <param name="sendOrReceive">ユーザーIDが送信者側、受信者側どちらなのか</param>
        private void IndicateMail(string userID, SendOrReceive sendOrReceive)
        {
            try
            {
                this.MailInfoList = new MailLogic().GetMail(userID, sendOrReceive);
            }
            catch(NpgsqlException)
            {
                MessageBox.Show("メール情報の取得に失敗しました。", "お知らせ");
                return;
            }

            button4.Enabled = false; //返信ボタンを非活性にする

            IndicateMailList(); //メールを表に表示する
        }


        /// <summary>
        /// メール一覧を画面に表示
        /// </summary>
        private void IndicateMailList()
        {
            dataGridView1.Rows.Clear();

            for (int i = 0; i < this.MailInfoList.Count; i++)
            {
                //表に行を追加し、送信日時、ユーザー名、件名を表示
                dataGridView1.Rows.Add(
                    this.MailInfoList[i].AddedDate.ToString("g"),
                    this.MailInfoList[i].UserName,
                    this.MailInfoList[i].Subject);

                //メール情報をその行の0列目のTagに設定
                //目的:その行が選択されたときにメール情報のメール内容を表示したり、返信する際にその行のメール情報を使用するため
                dataGridView1.Rows[i].Cells[0].Tag = this.MailInfoList[i];
            }

            //表の1行目のメール内容を表示する
            var firstRowMailInfo = (MailInfo)dataGridView1.Rows[0].Cells[0].Tag;
            textBox1.Text = (string)firstRowMailInfo.Message;
        }


        /// <summary>
        /// フォーム終了時イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MailForm_FormClosing(object sender, FormClosingEventArgs e)
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
