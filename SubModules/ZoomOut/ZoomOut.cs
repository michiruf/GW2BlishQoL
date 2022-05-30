using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Settings;
using Kenedia.Modules.QoL.Classes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kenedia.Modules.QoL.SubModules
{
    public class ZoomOut : SubModule
    {
        private bool MouseScrolled;
        private float Distance;
        private float Zoom;
        private int ZoomTicks = 0;
        public SettingEntry<Blish_HUD.Input.KeyBinding> ManualMaxZoomOut;
        public SettingEntry<bool> ZoomOnCameraChange;
        public SettingEntry<bool> AllowManualZoom;

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
            base.DefineSettings(settings);

            ToggleModule_Key = settings.DefineSetting(Name + nameof(ToggleModule_Key),
                                                      new Blish_HUD.Input.KeyBinding(ModifierKeys.Ctrl, Keys.NumPad0),
                                                      () => string.Format(Strings.common.Toggle, Name));


            ManualMaxZoomOut = settings.DefineSetting(nameof(ManualMaxZoomOut),
                                                      new Blish_HUD.Input.KeyBinding(Keys.None),
                                                      () => Strings.common.ManualMaxZoomOut_Name,
                                                      () => Strings.common.ManualMaxZoomOut_Tooltip);

            ZoomOnCameraChange = settings.DefineSetting(nameof(ZoomOnCameraChange),
                                                      true,
                                                      () => Strings.common.ZoomOnCameraChange_Name,
                                                      () => Strings.common.ZoomOnCameraChange_Tooltip);

            AllowManualZoom = settings.DefineSetting(nameof(AllowManualZoom),
                                                      true,
                                                      () => Strings.common.AllowManualZoom_Name,
                                                      () => Strings.common.AllowManualZoom_Tooltip);

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
            InputService.Input.Mouse.MouseWheelScrolled += Mouse_MouseWheelScrolled;
        }

        private void Mouse_MouseWheelScrolled(object sender, Blish_HUD.Input.MouseEventArgs e)
        {
            MouseScrolled = true;
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

            if (gameTime.TotalGameTime.Milliseconds - Ticks.global > 125)
            {
                Ticks.global = gameTime.TotalGameTime.Milliseconds;

            }

            if (ZoomTicks > 0)
            {
                Blish_HUD.Controls.Intern.Mouse.RotateWheel(-25);
                ZoomTicks -= 1;
            }

            var cameraDistance = (Math.Max(Mumble.PlayerCamera.Position.Z, Mumble.PlayerCharacter.Position.Z) - Math.Min(Mumble.PlayerCamera.Position.Z, Mumble.PlayerCharacter.Position.Z));
            var delta = (Math.Max(Distance, cameraDistance) - Math.Min(Distance, cameraDistance));
            var threshold = AllowManualZoom.Value ? 0.5 : 0.25;
            if (cameraDistance == Distance) ZoomTicks = ZoomTicks/2;

            if (delta > threshold)
            {
                if (ZoomOnCameraChange.Value && (!AllowManualZoom.Value || !MouseScrolled) && Distance != 0)
                {
                    ZoomTicks = 2;
                }
                MouseScrolled = false;
                Distance = cameraDistance;
            }

            if (Zoom < Mumble.PlayerCamera.FieldOfView)
            {
                ZoomTicks += 2;
            }

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
            InputService.Input.Mouse.MouseWheelScrolled -= Mouse_MouseWheelScrolled;

            base.Dispose();
        }
    }
}

