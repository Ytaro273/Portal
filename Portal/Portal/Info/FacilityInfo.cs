using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal
{
    /// <summary>
    /// 施設情報
    /// </summary>
    public class FacilityInfo
    {
        //コンストラクタ
        public FacilityInfo(short facilityID, string facilityName, DateTime openingStartTime, DateTime OpeningEndTIme)
        {
            this.FacilityID = facilityID;
            this.FacilityName = facilityName;
            this.OpeningStartTime = openingStartTime;
            this.OpeningEndTime = OpeningEndTIme;
        }

        public short FacilityID { get; } //施設ID
        public string FacilityName { get;} //施設名
        public DateTime OpeningStartTime { get; } //開放開始時間
        public DateTime OpeningEndTime { get; } //開放終了時間
    }
}
