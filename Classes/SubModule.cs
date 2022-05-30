using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Settings;
using Kenedia.Modules.QoL.UI;
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
        public bool Active
        {
            get {
                return _Active != null && _Active.Value;
            }
            set
            {
                if(_Active != null)
                {
                    _Active.Value = value;
                }
            }
        }
        public bool Include;

        public Texture2D ModuleIcon;
        public Texture2D ModuleIconHovered;
        public Texture2D ModuleIcon_Active;
        public Texture2D ModuleIconHovered_Active;

        public event EventHandler Toggled;

        public SettingEntry<Blish_HUD.Input.KeyBinding> ToggleModule_Key;
        public SettingEntry<bool> _Active;

        public Ticks Ticks = new Ticks();
        public Hotbar_Button Hotbar_Button;

        public virtual void Initialize() 
        {
            
        }

        public virtual void DefineSettings(SettingCollection settings)
        {
            var internal_settings = settings.AddSubCollection(Name + " Internal Settings", false, false);
            _Active = internal_settings.DefineSetting(nameof(_Active), false);
        }

        private void CornerIcon_Click(object sender, Blish_HUD.Input.MouseEventArgs e)
        {
            ToggleModule();
        }

        public abstract void LoadData();
        public virtual void ToggleModule()
        {
            Active = !Active;

            ScreenNotification.ShowNotification(string.Format(Strings.common.RunStateChange, Name, !Active ? Strings.common.Deactivated : Strings.common.Activated), ScreenNotification.NotificationType.Warning);

            this.Toggled?.Invoke(this, EventArgs.Empty);
        }

        public abstract void Update(GameTime gameTime);
        public virtual void UpdateLanguage(object sender, EventArgs e)
        {
            if (Hotbar_Button != null)
            {
                Hotbar_Button.BasicTooltipText = string.Format(Strings.common.Toggle, $"{Name}");
            }
        }
        public virtual void Dispose()
        {
            QoL.ModuleInstance.LanguageChanged -= UpdateLanguage;

            ModuleIcon = null;
            ModuleIconHovered = null;

            ModuleIcon_Active = null;
            ModuleIconHovered_Active = null;
        }
    }
}
