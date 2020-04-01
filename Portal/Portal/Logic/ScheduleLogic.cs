using Portal.DBAccess;
using Portal.Enum;
using Portal.Info;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal
{
    /// <summary>
    /// 予定ロジック
    /// </summary>
    public class ScheduleLogic
    {
        /// <summary>
        /// 予定更新
        /// </summary>
        /// <param name="scheduleRegistrationInfo">予定登録情報</param>
        /// <param name="scheduleUpdateType">更新種別</param>
        public void UpdateSchedule(ScheduleRegistrationInfo scheduleRegistrationInfo, ScheduleUpdateType scheduleUpdateType)
        {
            new ScheduleDBAccess().UpdateSchedule(scheduleRegistrationInfo, scheduleUpdateType);
        }


        /// <summary>
        /// 予定一覧取得
        /// </summary>
        /// <returns>予定一覧リスト</returns>
        public List<string> GetScheduleType()
        {
            return new ScheduleDBAccess().GetScheduleType();
        }


        /// <summary>
        /// 施設情報取得
        /// </summary>
        /// <returns>施設情報リスト</returns>
        public List<FacilityInfo> GetFacilityInfo()
        {
            return new ScheduleDBAccess().GetFacilityInfo();
        }


        /// <summary>
        /// 全ユーザーの1週間分の予定を取得し、ユーザー1人につき1要素の形にしたListを返す
        /// </summary>
        /// <param name="firstDate">取得する予定のうち、最初の日付</param>
        public List<ScheduleDisplayInfo> GetOneWeekSchedule(DateTime firstDate)
        {
            //画面に表示する予定情報
            var scheduleDisplayInfoList = new List<ScheduleDisplayInfo>();

            //1週間分の予定を、予定1件につき1要素の形で取得
            var scheduleInfoList = new ScheduleDBAccess().GetScheduleInfo(firstDate, firstDate.AddDays(6));

            //全てのユーザーの名前とIDを取得
            var userIDAndNameList = new CommonLogic().GetUserIDAndName();

            //ユーザー1人につき1要素の形にする
            foreach (var userIDAndName in userIDAndNameList)
            {
                scheduleDisplayInfoList.Add(new ScheduleDisplayInfo(
                   userIDAndName.UserName,
                   userIDAndName.UserID,
                   scheduleInfoList.Where(x => x.UserID == userIDAndName.UserID).ToList()));
            }

            return scheduleDisplayInfoList;
        }


        /// <summary>
        /// 画面に表示するための、一週間分の予定情報を取得
        /// </summary>
        /// <param name="firstDate">取得する予定のうち、最初の日付</param>
        /// <returns>予定表示情報</returns>
        public List<FacilityDisplayInfo> GetOneWeekFacilityDisplayInfo(DateTime firstDate)
        {
            var facilityDisplayInfoList = new List<FacilityDisplayInfo>(); //施設表示情報リスト
            var scheduleDBAccess = new ScheduleDBAccess();

            //施設利用情報を取得
            var facilityUsageInfoList = scheduleDBAccess.GetFacilityUsageInfo(firstDate, firstDate.AddDays(6));

            //施設名一覧を取得
            var facilityNameInfoList = scheduleDBAccess.GetFacilityNameInfo();

            //施設を利用しない予定を削除
            facilityNameInfoList.RemoveAll(x => x.FacilityName == "施設を利用しない");

            //施設利用情報、施設名一覧をもとに、施設1つにつき1要素の形のリストを作成する
            foreach (var facilityNameInfo in facilityNameInfoList)
            {
                facilityDisplayInfoList.Add(new FacilityDisplayInfo(
                    facilityNameInfo.FacilityName,
                    facilityUsageInfoList.
                        Where(x => x.FacilityID == facilityNameInfo.FacilityID).ToList()));
            }

            return facilityDisplayInfoList;
        }


        /// <summary>
        /// ログインユーザーの登録済みの予定一覧を取得
        /// </summary>
        /// <param name="userID">ログインユーザーのユーザーID</param>
        /// <returns>登録済みの予定一覧</returns>
        public List<ScheduleInfo> GetRegisteredSchedule(string userID)
        {
            return new ScheduleDBAccess().GetRegisteredSchedule(userID);
        }
    }
}
