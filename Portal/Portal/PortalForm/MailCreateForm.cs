using Npgsql;
using System;
using Portal.Enum;
using Portal.Info;
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
    /// メール作成フォーム
    /// </summary>
    public partial class MailCreateForm : Form
    {
        UserInfo UserInfo; //ログインユーザーのユーザー情報

        List<MailInfo> MailInfoList; //送信先の情報および送信するメールの内容
        
        ClosingType ClosingStatus = ClosingType.DoNothing; //FormClosingイベントが発生した際の挙動の種類


        /// <summary>
        /// コンストラクタ（返信の場合）
        /// </summary>
        /// <param name="userInfo">ログインユーザーの情報</param>
        /// <param name="mailInfo">返信するメールの情報</param>
        /// <param name="callerLocation">呼び出し元の座標</param>
        public MailCreateForm(UserInfo userInfo, MailInfo mailInfo, Point callerLocation)
        {
            InitializeComponent();

            this.MailInfoList = new List<MailInfo>();

            Controls.Remove(button1); //宛先ボタンを削除する     

            this.UserInfo = userInfo;

            this.Location = callerLocation;

            this.MailInfoList.Add(mailInfo); //メールフォームから受け取ったメール情報を送信先として設定
           
            textBox1.Text = "Re:" + mailInfo.Subject; //返信するメールの件名を表示
        }


        /// <summary>
        /// コンストラクタ（新規作成の場合）
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="callerLocation">呼び出し元の座標</param>
        public MailCreateForm(UserInfo userInfo, Point callerLocation)
        {
            InitializeComponent();

            this.MailInfoList = new List<MailInfo>();

            button2.Enabled = false; //送信ボタンを非活性にする

            this.UserInfo = userInfo;

            this.Location = callerLocation;
        }


        /// <summary>
        /// 宛先ボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            string addresName = null; //画面に表示する宛先
            var mailLogic = new MailLogic();

            this.Hide();

            //宛先フォームに遷移し、宛先を選択
            var mailDestinationForm = new MailDestinationForm(this.UserInfo, this.Location); 

            mailDestinationForm.ShowDialog();

            this.Show();

            if (mailDestinationForm.MailInfoList.Count > 1)
            {
                //宛先フォームで選択した宛先を、メール作成フォームのフィールドに設定
                this.MailInfoList = mailDestinationForm.MailInfoList; 

                //送信先のアドレスすべてを連結して一つにする
                for (int i = 0; i < mailDestinationForm.MailInfoList.Count; i++)
                {
                    addresName += mailLogic.GenerateAddressName(mailDestinationForm.MailInfoList[i]);

                    //繰り返し処理の最後の周以外は、文字列にカンマを付け加える
                    if (i < mailDestinationForm.MailInfoList.Count - 1) addresName += ",";
                }

                label1.Text = addresName; //送信先のアドレスを表示

                button2.Enabled = true; //送信ボタンを活性に
            }
            else button2.Enabled = false; //送信ボタンを非活性に

            mailDestinationForm.Dispose(); //宛先フォームのリソースを解放
        }


        /// <summary>
        /// 送信ボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            //件名かメッセージ内容が空白なら、入力を要求
            if (textBox1.Text == "" || textBox2.Text == "")
            {
                MessageBox.Show("件名とメッセージを入力してください。", "お知らせ");
                return;
            }

            var result = MessageBox.Show("メールを送信します。よろしいですか？", "お知らせ", MessageBoxButtons.OKCancel);

            if (result == DialogResult.OK)
            {
                var mailLogic = new MailLogic();

                //件名とメッセージ内容を設定
                foreach (var mailInfo in this.MailInfoList)
                {
                    mailInfo.Subject = textBox1.Text;
                    mailInfo.Message = textBox2.Text;
                }

                //メールを送信
                try
                {
                    mailLogic.SendMail(UserInfo.UserID, this.MailInfoList);
                }
                catch (NpgsqlException)
                {
                    MessageBox.Show("メールの送信に失敗しました。", "お知らせ");
                    return;
                }

                MessageBox.Show("送信が完了しました。", "お知らせ");

                this.Hide();
                new MailForm(this.UserInfo, this.Location).Show();
                this.Close();
            }
        }


        /// <summary>
        /// キャンセルボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("メール一覧メニューに戻ります。よろしいですか？", "お知らせ", MessageBoxButtons.OKCancel);

            if (result == DialogResult.OK)
            {
                this.Hide();
                new MailForm(this.UserInfo, this.Location).Show();
                this.Close();
            }
        }


        /// <summary>
        /// フォーム終了時イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MailCreateForm_FormClosing(object sender, FormClosingEventArgs e)
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
