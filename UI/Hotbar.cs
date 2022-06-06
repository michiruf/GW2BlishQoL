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
    public enum ExpandDirection
    {
        LeftToRight,
        RightToLeft,
        TopToBottom,
        BottomToTop,
    }

    public class Hotbar_Button : Control
    {
        public SubModule SubModule;
        public int Index;

        protected override void Paint(SpriteBatch spriteBatch, Rectangle bounds)
        {
            var texture = SubModule.Active ? SubModule.ModuleIcon_Active : SubModule.ModuleIcon;
            if (MouseOver) texture = SubModule.Active ? SubModule.ModuleIconHovered_Active : SubModule.ModuleIconHovered;

            if (texture != null)
            {
                spriteBatch.DrawOnCtrl(this,
                                    texture,
                                    bounds,
                                    SubModule.ModuleIcon.Bounds,
                                    Color.White,
                                    0f,
                                    default);
            }
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
        class Sizes
        {
            public Point Collapsed = Point.Zero;
            public Point Expanded = Point.Zero;
            public Point Delta = Point.Zero;
            public bool isExpanded = false;
            public Point Size
            {
                get
                {
                    return isExpanded ? Expanded : Collapsed;
                }
            }
        }

        List<Hotbar_Button> SubControls = new List<Hotbar_Button>();
        FlowPanel FlowPanel;

        AsyncTexture2D Background;
        AsyncTexture2D Expand;
        AsyncTexture2D Expand_Hovered;

        event EventHandler ExpandDirectionChanged;
        ExpandDirection _ExpandDirection;
        public ExpandDirection ExpandDirection
        {
            get => _ExpandDirection;
            set
            {
                OnExpandDirectionChanged(this, null);
                if (value != _ExpandDirection)
                {
                    if (FlowPanel != null)
                    {
                        switch (value)
                        {
                            case ExpandDirection.LeftToRight:
                                FlowPanel.FlowDirection = ControlFlowDirection.SingleLeftToRight;
                                FlowPanel.Location = new Point(0,0);
                                break;

                            case ExpandDirection.RightToLeft:
                                FlowPanel.FlowDirection = ControlFlowDirection.SingleRightToLeft;
                                FlowPanel.Location = new Point(ExpanderSize.X, 0);
                                break;

                            case ExpandDirection.TopToBottom:
                                FlowPanel.FlowDirection = ControlFlowDirection.SingleTopToBottom;
                                FlowPanel.Location = new Point(0,0);
                                break;

                            case ExpandDirection.BottomToTop:
                                FlowPanel.FlowDirection = ControlFlowDirection.SingleBottomToTop;
                                FlowPanel.Location = new Point(0, ExpanderSize.X);
                                break;
                        }

                    }

                    _ExpandDirection = value;
                    AdjustSize();
                }
            }
        }

        Sizes FlowPanelSizes = new Sizes();
        Sizes HotBarSizes = new Sizes();


        bool Expanded;
        bool Dragging;
        Point DraggingStart;
        Point DraggingDestination;

        int BtnPadding = 8;
        public Point CollapsedSize
        {
            get => HotBarSizes.Collapsed;
        }
        public Point ButtonSize = new Point(24, 24);
        Point ExpanderSize = new Point(32, 32);

        Rectangle ExpanderBounds = new Rectangle(0, 0, 32, 32);
        Point _PreSize = Point.Zero;
        Point _PreLocation = Point.Zero;

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

            FlowPanelSizes.Collapsed = FlowPanel.Size;
            FlowPanelSizes.Expanded = FlowPanel.Size;

            HotBarSizes.Collapsed = Size;
            HotBarSizes.Expanded = Size;

            ExpandDirection = ExpandDirection.BottomToTop;
        }

        void OnExpandDirectionChanged(object sender, EventArgs e)
        {
            this.ExpandDirectionChanged?.Invoke(this, EventArgs.Empty);
            AdjustSize();
        }

        public void AddButton(Hotbar_Button btn)
        {
            btn.Parent = FlowPanel;
            btn.Visible = btn.SubModule.Active;
            btn.Size = ButtonSize;
            btn.Index = SubControls.Count;
            btn.SubModule.Toggled += SubModule_Toggled;
            SubControls.Add(btn);

            AdjustSize();

            SubModule_Toggled(null, null);
        }

        void AdjustSize()
        {
            var size = ExpanderSize.X + (BtnPadding / 2);
            var visible = SubControls.Where(e => e.SubModule.Active).Count();

            switch (ExpandDirection)
            {
                case ExpandDirection.LeftToRight:
                    FlowPanelSizes.Collapsed = new Point(visible * (ButtonSize.X + (int)FlowPanel.ControlPadding.X), ExpanderSize.X);
                    FlowPanelSizes.Expanded = new Point(SubControls.Count * (ButtonSize.X + (int)FlowPanel.ControlPadding.X), ExpanderSize.X);
                    FlowPanelSizes.Delta = new Point(FlowPanelSizes.Expanded.X - FlowPanelSizes.Collapsed.X, FlowPanelSizes.Expanded.Y - FlowPanelSizes.Collapsed.Y);

                    HotBarSizes.Collapsed = new Point(ExpanderSize.X + FlowPanelSizes.Collapsed.X, size);
                    HotBarSizes.Expanded = new Point(ExpanderSize.X + FlowPanelSizes.Expanded.X, size);
                    HotBarSizes.Delta = new Point(HotBarSizes.Expanded.X - HotBarSizes.Collapsed.X, HotBarSizes.Expanded.Y - HotBarSizes.Collapsed.Y);
                    break;

                case ExpandDirection.RightToLeft:
                    FlowPanelSizes.Collapsed = new Point(visible * (ButtonSize.X + (int)FlowPanel.ControlPadding.X), size);
                    FlowPanelSizes.Expanded = new Point(SubControls.Count * (ButtonSize.X + (int)FlowPanel.ControlPadding.X), size);
                    FlowPanelSizes.Delta = new Point(FlowPanelSizes.Expanded.X - FlowPanelSizes.Collapsed.X, FlowPanelSizes.Expanded.Y - FlowPanelSizes.Collapsed.Y);

                    HotBarSizes.Collapsed = new Point(ExpanderSize.X + FlowPanelSizes.Collapsed.X, size);
                    HotBarSizes.Expanded = new Point(ExpanderSize.X + FlowPanelSizes.Expanded.X, size);
                    HotBarSizes.Delta = new Point(HotBarSizes.Expanded.X - HotBarSizes.Collapsed.X, HotBarSizes.Expanded.Y - HotBarSizes.Collapsed.Y);
                    break;

                case ExpandDirection.TopToBottom:
                    FlowPanelSizes.Collapsed = new Point(size, visible * (ButtonSize.Y + (int)FlowPanel.ControlPadding.Y));
                    FlowPanelSizes.Expanded = new Point(size, SubControls.Count * (ButtonSize.Y + (int)FlowPanel.ControlPadding.Y));
                    FlowPanelSizes.Delta = new Point(FlowPanelSizes.Expanded.X - FlowPanelSizes.Collapsed.X, FlowPanelSizes.Expanded.Y - FlowPanelSizes.Collapsed.Y);

                    HotBarSizes.Collapsed = new Point(size, ExpanderSize.Y + FlowPanelSizes.Collapsed.Y);
                    HotBarSizes.Expanded = new Point(size, ExpanderSize.Y + FlowPanelSizes.Expanded.Y);
                    HotBarSizes.Delta = new Point(HotBarSizes.Expanded.X - HotBarSizes.Collapsed.X, HotBarSizes.Expanded.Y - HotBarSizes.Collapsed.Y);
                    break;

                case ExpandDirection.BottomToTop:
                    FlowPanelSizes.Collapsed = new Point(size, visible * (ButtonSize.Y + (int)FlowPanel.ControlPadding.Y));
                    FlowPanelSizes.Expanded = new Point(size, SubControls.Count * (ButtonSize.Y + (int)FlowPanel.ControlPadding.Y));
                    FlowPanelSizes.Delta = new Point(FlowPanelSizes.Expanded.X - FlowPanelSizes.Collapsed.X, FlowPanelSizes.Expanded.Y - FlowPanelSizes.Collapsed.Y);

                    HotBarSizes.Collapsed = new Point(size, ExpanderSize.Y + FlowPanelSizes.Collapsed.Y);
                    HotBarSizes.Expanded = new Point(size, ExpanderSize.Y + FlowPanelSizes.Expanded.Y);
                    HotBarSizes.Delta = new Point(HotBarSizes.Expanded.X - HotBarSizes.Collapsed.X, HotBarSizes.Expanded.Y - HotBarSizes.Collapsed.Y);
                    break;
            }
        }

        private void SubModule_Toggled(object sender, EventArgs e)
        {
            FlowPanel.SortChildren<Hotbar_Button>((a, b) => (b.Visible.CompareTo(a.Visible)));
            var submodule = (SubModule)sender;

            if (_PreLocation != Point.Zero && MouseOver)
            {
                switch (submodule.Active)
                {
                    case true:
                        if (ExpandDirection == ExpandDirection.BottomToTop)
                        {
                            _PreLocation = new Point(_PreLocation.X, _PreLocation.Y - (ButtonSize.Y + (int)FlowPanel.ControlPadding.Y));
                        }
                        else if (ExpandDirection == ExpandDirection.RightToLeft)
                        {
                            _PreLocation = new Point(_PreLocation.X - (ButtonSize.X + (int)FlowPanel.ControlPadding.X), _PreLocation.Y);
                        }
                        break;

                    case false:
                        if (ExpandDirection == ExpandDirection.BottomToTop)
                        {
                            _PreLocation = new Point(_PreLocation.X, _PreLocation.Y + (ButtonSize.Y + (int)FlowPanel.ControlPadding.Y));
                        }
                        else if (ExpandDirection == ExpandDirection.RightToLeft)
                        {
                            _PreLocation = new Point(_PreLocation.X + (ButtonSize.X + (int)FlowPanel.ControlPadding.X), _PreLocation.Y);
                        }
                        break;
                }
            }
        }

        public void RemoveButton(Hotbar_Button btn)
        {
            if (SubControls.Contains(btn)) SubControls.Remove(btn);
            btn.SubModule.Toggled -= SubModule_Toggled;
            btn.Dispose();
        }

        protected override void OnLeftMouseButtonPressed(MouseEventArgs e)
        {
            base.OnLeftMouseButtonPressed(e);
            Dragging = Input.Keyboard.ActiveModifiers == Microsoft.Xna.Framework.Input.ModifierKeys.Alt;
            DraggingStart = Dragging ? RelativeMousePosition : Point.Zero;
        }

        protected override void OnLeftMouseButtonReleased(MouseEventArgs e)
        {
            base.OnLeftMouseButtonReleased(e);
            Dragging = false;
        }

        public override void UpdateContainer(GameTime gameTime)
        {
            base.UpdateContainer(gameTime);

            Dragging = Dragging && MouseOver;

            Expanded = MouseOver || FlowPanel.MouseOver || Input.Keyboard.ActiveModifiers == Microsoft.Xna.Framework.Input.ModifierKeys.Alt;
            FlowPanelSizes.isExpanded = Expanded;
            HotBarSizes.isExpanded = Expanded;

            AdjustSize();

            if (Dragging)
            {
                if (ExpandDirection == ExpandDirection.RightToLeft)
                {
                    Location = Input.Mouse.Position.Add(new Point(-DraggingStart.X, -DraggingStart.Y));
                    _PreLocation = Input.Mouse.Position.Add(new Point(-DraggingStart.X + HotBarSizes.Delta.X, -DraggingStart.Y));
                }
                else
                {
                    Location = Input.Mouse.Position.Add(new Point(-DraggingStart.X, -DraggingStart.Y));
                    _PreLocation = Input.Mouse.Position.Add(new Point(-DraggingStart.X, -DraggingStart.Y + HotBarSizes.Delta.Y));
                }
            }

            if (Expanded)
            {
                if (HotBarSizes.Size != Size)
                {
                    _PreLocation = _PreLocation == Point.Zero ? Location : _PreLocation;

                    foreach (Hotbar_Button btn in SubControls)
                    {
                        btn.Visible = true;
                    }
                    FlowPanel.Invalidate();

                    FlowPanel.Size = FlowPanelSizes.Size;
                    Size = HotBarSizes.Size;

                    if (ExpandDirection == ExpandDirection.BottomToTop)
                    {
                        Location = new Point(_PreLocation.X, _PreLocation.Y - HotBarSizes.Delta.Y);
                    }
                    else if (ExpandDirection == ExpandDirection.RightToLeft)
                    {
                        Location = new Point(_PreLocation.X - HotBarSizes.Delta.X, _PreLocation.Y);
                    }
                }
            }
            else if (HotBarSizes.Size != Size)
            {
                foreach (Hotbar_Button btn in SubControls)
                {
                    btn.Visible = btn.SubModule.Active;
                }
                FlowPanel.SortChildren<Hotbar_Button>((a, b) => (b.Visible.CompareTo(a.Visible)));
                FlowPanel.Invalidate();

                FlowPanel.Size = FlowPanelSizes.Size;
                Size = HotBarSizes.Size;

                if (_PreLocation != Point.Zero)
                {
                    if (ExpandDirection == ExpandDirection.BottomToTop)
                    {
                        Location = _PreLocation;
                    }
                    else if (ExpandDirection == ExpandDirection.RightToLeft)
                    {
                        Location = _PreLocation;
                    }
                    _PreLocation = Point.Zero;
                }
            }
        }

        public override void PaintBeforeChildren(SpriteBatch spriteBatch, Rectangle bounds)
        {
            base.PaintBeforeChildren(spriteBatch, bounds);

            var pad = 8;
            float rotation = 0f;
            var expanderSize = ExpanderSize.X;

            switch (ExpandDirection)
            {
                case ExpandDirection.LeftToRight:
                    ExpanderBounds = new Rectangle(bounds.Right - expanderSize + 6, bounds.Y + 6, expanderSize - pad, expanderSize - pad);
                    rotation = 0f;
                    break;

                case ExpandDirection.RightToLeft:
                    ExpanderBounds = new Rectangle(bounds.Left + expanderSize - (pad / 2), bounds.Y + expanderSize - 2, expanderSize - pad, expanderSize - pad);
                    rotation = 3.15f;
                    break;

                case ExpandDirection.TopToBottom:
                    ExpanderBounds = new Rectangle(expanderSize - 2, Height - expanderSize + (pad / 2), expanderSize - pad, expanderSize - pad);
                    rotation = 1.55f;
                    break;

                case ExpandDirection.BottomToTop:
                    ExpanderBounds = new Rectangle(6, expanderSize - 4, expanderSize - pad, expanderSize - pad);
                    rotation = -1.55f;

                    break;

            }

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
                                rotation,
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
                if (btn.SubModule != null) btn.SubModule.Toggled -= SubModule_Toggled;
            }

            SubControls?.Clear();
            FlowPanel?.Dispose();

            Background = null;
        }
    }
}
