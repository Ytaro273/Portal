using Npgsql;
using Portal.Enum;
using Portal.Info;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace Portal.DBAccess
{
    /// <summary>
    /// 共通DBアクセス
    /// </summary>
    public class CommonDBAccess
    {
        /// <summary>
        /// データベース接続用の文字列生成
        /// </summary>
        /// <returns>生成した文字列</returns>
        public string MakeAccessString()
        {
            string appPath = System.Reflection.Assembly.GetExecutingAssembly().Location;

            appPath = appPath.Replace("Portal.exe", "..\\..\\..\\test.xml");

            XElement xml = XElement.Load(appPath);

            IEnumerable<XElement> infos = from item in xml.Elements("ユーザー")
                                          select item;

            var sb = new StringBuilder();

            foreach (var info in infos)
            {
                sb.Append("Server=" + info.Element("IPアドレス").Value + ";");
                sb.Append("Port=" + info.Element("ポート番号").Value + ";");
                sb.Append("User Id=" + info.Element("ID").Value + ";");
                sb.Append("Password=" + info.Element("パスワード").Value + ";");
                sb.Append("Database=" + info.Element("データベース名").Value);
            }

            return sb.ToString();
        }


        /// <summary>
        /// 全てのユーザーの名前とIDを取得
        /// </summary>
        /// <returns>ユーザー名とユーザーIDのリスト</returns>
        public List<UserIDAndName> GetUserIDAndName()
        {
            var UserIDAndNameList = new List<UserIDAndName>();

            //データベースに接続するための文字列を取得
            string connString = MakeAccessString();

            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                string sql = @"select ユーザーid,名前 from ユーザーマスタ where 削除フラグ = 0";
                var cmd = new NpgsqlCommand(sql, conn);

                var dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    UserIDAndNameList.Add(new UserIDAndName(
                        (string)dataReader.GetValue(0),
                        (string)dataReader.GetValue(1)));
                }
            }

            return UserIDAndNameList;
        }
    }
}