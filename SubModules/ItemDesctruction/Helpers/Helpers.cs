using Blish_HUD;
using Blish_HUD.Controls.Extern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kenedia.Modules.QoL.SubModules
{
    public partial class ItemDestruction
    {
        private void Cancel_Key_Activated(object sender, EventArgs e)
        {
            DeletePrepared = false;
        }

        public async void Paste()
        {
            await Task.Run(() =>
            {
                Blish_HUD.Controls.Intern.Keyboard.Press(VirtualKeyShort.LCONTROL, true);
                Blish_HUD.Controls.Intern.Keyboard.Stroke(VirtualKeyShort.KEY_A, true);
                Thread.Sleep(5);
                Blish_HUD.Controls.Intern.Keyboard.Stroke(VirtualKeyShort.KEY_V, true);
                Thread.Sleep(5);
                Blish_HUD.Controls.Intern.Keyboard.Release(VirtualKeyShort.LCONTROL, true);
                DeletePrepared = false;
            });
        }

        public async void Copy()
        {
            DeleteRunning = true;
            await Task.Run(() =>
            {
                Blish_HUD.Controls.Intern.Keyboard.Press(VirtualKeyShort.LCONTROL, true);
                Blish_HUD.Controls.Intern.Keyboard.Stroke(VirtualKeyShort.KEY_A, true);
                Thread.Sleep(5);
                Blish_HUD.Controls.Intern.Keyboard.Release(VirtualKeyShort.LCONTROL, true);


                Blish_HUD.Controls.Intern.Keyboard.Press(VirtualKeyShort.LCONTROL, true);
                Blish_HUD.Controls.Intern.Keyboard.Stroke(VirtualKeyShort.KEY_C, true);
                Thread.Sleep(5);
                Blish_HUD.Controls.Intern.Keyboard.Release(VirtualKeyShort.LCONTROL, true);

                Blish_HUD.Controls.Intern.Keyboard.Release(VirtualKeyShort.LSHIFT, true);
                Thread.Sleep(5);
                Blish_HUD.Controls.Intern.Keyboard.Stroke(VirtualKeyShort.BACK, true);
                Blish_HUD.Controls.Intern.Keyboard.Stroke(VirtualKeyShort.RETURN, true);
            });

            await Task.Run(() =>
            {
                var text = ClipboardUtil.WindowsClipboardService.GetTextAsync()?.Result;
                text = text.Length > 3 ? text.Substring(1, text.Length - 2) : "";

                if (text.Length > 0) ClipboardUtil.WindowsClipboardService.SetTextAsync(text);
                DeletePrepared = true;
            });

            DeleteRunning = false;
        }
    }
}
