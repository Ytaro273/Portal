using Npgsql;
using Portal.DBAccess;
using Portal.Enum;
using Portal.Info;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Logic
{
    /// <summary>
    /// 勤怠ロジック
    /// </summary>
    public class AttendanceLogic
    {
        /// <summary>
        /// 勤怠情報を取得
        /// </summary>
        /// <param name="userID">ログインユーザーのユーザーID</param>
        /// <param name="attendanceMonth">何月の情報を取得するか</param>
        /// <returns>勤怠情報リスト</returns>
        public List<AttendanceInfo> GetAttendanceInfo(string userID, AttendanceMonth attendanceMonth)
        {
            const int regularWorkingMinutes = 480; //所定労働時間を480分（8時間）とする
            var attendanceTimeInfoList = new List<AttendanceTimeInfo>(); //勤務時間情報リスト
            var attendanceInfoList = new List<AttendanceInfo>(); //勤怠情報リスト

            //データベースから勤務時間情報を取得
　　　　　   attendanceTimeInfoList = new AttendanceDBAccess().GetAttendanceTimeInfo(userID, attendanceMonth);          

            //勤務時間情報を元に、勤怠情報リストを作成
            foreach (var attendanceTimeInfo in attendanceTimeInfoList)
            {
                //勤務開始から勤務終了までの時間を算出
                TimeSpan ts = attendanceTimeInfo.EndTime - attendanceTimeInfo.StartTime;

                //労働時間を算出
                int workingTime = (int)ts.TotalMinutes - attendanceTimeInfo.BreakTime;

                //残業時間を算出
                int overTime = workingTime - regularWorkingMinutes;
                if (overTime < 0) overTime = 0;

                //リストに要素を追加
                attendanceInfoList.Add(new AttendanceInfo(
                    attendanceTimeInfo.StartTime.Day,
                    attendanceTimeInfo.StartTime.
                        ToString("ddd", System.Globalization.CultureInfo.GetCultureInfo("ja-JP")),
                    attendanceTimeInfo,
                    overTime
                    ));
            }

            return attendanceInfoList;
        }


        /// <summary>
        /// 勤務時間情報更新
        /// </summary>
        /// <param name="userID">ログインユーザーのユーザーID</param>
        /// <param name="attendanceTimeInfoList">勤務時間情報リスト</param>
        public void UpdateAttendanceTimeInfo(string userID, List<AttendanceTimeInfo> attendanceTimeInfoList)
        {               
            new AttendanceDBAccess().UpdateAttendanceTimeInfo(userID, attendanceTimeInfoList);
        }
    }
}
