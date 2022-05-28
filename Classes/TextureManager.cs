using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Blish_HUD.Content;
using Blish_HUD.Modules.Managers;
using Blish_HUD;

namespace Kenedia.Modules.QoL
{
    public enum _Controls
    {
        Delete = 25,
        Delete_Hovered,
    }
    public enum _Icons
    {
        Bug,
        ModuleIcon,
        ModuleIcon_Hovered,
        ModuleIcon_HoveredWhite,
        ModuleIcon_Active,
        ModuleIcon_Active_Hovered,
        ModuleIcon_Active_HoveredWhite,
        Expand,
        Expand_Hovered,
    }
    public enum _Emblems
    {
        QuestionMark,
    }
    public enum _Backgrounds
    {
        MainWindow,
        Tooltip,
        No3,
    }

    public class TextureManager : IDisposable
    {
        public IDictionary<string, AsyncTexture2D> Textures = new Dictionary<string, AsyncTexture2D>();
        private ContentsManager ContentsManager;
        public AsyncTexture2D PlaceHolder;

        public TextureManager()
        {
            ContentsManager = QoL.ModuleInstance.ContentsManager;

            PlaceHolder = ContentsManager.GetTexture(@"textures\icons\0.png");
        }

        private bool disposed = false;
        public void Dispose()
        {
            if (!disposed)
            {
                disposed = true;
                PlaceHolder.Dispose();
                foreach (KeyValuePair<string, AsyncTexture2D> texture2D in Textures) { texture2D.Value?.Dispose(); }
            }
        }


        public Texture2D get(string subfolder, int e)
        {
            var key = $@"textures\{subfolder}\" + e + ".png";
            if (!Textures.ContainsKey(key))
            {
                var texture = ContentsManager.GetTexture(key);
                Textures[key] = new AsyncTexture2D(texture);
                return Textures[key];
            }
            else
            {
                return Textures[key];
            }
        }


        public Texture2D getBackground(_Backgrounds e = 0) => getBackground(default, e);
        public Texture2D getBackground(string subfolder = default, _Backgrounds e = 0)
        {
            subfolder = subfolder == default ? "" : subfolder + @"\";
            var key = $@"textures\backgrounds\{subfolder}" + (int)e + ".png";

            if (!Textures.ContainsKey(key))
            {
                var texture = ContentsManager.GetTexture(key);
                Textures[key] = new AsyncTexture2D(texture);
                return Textures[key];
            }
            else
            {
                return Textures[key];
            }
        }

        public Texture2D getIcon(_Icons e = 0) => getIcon(default, e);        
        public Texture2D getIcon(string subfolder = default, _Icons e = 0)
        {
            subfolder = subfolder == default ? "" : subfolder + @"\";
            var key = $@"textures\icons\{subfolder}" + (int)e + ".png";
            QoL.Logger.Debug(key);
            if (!Textures.ContainsKey(key))
            {
                var texture = ContentsManager.GetTexture(key);
                Textures[key] = new AsyncTexture2D(texture);
                return Textures[key];
            }
            else
            {
                return Textures[key];
            }
        }

        public Texture2D getEmblem(_Emblems e = 0) => getEmblem(default, e);
        public Texture2D getEmblem(string subfolder = default, _Emblems e = 0)
        {
            subfolder = subfolder == default ? "" : subfolder + @"\";
            var key = $@"textures\emblems\{subfolder}" + (int)e + ".png";

            if (!Textures.ContainsKey(key))
            {
                var texture = ContentsManager.GetTexture(key);
                Textures[key] = new AsyncTexture2D(texture);
                return Textures[key];
            }
            else
            {
                return Textures[key];
            }
        }

        public Texture2D getControl(_Controls e = 0) => getControl(default, e);
        public Texture2D getControl(string subfolder = "controls", _Controls e = 0)
        {
            subfolder = subfolder == default ? "" : subfolder + @"\";
            var key = $@"textures\controls\{subfolder}" + (int)e + ".png";

            if (!Textures.ContainsKey(key))
            {
                var texture = ContentsManager.GetTexture(key);
                Textures[key] = new AsyncTexture2D(texture);
                return Textures[key];
            }
            else
            {
                return Textures[key];
            }
        }
    }
}
