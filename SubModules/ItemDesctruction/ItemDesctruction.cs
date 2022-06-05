using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Settings;
using Kenedia.Modules.QoL.Classes;
using Kenedia.Modules.QoL.SubModules.ItemDesctruction.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blish_HUD.Controls.Extern;

namespace Kenedia.Modules.QoL.SubModules
{
    public partial class ItemDestruction : SubModule
    {
        private enum State
        {
            Ready,
            Copying,
            Copied,
            Dragging,
            ReadyToPaste,
            Pasting,
            Pasted,
            Done,
        }

        public SettingEntry<Blish_HUD.Input.KeyBinding> Cancel_Key;
        private LoadingSpinner LoadingSpinner;
        private CursorSpinner CursorIcon;
        private DeleteIndicator DeleteIndicator;

        private State ModuleState;
        private Point MousePos = Point.Zero;
        private Point ItemPos = Point.Zero;

        private string _Instruction = Strings.common.ClickItem;
        public string Instruction
        {
            get => _Instruction;
            set
            {
                _Instruction = value;
                CursorIcon.Instruction = value;
            }
        }

        public ItemDestruction()
        {
            Name = "Item Desctruction";
            ModuleIcon = QoL.ModuleInstance.TextureManager.getIcon("ItemDestruction", _Icons.ModuleIcon);
            ModuleIconHovered = QoL.ModuleInstance.TextureManager.getIcon("ItemDestruction", _Icons.ModuleIcon_HoveredWhite);

            ModuleIcon_Active = QoL.ModuleInstance.TextureManager.getIcon("ItemDestruction", _Icons.ModuleIcon_Active);
            ModuleIconHovered_Active = QoL.ModuleInstance.TextureManager.getIcon("ItemDestruction", _Icons.ModuleIcon_Active_HoveredWhite);

            Initialize();
            LoadData();
        }
        public override void DefineSettings(SettingCollection settings)
        {
            base.DefineSettings(settings);

            ToggleModule_Key = settings.DefineSetting(Name + nameof(ToggleModule_Key),
                                                      new Blish_HUD.Input.KeyBinding(ModifierKeys.Ctrl, Keys.Delete),
                                                      () => string.Format(Strings.common.Toggle, Name));

            var internal_settings = settings.AddSubCollection(Name + " Internal Settings", false, false);
            Cancel_Key = internal_settings.DefineSetting(Name + nameof(Cancel_Key), new Blish_HUD.Input.KeyBinding(Keys.Escape));


            Cancel_Key.Value.Enabled = true;
            Cancel_Key.Value.Activated += Cancel_Key_Activated;

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

            CursorIcon.Visible = Active;
            DeleteIndicator?.Hide();
        }

        public override void Initialize()
        {
            base.Initialize();

            CursorIcon = new CursorSpinner()
            {
                Name = Name,
                Parent = GameService.Graphics.SpriteScreen,
                Background = QoL.ModuleInstance.TextureManager.getBackground(_Backgrounds.Tooltip),
                Visible = false,
                Instruction = Strings.common.ClickItem,
            };

            string[] instructions = {
                Strings.common.ClickItem,
                Strings.common.ThrowItem
            };
            var Font = GameService.Content.DefaultFont14;
            int width = 0;
            foreach (string s in instructions)
            {
                width = Math.Max((int)Font.MeasureString(s).Width, width);
            }

            CursorIcon.Size = new Point(50 + width + 5, 50);

            DeleteIndicator = new DeleteIndicator()
            {
                Parent = GameService.Graphics.SpriteScreen,
                Size = new Point(32, 32),
                Visible = false,
                Texture = QoL.ModuleInstance.TextureManager.getControl(_Controls.Delete),
                ClipsBounds = false,
            };


            InputService.Input.Mouse.LeftMouseButtonPressed += Mouse_LeftMouseButtonPressed;
            InputService.Input.Mouse.LeftMouseButtonReleased += Mouse_LeftMouseButtonReleased;
        }

        private async void Mouse_LeftMouseButtonReleased(object sender, Blish_HUD.Input.MouseEventArgs e)
        {
            if (Active)
            {
                if (ModuleState == State.Dragging)
                {
                    var mouse = Mouse.GetState();

                    if (MousePos.Distance2D(mouse.Position) > 15)
                    {
                        await Paste();
                    }
                    else
                    {
                        ModuleState = State.Done;
                    }
                }
            }
        }

