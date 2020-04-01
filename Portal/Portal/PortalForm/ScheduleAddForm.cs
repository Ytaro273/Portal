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
    /// 予定更新フォーム
    /// </summary>
    public partial class ScheduleAddForm : Form
    {
        UserInfo UserInfo; //ログインユーザーのユーザー情報

        short ScheduleID; //予定編集モードで用いる、編集する予定の予定ID

        DateTime ShowingFirstDate; //表示している日の中で、最初の日

        List<FacilityInfo> FacilityInfoList; //施設情報リスト

        //FormClosingイベントが発生した際の挙動の種類
        ClosingType ClosingStatus = ClosingType.DoNothing;


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="userInfo">ログインユーザーのユーザー情報</param>
        /// <param name="scheduleID">予定ID（予定を新規作成する場合はnull）</param>
        /// <param name="callerLocation">呼び出し元の座標</param>
        public ScheduleAddForm(UserInfo userInfo, object scheduleID, Point callerLocation)
        {
            InitializeComponent();

            this.FacilityInfoList = new List<FacilityInfo>();

            this.UserInfo = userInfo;

            this.Location = callerLocation;

            //予定フォームから受け取った予定IDがnullの場合は、予定を新規追加するための画面を表示する
            //nullでない場合は、受け取った予定を編集する画面を表示する
            if (scheduleID == null) IndicateAddMenu();
            else
            {
                this.ScheduleID = (short)scheduleID;
                IndicateEditMenu();
            }
        }


        /// <summary>
        /// 追加ボタンクリックイベント
        /// 入力した情報をもとに、新たに予定を追加する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("データを追加します。よろしいですか？", "質問", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No) return;

            SettingSchedule(ScheduleUpdateType.Add);
        }


        /// <summary>
        /// 保存ボタンクリックイベント
        /// 予定フォームで選択した予定について、編集した内容を保存する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("データを保存します。よろしいですか？", "質問", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No) return;

            SettingSchedule(ScheduleUpdateType.Edit);
        }


        /// <summary>
        /// 削除ボタンクリックイベント
        /// 予定フォームで選択した予定を削除する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            //予定登録情報に、削除する予定の予定IDを設定
            var scheduleRegistrationInfo = new ScheduleRegistrationInfo() { ScheduleID = this.ScheduleID };

            var result = MessageBox.Show("予定を削除します。よろしいですか？", "質問", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No) return;

            try
            {
                new ScheduleLogic().UpdateSchedule(scheduleRegistrationInfo, ScheduleUpdateType.Delete);
            }
            catch (NpgsqlException)
            {
                MessageBox.Show("予定の削除に失敗しました。", "お知らせ");
                return;
            }

            MessageBox.Show("予定の削除に成功しました。", "お知らせ");

            //予定フォームに戻る
            this.Hide();
            new ScheduleForm(this.UserInfo, this.Location).Show();
            this.Close();
        }


        /// <summary>
        /// 押したボタン（保存、編集）に応じて、予定を保存、編集する
        /// </summary>
        /// <param name="scheduleUpdateType">更新種別</param>
        private void SettingSchedule(ScheduleUpdateType scheduleUpdateType)
        {
            string scheduleContens; //予定内容
            var scheduleRegistrationInfo = new ScheduleRegistrationInfo(); //予定登録情報


            if (CheckInput(scheduleUpdateType)) //情報が入力されており、入力された時間が適切かを確認
            {
                if (checkBox1.Checked) scheduleContens = textBox1.Text; //予定内容を新規で追加する場合               
                else scheduleContens = (string)comboBox1.SelectedItem; //予定内容をコンボボックスから選択する場合                

                //画面に入力した情報をもとに、登録する情報のインスタンスを作成
                try
                {
                    //追加の場合
                    if (scheduleUpdateType == ScheduleUpdateType.Add)
                    {
                        scheduleRegistrationInfo = new ScheduleRegistrationInfo()
                        {
                            FacilityID = this.FacilityInfoList[comboBox2.SelectedIndex].FacilityID,
                            UserID = this.UserInfo.UserID,
                            ScheduleContents = scheduleContens,
                            StartTime = new DateTime(
                                int.Parse((string)comboBox3.SelectedItem),
                                int.Parse(textBox2.Text),
                                int.Parse(textBox3.Text),
                                int.Parse((string)comboBox4.SelectedItem),
                                int.Parse((string)comboBox5.SelectedItem),
                                0),
                            EndTime = new DateTime(
                                int.Parse((string)comboBox3.SelectedItem),
                                int.Parse(textBox2.Text),
                                int.Parse(textBox3.Text),
                                int.Parse((string)comboBox6.SelectedItem),
                                int.Parse((string)comboBox7.SelectedItem),
                                0)
                        };
                    }

                    //編集の場合
                    if (scheduleUpdateType == ScheduleUpdateType.Edit)
                    {
                        scheduleRegistrationInfo = new ScheduleRegistrationInfo()
                        {
                            ScheduleID = this.ScheduleID,
                            FacilityID = this.FacilityInfoList[comboBox2.SelectedIndex].FacilityID,
                            UserID = this.UserInfo.UserID,
                            ScheduleContents = scheduleContens,
                            StartTime = new DateTime(
                                int.Parse((string)comboBox3.SelectedItem),
                                int.Parse(textBox2.Text),
                                int.Parse(textBox3.Text),
                                int.Parse((string)comboBox4.SelectedItem),
                                int.Parse((string)comboBox5.SelectedItem),
                                0),
                            EndTime = new DateTime(
                                int.Parse((string)comboBox3.SelectedItem),
                                int.Parse(textBox2.Text),
                                int.Parse(textBox3.Text),
                                int.Parse((string)comboBox6.SelectedItem),
                                int.Parse((string)comboBox7.SelectedItem),
                                0)
                        };
                    }
                }
                catch (FormatException)
                {
                    MessageBox.Show("正しい日付を入力してください。", "お知らせ");
                    return;
                }
                catch (ArgumentOutOfRangeException)
                {
                    MessageBox.Show("正しい日付を入力してください。", "お知らせ");
                    return;
                }

                try
                {
                    new ScheduleLogic().UpdateSchedule(scheduleRegistrationInfo, scheduleUpdateType);
                }
                catch (NpgsqlException)
                {
                    if (scheduleUpdateType == ScheduleUpdateType.Add) MessageBox.Show("予定の追加に失敗しました。", "お知らせ");
                    if (scheduleUpdateType == ScheduleUpdateType.Edit) MessageBox.Show("予定の編集に失敗しました。", "お知らせ");
                    return;
                }

                if (scheduleUpdateType == ScheduleUpdateType.Add) MessageBox.Show("予定の追加に成功しました。", "お知らせ");
                if (scheduleUpdateType == ScheduleUpdateType.Edit) MessageBox.Show("予定の編集に成功しました。", "お知らせ");


                //予定フォームに戻る
                this.Hide();
                new ScheduleForm(this.UserInfo, this.Location).Show();
                this.Close();
            }
        }


        /// <summary>
        /// キャンセルボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("予定メニューに戻ります。よろしいですか？", "お知らせ", MessageBoxButtons.OKCancel);

            if (result == DialogResult.OK)
            {
                this.Hide();
                new ScheduleForm(this.UserInfo, this.Location).Show();
                this.Close();
            }
        }


        /// <summary>
        /// 先週ボタンクリックイベント
        /// 現在表示中のものより1週間前の施設利用予定時間を表示する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            //表示中の最初の日付が現在の日付の7日後より前の場合、表示する最初の日付を現在の日付とする
            if (this.ShowingFirstDate.Date < DateTime.Now.AddDays(7).Date)
            {
                ShowingFirstDate = DateTime.Now;
                IndicateUsageSituation();
            }
            else
            {
                ShowingFirstDate = ShowingFirstDate.AddDays(-7);
                IndicateUsageSituation();
            }
        }


        /// <summary>
        /// 来週ボタンクリックイベント
        /// 現在表示中のものより1週間後の施設利用予定時間を表示する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            ShowingFirstDate = ShowingFirstDate.AddDays(7);
            IndicateUsageSituation();
        }


        /// <summary>
        /// 「予定を新規で追加する」のチェックボックス変更イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            //予定を新規で追加するにチェックが入っているなら、新規の予定を入力するテキストボックスを活性にし、
            //予定を選択するコンボボックスを非活性にする
            if (checkBox1.Checked)
            {
                textBox1.Enabled = true;
                comboBox1.Enabled = false;
            }
            else
            {
                textBox1.Enabled = false;
                comboBox1.Enabled = true;
            }
        }


        /// <summary>
        /// 施設名コンボボックス変更時イベント
        /// 選択された施設の開放時間を表示する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox2_TextChanged(object sender, EventArgs e)
        {
            //「施設を利用しない」が選択されている場合、開放時間を空白にする 
            //施設が選択されている場合、その利用時間を表示する
            if (comboBox2.Text == "施設を利用しない") label4.Text = "";
            else
            {
                var sb = new StringBuilder();

                sb.Append("開放時間 ");
                sb.Append(FacilityInfoList.Find(x => x.FacilityName == comboBox2.Text).
                    OpeningStartTime.ToShortTimeString());
                sb.Append("～");
                sb.Append(FacilityInfoList.Find(x => x.FacilityName == comboBox2.Text).
                    OpeningEndTime.ToShortTimeString());

                label4.Text = sb.ToString();
            }
        }


        /// <summary>
        /// 予定フォームから受け取った予定を編集する画面を表示する
        /// </summary>
        private void IndicateEditMenu()
        {
            SetCommonItem(); //画面共通項目設定

            //追加ボタンを削除するにあたり、保存ボタンと削除ボタンの座標を調整
            button3.Location = button2.Location;
            button2.Location = button1.Location;

            Controls.Remove(button1); //追加ボタンを削除
        }


        /// <summary>
        /// 予定を新規追加するための画面を表示する
        /// </summary>
        private void IndicateAddMenu()
        {
            SetCommonItem();　//画面共通項目設定

            Controls.Remove(button2); //保存ボタンを削除
            Controls.Remove(button3); //削除ボタンを削除

            this.Text = "予定追加";
        }


        /// <summary>
        /// 入力された時間が正しいかをチェック
        /// </summary>
        /// <param name="scheduleUpdateType">更新種別</param>
        /// <returns>チェックの結果</returns>
        private bool CheckInput(ScheduleUpdateType scheduleUpdateType)
        {
            var selectedStartTime = new DateTime(); //入力された予定開始時間
            var selectedEndTime = new DateTime(); //入力された予定終了時間


            //ログインユーザーの登録済みの予定一覧を取得
            var registeredScheduleList = new ScheduleLogic().GetRegisteredSchedule(this.UserInfo.UserID);


            //新規で追加するにチェックがあり、新規の予定のテキストボックスが空白の場合
            if (checkBox1.Checked && textBox1.Text == "")
            {
                MessageBox.Show("追加する予定内容を入力してください。", "お知らせ");
                return false;
            }
            
            //新規で追加するにチェックがなく、予定内容が選択されていない場合
            if (checkBox1.Checked == false && comboBox1.SelectedItem == null)
            {
                MessageBox.Show("予定内容を選択してください。", "お知らせ");
                return false;
            }

            //利用する施設が選択されていない場合
            if (comboBox2.SelectedItem == null)
            {
                MessageBox.Show("利用する施設を選択してください。", "お知らせ");
                return false;
            }


            //コンボボックスで選択された開始時間の時、分をまとめる
            var selectedStartTimeOfDay = new TimeSpan(int.Parse((string)comboBox4.SelectedItem), int.Parse((string)comboBox5.SelectedItem), 0);

            //コンボボックスで選択された終了時間の時、分をまとめる
            var selectedEndTimeOfDay = new TimeSpan(int.Parse((string)comboBox6.SelectedItem), int.Parse((string)comboBox7.SelectedItem), 0);


            //選択された開始時間が終了時間より後になっていた場合
            if (selectedStartTimeOfDay > selectedEndTimeOfDay)
            {
                MessageBox.Show("選択された開始時間が終了時間より後になっています。", "お知らせ");
                return false;
            }


            //コンボボックスで選択された利用開始時間が、施設の開放開始時間より前の場合
            if (this.FacilityInfoList[comboBox2.SelectedIndex].OpeningStartTime.TimeOfDay > selectedStartTimeOfDay)
            {
                MessageBox.Show("選択された時間が施設の開放時間外です。", "お知らせ");
                return false;
            }

            //コンボボックスで選択された利用終了時間が、施設の開放終了時間より後の場合
            if (this.FacilityInfoList[comboBox2.SelectedIndex].OpeningEndTime.TimeOfDay < selectedEndTimeOfDay)
            {
                MessageBox.Show("選択された時間が施設の開放時間外です。", "お知らせ");
                return false;
            }


            //入力された時間の書式が正しいかをチェックし、1つの変数にまとめる
            try
            {
                selectedStartTime = new DateTime(
                    int.Parse((string)comboBox3.SelectedItem),
                    int.Parse(textBox2.Text),
                    int.Parse(textBox3.Text),
                    int.Parse((string)comboBox4.SelectedItem),
                    int.Parse((string)comboBox5.SelectedItem),
                    0);

                selectedEndTime = new DateTime(
                    int.Parse((string)comboBox3.SelectedItem),
                    int.Parse(textBox2.Text),
                    int.Parse(textBox3.Text),
                    int.Parse((string)comboBox6.SelectedItem),
                    int.Parse((string)comboBox7.SelectedItem),
                    0);
            }
            catch (FormatException)
            {
                MessageBox.Show("正しい日付を入力してください。", "お知らせ");
                return false;
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("正しい日付を入力してください。", "お知らせ");
                return false;
            }

            if (selectedStartTime.Date < DateTime.Now.Date)
            {
                MessageBox.Show("本日以降の日付を入力してください。", "お知らせ");
                return false;
            }

            //更新種別が編集の場合、編集対象の予定IDと同じ予定IDの要素を、登録済み予定一覧から削除する
            if (scheduleUpdateType == ScheduleUpdateType.Edit)
            {
                registeredScheduleList.RemoveAll(x => x.ScheduleID == this.ScheduleID);
            }


            //登録済み予定の時間と選択した時間が重なる場合
            if (registeredScheduleList.Any(x => x.StartTime >= selectedStartTime && x.StartTime < selectedEndTime))
            {
                MessageBox.Show("選択された時間には既に予定が登録されています。", "お知らせ");
                return false;
            }
            if (registeredScheduleList.Any(x => x.StartTime <= selectedStartTime && x.EndTime > selectedStartTime))
            {
                MessageBox.Show("選択された時間には既に予定が登録されています。", "お知らせ");
                return false;
            }

            return true;
        }


        /// <summary>
        /// 画面共通項目設定
        /// </summary>
        private void SetCommonItem()
        {
            var scheduleTypeList = new List<string>(); //予定一覧
            const int selectableYearRange = 3; //選択可能な年数の範囲

            //定時（勤務開始）
            const string HourOfRegularStartTime = "9";
            const string MinuteOfRegularStartTime = "00";

            //定時（勤務終了）
            const string HourOfRegularEndTime = "17";
            const string MinuteOfRegularEndTime = "45";

            var scheduleLogic = new ScheduleLogic();

            try
            {
                scheduleTypeList = scheduleLogic.GetScheduleType(); //予定一覧を取得
                this.FacilityInfoList = scheduleLogic.GetFacilityInfo(); //施設情報リストを取得
            }
            catch
            {
                MessageBox.Show("予定情報の取得に失敗しました。", "お知らせ");
                return;
            }

            this.ShowingFirstDate = DateTime.Now; //表示する最初の日付を今日の日付に設定

            IndicateUsageSituation(); //施設利用状況表示

            textBox1.Enabled = false; //新規予定内容追加のテキストボックスを非活性に

            //コンボボックスの選択肢に予定名の一覧を追加
            foreach (var scheduleType in scheduleTypeList)
            {
                comboBox1.Items.Add(scheduleType);
            }

            //コンボボックスの選択肢に施設名の一覧を追加
            foreach (var facilityInfo in this.FacilityInfoList)
            {
                comboBox2.Items.Add(facilityInfo.FacilityName);
            }

            //年のコンボボックスの選択肢を追加
            for (int i = 0; i < selectableYearRange; i++)
            {
                comboBox3.Items.Add((DateTime.Now.Year + i).ToString());
            }

            //年月日のコンボボックスの初期値を、現在の年月日に設定
            comboBox3.Text = DateTime.Now.Year.ToString();


            //時、分のコンボボックスの初期値を、定時に設定
            comboBox4.Text = HourOfRegularStartTime;
            comboBox5.Text = MinuteOfRegularStartTime;

            comboBox6.Text = HourOfRegularEndTime;
            comboBox7.Text = MinuteOfRegularEndTime;
        }


        /// <summary>
        /// 施設利用状況表示
        /// </summary>
        private void IndicateUsageSituation()
        {
            var facilityDisplayInfoList = new List<FacilityDisplayInfo>(); //施設表示情報

            dataGridView1.Rows.Clear();

            try
            {
                //1週間分の施設利用情報を取得
                facilityDisplayInfoList = new ScheduleLogic().GetOneWeekFacilityDisplayInfo(this.ShowingFirstDate);
            }
            catch (NpgsqlException)
            {
                MessageBox.Show("施設情報の取得に失敗しました。", "お知らせ");
            }

            //ヘッダーに日付と曜日を表示
            for (int i = 0; i < 7; i++)
            {
                dataGridView1.Columns[i + 1].HeaderText =
                    this.ShowingFirstDate.AddDays(i).ToString("M/d") +
                    "(" +
                    this.ShowingFirstDate.AddDays(i).ToString("ddd") +
                    ")";
            }

            for (int i = 0; i < facilityDisplayInfoList.Count; i++)
            {
                //表に行を追加し、施設名を表示
                dataGridView1.Rows.Add(facilityDisplayInfoList[i].FacilityName);

                //その施設が利用される時間を表示
                foreach (var facilityUsageInfo in facilityDisplayInfoList[i].FacilityUsageInfoList)
                {
                    //表示する最初の日付と、予定の日付が何日離れているかを算出することで、
                    //表の何列目にその利用時間を表示するかを算出し、その列に利用時間を表示
                    //（最も左の列は施設名が表示されている）
                    TimeSpan span = facilityUsageInfo.StartTime.Date - this.ShowingFirstDate.Date;
                    dataGridView1.Rows[i].Cells[span.Days + 1].Value =
                        facilityUsageInfo.StartTime.ToString("t") +
                        "～" +
                        facilityUsageInfo.EndTime.ToString("t");
                }
            }

            //何年の情報かを表示
            label14.Text = "(" + this.ShowingFirstDate.Year.ToString() + "年)";

            //表示している最初の日付が今日の場合、先週ボタンを非活性にする
            if (this.ShowingFirstDate.Date == DateTime.Now.Date) button5.Enabled = false;
            else button5.Enabled = true;
        }


        /// <summary>
        /// フォーム終了時イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScheduleAddForm_FormClosing(object sender, FormClosingEventArgs e)
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
