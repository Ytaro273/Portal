using Npgsql;
using Portal.Info;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.DBAccess
{
    /// <summary>
    /// ログインDBアクセス
    /// </summary>
    public class LoginDBAccess
    {
        /// <summary>
        /// データベースからユーザー情報を取得する
        /// </summary>
        /// <param name="inputID">入力されたユーザーID</param>
        /// <returns>取得したユーザー情報</returns>
        public UserInfo GetUserInfo(string inputID)
        {
            var userInfo = new UserInfo();

            //データベースに接続するための文字列を取得
            string connString = new CommonDBAccess().MakeAccessString();

            using (var conn = new NpgsqlConnection(connString))
            {
                    conn.Open();

                    string sql = @"select ユーザーid,名前,パスワード,ユーザーマスタ.権限id,権限マスタ.権限名 
                        from ユーザーマスタ inner join 権限マスタ 
                        on ユーザーマスタ.権限id = 権限マスタ.権限id 
                        where ユーザーid = :inputID and 削除フラグ = 0";

                    var cmd = new NpgsqlCommand(sql, conn);
                    cmd.Parameters.Add(new NpgsqlParameter("inputID", NpgsqlTypes.NpgsqlDbType.Varchar));
                    cmd.Parameters["inputID"].Value = inputID;

                    var dataReader = cmd.ExecuteReader();

                    while (dataReader.Read())
                    {
                        userInfo.UserID = (string)dataReader.GetValue(0);
                        userInfo.UserName = (string)dataReader.GetValue(1);
                        userInfo.Pass = (string)dataReader.GetValue(2);
                        userInfo.AuthorityID = (int)(decimal)dataReader.GetValue(3);
                        userInfo.AuthorityName = (string)dataReader.GetValue(4);
                    }               
            }

            return userInfo;
        }
    }
}
