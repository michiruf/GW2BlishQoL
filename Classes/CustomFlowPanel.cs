using Blish_HUD;
using Blish_HUD.Controls;
using Blish_HUD.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kenedia.Modules.QoL
{
    public class CustomFlowPanel : FlowPanel
    {
        bool Dragging;
        Point DraggingStart;
        public Texture2D Background;

        protected override void OnLeftMouseButtonPressed(MouseEventArgs e)
        {
            base.OnLeftMouseButtonPressed(e);
            Dragging = Input.Keyboard.ActiveModifiers == Microsoft.Xna.Framework.Input.ModifierKeys.Alt;
            DraggingStart = Dragging ? RelativeMousePosition : Point.Zero;
        }
        
        protected override void OnLeftMouseButtonReleased(MouseEventArgs e)
        {
            base.OnLeftMouseButtonPressed(e);
            Dragging = false;
        }

        public override void UpdateContainer(GameTime gameTime)
        {
            base.UpdateContainer(gameTime);

            if (Dragging)
            {
                Location = Input.Mouse.Position.Add(new Point(-DraggingStart.X, -DraggingStart.Y));
            }
        }

        public override void PaintBeforeChildren(SpriteBatch spriteBatch, Rectangle bounds)
        {
            base.PaintBeforeChildren(spriteBatch, bounds);

            if (false)
            {
                if (Background != null)
                {
                    spriteBatch.DrawOnCtrl(this,
                                        Background,
                                        bounds,
                                        Background.Bounds,
                                        new Color(96, 96, 96, 105),
                                        0f,
                                        default);
                }

                var color = Color.Black;
                var rect = bounds;

                //Top
                spriteBatch.DrawOnCtrl(this, ContentService.Textures.Pixel, new Rectangle(rect.Left, rect.Top, rect.Width, 2), Rectangle.Empty, color * 0.5f);
                spriteBatch.DrawOnCtrl(this, ContentService.Textures.Pixel, new Rectangle(rect.Left, rect.Top, rect.Width, 1), Rectangle.Empty, color * 0.6f);

                //Bottom
                spriteBatch.DrawOnCtrl(this, ContentService.Textures.Pixel, new Rectangle(rect.Left, rect.Bottom - 2, rect.Width, 2), Rectangle.Empty, color * 0.5f);
                spriteBatch.DrawOnCtrl(this, ContentService.Textures.Pixel, new Rectangle(rect.Left, rect.Bottom - 1, rect.Width, 1), Rectangle.Empty, color * 0.6f);

                //Left
                spriteBatch.DrawOnCtrl(this, ContentService.Textures.Pixel, new Rectangle(rect.Left, rect.Top, 2, rect.Height), Rectangle.Empty, color * 0.5f);
                spriteBatch.DrawOnCtrl(this, ContentService.Textures.Pixel, new Rectangle(rect.Left, rect.Top, 1, rect.Height), Rectangle.Empty, color * 0.6f);

                //Right
                spriteBatch.DrawOnCtrl(this, ContentService.Textures.Pixel, new Rectangle(rect.Right - 2, rect.Top, 2, rect.Height), Rectangle.Empty, color * 0.5f);
                spriteBatch.DrawOnCtrl(this, ContentService.Textures.Pixel, new Rectangle(rect.Right - 1, rect.Top, 1, rect.Height), Rectangle.Empty, color * 0.6f);
            }
        }

        protected override void DisposeControl()
        {
            base.DisposeControl();

            Background = null;
        }
    }
}
