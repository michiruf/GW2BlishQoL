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
using System.Globalization;

namespace Kenedia.Modules.QoL.SubModules
{
    public class Resets : SubModule
    {
        CustomFlowPanel Container;
        IconLabel ServerReset;
        IconLabel WeeklyReset;

        public SettingEntry<Point> ControlPosition;

        public Resets()
        {
            Name = "Resets";
            ModuleIcon = QoL.ModuleInstance.TextureManager.getIcon("Resets", _Icons.ModuleIcon);
            ModuleIconHovered = QoL.ModuleInstance.TextureManager.getIcon("Resets", _Icons.ModuleIcon_HoveredWhite);

            ModuleIcon_Active = QoL.ModuleInstance.TextureManager.getIcon("Resets", _Icons.ModuleIcon_Active);
            ModuleIconHovered_Active = QoL.ModuleInstance.TextureManager.getIcon("Resets", _Icons.ModuleIcon_Active_HoveredWhite);
        }
        void EnsureControlsOnScreen()
        {
            GameService.Graphics.QueueMainThreadRender((graphicsDevice) =>
            {
                var bounds = GameService.Graphics.SpriteScreen.LocalBounds;

                if (bounds.Contains(Container.Location) && bounds.Contains(Container.Location.Add(Container.Size)))
                {
                    ControlPosition.Value = Container.Location;
                }
                else
                {
                    Container.Location = new Point(Math.Max(0, Math.Min(bounds.Right - Container.Width, Container.Location.X)), Math.Max(0, Math.Min(bounds.Bottom - Container.Height, Container.Location.Y)));
                }
            });
        }

        private void Container_Shown(object sender, EventArgs e)
        {
            EnsureControlsOnScreen();
        }

        private void Container_Moved(object sender, MovedEventArgs e)
        {
            if(Loaded) EnsureControlsOnScreen();
        }

        public override void DefineSettings(SettingCollection settings)
        {
            base.DefineSettings(settings);

            ToggleModule_Key = settings.DefineSetting(Name + nameof(ToggleModule_Key),
                                                      new Blish_HUD.Input.KeyBinding(Keys.None),
                                                      () => string.Format(Strings.common.Toggle, Name));

            Enabled = settings.DefineSetting(Name + nameof(Enabled),
                                                      true,
                                                      () => string.Format("Enable {0}", Name));

            ShowOnBar = settings.DefineSetting(Name + nameof(ShowOnBar),
                                                      true,
                                                      () => string.Format("Show Icon", Name));

            var internal_settings = settings.AddSubCollection("Internal Settings " + Name, false);
            ControlPosition = internal_settings.DefineSetting(nameof(ControlPosition), new Point(150, 150));
        }

        public override void Dispose()
        {
            Container?.Dispose();
            WeeklyReset?.Dispose();
            ServerReset?.Dispose();

            var Mumble = GameService.Gw2Mumble;
            Mumble.UI.UISizeChanged -= UI_UISizeChanged;

            base.Dispose();
        }

        public override void Initialize()
        {
            base.Initialize();

            var tRect = GameService.Content.DefaultFont14.GetStringRectangle("7 Tage 00:00:00");
            Container = new CustomFlowPanel()
            {
                Parent = GameService.Graphics.SpriteScreen,
                Visible = true,
                Width = (int)(tRect.Width + 6 + tRect.Height),
                HeightSizingMode = SizingMode.AutoSize,
                FlowDirection = ControlFlowDirection.LeftToRight,
                Background = QoL.ModuleInstance.TextureManager.getBackground(_Backgrounds.Tooltip),
                OuterControlPadding = new Vector2(2, 2),
                ControlPadding = new Vector2(0, 2),
                Location = ControlPosition.Value,
            };
            Container.Moved += Container_Moved;
            Container.Shown += Container_Shown;

            ServerReset = new IconLabel()
            {
                Parent = Container,
                Texture = QoL.ModuleInstance.TextureManager.getIcon(_Icons.TyriaDayNight),
                Text = "00:00:00",
                BasicTooltipText = "Server Reset",
                AutoSize = true,
            };

            WeeklyReset = new IconLabel()
            {
                Parent = Container,
                Texture = QoL.ModuleInstance.TextureManager.getIcon(_Icons.Calendar),
                Text = "0 days 00:00:00",
                BasicTooltipText = "Weekly Reset",
                AutoSize = true,
            };

            var Mumble = GameService.Gw2Mumble;
            Mumble.UI.UISizeChanged += UI_UISizeChanged;

            LoadData();
        }

        private void UI_UISizeChanged(object sender, ValueEventArgs<Gw2Sharp.Mumble.Models.UiSize> e)
        {
            EnsureControlsOnScreen();
        }

        public override void ToggleModule()
        {
            base.ToggleModule();
            Container.Visible = Active;
        }

        public override void LoadData()
        {
            Loaded = true;
        }

        public override void Update(GameTime gameTime)
        {
            var now = DateTime.UtcNow;
            var nextDay = DateTime.UtcNow.AddDays(1);
            var nextWeek = DateTime.UtcNow;
            for (int i = 1; i < 7; i++)
            {
                nextWeek = DateTime.UtcNow.AddDays(i);
                if (nextWeek.DayOfWeek == DayOfWeek.Monday) break;
            }
            var t = new DateTime(nextDay.Year, nextDay.Month, nextDay.Day, 0, 0, 0);
            var w = new DateTime(nextWeek.Year, nextWeek.Month, nextWeek.Day, 7, 30, 0);


            var weeklyReset = w.Subtract(now);
            WeeklyReset.Text = string.Format("{0:0} days {1:00}:{2:00}:{3:00}", weeklyReset.Days, weeklyReset.Hours, weeklyReset.Minutes, weeklyReset.Seconds);

            var serverReset = t.Subtract(now);
            ServerReset.Text = string.Format("{0:00}:{1:00}:{2:00}", serverReset.Hours, serverReset.Minutes, serverReset.Seconds);
        }

        public override void UpdateLanguage(object sender, EventArgs e)
        {
            base.UpdateLanguage(sender, e);
        }
    }
}
