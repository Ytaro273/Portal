using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Info
{
    /// <summary>
    /// 予定情報
    /// </summary>
    public class ScheduleInfo
    {
        //コンストラクタ
        public ScheduleInfo(short scheduleID, string userID, string scheduleContents, DateTime startTime, DateTime endTime)
        {
            this.ScheduleID = scheduleID;
            this.UserID = userID;
            this.ScheduleContents = scheduleContents;
            this.StartTime = startTime;
            this.EndTime = endTime;
        }

        public short ScheduleID { get; } //予定ID
        public string UserID { get; } //ユーザーID
        public string ScheduleContents { get; } //予定内容
        public DateTime StartTime { get; } //予定開始時間
        public DateTime EndTime { get; } //予定終了時間
    }
}
