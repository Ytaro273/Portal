using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Info
{
    //ユーザーIDとユーザー名
    public class UserIDAndName
    {
        //コンストラクタ
        public UserIDAndName(string userID, string userName)
        {
            this.UserID = userID;
            this.UserName = userName;
        }

        public string UserID { get; } //ユーザーID
        public string UserName { get; } //ユーザー名
    }
}
