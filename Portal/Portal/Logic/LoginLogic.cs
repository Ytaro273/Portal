using BCrypt.Net;
using Portal.DBAccess;
using Portal.Enum;
using Portal.Info;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace Portal
{
    /// <summary>
    /// ログインロジック
    /// </summary>
    public class LoginLogic
    {
        public UserInfo UserInfo; //ユーザー情報

        /// <summary>
        /// IDとパスワードが一致するかどうかをチェック
        /// </summary>
        /// <param name="inputID">入力されたID</param>
        /// <param name="inputPass">入力されたパスワード</param>
        /// <returns>チェックの結果</returns>
        public bool UserCheck(string inputID, string inputPass)
        {
            //ユーザー情報取得
            this.UserInfo = new LoginDBAccess().GetUserInfo(inputID);

            if (this.UserInfo.UserID == null) return false;

            //パスワードが正解かどうかチェック
            if (CheckPass(inputPass, this.UserInfo.Pass)) return true;
            else return false;
        }


        /// <summary>
        /// 入力されたパスワードが正解かどうかチェック
        /// </summary>
        /// <param name="inputPass">入力されたパスワード</param>
        /// <param name="hashedPass">正解のパスワード（ハッシュ化されている）</param>
        /// <returns>チェックの結果</returns>
        private bool CheckPass(string inputPass, string hashedPass)
        {
            try
            {
                //入力されたパスワードが一致するかをチェック
                if (BCrypt.Net.BCrypt.Verify(inputPass, hashedPass)) return true; 
                else return false;
            }
            //データベースに登録してあるパスワードがハッシュ化されていなかった場合
            catch(BCrypt.Net.SaltParseException)
            {
                return false;
            }
        }
    }
}
