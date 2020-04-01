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
    /// 予定フォーム
    /// </summary>
    public partial class ScheduleForm : Form
    {
        UserInfo UserInfo; //ログインユーザーのユーザー情報

        DateTime ShowingFirstDate; //表示している日の中で、最初の日

        ClosingType ClosingStatus = ClosingType.DoNothing; //FormClosingイベントが発生した際の挙動の種類


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="userInfo">ログインユーザーのユーザー情報</param>
        /// <param name="callerLocation">呼び出し元の座標</param>
        public ScheduleForm(UserInfo userInfo, Point callerLocation)
        {
            InitializeComponent();

            this.UserInfo = userInfo;

            this.Location = callerLocation;

            //ログインユーザーの名前を画面に表示
            label1.Text = "ログインユーザー：" + this.UserInfo.UserName;

            //現在の日付を、表示中の最初の日付として設定
            this.ShowingFirstDate = DateTime.Now;

            //今日から1週間分の予定を画面に表示
            IndicateSchedule();
        }


        /// <summary>
        /// 先週ボタンクリックイベント
        /// 現在表示中の予定より1週間前の予定を表示する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            //表示中の最初の日付の7日前が今日より前なら、今日から1週間分の予定を表示
            if (this.ShowingFirstDate.Date < DateTime.Now.AddDays(7).Date)
            {
                this.ShowingFirstDate = DateTime.Now;
                IndicateSchedule();
            }
            else
            {
                this.ShowingFirstDate = this.ShowingFirstDate.AddDays(-7);
                IndicateSchedule();
            }
        }


        /// <summary>
        /// 来週ボタンクリックイベント
        /// 現在表示中の予定より1週間後の予定を表示する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            this.ShowingFirstDate = this.ShowingFirstDate.AddDays(7);
            IndicateSchedule();
        }


        /// <summary>
        /// 終了ボタンクリックイベント
        /// メインメニューに戻る
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
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
        /// 表クリックイベント
        /// 表のボタンをクリックしていた場合、予定追加フォームに遷移
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //クリックしたのがヘッダーの場合、何もしない
            if (e.RowIndex == -1) return;

            //クリックしたのがボタンの場合、予定追加フォームに移動
            if (dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex] is DataGridViewButtonCell)
            {
                this.Hide();

                new ScheduleAddForm(
                    this.UserInfo, 
                    dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag, 
                    this.Location).
                        ShowDialog();

                this.Close();
            }
        }


        /// <summary>
        /// 1週間分の予定を表示
        /// </summary>
        private void IndicateSchedule()
        {
            //表示するための予定情報リスト
            var scheduleDisplayInfoList = new List<ScheduleDisplayInfo>();

            //表を初期化
            dataGridView1.Rows.Clear();

            //年を表示
            label2.Text = "予定" + "(" + this.ShowingFirstDate.Year.ToString() + "年)";

            //1週間分の予定を取得
            try
            {
                scheduleDisplayInfoList = new ScheduleLogic().GetOneWeekSchedule(this.ShowingFirstDate);
            }
            catch (NpgsqlException)
            {
                MessageBox.Show("予定情報の取得に失敗しました。", "お知らせ");
                return;
            }

            //表のヘッダに日付と曜日を表示
            for (int i = 0; i < 7; i++)
            {
                dataGridView1.Columns[i + 1].HeaderText =
                    ShowingFirstDate.AddDays(i).ToString("M/d") +
                    "(" +
                    ShowingFirstDate.AddDays(i).ToString("ddd") +
                    ")";
            }

            //表にユーザー名と予定を表示
            for (int i = 0; i < scheduleDisplayInfoList.Count; i++)
            {
                //行を追加し、ユーザー名を表示
                dataGridView1.Rows.Add(scheduleDisplayInfoList[i].UserName);

                //予定内容と時間を表示
                foreach (var scheduleInfo in scheduleDisplayInfoList[i].ScheduleInfoList)
                {
                    //表示する最初の日付と、予定の日付が何日離れているかを算出することで、
                    //表の何列目にその予定を表示するかを算出し、その列に予定を表示
                    //（最も左の列はユーザー名が表示されている）
                    TimeSpan span = scheduleInfo.StartTime.Date - ShowingFirstDate.Date;
                    dataGridView1.Rows[i].Cells[span.Days + 1].Value =
                        scheduleInfo.ScheduleContents +
                        "(" +
                        scheduleInfo.StartTime.ToString("t") +
                        "～" +
                        scheduleInfo.EndTime.ToString("t") +
                        ")";

                    //ログインユーザーの行の予定が入っているセルのタグに、その予定のIDを設定
                    dataGridView1.Rows[i].Cells[span.Days + 1].Tag = scheduleInfo.ScheduleID;
                }

                //ログインユーザーの予定の行をボタンにする
                if (scheduleDisplayInfoList[i].UserID == UserInfo.UserID)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        dataGridView1.Rows[i].Cells[j] =
                            new DataGridViewButtonCell() 
                            { 
                                Value = dataGridView1.Rows[i].Cells[j].Value,
                                Tag = dataGridView1.Rows[i].Cells[j].Tag
                            };
                    }
                }
            }

            //表示されている最初の日付が今日の場合、先週ボタンを非活性に
            if (this.ShowingFirstDate.Date == DateTime.Now.Date) button1.Enabled = false;
            else button1.Enabled = true;
        }


        /// <summary>
        /// フォーム終了時イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScheduleForm_FormClosing(object sender, FormClosingEventArgs e)
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
