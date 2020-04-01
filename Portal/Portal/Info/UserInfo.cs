using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Info
{
    /// <summary>
    /// ユーザー情報
    /// </summary>
    public class UserInfo
    {
        public string UserID { get; set; } //ユーザーID

        public string UserName { get; set; } //ユーザー名
        
        public string Pass { get; set; } //ログインパスワード（ハッシュ化されている)
        
        public int AuthorityID { get; set; } //権限ID
        
        public string AuthorityName { get; set; } //権限名
    }
}
