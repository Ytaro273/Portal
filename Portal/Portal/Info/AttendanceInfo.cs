using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Info
{
    /// <summary>
    /// 勤怠情報
    /// </summary>
    public class AttendanceInfo
    {
        //コンストラクタ
        public AttendanceInfo(int date, string dayOfWeek, AttendanceTimeInfo attendanceTime, int overTime)
        {
            this.Date = date;
            this.DayOfWeek = dayOfWeek;
            this.AttendanceTimeInfo = attendanceTime;
            this.OverTime = overTime;
        }

        public int Date { get; } //日付

        public string DayOfWeek { get; } //曜日

        public AttendanceTimeInfo AttendanceTimeInfo { get; } //勤務時間情報

        public int OverTime { get; } //残業時間
    }
}
