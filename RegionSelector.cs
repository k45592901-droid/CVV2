using System;
using System.Drawing;
using System.Windows.Forms;

namespace PersianScreenTranslator;

public sealed class RegionSelector : Form
{
    private Point start;
    private bool dragging;
    public Rectangle SelectedBounds { get; private set; }

    public RegionSelector()
    {
        FormBorderStyle = FormBorderStyle.None;
        WindowState = FormWindowState.Maximized;
        TopMost = true;
        ShowInTaskbar = false;
        DoubleBuffered = true;
        BackColor = Color.Black;
        Opacity = 0.28;
        Cursor = Cursors.Cross;
        KeyPreview = true;
        KeyDown += (_, e) => { if (e.KeyCode == Keys.Escape) DialogResult = DialogResult.Cancel; };
        MouseDown += (_, e) => { if (e.Button == MouseButtons.Left) { start = PointToScreen(e.Location); dragging = true; } };
        MouseMove += (_, e) => { if (dragging) { SelectedBounds = MakeRect(start, PointToScreen(e.Location)); Invalidate(); } };
        MouseUp += (_, e) =>
        {
            if (e.Button == MouseButtons.Left && dragging)
            {
                dragging = false;
                SelectedBounds = MakeRect(start, PointToScreen(e.Location));
                DialogResult = DialogResult.OK;
            }
        };
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        if (SelectedBounds.Width > 0 && SelectedBounds.Height > 0)
        {
            var local = RectangleToClient(SelectedBounds);
            using var pen = new Pen(Color.Lime, 3);
            e.Graphics.DrawRectangle(pen, local);
        }
        using var brush = new SolidBrush(Color.White);
        e.Graphics.DrawString("برای انتخاب، ماوس را بکشید — برای لغو Esc", Font, brush, 20, 20);
    }

    private static Rectangle MakeRect(Point a, Point b) =>
        Rectangle.FromLTRB(Math.Min(a.X,b.X), Math.Min(a.Y,b.Y), Math.Max(a.X,b.X), Math.Max(a.Y,b.Y));
}
