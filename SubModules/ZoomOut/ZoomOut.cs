using Blish_HUD;
using Blish_HUD.Settings;
using Kenedia.Modules.QoL.Classes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kenedia.Modules.QoL.SubModules
{
    public class ZoomOut : SubModule
    {
        private int MumbleTick;
        private Point Resolution;
        private bool InGame;
        private float Zoom;
        private int ZoomTicks = 0;
        public SettingEntry<Blish_HUD.Input.KeyBinding> ManualMaxZoomOut;

        public ZoomOut()
        {
            Name = "Zoom Out";
            ModuleIcon = QoL.ModuleInstance.TextureManager.getIcon("ZoomOut",_Icons.ModuleIcon);
            ModuleIconHovered = QoL.ModuleInstance.TextureManager.getIcon("ZoomOut", _Icons.ModuleIcon_HoveredWhite);

            ModuleIcon_Active = QoL.ModuleInstance.TextureManager.getIcon("ZoomOut", _Icons.ModuleIcon_Active);
            ModuleIconHovered_Active = QoL.ModuleInstance.TextureManager.getIcon("ZoomOut", _Icons.ModuleIcon_Active_HoveredWhite);

            Initialize();
            LoadData();
        }

        public override void DefineSettings(SettingCollection settings)
        {
            ToggleModule_Key = settings.DefineSetting(Name + nameof(ToggleModule_Key),
                                                      new Blish_HUD.Input.KeyBinding(ModifierKeys.Ctrl, Keys.NumPad0),
                                                      () => string.Format(Strings.common.Toggle, Name));


            ManualMaxZoomOut = settings.DefineSetting(nameof(ManualMaxZoomOut),
                                                      new Blish_HUD.Input.KeyBinding(Keys.None),
                                                      () => Strings.common.ManualMaxZoomOut_Name,
                                                      () => Strings.common.ManualMaxZoomOut_Tooltip);

            ManualMaxZoomOut.Value.Enabled = true;
            ManualMaxZoomOut.Value.Activated += ManualMaxZoomOut_Triggered;

            ToggleModule_Key.Value.Enabled = true;
            ToggleModule_Key.Value.Activated += ToggleModule_Key_Activated;
        }

        private void ToggleModule_Key_Activated(object sender, EventArgs e)
        {
            ToggleModule();
        }

        public override void ToggleModule()
        {
            base.ToggleModule();

        }

        public override void Initialize()
        {
            base.Initialize();

        }
        private void ManualMaxZoomOut_Triggered(object sender, EventArgs e)
        {
            ZoomTicks = 40;
        }

        public override void LoadData()
        {

            Loaded = true;
        }

        public override void Update(GameTime gameTime)
        {
            var Mumble = GameService.Gw2Mumble;

            if (Zoom < Mumble.PlayerCamera.FieldOfView)
            {
                ZoomTicks += 2;
            }
            else if (ZoomTicks > 0)
            {
                Blish_HUD.Controls.Intern.Mouse.RotateWheel(-25);
                ZoomTicks -= 1;
            }
            var mouse = Mouse.GetState();
            var mouseState = (mouse.LeftButton == ButtonState.Released) ? ButtonState.Released : ButtonState.Pressed;

            if (mouseState == ButtonState.Pressed || GameService.Graphics.Resolution != Resolution)
            {
                Resolution = GameService.Graphics.Resolution;
                MumbleTick = Mumble.Tick + 5;
                return;
            }

            if (!GameService.GameIntegration.Gw2Instance.IsInGame && InGame && Mumble.Tick > MumbleTick)
            {
                Blish_HUD.Controls.Intern.Keyboard.Stroke(Blish_HUD.Controls.Extern.VirtualKeyShort.ESCAPE, false);
                Blish_HUD.Controls.Intern.Mouse.Click(Blish_HUD.Controls.Intern.MouseButton.LEFT, 5, 5);

                MumbleTick = Mumble.Tick + 1;
            }
            InGame = GameService.GameIntegration.Gw2Instance.IsInGame;

            Zoom = Mumble.PlayerCamera.FieldOfView;
        }
        public override void UpdateLanguage(object sender, EventArgs e)
        {
            base.UpdateLanguage(sender, e);

        }
        public override void Dispose()
        {
            ToggleModule_Key.Value.Activated -= ToggleModule_Key_Activated;
            ManualMaxZoomOut.Value.Activated -= ManualMaxZoomOut_Triggered;

            base.Dispose();
        }
    }
}

