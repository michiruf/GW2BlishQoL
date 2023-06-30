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
using Blish_HUD.Controls.Extern;

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
        public SettingEntry<bool> UseHotkeyInsteadOfMouseWheel;

        public ZoomOut()
        {
            Name = "Zoom Out";
            ModuleIcon = QoL.ModuleInstance.TextureManager.getIcon("ZoomOut",_Icons.ModuleIcon);
            ModuleIconHovered = QoL.ModuleInstance.TextureManager.getIcon("ZoomOut", _Icons.ModuleIcon_HoveredWhite);

            ModuleIcon_Active = QoL.ModuleInstance.TextureManager.getIcon("ZoomOut", _Icons.ModuleIcon_Active);
            ModuleIconHovered_Active = QoL.ModuleInstance.TextureManager.getIcon("ZoomOut", _Icons.ModuleIcon_Active_HoveredWhite);
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
            
            UseHotkeyInsteadOfMouseWheel = settings.DefineSetting(nameof(UseHotkeyInsteadOfMouseWheel),
                true,
                () => Strings.common.UseHotkeyInsteadOfMouseWheel_Name);

            ManualMaxZoomOut.Value.Enabled = true;
            ManualMaxZoomOut.Value.Activated += ManualMaxZoomOut_Triggered;

            ToggleModule_Key.Value.Enabled = true;
            ToggleModule_Key.Value.Activated += ToggleModule_Key_Activated;

            Enabled = settings.DefineSetting(Name + nameof(Enabled),
                                                      true,
                                                      () => string.Format(Strings.common.Enable_Name, Name), () => string.Format(Strings.common.Enable_Tooltip, Name));

            ShowOnBar = settings.DefineSetting(Name + nameof(ShowOnBar),
                                                      true,
                                                      () => string.Format(Strings.common.ShowIcon_Name, Name), () => string.Format(Strings.common.ShowIcon_Tooltip, Name));
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

            var Mumble = GameService.Gw2Mumble;
            Mumble.CurrentMap.MapChanged += CurrentMap_MapChanged;
            Mumble.PlayerCharacter.NameChanged += PlayerCharacter_NameChanged;

            LoadData();
        }

        private void PlayerCharacter_NameChanged(object sender, ValueEventArgs<string> e)
        {
            ZoomTicks = 0;
        }

        private void CurrentMap_MapChanged(object sender, ValueEventArgs<int> e)
        {
            ZoomTicks = 0;
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
            var mumble = GameService.Gw2Mumble;

            // Check if mouse movement was present and reset it (always)
            var mousePreviouslyScrolled = MouseScrolled;
            MouseScrolled = false;
            
            // Cancel early if functionality is disabled
            // Cancel early if map was opened
            // Cancel early if mouse was previously scrolled
            if (
                !ZoomOnCameraChange.Value ||
                mumble.UI.IsMapOpen ||
                (AllowManualZoom.Value && mousePreviouslyScrolled)
            )
            {
                ZoomTicks = 0;
                return;
            }

            // Calculate distances and delta
            // I really do not know, why there is just the Z coordinate and not the others. Maybe its oriented and relative to the player?
            var cameraDistance = Math.Abs(mumble.PlayerCamera.Position.Z - mumble.PlayerCharacter.Position.Z);
            var delta = Math.Abs(Distance - cameraDistance);
            var threshold = AllowManualZoom.Value ? 0.5f : 0f;

            if (delta > threshold)
            {
                ZoomTicks += 2;
            }
            Distance = cameraDistance;

            if (Zoom < mumble.PlayerCamera.FieldOfView)
            {
                ZoomTicks += 2;
            }
            Zoom = mumble.PlayerCamera.FieldOfView;

            // Finally, perform the zooming
            if (ZoomTicks > 0)
            {
                if (UseHotkeyInsteadOfMouseWheel.Value)
                    Blish_HUD.Controls.Intern.Keyboard.Stroke(VirtualKeyShort.NEXT);
                else
                    Blish_HUD.Controls.Intern.Mouse.RotateWheel(-25);
                ZoomTicks -= 1;
            }
        }
        
        public override void Dispose()
        {
            var Mumble = GameService.Gw2Mumble;

            ToggleModule_Key.Value.Activated -= ToggleModule_Key_Activated;
            ManualMaxZoomOut.Value.Activated -= ManualMaxZoomOut_Triggered;
            InputService.Input.Mouse.MouseWheelScrolled -= Mouse_MouseWheelScrolled;
            Mumble.PlayerCharacter.NameChanged -= PlayerCharacter_NameChanged;
            Mumble.CurrentMap.MapChanged -= CurrentMap_MapChanged;

            base.Dispose();
        }
    }
}

