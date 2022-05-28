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

namespace Kenedia.Modules.QoL.SubModules
{
    public partial class ItemDestruction : SubModule
    {
        public SettingEntry<Blish_HUD.Input.KeyBinding> Cancel_Key;
        private LoadingSpinner LoadingSpinner;
        private CursorSpinner CursorIcon;
        private DeleteIndicator DeleteIndicator;

        private ButtonState MouseState;
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

        private bool DeleteRunning;
        private bool DeletePrepared;

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

        }

        public override void LoadData()
        {

            Loaded = true;
        }

        public override void Update(GameTime gameTime)
        {

            if (GameIntegrationService.GameIntegration.Gw2Instance.Gw2HasFocus && !DeleteRunning)
            {
                var mouse = Mouse.GetState();
                var keyboard = Keyboard.GetState();

                var Clicked = MouseState == ButtonState.Pressed && mouse.LeftButton == ButtonState.Released;
                if (mouse.LeftButton == ButtonState.Pressed && MouseState == ButtonState.Released)
                {
                    if (ItemPos.Distance2D(mouse.Position) < 50)
                    {
                        MousePos = MousePos == Point.Zero ? mouse.Position : MousePos;
                    }
                    else
                    {
                        DeletePrepared = false;
                    }
                }

                if (Clicked)
                {
                    DeleteIndicator.Visible = false;

                    if (keyboard.IsKeyDown(Keys.LeftShift))
                    {
                        ItemPos = mouse.Position;
                        DeleteIndicator.Visible = true;
                        Instruction = Strings.common.ThrowItem;
                        Copy();
                    }
                    else if (DeletePrepared)
                    {
                        if (MousePos.Distance2D(mouse.Position) > 100)
                        {
                            Paste();
                        }

                        DeletePrepared = false;
                        DeleteIndicator.Visible = false;
                    }
                    else
                    {
                        MousePos = Point.Zero;
                        Instruction = Strings.common.ClickItem;
                    }
                }

                MouseState = mouse.LeftButton;
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

            base.Dispose();
        }
    }
}
