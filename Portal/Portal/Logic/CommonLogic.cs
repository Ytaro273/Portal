using Npgsql;
using Portal.DBAccess;
using Portal.Enum;
using Portal.Info;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Portal
{
    /// <summary>
    /// 共通ロジック
    /// </summary>
    public class CommonLogic
    {
        /// <summary>
        /// 全てのユーザーの名前とIDを取得
        /// </summary>
        /// <returns>ユーザーIDとユーザー名一覧</returns>
        public List<UserIDAndName> GetUserIDAndName()
        {
            return new CommonDBAccess().GetUserIDAndName();
        }


        /// <summary>
        /// アプリケーションを終了するかどうかチェック
        /// フォームが閉じられるときに呼ばれ、アプリケーションを終了するかどうか判定する
        /// </summary>
        /// <param name="formIsVisible">閉じられるフォームが可視状態なのか</param>
        /// <param name="closingStatus">挙動</param>
        /// <returns></returns>
        public ClosingType CheckWhetherToExit (bool formIsVisible, ClosingType closingStatus)
        {
            if (formIsVisible)
            {
                //フォームが可視状態でClosingTypeがデフォルトの値なら、×ボタンのクリックによるフォームの終了と判断し、
                //質問を表示する
                if (closingStatus == ClosingType.DoNothing)
                {
                    var result = MessageBox.Show("アプリケーションを終了しますか？", "質問", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes) return ClosingType.Exit; //アプリケーションを終了
                    else return ClosingType.Cancel; //終了を取り消す
                }
            }

            //フォームが不可視の状態で呼ばれた場合はフォームを移動しただけと判断し、何もしない
            return ClosingType.DoNothing;
        }
    }
}
