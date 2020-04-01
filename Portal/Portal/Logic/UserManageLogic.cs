using System;
using Portal.DBAccess;
using Portal.Info;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Logic
{
    /// <summary>
    /// ユーザー管理ロジック
    /// </summary>
    class UserManageLogic
    {
        /// <summary>
        /// 一般ユーザー情報取得
        /// </summary>
        /// <returns>一般ユーザー情報</returns>
        public List<UserInfo> GetGeneralUserInfo()
        {
            return new UserManageDBAccess().GetGeneralUserInfo();
        }


        /// <summary>
        /// ユーザー追加
        /// </summary>
        /// <param name="userInfoList">追加するユーザーの情報</param>
        public void InsertUser(List<UserInfo> userInfoList)
        {
            new UserManageDBAccess().InsertUser(userInfoList);
        }


        /// <summary>
        /// ユーザー情報更新
        /// </summary>
        /// <param name="userInfoList">ユーザー情報</param>
        public void UpdateUserInfo(List<UserInfo> userInfoList)
        {
            new UserManageDBAccess().UpdateUserInfo(userInfoList);
        }


        /// <summary>
        /// ユーザー削除
        /// </summary>
        /// <param name="userID">削除するユーザーのID</param>
        /// <param name="deleteFlag">削除フラグ</param>
        public void DeleteUser(string userID, int deleteFlag)
        {
            new UserManageDBAccess().UpdateUserDeleteFlag(userID, deleteFlag);
        }
    }
}
