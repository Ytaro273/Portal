using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Info
{
    /// <summary>
    /// メール情報
    /// </summary>
    public class MailInfo
    {
        //ログインユーザーが受信者なら送信者のIDで、ログインユーザーが送信者なら受信者のIDとなる
        public string UserID { get; set; }

        //ログインユーザーが受信者なら送信者の名前で、ログインユーザーが送信者なら受信者の名前となる
        public string UserName { get; set; } //ユーザー名

        public string Subject { get; set; } //件名
        public string Message { get; set; } //メール内容
        public DateTime AddedDate { get; set; } //送信日時
    } 
}
