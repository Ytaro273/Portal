using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Info
{
    /// <summary>
    /// 施設利用情報
    /// </summary>
    public class FacilityUsageInfo
    {
        //コンストラクタ
        public FacilityUsageInfo(short facilityID, DateTime startTime, DateTime endTime)
        {
            this.FacilityID = facilityID;
            this.StartTime = startTime;
            this.EndTime = endTime;
        }

        public short FacilityID { get;} //施設ID
        public DateTime StartTime { get; } //利用開始時間
        public DateTime EndTime { get; } //利用終了時間
    }
}
