using Portal.Enum;
using Portal.DBAccess;
using Portal.Info;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal
{
    public class MailLogic
    {
        /// <summary>
        /// アドレス情報取得
        /// ログインユーザー以外のユーザーのIDと名前を取得する
        /// </summary>
        /// <param name="loginUserID">ログインユーザーのユーザーID</param>
        /// <returns>メール情報</returns>
        public List<MailInfo> GetAddresInfo(string loginUserID)
        {
            var mailInfoList = new List<MailInfo>();

            var userIDAndNameList = new CommonLogic().GetUserIDAndName(); //全てのユーザーのIDと名前を取得
            
            //ログインユーザー以外のユーザーのIDと名前をメール情報リストに設定
            foreach(var userIDAndName in userIDAndNameList)
            {
                if(userIDAndName.UserID != loginUserID)
                mailInfoList.Add(new MailInfo()
                {
                    UserID = userIDAndName.UserID,
                    UserName = userIDAndName.UserName,
                });
            }
            
            return mailInfoList;
        }


        /// <summary>
        /// アドレス名生成
        /// </summary>
        /// <param name="mailInfo">メール情報</param>
        /// <returns>アドレス名</returns>
        public string GenerateAddressName(MailInfo mailInfo)
        {
            return mailInfo.UserID + mailInfo.UserName;
        }


        /// <summary>
        /// メール取得
        /// ログインユーザーが受信したメール、もしくは送信したメールを取得する
        /// </summary>
        /// <param name="userID">ログインユーザーのユーザーID</param>
        /// <param name="sendOrReceive">送信したメール、受信したメールどちらを取得するのか</param>
        /// <returns>メール情報</returns>
        public List<MailInfo> GetMail(string userID, SendOrReceive sendOrReceive)
        {
            //引き数のsendOrReceiveが受信なら受信済みメールを、そうでないなら送信済みメールを取得する
            if(sendOrReceive == SendOrReceive.Receiver) return new MailDBAccess().GetIncomingMail(userID);
            else return new MailDBAccess().GetTransmittedMail(userID);
        }


        /// <summary>
        /// メール送信
        /// </summary>
        /// <param name="userID">ログインユーザーのユーザーID</param>
        /// <param name="mailInfo">送信するメールの情報</param>
        public void SendMail(string userID, List<MailInfo> mailInfoList)
        {
            new MailDBAccess().SendMail(userID, mailInfoList);
        }
    }
}
