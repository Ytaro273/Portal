using Npgsql;
using Portal.Enum;
using Portal.Info;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.DBAccess
{
    /// <summary>
    /// 勤怠DBアクセス
    /// </summary>
    public class AttendanceDBAccess
    {
        /// <summary>
        /// データベースから勤務時間情報を取得
        /// </summary>
        /// <param name="userID">ログインユーザーのユーザーID</param>
        /// <param name="attendanceMonth">何月の情報を取得するか</param>
        /// <returns>勤務時間情報リスト</returns>
        public List<AttendanceTimeInfo> GetAttendanceTimeInfo(string userID, AttendanceMonth attendanceMonth)
        {
            var attendanceTimeInfoList = new List<AttendanceTimeInfo>(); //勤務時間情報

            //データベースに接続するための文字列を取得
            string connString = new CommonDBAccess().MakeAccessString();

            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                string sql = @"select 勤務開始時間,勤務終了時間,休憩時間 
                    from 勤怠テーブル 
                    where ユーザーid = :userID 
                    and extract(year from 勤務開始時間) = :DateTime.Now.Year 
                    and extract(month from 勤務開始時間) = :attendanceMonth 
                    order by 勤務開始時間";

                var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.Add(new NpgsqlParameter("userID", NpgsqlTypes.NpgsqlDbType.Varchar));
                cmd.Parameters["userID"].Value = userID;
                cmd.Parameters.Add(new NpgsqlParameter("attendanceMonth", NpgsqlTypes.NpgsqlDbType.Integer));
                cmd.Parameters["attendanceMonth"].Value = (int)attendanceMonth;
                cmd.Parameters.Add(new NpgsqlParameter("DateTime.Now.Year", NpgsqlTypes.NpgsqlDbType.Integer));
                cmd.Parameters["DateTime.Now.Year"].Value = DateTime.Now.Year;

                var dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    attendanceTimeInfoList.Add(new AttendanceTimeInfo(
                            (DateTime)dataReader.GetValue(0),
                            (DateTime)dataReader.GetValue(1),
                            (int)(decimal)dataReader.GetValue(2)));
                }
            }

            return attendanceTimeInfoList;
        }


        /// <summary>
        /// 勤務時間情報更新
        /// </summary>
        /// <param name="userID">ログインユーザーのユーザーID</param>
        /// <param name="attendanceTimeInfoList">勤務時間情報リスト</param>
        public void UpdateAttendanceTimeInfo(string userID, List<AttendanceTimeInfo> attendanceTimeInfoList)
        {
            //データベースに接続するための文字列を取得
            string connString = new CommonDBAccess().MakeAccessString();

            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        //更新する年および月と一致する勤務時間情報を削除
                        string sql = @"delete from 勤怠テーブル 
                            where ユーザーid = :userID and 
                            extract(year from 勤務開始時間) = :year and 
                            extract(month from 勤務開始時間) = :month";
                        
                        var cmd = new NpgsqlCommand(sql, conn);
                        cmd.Parameters.Add(new NpgsqlParameter("userID", NpgsqlTypes.NpgsqlDbType.Varchar));
                        cmd.Parameters["userID"].Value = userID;
                        cmd.Parameters.Add(new NpgsqlParameter("year", NpgsqlTypes.NpgsqlDbType.Integer));
                        cmd.Parameters["year"].Value = attendanceTimeInfoList[0].StartTime.Year;
                        cmd.Parameters.Add(new NpgsqlParameter("month", NpgsqlTypes.NpgsqlDbType.Integer));
                        cmd.Parameters["month"].Value = attendanceTimeInfoList[0].StartTime.Month;

                        cmd.ExecuteNonQuery();


                        //勤務時間情報をデータベースに追加
                        foreach (var attendanceTime in attendanceTimeInfoList)
                        {
                            sql = @"insert into 勤怠テーブル 
                                values(:userID, :attendanceTime.StartTime, :attendanceTime.EndTime, :attendanceTime.BreakTime)";
                            
                            cmd = new NpgsqlCommand(sql, conn);
                            cmd.Parameters.Add(new NpgsqlParameter("userID", NpgsqlTypes.NpgsqlDbType.Varchar));
                            cmd.Parameters["userID"].Value = userID;
                            cmd.Parameters.Add(new NpgsqlParameter("attendanceTime.StartTime", NpgsqlTypes.NpgsqlDbType.Timestamp));
                            cmd.Parameters["attendanceTime.StartTime"].Value = attendanceTime.StartTime;
                            cmd.Parameters.Add(new NpgsqlParameter("attendanceTime.EndTime", NpgsqlTypes.NpgsqlDbType.Timestamp));
                            cmd.Parameters["attendanceTime.EndTime"].Value = attendanceTime.EndTime;
                            cmd.Parameters.Add(new NpgsqlParameter("attendanceTime.BreakTime", NpgsqlTypes.NpgsqlDbType.Numeric));
                            cmd.Parameters["attendanceTime.BreakTime"].Value = attendanceTime.BreakTime;

                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch (NpgsqlException)
                    {
                        transaction.Rollback();                       
                        throw;
                    }
                }
            }
        }
    }
}
