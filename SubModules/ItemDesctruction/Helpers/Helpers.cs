using Blish_HUD;
using Blish_HUD.Controls;
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

        public async Task Paste()
        {
            var text = await ClipboardUtil.WindowsClipboardService.GetTextAsync();
            await Task.Delay(50);

            Blish_HUD.Controls.Intern.Keyboard.Press(VirtualKeyShort.LCONTROL, true);
            Blish_HUD.Controls.Intern.Keyboard.Stroke(VirtualKeyShort.KEY_A, true);
            await Task.Delay(5);
            Blish_HUD.Controls.Intern.Keyboard.Stroke(VirtualKeyShort.KEY_V, true);
            await Task.Delay(5);
            Blish_HUD.Controls.Intern.Keyboard.Release(VirtualKeyShort.LCONTROL, true);

            DeletePrepared = false;
        }

        public async Task Copy()
        {
            DeleteRunning = true;
            Blish_HUD.Controls.Intern.Keyboard.Release(VirtualKeyShort.LSHIFT, true);
            await Task.Delay(5);

            Blish_HUD.Controls.Intern.Keyboard.Press(VirtualKeyShort.LCONTROL, true);
            Blish_HUD.Controls.Intern.Keyboard.Stroke(VirtualKeyShort.KEY_A, true);
            await Task.Delay(15);
            Blish_HUD.Controls.Intern.Keyboard.Release(VirtualKeyShort.LCONTROL, true);

            await Task.Delay(25);


            Blish_HUD.Controls.Intern.Keyboard.Press(VirtualKeyShort.LCONTROL, true);
            Blish_HUD.Controls.Intern.Keyboard.Stroke(VirtualKeyShort.KEY_C, true);
            await Task.Delay(15);
            Blish_HUD.Controls.Intern.Keyboard.Release(VirtualKeyShort.LCONTROL, true);


            await Task.Delay(5);
            Blish_HUD.Controls.Intern.Keyboard.Stroke(VirtualKeyShort.BACK, true);
            Blish_HUD.Controls.Intern.Keyboard.Stroke(VirtualKeyShort.RETURN, true);
            await Task.Delay(5);

            var text = await ClipboardUtil.WindowsClipboardService.GetTextAsync();

            if (text.Length > 0)
            {
                text = text.StartsWith("[") ? text.Substring(1, text.Length - 1) : text;
                text = text.EndsWith("]") ? text.Substring(0, text.Length - 1) : text;

                await ClipboardUtil.WindowsClipboardService.SetTextAsync(text);
            }

            DeletePrepared = true;

            DeleteRunning = false;
        }
    }
}
