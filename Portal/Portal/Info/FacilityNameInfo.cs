using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Info
{
    /// <summary>
    /// 施設名情報
    /// </summary>
    public class FacilityNameInfo
    {
        //コンストラクタ
        public FacilityNameInfo(short facilityID, string facilityName)
        {
            this.FacilityID = facilityID;
            this.FacilityName = facilityName;
        }

        public short FacilityID { get; } //施設ID
        public string FacilityName { get; } //施設名
    }
}
