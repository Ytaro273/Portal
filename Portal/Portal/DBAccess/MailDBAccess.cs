using System;
using Npgsql;
using Portal.Info;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.DBAccess
{
    class MailDBAccess
    {
        /// <summary>
        /// ログインユーザーが受信したメールを取得
        /// </summary>
        /// <param name="userID">ログインユーザーのユーザーID</param>
        /// <returns>メール情報</returns>
        public List<MailInfo> GetIncomingMail(string userID)
        {
            var mailInfoList = new List<MailInfo>();

            //データベースに接続するための文字列を取得
            string connString = new CommonDBAccess().MakeAccessString();

            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();
                
                string sql = @"select 送信者ユーザーid,名前,件名,メッセージ,メールテーブル.更新日時
                    from メールテーブル inner join ユーザーマスタ 
                    on メールテーブル.送信者ユーザーid = ユーザーマスタ.ユーザーid
                    where 受信者ユーザーid = :userID";

                var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.Add(new NpgsqlParameter("userID", NpgsqlTypes.NpgsqlDbType.Varchar));
                cmd.Parameters["userID"].Value = userID;

                var dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    mailInfoList.Add(new MailInfo()
                    {
                        UserID = (string)dataReader.GetValue(0),
                        UserName = (string)dataReader.GetValue(1),
                        Subject = (string)dataReader.GetValue(2),
                        Message = (string)dataReader.GetValue(3),
                        AddedDate = (DateTime)dataReader.GetValue(4)
                    });                       
                }
            }

            return mailInfoList;
        }


        /// <summary>
        /// ログインユーザーが送信したメールを取得
        /// </summary>
        /// <param name="userID">ログインユーザーのユーザーID</param>
        /// <returns>メール情報</returns>
        public List<MailInfo> GetTransmittedMail(string userID)
        {
            var mailInfoList = new List<MailInfo>();

            //データベースに接続するための文字列を取得
            string connString = new CommonDBAccess().MakeAccessString();

            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                string sql = @"select 受信者ユーザーid,名前,件名,メッセージ,メールテーブル.更新日時
                    from メールテーブル inner join ユーザーマスタ
                    on メールテーブル.受信者ユーザーid = ユーザーマスタ.ユーザーid
                    where 送信者ユーザーid = :userID";

                var cmd = new NpgsqlCommand(sql, conn);
                cmd.Parameters.Add(new NpgsqlParameter("userID", NpgsqlTypes.NpgsqlDbType.Varchar));
                cmd.Parameters["userID"].Value = userID;

                var dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    mailInfoList.Add(new MailInfo()
                    {
                        UserID = (string)dataReader.GetValue(0),
                        UserName = (string)dataReader.GetValue(1),
                        Subject = (string)dataReader.GetValue(2),
                        Message = (string)dataReader.GetValue(3),
                        AddedDate = (DateTime)dataReader.GetValue(4)
                    });
                }
            }

            return mailInfoList;
        }


        /// <summary>
        /// メール送信
        /// </summary>
        /// <param name="userID">ログインユーザーのユーザーID</param>
        /// <param name="mailInfo">送信するメールの情報</param>
        public void SendMail(string userID, List<MailInfo> mailInfoList)
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
                        foreach (var mailInfo in mailInfoList)
                        {
                            string sql = @"insert into メールテーブル values (:senderUserID,:receiverUserID,:Subject,:Message)";

                            var cmd = new NpgsqlCommand(sql, conn);
                            cmd.Parameters.Add(new NpgsqlParameter("senderUserID", NpgsqlTypes.NpgsqlDbType.Varchar));
                            cmd.Parameters["senderUserID"].Value = userID;
                            cmd.Parameters.Add(new NpgsqlParameter("receiverUserID", NpgsqlTypes.NpgsqlDbType.Varchar));
                            cmd.Parameters["receiverUserID"].Value = mailInfo.UserID;
                            cmd.Parameters.Add(new NpgsqlParameter("Subject", NpgsqlTypes.NpgsqlDbType.Text));
                            cmd.Parameters["Subject"].Value = mailInfo.Subject;
                            cmd.Parameters.Add(new NpgsqlParameter("Message", NpgsqlTypes.NpgsqlDbType.Text));
                            cmd.Parameters["Message"].Value = mailInfo.Message;

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
