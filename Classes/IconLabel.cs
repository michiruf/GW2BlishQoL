using Blish_HUD;
using Blish_HUD.Controls;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kenedia.Modules.QoL.Classes
{
    class IconLabel : Control
    {
        public bool AutoSize;
        string _Text;
        public string Text
        {
            get => _Text;
            set
            {
                _Text = value;
            }
        }
        Texture2D _Texture;
        public Texture2D Texture
        {
            get => _Texture;
            set
            {
                _Texture = value;
            }
        }

        public override void DoUpdate(GameTime gameTime)
        {
            base.DoUpdate(gameTime);

            if (AutoSize)
            {
                var width = GameService.Content.DefaultFont14.GetStringRectangle(Text).Width + Height + 6;
                if((width) != Width)
                {
                    Width = (int) width;
                }

                var height = GameService.Content.DefaultFont14.GetStringRectangle(Text).Height + 6;
                if ((height) != Height)
                {
                    Height = (int)height;
                }
            }
        }

        protected override void Paint(SpriteBatch spriteBatch, Rectangle bounds)
        {
            if (Texture != null)
            {
                spriteBatch.DrawOnCtrl(this,
                                    Texture,
                                    new Rectangle(2, 2, Height - 4, Height - 4),
                                    Texture.Bounds,
                                    Color.LightGray,
                                    0f,
                                    default);
            }

            spriteBatch.DrawStringOnCtrl(this, Text, GameService.Content.DefaultFont14, new Rectangle(Height + 2, 2, Width - (Height + 4), Height - 4), Color.White, false, true, 1, HorizontalAlignment.Left, VerticalAlignment.Middle);
        }

        protected override void DisposeControl()
        {
            base.DisposeControl();
            Texture = null;
        }
    }
}
