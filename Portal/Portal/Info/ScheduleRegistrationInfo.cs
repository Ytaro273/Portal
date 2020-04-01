using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Info
{
    /// <summary>
    /// 予定登録情報
    /// 予定を新たに追加する際に用いる
    /// </summary>
    public class ScheduleRegistrationInfo
    {
        public short ScheduleID { get; set; } //予定ID
        public short FacilityID { get; set; } //施設ID
        public string UserID { get; set; } //ユーザーID
        public string ScheduleContents { get; set; } //予定内容
        public DateTime StartTime { get; set; } //開始時間
        public DateTime EndTime { get; set; } //終了時間

    }
}
