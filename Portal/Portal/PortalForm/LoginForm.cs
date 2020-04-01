using Npgsql;
using Portal.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Portal.PortalForm
{
    /// <summary>
    /// ログインフォーム
    /// </summary>
    public partial class LoginForm : Form
    {      
        ClosingType ClosingStatus = ClosingType.DoNothing; //FormClosingイベントが発生した際の挙動の種類
        
        bool IsFirstForm; //一番最初に呼び出されたフォームかどうか


        /// <summary>
        /// コンストラクタ（Main関数から呼ばれた場合）
        /// </summary>
        public LoginForm()
        {
            InitializeComponent();

            this.StartPosition = FormStartPosition.CenterScreen;

            this.IsFirstForm = true;
        }


        /// <summary>
        /// コンストラクタ(メニューフォームから呼ばれた場合）
        /// </summary>
        /// <param name="callerLoaction">呼び出し元の座標</param>
        public LoginForm(Point callerLoaction)
        {
            InitializeComponent();

            this.Location = callerLoaction;
        }


        /// <summary>
        /// ログインボタンクリックイベント
        /// 入力されたIDとパスワードが正しければログインする
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button1_Click(object sender, EventArgs e)
        {
            if (CheckInput())
            {
                var loginLogic = new LoginLogic();

                //IDとパスワードが一致すればメインメニューを表示
                try
                {
                    if (loginLogic.UserCheck(textBox1.Text, textBox2.Text))
                    {
                        this.Hide();
                        new MenuForm(loginLogic.UserInfo, this.Location).Show();

                        //このフォームが一番最初に呼び出されたフォームでなければ閉じる
                        if (this.IsFirstForm != true) this.Close();
                    }
                    else MessageBox.Show("IDとパスワードが一致しません。", "お知らせ");
                }
                catch (NpgsqlException)
                {
                    MessageBox.Show("ログインに失敗しました。", "お知らせ");
                }
            }

            textBox1.ResetText();
            textBox2.ResetText();
        }


        /// <summary>
        /// IDかパスワードが未入力でないかを確認
        /// IDとパスワードが両方入力されていればtrueを返す
        /// </summary>
        /// <returns>チェックの結果</returns>
        private bool CheckInput()
        {
            textBox1.BackColor = Color.White;
            textBox2.BackColor = Color.White;

            if (textBox1.Text != "" && textBox2.Text != "") return true;
            else
            {
                //未入力の欄を赤色にする
                if (textBox1.Text == "") textBox1.BackColor = Color.Red;
                if (textBox2.Text == "") textBox2.BackColor = Color.Red;

                MessageBox.Show("未入力の項目があります。", "お知らせ");
                return false;
            }
        }


        /// <summary>
        /// フォーム終了時イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoginForm_FormClosing(object sender, FormClosingEventArgs e)
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



