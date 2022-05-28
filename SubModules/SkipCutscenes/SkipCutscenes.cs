using Blish_HUD;
using Blish_HUD.Settings;
using Kenedia.Modules.QoL.Classes;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blish_HUD.Controls.Intern;
using Point = Microsoft.Xna.Framework.Point;
using SysMouse = Microsoft.Xna.Framework.Input.Mouse;
using Mouse = Blish_HUD.Controls.Intern.Mouse;
using Keyboard = Blish_HUD.Controls.Intern.Keyboard;
using System.Threading;
using Blish_HUD.Controls;
using Microsoft.Xna.Framework.Input;

namespace Kenedia.Modules.QoL.SubModules
{
    public partial class SkipCutscenes : SubModule
    {
        private int MumbleTick;
        private Point Resolution;
        private Vector3 PPos = Vector3.Zero;
        private bool InGame;
        private bool ModuleActive;
        private bool ClickAgain;
        private bool SleptBeforeClick;
        private bool IntroCutscene;

        public SettingEntry<Blish_HUD.Input.KeyBinding> Cancel_Key;

        List<int> IntroMaps = new List<int>()
        {
            573, //Queensdale
            458, //Plains of Ashford
            138, //Wayfarer Foothills
            379, //Caledon Forest
            432 //Metrica Province
        };
        List<int> StarterMaps = new List<int>(){
            15, //Queensdale
            19, //Plains of Ashford
            28, //Wayfarer Foothills
            34, //Caledon Forest
            35 //Metrica Province
        };


        public SkipCutscenes()
        {
            Name = "Skip Cutscenes";
            ModuleIcon = QoL.ModuleInstance.TextureManager.getIcon("SkipCutscenes", _Icons.ModuleIcon);
            ModuleIconHovered = QoL.ModuleInstance.TextureManager.getIcon("SkipCutscenes", _Icons.ModuleIcon_HoveredWhite);

            ModuleIcon_Active = QoL.ModuleInstance.TextureManager.getIcon("SkipCutscenes", _Icons.ModuleIcon_Active);
            ModuleIconHovered_Active = QoL.ModuleInstance.TextureManager.getIcon("SkipCutscenes", _Icons.ModuleIcon_Active_HoveredWhite);

            Initialize();
            LoadData();
        }


        public override void DefineSettings(SettingCollection settings)
        {
            ToggleModule_Key = settings.DefineSetting(Name + nameof(ToggleModule_Key),
                                                      new Blish_HUD.Input.KeyBinding(Keys.None),
                                                      () => string.Format(Strings.common.Toggle, Name));

            var internal_settings = settings.AddSubCollection(Name + " Internal Settings", false, false);
            Cancel_Key = internal_settings.DefineSetting(Name + nameof(Cancel_Key), new Blish_HUD.Input.KeyBinding(Keys.Escape));

            Cancel_Key.Value.Enabled = true;
            Cancel_Key.Value.Activated += Cancel_Key_Activated;

            ToggleModule_Key.Value.Enabled = true;
            ToggleModule_Key.Value.Activated += ToggleModule_Key_Activated;
        }

        public override void ToggleModule()
        {
            base.ToggleModule();

        }

        public override void Initialize()
        {
            base.Initialize();

            GameService.Gw2Mumble.CurrentMap.MapChanged += CurrentMap_MapChanged;
            GameService.Gw2Mumble.PlayerCharacter.NameChanged += PlayerCharacter_NameChanged; ;
        }
        private void PlayerCharacter_NameChanged(object sender, ValueEventArgs<string> e)
        {
            IntroCutscene = false;
        }

        private void CurrentMap_MapChanged(object sender, ValueEventArgs<int> e)
        {
            ClickAgain = false;
            SleptBeforeClick = false;
            Ticks.global += 2000;
            MumbleTick = GameService.Gw2Mumble.Tick;

            var p = GameService.Gw2Mumble.PlayerCharacter.Position;
            PPos = p;

            if (IntroCutscene && StarterMaps.Contains(GameService.Gw2Mumble.CurrentMap.Id))
            {
                Thread.Sleep(1250);
                Click();
            }
        }

        private void ToggleModule_Key_Activated(object sender, EventArgs e)
        {
            ToggleModule();
        }

        private void Cancel_Key_Activated(object sender, EventArgs e)
        {
            Ticks.global += 2500;
            ClickAgain = false;
            SleptBeforeClick = false;
            MumbleTick = GameService.Gw2Mumble.Tick + 5;
        }

        public override void LoadData()
        {

            Loaded = true;
        }

        public override void Update(GameTime gameTime)
        {
            var Mumble = GameService.Gw2Mumble;
            var resolution = GameService.Graphics.Resolution;
            var _inGame = GameService.GameIntegration.Gw2Instance.IsInGame;

            if (IntroMaps.Contains(Mumble.CurrentMap.Id))
            {
                IntroCutscene = true;
            }

            if (GameService.Graphics.Resolution != resolution)
            {
                Resolution = resolution;
                MumbleTick = Mumble.Tick + 5;
                return;
            }

            if (!_inGame && (InGame || ClickAgain) && GameService.GameIntegration.Gw2Instance.Gw2HasFocus)
            {
                if (Mumble.Tick > MumbleTick)
                {
                    //ScreenNotification.ShowNotification("Click ... ", ScreenNotification.NotificationType.Error);
                    Click();

                    ClickAgain = true;
                    MumbleTick = Mumble.Tick;
                    Ticks.global = gameTime.TotalGameTime.TotalMilliseconds + 250;
                }
                else if (ClickAgain)
                {
                    if (!SleptBeforeClick)
                    {
                        //ScreenNotification.ShowNotification("Sleep before we click again... ", ScreenNotification.NotificationType.Error);
                        Ticks.global = gameTime.TotalGameTime.TotalMilliseconds + 3500;
                        SleptBeforeClick = true;
                        return;
                    }

                    //ScreenNotification.ShowNotification("Click Again... ", ScreenNotification.NotificationType.Error);
                    ClickAgain = false;

                    Click();

                    //Thread.Sleep(5);
                    Mouse.Click(MouseButton.LEFT, 15, 15);

                    Thread.Sleep(5);
                    Keyboard.Stroke(Blish_HUD.Controls.Extern.VirtualKeyShort.ESCAPE);
                }
            }
            else
            {
                ClickAgain = false;
                SleptBeforeClick = false;
            }

            InGame = GameService.GameIntegration.Gw2Instance.IsInGame;
        }
        public override void UpdateLanguage(object sender, EventArgs e)
        {
            base.UpdateLanguage(sender, e);
        }
        public override void Dispose()
        {
            Cancel_Key.Value.Activated -= Cancel_Key_Activated;
            ToggleModule_Key.Value.Activated -= ToggleModule_Key_Activated;

            base.Dispose();
        }
    }
}

