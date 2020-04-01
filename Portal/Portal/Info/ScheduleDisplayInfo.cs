using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Info
{
    /// <summary>
    /// 予定フォームで、表に表示するための予定の情報
    /// 特定のユーザー1名の名前とID、そのユーザーの予定の情報を持つ
    /// </summary>
    public class ScheduleDisplayInfo
    {
        //コンストラクタ
        public ScheduleDisplayInfo(string userName, string userID, List<ScheduleInfo> scheduleInfoList)
        {
            this.UserName = userName;
            this.UserID = userID;
            this.ScheduleInfoList = scheduleInfoList;
        }

        public string UserName { get; } //ユーザー名
        public string UserID { get; } //ユーザーID
        public List<ScheduleInfo> ScheduleInfoList { get; } //予定情報
    }
}
