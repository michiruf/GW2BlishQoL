using Blish_HUD;
using Blish_HUD.Controls.Intern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kenedia.Modules.QoL.SubModules
{
    public partial class SkipCutscenes
    {
        public async Task CloseGameMenu()
        {
            Mouse.Click(MouseButton.LEFT, 15, 15);

            await Task.Delay(5);
            Keyboard.Stroke(Blish_HUD.Controls.Extern.VirtualKeyShort.ESCAPE);
        }

        public async Task Click()
        {
            var mousePos = Mouse.GetPosition();
            mousePos = new System.Drawing.Point(mousePos.X, mousePos.Y);

            WindowUtil.RECT pos;
            WindowUtil.GetWindowRect(GameService.GameIntegration.Gw2Instance.Gw2WindowHandle, out pos);
            var p = new System.Drawing.Point(GameService.Graphics.Resolution.X + pos.Left - 35, GameService.Graphics.Resolution.Y + pos.Top);

            Mouse.SetPosition(p.X, p.Y, true);
            await Task.Delay(25);

            Mouse.Click(MouseButton.LEFT, p.X, p.Y, true);

            await Task.Delay(10);
            Mouse.SetPosition(mousePos.X, mousePos.Y, true);
        }
    }
}
