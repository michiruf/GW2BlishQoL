using Blish_HUD;
using Blish_HUD.Content;
using Blish_HUD.Controls;
using Blish_HUD.Input;
using Kenedia.Modules.QoL.Classes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kenedia.Modules.QoL.UI
{
    public class Hotbar_Button : Control
    {
        public SubModule SubModule;
        public int Index;

        protected override void Paint(SpriteBatch spriteBatch, Rectangle bounds)
        {
            var texture = SubModule.Active ? SubModule.ModuleIcon_Active : SubModule.ModuleIcon;
            if (MouseOver) texture = SubModule.Active ? SubModule.ModuleIconHovered_Active : SubModule.ModuleIconHovered;

            spriteBatch.DrawOnCtrl(this,
                                texture,
                                bounds,
                                SubModule.ModuleIcon.Bounds,
                                Color.White,
                                0f,
                                default);

        }

        protected override void OnClick(MouseEventArgs e)
        {
            base.OnClick(e);

            SubModule.ToggleModule();
        }

        protected override void DisposeControl()
        {
            base.DisposeControl();
            SubModule = null;
        }
    }

    public class Hotbar : Container
    {
        List<Hotbar_Button> SubControls = new List<Hotbar_Button>();
        FlowPanel FlowPanel;

        AsyncTexture2D Background;
        AsyncTexture2D Expand;
        AsyncTexture2D Expand_Hovered;

        bool Expanded;

        public Point ButtonSize = new Point(24, 24);
        Point ExpanderSize = new Point(32, 32);

        Rectangle ExpanderBounds = new Rectangle(0, 0, 32, 32);
        Rectangle TotalBounds = new Rectangle(0, 0, 32, 32);
        Point _PreSize = Point.Zero;

        public Hotbar()
        {
            var texture = QoL.ModuleInstance.TextureManager.getBackground(_Backgrounds.No3);
            Background = texture.GetRegion(25, 25, texture.Width - 25, texture.Height - 25);
            Expand = QoL.ModuleInstance.TextureManager.getIcon(_Icons.Expand);
            Expand_Hovered = QoL.ModuleInstance.TextureManager.getIcon(_Icons.Expand_Hovered);
            FlowPanel = new FlowPanel()
            {
                Parent = this,
                ControlPadding = new Vector2(2, 2),
                OuterControlPadding = new Vector2(4, 4),
                FlowDirection = ControlFlowDirection.LeftToRight,
                Location = new Point(0, 0),
            };
        }

        public void AddButton(Hotbar_Button btn)
        {
            btn.Parent = FlowPanel;
            btn.Visible = btn.SubModule.Active;
            btn.Size = ButtonSize;
            btn.Index = SubControls.Count;
            btn.SubModule.Toggled += SubModule_Toggled;
            SubControls.Add(btn);

            FlowPanel.Size = new Point((int) (FlowPanel.OuterControlPadding.X *2) + SubControls.Count * (ButtonSize.X + (int) FlowPanel.ControlPadding.X), Height);
            TotalBounds = new Rectangle(Point.Zero, new Point(FlowPanel.Size.X + ExpanderSize.X, Height));
        }

        private void SubModule_Toggled(object sender, EventArgs e)
        {
            var active = SubControls.Where(btn => btn.SubModule != null && btn.SubModule.Active).Count();
            FlowPanel.SortChildren<Hotbar_Button>((a, b) => (b.Visible.CompareTo(a.Visible)));

            Size = new Point(ExpanderSize.X + (active * ((int)(FlowPanel.ControlPadding.X) + ButtonSize.X)), Height);
            _PreSize = Size;
        }

        public void RemoveButton(Hotbar_Button btn)
        {
            if (SubControls.Contains(btn)) SubControls.Remove(btn);
            btn.SubModule.Toggled -= SubModule_Toggled;
            btn.Dispose();
        }

        public override void UpdateContainer(GameTime gameTime)
        {
            base.UpdateContainer(gameTime);

            Expanded = MouseOver || FlowPanel.MouseOver;
            if (Expanded)
            {

                if (TotalBounds.Size != Size)
                {
                    _PreSize = _PreSize == Point.Zero ? Size : _PreSize;
                    Size = TotalBounds.Size;

                    foreach (Hotbar_Button btn in SubControls)
                    {
                        btn.Visible = true;
                    }
                    FlowPanel.Invalidate();
                }
            }
            else if (_PreSize != Point.Zero)
            {
                Size = _PreSize;
                _PreSize = Point.Zero;

                foreach (Hotbar_Button btn in SubControls)
                {
                    btn.Visible = btn.SubModule.Active;
                }
                FlowPanel.SortChildren<Hotbar_Button>((a, b) => (b.Visible.CompareTo(a.Visible)));
                FlowPanel.Invalidate();
            }
        }

        public override void PaintBeforeChildren(SpriteBatch spriteBatch, Rectangle bounds)
        {
            base.PaintBeforeChildren(spriteBatch, bounds);

            var pad = 8;
            ExpanderBounds = new Rectangle(bounds.Right -  bounds.Height + pad, bounds.Y + (pad / 2), bounds.Height - pad, bounds.Height - pad);

            spriteBatch.DrawOnCtrl(this,
                                Background,
                                bounds,
                                bounds,
                                new Color(96, 96, 96, 105),
                                0f,
                                default);

            spriteBatch.DrawOnCtrl(this,
                                Expand,
                                ExpanderBounds,
                                Expand.Texture.Bounds,
                                Color.White,
                                0f,
                                default);

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

        public override void Draw(SpriteBatch spriteBatch, Rectangle drawBounds, Rectangle scissor)
        {
            base.Draw(spriteBatch, drawBounds, scissor);
        }

        protected override void DisposeControl()
        {
            base.DisposeControl();

            foreach (Hotbar_Button btn in SubControls)
            {
                if(btn.SubModule != null) btn.SubModule.Toggled -= SubModule_Toggled;
            }

            SubControls?.Clear();
            FlowPanel?.Dispose();

            Background = null;
        }
    }
}
