using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portal.Enum
{
    /// <summary>
    /// FormClosingイベントの際の挙動の種類
    /// </summary>
    public enum ClosingType
    {
        DoNothing, //何もしない
        Exit, //アプリケーションを終了する
        Cancel //アプリケーションの終了をキャンセルする
    }
}
