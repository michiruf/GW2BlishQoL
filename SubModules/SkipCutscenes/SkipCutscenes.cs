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
        enum State
        {
            Ready,
            MouseMoved,
            Clicked,
            MouseMovedBack,
            Clicked_Again,
            Menu_Opened,
            Menu_Closed,
            Done,
        }
        enum CinematicState
        {
            Ready,
            InitialSleep,
            Clicked_Once,
            Sleeping,
            Clicked_Twice,
            Done,
        }
        private State ModuleState;
        private CinematicState CutsceneState;
        private Point MouseStartPosition;

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
        }


        public override void DefineSettings(SettingCollection settings)
        {
            base.DefineSettings(settings);

            ToggleModule_Key = settings.DefineSetting(Name + nameof(ToggleModule_Key),
                                                      new Blish_HUD.Input.KeyBinding(Keys.None),
                                                      () => string.Format(Strings.common.Toggle, Name));

            var internal_settings = settings.AddSubCollection(Name + " Internal Settings", false, false);
            Cancel_Key = internal_settings.DefineSetting(Name + nameof(Cancel_Key), new Blish_HUD.Input.KeyBinding(Keys.Escape));

            Cancel_Key.Value.Enabled = true;
            Cancel_Key.Value.Activated += Cancel_Key_Activated;

            ToggleModule_Key.Value.Enabled = true;
            ToggleModule_Key.Value.Activated += ToggleModule_Key_Activated;

            Enabled = settings.DefineSetting(Name + nameof(Enabled),
                                                      true,
                                                      () => string.Format("Enable {0}", Name));

            ShowOnBar = settings.DefineSetting(Name + nameof(ShowOnBar),
                                                      true,
                                                      () => string.Format("Show Icon", Name));
        }

        public override void ToggleModule()
        {
            base.ToggleModule();

        }

        public override void Initialize()
        {
            base.Initialize();

            GameService.Gw2Mumble.CurrentMap.MapChanged += CurrentMap_MapChanged;
            GameService.Gw2Mumble.PlayerCharacter.NameChanged += PlayerCharacter_NameChanged;

            LoadData();
        }
        private void PlayerCharacter_NameChanged(object sender, ValueEventArgs<string> e)
        {
            IntroCutscene = false;
        }

        private void CurrentMap_MapChanged(object sender, ValueEventArgs<int> e)
        {
            if (Active)
            {
                MumbleTick = GameService.Gw2Mumble.Tick;

                var p = GameService.Gw2Mumble.PlayerCharacter.Position;
                PPos = p;

                if (IntroCutscene && StarterMaps.Contains(GameService.Gw2Mumble.CurrentMap.Id))
                {
                    Ticks.global = Ticks.global + 1250;
                    CutsceneState = CinematicState.InitialSleep;
                    ModuleState = State.Ready;
                }
            }
        }

        private void ToggleModule_Key_Activated(object sender, EventArgs e)
        {
            ToggleModule();
        }

        private void Cancel_Key_Activated(object sender, EventArgs e)
        {
            if (Active)
            {
                Ticks.global += 2500;
                ClickAgain = false;
                SleptBeforeClick = false;
                MumbleTick = GameService.Gw2Mumble.Tick + 5;
            }
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

            if (gameTime.TotalGameTime.TotalMilliseconds - Ticks.global > 0)
            {
                Ticks.global = gameTime.TotalGameTime.TotalMilliseconds;

                WindowUtil.RECT pos;
                WindowUtil.GetWindowRect(GameService.GameIntegration.Gw2Instance.Gw2WindowHandle, out pos);
                var p = new System.Drawing.Point(GameService.Graphics.Resolution.X + pos.Left - 35, GameService.Graphics.Resolution.Y + pos.Top);

                var mousePos = Mouse.GetPosition();
                mousePos = new System.Drawing.Point(mousePos.X, mousePos.Y);

                if (!_inGame && (Mumble.Tick > MumbleTick || CutsceneState != CinematicState.Done))
                {
                    //ScreenNotification.ShowNotification("CutsceneState: " + CutsceneState.ToString());
                    //QoL.Logger.Debug("CutsceneState: " + CutsceneState.ToString());
                    //ScreenNotification.ShowNotification("ModuleState: " + ModuleState.ToString());
                    //QoL.Logger.Debug("ModuleState: " + ModuleState.ToString());

                    if (CutsceneState == CinematicState.Clicked_Once)
                    {
                        Ticks.global = gameTime.TotalGameTime.TotalMilliseconds + 2500;
                        CutsceneState = CinematicState.Sleeping;
                        ModuleState = State.Ready;
                        return;
                    }
                    else if (CutsceneState == CinematicState.Ready)
                    {
                        Ticks.global = gameTime.TotalGameTime.TotalMilliseconds + 250;
                        CutsceneState = CinematicState.InitialSleep;
                        ModuleState = State.Ready;
                        return;
                    }

                    if (CutsceneState == CinematicState.Sleeping || CutsceneState == CinematicState.InitialSleep)
                    {
                        if (ModuleState == State.Ready)
                        {
                            Mouse.SetPosition(p.X, p.Y, true);
                            MouseStartPosition = new Point(mousePos.X, mousePos.Y);
                            ModuleState = State.MouseMoved;
                        }
                        else if (ModuleState == State.MouseMoved)
                        {
                            Mouse.Click(MouseButton.LEFT, p.X, p.Y, true);
                            ModuleState = State.Clicked;
                        }
                        else if (ModuleState == State.Clicked)
                        {
                            Mouse.SetPosition(MouseStartPosition.X, MouseStartPosition.Y, true);
                            ModuleState = State.MouseMovedBack;
                            CutsceneState = CutsceneState == CinematicState.Ready ? CinematicState.Clicked_Once : CinematicState.Clicked_Twice;
                        }
                    }

                    else if (CutsceneState == CinematicState.Clicked_Twice)
                    {
                        if (ModuleState == State.MouseMovedBack)
                        {
                            Mouse.Click(MouseButton.LEFT, 15, 15);
                            ModuleState = State.Menu_Opened;
                        }
                        else if(ModuleState == State.Menu_Opened)
                        {
                            Keyboard.Stroke(Blish_HUD.Controls.Extern.VirtualKeyShort.ESCAPE);
                            ModuleState = State.Menu_Closed;
                            CutsceneState = CinematicState.Done;
                        }
                    }
                }
                else if (ModuleState != State.Ready || CutsceneState != CinematicState.Ready)
                {
                    if (ModuleState == State.Clicked && MouseStartPosition != Point.Zero)
                    {
                        if (ModuleState == State.Clicked)
                        {
                            Mouse.Click(MouseButton.LEFT, 15, 15);
                            ModuleState = State.Menu_Opened;
                        }
                        else if (ModuleState == State.Menu_Opened)
                        {
                            Keyboard.Stroke(Blish_HUD.Controls.Extern.VirtualKeyShort.ESCAPE);
                            ModuleState = State.Menu_Closed;
                        }

                        Mouse.SetPosition(MouseStartPosition.X, MouseStartPosition.Y, true);
                        CutsceneState = CinematicState.Done;
                    }

                    ModuleState = State.Ready;
                    CutsceneState = CinematicState.Ready;
                    MouseStartPosition = Point.Zero;
                }
            }
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

