using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Info
{
    /// <summary>
    /// 勤務時間情報
    /// </summary>
    public class AttendanceTimeInfo
    {      
        //コンストラクタ
        public AttendanceTimeInfo(DateTime startTime, DateTime endTime, int breakTime)
        {
            this.StartTime = startTime;
            this.EndTime = endTime;
            this.BreakTime = breakTime;
        }

        public DateTime StartTime { get; } //勤務開始時間

        public DateTime EndTime { get; } //勤務終了時間

        public int BreakTime { get; } //休憩時間

    }
}
