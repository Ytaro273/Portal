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
using Npgsql;

namespace Portal.PortalForm
{
    /// <summary>
    /// メニューフォーム
    /// </summary>
    public partial class MenuForm : Form
    {
        UserInfo UserInfo; //ログインユーザーのユーザー情報

        ClosingType ClosingStatus = ClosingType.DoNothing; //FormClosingイベントが発生した際の挙動の種類


        /// <summary>
        /// コンストラクタ
        /// フォーム起動時にすべき画面表示を行う
        /// </summary>
        /// <param name="userInfo">ログインユーザーのユーザー情報</param>
        /// <param name="callerLocation">呼び出し元の座標</param>
        public MenuForm(UserInfo userInfo, Point callerLocation)
        {
            InitializeComponent();

            this.UserInfo = userInfo;
            this.Location = callerLocation;

            this.Text = "メインメニュー（" + this.UserInfo.AuthorityName + ")";
            label1.Text = "ログインユーザー：" + this.UserInfo.UserName;

            //権限IDが一般の場合、一般モードに設定（デフォルトは管理者モード）
            if (this.UserInfo.AuthorityID == (int)Authority.General) SetToGeneralMode();
        }


        /// <summary>
        /// 画面を一般モードに設定
        /// </summary>
        private void SetToGeneralMode()
        {
            //ボタンを1つ無くすことによって削られる、フォームの縦の長さ
            const int reduceValue = 30;

            //ログアウトボタンの座標を調整
            button5.Location = button4.Location;

            //管理者ユーザー専用ボタンを削除
            Controls.Remove(button4);

            //フォームのサイズを設定
            Height -= reduceValue; 
        }


        /// <summary>
        /// 勤怠ボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            new AttendanceForm(this.UserInfo, this.Location).Show();
            this.Close();
        }


        /// <summary>
        /// 予定ボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button2_Click(object sender, EventArgs e)
        {
            this.Hide();
            new ScheduleForm(this.UserInfo, this.Location).Show();
            this.Close();
        }


        /// <summary>
        /// メールボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button3_Click(object sender, EventArgs e)
        {
            this.Hide();
            new MailForm(this.UserInfo, this.Location).Show();
            this.Close();
        }


        /// <summary>
        /// ユーザー管理ボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            this.Hide();
            new UserManageForm(this.UserInfo, this.Location).Show();
            this.Close();
        }


        /// <summary>
        /// ログアウトボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            this.Hide();
            new LoginForm(this.Location).Show();
            this.Close();
        }


        /// <summary>
        /// フォーム終了時イベント
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// </summary>
        private void MenuForm_FormClosing(object sender, FormClosingEventArgs e)
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
