using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Settings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kenedia.Modules.QoL.Classes
{
    public abstract class SubModule
    {
        public SubModule()
        {
            QoL.ModuleInstance.LanguageChanged += UpdateLanguage;

            ModuleIcon = QoL.ModuleInstance.TextureManager.getIcon(_Icons.ModuleIcon);
            ModuleIconHovered = QoL.ModuleInstance.TextureManager.getIcon(_Icons.ModuleIcon_HoveredWhite);

            ModuleIcon_Active = QoL.ModuleInstance.TextureManager.getIcon(_Icons.ModuleIcon_Active);
            ModuleIconHovered_Active = QoL.ModuleInstance.TextureManager.getIcon(_Icons.ModuleIcon_Active_HoveredWhite);
        }

        protected CornerIcon CornerIcon;
        public string Name;

        public bool Loaded;
        public bool Active;
        public bool Include;

        protected Texture2D ModuleIcon;
        protected Texture2D ModuleIconHovered;
        protected Texture2D ModuleIcon_Active;
        protected Texture2D ModuleIconHovered_Active;

        public event EventHandler Toggled;

        public SettingEntry<Blish_HUD.Input.KeyBinding> ToggleModule_Key;
        public SettingEntry<bool> ShowCornerIcon;

        public Ticks Ticks = new Ticks();

        public virtual void Initialize() 
        {
            CornerIcon = new CornerIcon()
            {
                Icon = ModuleIcon,
                HoverIcon = ModuleIconHovered,
                BasicTooltipText = string.Format(Strings.common.Toggle, $"{Name}"),
                Parent = GameService.Graphics.SpriteScreen,
                Visible = true,
            };
            CornerIcon.Click += CornerIcon_Click;        
        }

        public abstract void DefineSettings(SettingCollection parentSettings);

        private void CornerIcon_Click(object sender, Blish_HUD.Input.MouseEventArgs e)
        {
            ToggleModule();
        }

        public abstract void LoadData();
        public virtual void ToggleModule()
        {
            Active = !Active;

            ScreenNotification.ShowNotification(string.Format(Strings.common.RunStateChange, Name, !Active ? Strings.common.Deactivated : Strings.common.Activated), ScreenNotification.NotificationType.Warning);

            if (CornerIcon != null)
            {
                CornerIcon.Icon = Active ? ModuleIcon_Active : ModuleIcon;
                CornerIcon.HoverIcon = Active ? ModuleIconHovered_Active : ModuleIconHovered;
            }

            this.Toggled?.Invoke(this, EventArgs.Empty);
        }

        public abstract void Update(GameTime gameTime);
        public virtual void UpdateLanguage(object sender, EventArgs e)
        {
            if (CornerIcon != null)
            {
                CornerIcon.BasicTooltipText = string.Format(Strings.common.Toggle, $"{Name}");
            }
        }
        public virtual void Dispose()
        {
            QoL.ModuleInstance.LanguageChanged -= UpdateLanguage;
            CornerIcon?.Dispose();

            ModuleIcon = null;
            ModuleIconHovered = null;

            ModuleIcon_Active = null;
            ModuleIconHovered_Active = null;
        }
    }
}