        private async void Mouse_LeftMouseButtonPressed(object sender, Blish_HUD.Input.MouseEventArgs e)
        {
            if (Active)
            {
                var mouse = Mouse.GetState();
                var keyboard = Keyboard.GetState();
                DeleteIndicator.Visible = false;

                if (ModuleState != State.Copying && ModuleState != State.Pasting)
                {
                    if (keyboard.IsKeyDown(Keys.LeftShift))
                    {
                        ItemPos = mouse.Position;
                        Instruction = Strings.common.ThrowItem;
                        await Copy();
                    }
                }

                if (ItemPos.Distance2D(mouse.Position) > 100)
                {
                    ModuleState = State.Done;
                }
                else if (ModuleState == State.ReadyToPaste)
                {
                    ModuleState = State.Dragging;
                }
            }
        }

        public override void LoadData()
        {
            Loaded = true;
        }

        public override void Update(GameTime gameTime)
        {
            if (ModuleState == State.Copied)
            {
                ModuleState = State.ReadyToPaste;
            }
            else if (ModuleState == State.Done || ModuleState == State.Pasted)
            {
                MousePos = Point.Zero;
                DeleteIndicator.Visible = false;
                Instruction = Strings.common.ClickItem;
                ModuleState = State.Ready;
                DeleteIndicator.Visible = false;
            }
        }

        public override void UpdateLanguage(object sender, EventArgs e)
        {
            base.UpdateLanguage(sender, e);

            Instruction = Strings.common.ClickItem;

            string[] instructions = {
                Strings.common.ClickItem,
                Strings.common.ThrowItem
            };

            var Font = GameService.Content.DefaultFont14;
            int width = 0;
            foreach (string s in instructions)
            {
                width = Math.Max((int)Font.MeasureString(s).Width, width);
            }

            CursorIcon.Size = new Point(50 + width + 5, 50);
        }
        public override void Dispose()
        {
            LoadingSpinner?.Dispose();
            CursorIcon?.Dispose();
            DeleteIndicator?.Dispose();

            Cancel_Key.Value.Enabled = false;
            Cancel_Key.Value.Activated -= Cancel_Key_Activated;

            ToggleModule_Key.Value.Enabled = false;
            ToggleModule_Key.Value.Activated -= ToggleModule_Key_Activated;

            InputService.Input.Mouse.LeftMouseButtonPressed -= Mouse_LeftMouseButtonPressed;
            InputService.Input.Mouse.LeftMouseButtonReleased -= Mouse_LeftMouseButtonReleased;

            base.Dispose();
        }

        private void Cancel_Key_Activated(object sender, EventArgs e)
        {
            ModuleState = State.Done;
        }

        public async Task Paste()
        {
            ModuleState = State.Pasting;

            await Task.Delay(25);

            Blish_HUD.Controls.Intern.Keyboard.Press(VirtualKeyShort.LCONTROL, true);
            Blish_HUD.Controls.Intern.Keyboard.Stroke(VirtualKeyShort.KEY_V, true);
            await Task.Delay(5);
            Blish_HUD.Controls.Intern.Keyboard.Release(VirtualKeyShort.LCONTROL, true);

            ModuleState = State.Pasted;
        }

        public async Task Copy()
        {
            ModuleState = State.Copying;

            await Task.Delay(25);

            Blish_HUD.Controls.Intern.Keyboard.Release(VirtualKeyShort.LSHIFT, true);
            await Task.Delay(5);

            Blish_HUD.Controls.Intern.Keyboard.Press(VirtualKeyShort.LCONTROL, true);
            Blish_HUD.Controls.Intern.Keyboard.Stroke(VirtualKeyShort.KEY_A, true);
            await Task.Delay(5);
            Blish_HUD.Controls.Intern.Keyboard.Release(VirtualKeyShort.LCONTROL, true);

            Blish_HUD.Controls.Intern.Keyboard.Press(VirtualKeyShort.LCONTROL, true);
            Blish_HUD.Controls.Intern.Keyboard.Stroke(VirtualKeyShort.KEY_C, true);
            await Task.Delay(5);
            Blish_HUD.Controls.Intern.Keyboard.Release(VirtualKeyShort.LCONTROL, true);

            Blish_HUD.Controls.Intern.Keyboard.Stroke(VirtualKeyShort.BACK, true);
            Blish_HUD.Controls.Intern.Keyboard.Stroke(VirtualKeyShort.RETURN, true);

            var text = await ClipboardUtil.WindowsClipboardService.GetTextAsync();

            if (text != null && text.Length > 0)
            {
                text = text.StartsWith("[") ? text.Substring(1, text.Length - 1) : text;
                text = text.EndsWith("]") ? text.Substring(0, text.Length - 1) : text;

                await ClipboardUtil.WindowsClipboardService.SetTextAsync(text);
            }

            ModuleState = State.Copied;
            DeleteIndicator.Visible = true;
        }
    }
}
