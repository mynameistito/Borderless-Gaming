using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace BorderlessGaming.Logic.Core
{
    public static class ThemeManager
    {
        public static readonly Color DarkBackground = Color.FromArgb(32, 32, 32);
        public static readonly Color DarkSurface = Color.FromArgb(45, 45, 48);
        public static readonly Color DarkHover = Color.FromArgb(62, 62, 66);
        public static readonly Color DarkText = Color.FromArgb(240, 240, 240);
        public static readonly Color DarkBorder = Color.FromArgb(70, 70, 74);
        public static readonly Color DarkAccent = Color.FromArgb(0, 120, 215);

        private static readonly Color LightBackground = SystemColors.Control;
        private static readonly Color LightText = SystemColors.ControlText;
        private static readonly Color LightWindow = SystemColors.Window;

        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE_PRE_20H1 = 19;

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        public static bool IsDark { get; private set; }

        public static void Apply(Form form, bool dark)
        {
            if (form == null) return;
            IsDark = dark;
            SetTitleBarDark(form.Handle, dark);
            ApplyToControl(form, dark);
            ApplyMenuRenderer(form, dark);
        }

        private static void SetTitleBarDark(IntPtr hwnd, bool dark)
        {
            if (hwnd == IntPtr.Zero) return;
            int useDark = dark ? 1 : 0;
            if (DwmSetWindowAttribute(hwnd, DWMWA_USE_IMMERSIVE_DARK_MODE, ref useDark, sizeof(int)) != 0)
            {
                DwmSetWindowAttribute(hwnd, DWMWA_USE_IMMERSIVE_DARK_MODE_PRE_20H1, ref useDark, sizeof(int));
            }
        }

        private static void ApplyToControl(Control ctrl, bool dark)
        {
            var bg = dark ? DarkBackground : LightBackground;
            var fg = dark ? DarkText : LightText;
            var surface = dark ? DarkSurface : LightWindow;

            ctrl.BackColor = bg;
            ctrl.ForeColor = fg;

            switch (ctrl)
            {
                case ListBox lb:
                    lb.BackColor = surface;
                    lb.ForeColor = fg;
                    lb.BorderStyle = dark ? BorderStyle.FixedSingle : BorderStyle.Fixed3D;
                    break;
                case TextBox tb:
                    tb.BackColor = surface;
                    tb.ForeColor = fg;
                    tb.BorderStyle = dark ? BorderStyle.FixedSingle : BorderStyle.Fixed3D;
                    break;
                case Button btn:
                    btn.BackColor = dark ? DarkSurface : LightBackground;
                    btn.ForeColor = fg;
                    btn.FlatStyle = dark ? FlatStyle.Flat : FlatStyle.Standard;
                    btn.FlatAppearance.BorderColor = dark ? DarkBorder : SystemColors.ControlDark;
                    break;
                case LinkLabel ll:
                    ll.LinkColor = dark ? Color.FromArgb(102, 178, 255) : Color.Blue;
                    ll.ActiveLinkColor = dark ? Color.FromArgb(153, 204, 255) : Color.Red;
                    ll.VisitedLinkColor = dark ? Color.FromArgb(170, 130, 255) : Color.Purple;
                    break;
                case MenuStrip ms:
                    ms.BackColor = bg;
                    ms.ForeColor = fg;
                    break;
            }

            foreach (Control child in ctrl.Controls)
            {
                ApplyToControl(child, dark);
            }
        }

        private static void ApplyMenuRenderer(Form form, bool dark)
        {
            ToolStripManager.Renderer = dark
                ? (ToolStripRenderer)new DarkToolStripRenderer()
                : new ToolStripProfessionalRenderer();

            foreach (Control c in EnumerateAll(form))
            {
                if (c is ToolStrip ts)
                {
                    ts.Renderer = ToolStripManager.Renderer;
                    StyleToolStripItems(ts.Items, dark);
                }
                if (c is ListBox lb && dark)
                {
                    lb.DrawMode = DrawMode.OwnerDrawFixed;
                    lb.DrawItem -= DarkListBoxDrawItem;
                    lb.DrawItem += DarkListBoxDrawItem;
                }
                else if (c is ListBox lb2 && !dark)
                {
                    lb2.DrawMode = DrawMode.Normal;
                    lb2.DrawItem -= DarkListBoxDrawItem;
                }
            }
        }

        private static void DarkListBoxDrawItem(object sender, DrawItemEventArgs e)
        {
            var lb = (ListBox)sender;
            var selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            var bg = selected ? DarkAccent : DarkSurface;
            var fg = DarkText;
            using (var brush = new SolidBrush(bg))
            {
                e.Graphics.FillRectangle(brush, e.Bounds);
            }
            if (e.Index >= 0 && e.Index < lb.Items.Count)
            {
                var text = lb.GetItemText(lb.Items[e.Index]);
                TextRenderer.DrawText(e.Graphics, text, e.Font, e.Bounds, fg, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
            }
            e.DrawFocusRectangle();
        }

        private static IEnumerable<Control> EnumerateAll(Control root)
        {
            foreach (Control c in root.Controls)
            {
                yield return c;
                foreach (var sub in EnumerateAll(c)) yield return sub;
            }
        }

        private static void StyleToolStripItems(ToolStripItemCollection items, bool dark)
        {
            var bg = dark ? DarkSurface : SystemColors.Control;
            var fg = dark ? DarkText : SystemColors.ControlText;
            foreach (ToolStripItem item in items)
            {
                item.BackColor = bg;
                item.ForeColor = fg;
                if (item is ToolStripDropDownItem ddi)
                {
                    ddi.DropDown.BackColor = bg;
                    ddi.DropDown.ForeColor = fg;
                    StyleToolStripItems(ddi.DropDownItems, dark);
                }
            }
        }

    }

    internal class DarkToolStripRenderer : ToolStripProfessionalRenderer
    {
        public DarkToolStripRenderer() : base(new DarkColorTable()) { RoundedEdges = false; }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            e.TextColor = ThemeManager.DarkText;
            base.OnRenderItemText(e);
        }

        protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
        {
            e.ArrowColor = ThemeManager.DarkText;
            base.OnRenderArrow(e);
        }

        protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
        {
            using (var brush = new global::System.Drawing.SolidBrush(ThemeManager.DarkAccent))
            {
                e.Graphics.FillRectangle(brush, e.ImageRectangle);
            }
            using (var pen = new global::System.Drawing.Pen(ThemeManager.DarkText, 2))
            {
                var r = e.ImageRectangle;
                e.Graphics.DrawLines(pen, new[]
                {
                    new global::System.Drawing.Point(r.Left + 3, r.Top + r.Height / 2),
                    new global::System.Drawing.Point(r.Left + r.Width / 2 - 1, r.Bottom - 4),
                    new global::System.Drawing.Point(r.Right - 3, r.Top + 3),
                });
            }
        }
    }

    internal class DarkColorTable : ProfessionalColorTable
    {
        public override Color MenuItemSelected => ThemeManager.DarkHover;
        public override Color MenuItemSelectedGradientBegin => ThemeManager.DarkHover;
        public override Color MenuItemSelectedGradientEnd => ThemeManager.DarkHover;
        public override Color MenuItemBorder => ThemeManager.DarkAccent;
        public override Color MenuItemPressedGradientBegin => ThemeManager.DarkHover;
        public override Color MenuItemPressedGradientEnd => ThemeManager.DarkHover;
        public override Color MenuBorder => ThemeManager.DarkBorder;
        public override Color ToolStripDropDownBackground => ThemeManager.DarkSurface;
        public override Color ImageMarginGradientBegin => ThemeManager.DarkSurface;
        public override Color ImageMarginGradientMiddle => ThemeManager.DarkSurface;
        public override Color ImageMarginGradientEnd => ThemeManager.DarkSurface;
        public override Color SeparatorDark => ThemeManager.DarkBorder;
        public override Color SeparatorLight => ThemeManager.DarkBorder;
        public override Color CheckBackground => ThemeManager.DarkAccent;
        public override Color CheckSelectedBackground => ThemeManager.DarkAccent;
        public override Color CheckPressedBackground => ThemeManager.DarkAccent;
        public override Color ToolStripBorder => ThemeManager.DarkBorder;
        public override Color ToolStripGradientBegin => ThemeManager.DarkBackground;
        public override Color ToolStripGradientMiddle => ThemeManager.DarkBackground;
        public override Color ToolStripGradientEnd => ThemeManager.DarkBackground;
    }
}
