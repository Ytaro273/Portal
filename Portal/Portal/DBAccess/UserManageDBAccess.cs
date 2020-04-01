using Npgsql;
using System;
using Portal.Info;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.DBAccess
{
    /// <summary>
    /// ユーザー管理DBアクセス
    /// </summary>
    class UserManageDBAccess
    {
        /// <summary>
        /// 一般ユーザー情報取得
        /// </summary>
        /// <returns>一般ユーザー情報リスト</returns>
        public List<UserInfo> GetGeneralUserInfo()
        {
            var GeneralUserInfoList = new List<UserInfo>();

            //データベースに接続するための文字列を取得
            string connString = new CommonDBAccess().MakeAccessString();

            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                string sql = @"select ユーザーid,名前,パスワード,ユーザーマスタ.権限id,権限マスタ.権限名 
                        from ユーザーマスタ inner join 権限マスタ 
                        on ユーザーマスタ.権限id = 権限マスタ.権限id 
                        where ユーザーマスタ.権限id = 0 and 削除フラグ = 0";

                var cmd = new NpgsqlCommand(sql, conn);

                var dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    GeneralUserInfoList.Add(new UserInfo()
                    {
                        UserID = (string)dataReader.GetValue(0),
                        UserName = (string)dataReader.GetValue(1),
                        Pass = (string)dataReader.GetValue(2),
                        AuthorityID = (int)(decimal)dataReader.GetValue(3),
                        AuthorityName = (string)dataReader.GetValue(4)
                    });
                }
            }

            return GeneralUserInfoList;
        }


        /// <summary>
        /// ユーザー追加
        /// </summary>
        /// <param name="userInfoList">追加するユーザーの情報</param>
        public void InsertUser(List<UserInfo> userInfoList)
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
                        foreach (var userInfo in userInfoList)
                        {
                            string sql = @"insert into ユーザーマスタ values (:UserID,:UserName,:Pass,:AuthorityID,0)";

                            var cmd = new NpgsqlCommand(sql, conn);
                            cmd.Parameters.Add(new NpgsqlParameter("UserID", NpgsqlTypes.NpgsqlDbType.Varchar));
                            cmd.Parameters["UserID"].Value = userInfo.UserID;
                            cmd.Parameters.Add(new NpgsqlParameter("UserName", NpgsqlTypes.NpgsqlDbType.Text));
                            cmd.Parameters["UserName"].Value = userInfo.UserName;
                            cmd.Parameters.Add(new NpgsqlParameter("Pass", NpgsqlTypes.NpgsqlDbType.Text));
                            cmd.Parameters["Pass"].Value = userInfo.Pass;
                            cmd.Parameters.Add(new NpgsqlParameter("AuthorityID", NpgsqlTypes.NpgsqlDbType.Numeric));
                            cmd.Parameters["AuthorityID"].Value = userInfo.AuthorityID;

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


        /// <summary>
        /// ユーザー情報更新
        /// </summary>
        /// <param name="userInfoList">ユーザー情報</param>
        public void UpdateUserInfo(List<UserInfo> userInfoList)
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
                        foreach (var userInfo in userInfoList)
                        {
                            //パスワードを更新しない場合
                            if (userInfo.Pass == "")
                            {
                                string sql = @"update ユーザーマスタ set 名前 = :UserName, 権限id = :AuthorityID
                                    where ユーザーid = :UserID";

                                var cmd = new NpgsqlCommand(sql, conn);
                                cmd.Parameters.Add(new NpgsqlParameter("UserID", NpgsqlTypes.NpgsqlDbType.Varchar));
                                cmd.Parameters["UserID"].Value =userInfo.UserID;
                                cmd.Parameters.Add(new NpgsqlParameter("UserName", NpgsqlTypes.NpgsqlDbType.Varchar));
                                cmd.Parameters["UserName"].Value = userInfo.UserName;
                                cmd.Parameters.Add(new NpgsqlParameter("AuthorityID", NpgsqlTypes.NpgsqlDbType.Numeric));
                                cmd.Parameters["AuthorityID"].Value = userInfo.AuthorityID;

                                cmd.ExecuteNonQuery();
                            }
                            //パスワードを更新する場合
                            else 
                            {
                                string sql = @"update ユーザーマスタ set 名前 = :UserName, 権限id = :AuthorityID, パスワード = :Pass
                                    where ユーザーid = :UserID";

                                var cmd = new NpgsqlCommand(sql, conn);
                                cmd.Parameters.Add(new NpgsqlParameter("UserID", NpgsqlTypes.NpgsqlDbType.Varchar));
                                cmd.Parameters["UserID"].Value = userInfo.UserID;
                                cmd.Parameters.Add(new NpgsqlParameter("UserName", NpgsqlTypes.NpgsqlDbType.Text));
                                cmd.Parameters["UserName"].Value = userInfo.UserName;
                                cmd.Parameters.Add(new NpgsqlParameter("AuthorityID", NpgsqlTypes.NpgsqlDbType.Numeric));
                                cmd.Parameters["AuthorityID"].Value = userInfo.AuthorityID;
                                cmd.Parameters.Add(new NpgsqlParameter("Pass", NpgsqlTypes.NpgsqlDbType.Text));
                                cmd.Parameters["Pass"].Value = userInfo.Pass;

                                cmd.ExecuteNonQuery();
                            }
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
        /// ユーザー削除フラグ更新
        /// </summary>
        /// <param name="userID">ユーザーID</param>
        /// <param name="deleteFlag">削除フラグ</param>
        public void UpdateUserDeleteFlag(string userID, int deleteFlag)
        {
            //データベースに接続するための文字列を取得
            string connString = new CommonDBAccess().MakeAccessString();

            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                string sql = @"update ユーザーマスタ set 削除フラグ = :deleteFlag
                    where ユーザーid = :userID";

                var cmd = new NpgsqlCommand(sql, conn);

                cmd.Parameters.Add(new NpgsqlParameter(":deleteFlag", NpgsqlTypes.NpgsqlDbType.Numeric));
                cmd.Parameters["deleteFlag"].Value = deleteFlag;
                cmd.Parameters.Add(new NpgsqlParameter("userID", NpgsqlTypes.NpgsqlDbType.Varchar));
                cmd.Parameters["userID"].Value = userID;

                cmd.ExecuteNonQuery();
            }
        }
    }
}
