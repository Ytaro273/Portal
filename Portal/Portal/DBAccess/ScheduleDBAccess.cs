using Npgsql;
using Portal.Enum;
using Portal.Info;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.DBAccess
{
    public class ScheduleDBAccess
    {
        /// <summary>
        /// 予定更新
        /// </summary>
        /// <param name="scheduleRegistrationInfo">予定登録情報</param>
        /// <param name="scheduleUpdateType">更新種別</param>
        public void UpdateSchedule(ScheduleRegistrationInfo scheduleRegistrationInfo, ScheduleUpdateType scheduleUpdateType)
        {
            string sql; //実行するsql文

            //データベースに接続するための文字列を取得
            string connString = new CommonDBAccess().MakeAccessString();

            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                //トランザクション開始
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        switch (scheduleUpdateType)
                        {
                            //新規予定追加の場合
                            case ScheduleUpdateType.Add:

                                //予定情報を予定テーブルに追加
                                sql = @"insert into 予定テーブル(ユーザーid, 予定内容, 開始日時, 終了日時)
                                values(:UserID, :ScheduleContents, :StartTime, :EndTime)";

                                var cmd = new NpgsqlCommand(sql, conn);
                                cmd.Parameters.Add(new NpgsqlParameter("UserID", NpgsqlTypes.NpgsqlDbType.Varchar));
                                cmd.Parameters["UserID"].Value = scheduleRegistrationInfo.UserID;
                                cmd.Parameters.Add(new NpgsqlParameter("ScheduleContents", NpgsqlTypes.NpgsqlDbType.Varchar));
                                cmd.Parameters["ScheduleContents"].Value = scheduleRegistrationInfo.ScheduleContents;
                                cmd.Parameters.Add(new NpgsqlParameter("StartTime", NpgsqlTypes.NpgsqlDbType.Timestamp));
                                cmd.Parameters["StartTime"].Value = scheduleRegistrationInfo.StartTime;
                                cmd.Parameters.Add(new NpgsqlParameter("EndTime", NpgsqlTypes.NpgsqlDbType.Timestamp));
                                cmd.Parameters["EndTime"].Value = scheduleRegistrationInfo.EndTime;

                                cmd.ExecuteNonQuery();


                                //施設利用情報を施設利用状況テーブルに追加
                                sql = @"insert into 施設利用状況テーブル select 
                                    :UserID, 
                                    :FacilityID,    
                                    予定id from 予定テーブル where 
                                        ユーザーid = :UserID and
                                        開始日時 = :StartTime and
                                        終了日時 = :EndTime";

                                cmd = new NpgsqlCommand(sql, conn);
                                cmd.Parameters.Add(new NpgsqlParameter("UserID", NpgsqlTypes.NpgsqlDbType.Varchar));
                                cmd.Parameters["UserID"].Value = scheduleRegistrationInfo.UserID;
                                cmd.Parameters.Add(new NpgsqlParameter("FacilityID", NpgsqlTypes.NpgsqlDbType.Smallint));
                                cmd.Parameters["FacilityID"].Value = scheduleRegistrationInfo.FacilityID;
                                cmd.Parameters.Add(new NpgsqlParameter("StartTime", NpgsqlTypes.NpgsqlDbType.Timestamp));
                                cmd.Parameters["StartTime"].Value = scheduleRegistrationInfo.StartTime;
                                cmd.Parameters.Add(new NpgsqlParameter("EndTime", NpgsqlTypes.NpgsqlDbType.Timestamp));
                                cmd.Parameters["EndTime"].Value = scheduleRegistrationInfo.EndTime;

                                cmd.ExecuteNonQuery();

                                break;


                            //予定編集の場合
                            case ScheduleUpdateType.Edit:

                                //編集前のデータを予定テーブルから削除
                                sql = @"delete from 予定テーブル where 予定id = :ScheduleID";

                                cmd = new NpgsqlCommand(sql, conn);
                                cmd.Parameters.Add(new NpgsqlParameter("ScheduleID", NpgsqlTypes.NpgsqlDbType.Smallint));
                                cmd.Parameters["ScheduleID"].Value = scheduleRegistrationInfo.ScheduleID;

                                cmd.ExecuteNonQuery();


                                //編集前のデータを施設利用状況テーブルテーブルから削除
                                sql = @"delete from 施設利用状況テーブル where 予定id = :ScheduleID";

                                cmd = new NpgsqlCommand(sql, conn);
                                cmd.Parameters.Add(new NpgsqlParameter("ScheduleID", NpgsqlTypes.NpgsqlDbType.Smallint));
                                cmd.Parameters["ScheduleID"].Value = scheduleRegistrationInfo.ScheduleID;

                                cmd.ExecuteNonQuery();


                                //編集後のデータを予定テーブルに追加
                                sql = @"insert into 予定テーブル(ユーザーid, 予定内容, 開始日時, 終了日時)
                                values(:UserID, :ScheduleContents, :StartTime, :EndTime)";

                                cmd = new NpgsqlCommand(sql, conn);
                                cmd.Parameters.Add(new NpgsqlParameter("UserID", NpgsqlTypes.NpgsqlDbType.Varchar));
                                cmd.Parameters["UserID"].Value = scheduleRegistrationInfo.UserID;
                                cmd.Parameters.Add(new NpgsqlParameter("ScheduleContents", NpgsqlTypes.NpgsqlDbType.Varchar));
                                cmd.Parameters["ScheduleContents"].Value = scheduleRegistrationInfo.ScheduleContents;
                                cmd.Parameters.Add(new NpgsqlParameter("StartTime", NpgsqlTypes.NpgsqlDbType.Timestamp));
                                cmd.Parameters["StartTime"].Value = scheduleRegistrationInfo.StartTime;
                                cmd.Parameters.Add(new NpgsqlParameter("EndTime", NpgsqlTypes.NpgsqlDbType.Timestamp));
                                cmd.Parameters["EndTime"].Value = scheduleRegistrationInfo.EndTime;

                                cmd.ExecuteNonQuery();


                                //編集後のデータを施設利用状況テーブルに追加
                                sql = @"insert into 施設利用状況テーブル select 
                                    :UserID, 
                                    :FacilityID,    
                                    予定id from 予定テーブル where 
                                        ユーザーid = :UserID and
                                        開始日時 = :StartTime and
                                        終了日時 = :EndTime";

                                cmd = new NpgsqlCommand(sql, conn);
                                cmd.Parameters.Add(new NpgsqlParameter("UserID", NpgsqlTypes.NpgsqlDbType.Varchar));
                                cmd.Parameters["UserID"].Value = scheduleRegistrationInfo.UserID;
                                cmd.Parameters.Add(new NpgsqlParameter("FacilityID", NpgsqlTypes.NpgsqlDbType.Smallint));
                                cmd.Parameters["FacilityID"].Value = scheduleRegistrationInfo.FacilityID;
                                cmd.Parameters.Add(new NpgsqlParameter("StartTime", NpgsqlTypes.NpgsqlDbType.Timestamp));
                                cmd.Parameters["StartTime"].Value = scheduleRegistrationInfo.StartTime;
                                cmd.Parameters.Add(new NpgsqlParameter("EndTime", NpgsqlTypes.NpgsqlDbType.Timestamp));
                                cmd.Parameters["EndTime"].Value = scheduleRegistrationInfo.EndTime;

                                cmd.ExecuteNonQuery();

                                break;


                            //予定削除の場合
                            case ScheduleUpdateType.Delete:
                                //データを予定テーブルから削除
                                sql = @"delete from 予定テーブル where 予定id = :ScheduleID";

                                cmd = new NpgsqlCommand(sql, conn);
                                cmd.Parameters.Add(new NpgsqlParameter("ScheduleID", NpgsqlTypes.NpgsqlDbType.Smallint));
                                cmd.Parameters["ScheduleID"].Value = scheduleRegistrationInfo.ScheduleID;

                                cmd.ExecuteNonQuery();


                                //データを施設利用状況テーブルテーブルから削除
                                sql = @"delete from 施設利用状況テーブル where 予定id = :ScheduleID";

                                cmd = new NpgsqlCommand(sql, conn);
                                cmd.Parameters.Add(new NpgsqlParameter("ScheduleID", NpgsqlTypes.NpgsqlDbType.Smallint));
                                cmd.Parameters["ScheduleID"].Value = scheduleRegistrationInfo.ScheduleID;

                                cmd.ExecuteNonQuery();
                                break;
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


        /// <summary>
        /// 全ユーザーの1週間分の予定を取得
        /// </summary>
        /// <param name="firstDate">取得する予定のうち、最初の日付</param>
        /// <param name="lastDate">取得する予定のうち、最後の日付</param>
        /// <returns>予定リスト</returns>
        public List<ScheduleInfo> GetScheduleInfo(DateTime firstDate, DateTime lastDate)
        {
            var scheduleInfoList = new List<ScheduleInfo>();

            //データベースに接続するための文字列を取得
            string connString = new CommonDBAccess().MakeAccessString();

            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                string sql = @"select 予定id,ユーザーid,予定内容,開始日時,終了日時 from 予定テーブル 
                    where exists (select 1 from ユーザーマスタ where 予定テーブル.ユーザーid = ユーザーマスタ.ユーザーid and 削除フラグ = 0)
                    and cast(開始日時 as date) >= :firstDate.Date 
                    and cast(開始日時 as date) <= :lastDate.Date";

                var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.Add(new NpgsqlParameter("firstDate.Date", NpgsqlTypes.NpgsqlDbType.Timestamp));
                cmd.Parameters["firstDate.Date"].Value = firstDate.Date;
                cmd.Parameters.Add(new NpgsqlParameter("lastDate.Date", NpgsqlTypes.NpgsqlDbType.Timestamp));
                cmd.Parameters["lastDate.Date"].Value = lastDate.Date;

                var dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    scheduleInfoList.Add(new ScheduleInfo(
                        (short)dataReader.GetValue(0),
                        (string)dataReader.GetValue(1),
                        (string)dataReader.GetValue(2),
                        (DateTime)dataReader.GetValue(3),
                        (DateTime)dataReader.GetValue(4)));
                }
            }

            return scheduleInfoList;
        }


        /// <summary>
        /// 予定の種類の一覧を取得
        /// </summary>
        /// <returns>予定一覧</returns>
        public List<string> GetScheduleType()
        {
            var scheduleTypeList = new List<string>();

            //データベースに接続するための文字列を取得
            string connString = new CommonDBAccess().MakeAccessString();

            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                string sql = @"select distinct 予定内容 from 予定テーブル";
                var cmd = new NpgsqlCommand(sql, conn);

                var dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    scheduleTypeList.Add((string)dataReader.GetValue(0));
                }
            }

            return scheduleTypeList;
        }


        /// <summary>
        /// 施設情報取得
        /// </summary>
        /// <returns>施設情報リスト</returns>
        public List<FacilityInfo> GetFacilityInfo()
        {
            var facilityInfoList = new List<FacilityInfo>();

            //データベースに接続するための文字列を取得
            string connString = new CommonDBAccess().MakeAccessString();

            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                string sql = @"select 施設id,施設名,開放開始時間,開放終了時間 from 施設テーブル";
                var cmd = new NpgsqlCommand(sql, conn);

                var dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    facilityInfoList.Add(new FacilityInfo(
                        (short)dataReader.GetValue(0),
                        (string)dataReader.GetValue(1),
                        (DateTime)dataReader.GetValue(2),
                        (DateTime)dataReader.GetValue(3)));
                }
            }

            return facilityInfoList;
        }


        /// <summary>
        /// 施設名一覧取得
        /// </summary>
        /// <returns>施設名一覧</returns>
        public List<FacilityNameInfo> GetFacilityNameInfo()
        {
            var facilityNameInfoList = new List<FacilityNameInfo>();

            //データベースに接続するための文字列を取得
            string connString = new CommonDBAccess().MakeAccessString();

            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                string sql = @"select 施設id,施設名 from 施設テーブル";
                var cmd = new NpgsqlCommand(sql, conn);

                var dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    facilityNameInfoList.Add(new FacilityNameInfo(
                        (short)dataReader.GetValue(0),
                        (string)dataReader.GetValue(1)));
                }
            }

            return facilityNameInfoList;
        }


        /// <summary>
        /// 施設利用情報取得
        /// </summary>
        /// <param name="firstDate">取得する施設利用情報のうち、最初の日付</param>
        /// <param name="lastDate">取得する施設利用情報のうち、最後の日付</param>
        /// <returns>施設利用情報リスト</returns>
        public List<FacilityUsageInfo> GetFacilityUsageInfo(DateTime firstDate, DateTime lastDate)
        {
            var facilityUsageInfoList = new List<FacilityUsageInfo>();

            //データベースに接続するための文字列を取得
            string connString = new CommonDBAccess().MakeAccessString();

            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                string sql = @"select 施設id,開始日時,終了日時 
                    from 施設利用状況テーブル inner join 予定テーブル
                    on 施設利用状況テーブル.予定id = 予定テーブル.予定id
                    where cast(開始日時 as date) >= :firstDate.Date 
                    and cast(開始日時 as date) <= :lastDate.Date";

                var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.Add(new NpgsqlParameter("firstDate.Date", NpgsqlTypes.NpgsqlDbType.Timestamp));
                cmd.Parameters["firstDate.Date"].Value = firstDate.Date;
                cmd.Parameters.Add(new NpgsqlParameter("lastDate.Date", NpgsqlTypes.NpgsqlDbType.Timestamp));
                cmd.Parameters["lastDate.Date"].Value = lastDate.Date;

                var dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    facilityUsageInfoList.Add(new FacilityUsageInfo(
                        (short)dataReader.GetValue(0),
                        (DateTime)dataReader.GetValue(1),
                        (DateTime)dataReader.GetValue(2)));
                }

            }

            return facilityUsageInfoList;
        }


        /// <summary>
        /// ログインユーザーの登録済みの予定一覧を、本日以降の予定のもののみ取得する
        /// </summary>
        /// <param name="userID">ログインユーザーのユーザーID</param>
        /// <returns>登録済みの予定一覧</returns>
        public List<ScheduleInfo> GetRegisteredSchedule(string userID)
        {
            var registeredScheduleList = new List<ScheduleInfo>();

            //データベースに接続するための文字列を取得
            string connString = new CommonDBAccess().MakeAccessString();

            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                string sql = @"select 予定id, ユーザーid, 予定内容, 開始日時, 終了日時 from 予定テーブル
                    where ユーザーid = :userID 
                    and cast(開始日時 as date) >= :DateTime.Now.Date";

                var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.Add(new NpgsqlParameter("userID", NpgsqlTypes.NpgsqlDbType.Varchar));
                cmd.Parameters["userID"].Value = userID;
                cmd.Parameters.Add(new NpgsqlParameter("DateTime.Now.Date", NpgsqlTypes.NpgsqlDbType.Timestamp));
                cmd.Parameters["DateTime.Now.Date"].Value = DateTime.Now.Date;

                var dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    registeredScheduleList.Add(new ScheduleInfo(
                        (short)dataReader.GetValue(0),
                        (string)dataReader.GetValue(1),
                        (string)dataReader.GetValue(2),
                        (DateTime)dataReader.GetValue(3),
                        (DateTime)dataReader.GetValue(4)));
                }

            }

            return registeredScheduleList;
        }
    }
}
