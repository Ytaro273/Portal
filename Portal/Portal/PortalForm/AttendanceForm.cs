using Npgsql;
using Portal.Enum;
using Portal.Logic;
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
    /// 勤怠フォーム
    /// </summary>
    public partial class AttendanceForm : Form
    {
        UserInfo UserInfo; //ログインユーザーのユーザー情報

        AttendanceMonth ShowingMonth; //表示中の月

        List<AttendanceInfo> AttendanceInfoList; // 勤怠情報リスト       
        
        ClosingType ClosingStatus = ClosingType.DoNothing; //FormClosingイベントが発生した際の挙動の種類


        /// <summary>
        /// コンストラクタ
        /// フォーム起動時にすべき画面表示を行う
        /// </summary>
        /// <param name="userInfo">ログインユーザーのユーザー情報</param>
        /// <param name="callerLocation">呼び出し元の座標</param>
        public AttendanceForm(UserInfo userInfo, Point callerLocation)
        {
            InitializeComponent();

            this.AttendanceInfoList = new List<AttendanceInfo>();

            this.UserInfo = userInfo;

            this.Location = callerLocation;

            //ログインユーザーの名前を画面に表示
            label1.Text = "ログインユーザー：" + this.UserInfo.UserName;

            //現在の月を、表示中の月として設定
            this.ShowingMonth = (AttendanceMonth)DateTime.Now.Month;

            //現在の月の勤怠情報を画面に表示
            ConfigureAttendanceInfo();

            //入力不可な列の色を灰色にする（曜日、残業時間）
            dataGridView1.Columns[1].DefaultCellStyle.BackColor = Color.LightGray;
            dataGridView1.Columns[5].DefaultCellStyle.BackColor = Color.LightGray;
        }


        /// <summary>
        /// 先月ボタンクリックイベント
        /// 表示中の月の1か月前の情報を表示する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button1_Click(object sender, EventArgs e)
        {
            //表示中の月が1月なら、表示する月を12月に設定
            if (this.ShowingMonth == AttendanceMonth.January) this.ShowingMonth = AttendanceMonth.December;
            else this.ShowingMonth--;

            //1か月前の勤怠情報を取得し、画面に表示
            ConfigureAttendanceInfo();
        }


        /// <summary>
        /// 来月ボタンクリックイベント
        /// 表示中の月の1か月の情報を表示する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button2_Click(object sender, EventArgs e)
        {
            //表示中の月が12月なら、表示する月を1月に設定
            if (ShowingMonth == AttendanceMonth.December) ShowingMonth = AttendanceMonth.January;
            else ShowingMonth++;

            // 1か月後の勤怠情報を取得し、画面に表示
            ConfigureAttendanceInfo();
        }


        /// <summary>
        /// 保存ボタンクリックイベント
        /// ユーザーが入力した勤怠情報を保存する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button3_Click(object sender, EventArgs e)
        {          
            var attendanceTimeInfoList = new List<AttendanceTimeInfo>(); //勤務時間情報リスト

            string year = DateTime.Now.Year.ToString(); //現在の年
            string month = ((int)ShowingMonth).ToString(); //表示中の月

            //新規追加用の行のみしかない場合、何らかのデータの入力を要求する
            if (dataGridView1.Rows.Count <= 1)
            {
                MessageBox.Show("データを入力してください。", "お知らせ");
                return;
            }          

            //ユーザーが入力した勤怠情報を、保存するための形（勤務時間情報）にする
            for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
            {
                //入力欄がすべて空白の場合、次の行に移る
                if(dataGridView1.Rows[i].Cells[0].Value == null &&
                    dataGridView1.Rows[i].Cells[2].Value == null &&
                    dataGridView1.Rows[i].Cells[3].Value == null &&
                    dataGridView1.Rows[i].Cells[4].Value == null)
                { 
                    continue; 
                }

                //日付の情報
                string day = Convert.ToString(dataGridView1.Rows[i].Cells[0].Value);

                //勤務開始時間の年、月、日、時、分の情報を結合して1つにする
                string startTimeString = 
                    year + 
                    "/" + 
                    month + 
                    "/" + 
                    day +
                    " " +
                    Convert.ToString(dataGridView1.Rows[i].Cells[2].Value);

                //勤務終了時間の年、月、日、時、分の情報を結合して1つにする
                string endTimeString =
                    year +
                    "/" +
                    month +
                    "/" +
                    day +
                    " " +
                    Convert.ToString(dataGridView1.Rows[i].Cells[3].Value);

                //勤務時間情報リストに要素を追加（勤務開始時間、勤務終了時間、休憩時間）
                try
                {
                    attendanceTimeInfoList.Add(new AttendanceTimeInfo(
                        DateTime.Parse(startTimeString),
                        DateTime.Parse(endTimeString),
                        Convert.ToInt32(dataGridView1.Rows[i].Cells[4].Value)));
                }
                //入力された値の書式が誤っている場合
                catch (FormatException)
                {
                    MessageBox.Show("正しい値を入力してください。", "お知らせ");
                    return;
                }
            }          

            //勤務時間情報を保存する
            try
            {
                new AttendanceLogic().UpdateAttendanceTimeInfo(this.UserInfo.UserID, attendanceTimeInfoList);
                ConfigureAttendanceInfo();
                MessageBox.Show("保存に成功しました。", "お知らせ");
            }
            catch (NpgsqlException)
            {
                ConfigureAttendanceInfo();
                MessageBox.Show("保存に失敗しました。", "お知らせ");
            }
        }


        /// <summary>
        /// 終了ボタンクリックイベント
        /// メインメニューに戻る
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button4_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("メインメニューに戻ります。よろしいですか？","お知らせ", MessageBoxButtons.OKCancel);
            if (result == DialogResult.OK)
            {
                this.Hide();
                new MenuForm(this.UserInfo, this.Location).Show();
                this.Close();
            }
        }


        /// <summary>
        /// 表示する月の勤怠情報を取得し、画面に表示する
        /// </summary>
        private void ConfigureAttendanceInfo()
        {
            //何年何月の情報なのかを表示
            label2.Text = DateTime.Now.Year.ToString() + "年" + ((int)this.ShowingMonth).ToString() + "月";

            //勤怠情報を取得し、画面に表示
            try
            {
                this.AttendanceInfoList = new AttendanceLogic().GetAttendanceInfo(this.UserInfo.UserID, this.ShowingMonth);
                IndicateAttendanceInfo();
            }
            catch (NpgsqlException)
            {
                MessageBox.Show("勤怠情報の取得に失敗しました。", "お知らせ");
            }
            catch (InvalidCastException)
            {
                MessageBox.Show("勤怠情報の取得に失敗しました。", "お知らせ");
            }
        }


        /// <summary>
        /// 画面に勤怠情報を表示する
        /// </summary>
        private void IndicateAttendanceInfo()
        {
            //表を初期化
            dataGridView1.Rows.Clear();

            //行を追加
            foreach (var attendanceInfo in this.AttendanceInfoList)
            {
                dataGridView1.Rows.Add(
                    attendanceInfo.Date,
                    attendanceInfo.DayOfWeek,
                    attendanceInfo.AttendanceTimeInfo.StartTime.ToString("t"),
                    attendanceInfo.AttendanceTimeInfo.EndTime.ToString("t"),
                    attendanceInfo.AttendanceTimeInfo.BreakTime,
                    attendanceInfo.OverTime);
            }
        }


        /// <summary>
        /// フォーム終了時イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AttendanceForm_FormClosing(object sender, FormClosingEventArgs e)
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
