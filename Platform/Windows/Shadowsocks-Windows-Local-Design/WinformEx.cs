/*
 * Shadowsocks-Net https://github.com/shadowsocks/Shadowsocks-Net
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Shadowsocks_Windows_Local
{
    public static class WinformEx
    {
        [DebuggerStepThrough]
        static public void UiThread(this Form form, Action action)
        {
            if (form.InvokeRequired)
            {
                form.Invoke(action);
                return;
            }
            action.Invoke();
        }

        [DebuggerStepThrough]
        static public void UiThread(this Control c, Action action)
        {
            if (c.InvokeRequired)
            {
                c.Invoke(action);
                return;
            }
            action.Invoke();
        }
    }
}
