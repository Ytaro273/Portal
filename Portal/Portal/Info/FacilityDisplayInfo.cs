using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Info
{
    /// <summary>
    /// 施設表示情報
    /// </summary>
    public class FacilityDisplayInfo
    {
        //コンストラクタ
        public FacilityDisplayInfo(string facilityName, List<FacilityUsageInfo> facilityUsageInfoList)
        {
            this.FacilityName = facilityName;
            this.FacilityUsageInfoList = facilityUsageInfoList;
        }

        public string FacilityName { get; } //施設名
        public List<FacilityUsageInfo> FacilityUsageInfoList { get; } //施設利用情報
    }
}
