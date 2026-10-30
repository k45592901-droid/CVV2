using System.Drawing;
namespace PersianScreenTranslator;
public sealed class RegionSelector : Form
{
    Point start; bool dragging;
    public Rectangle SelectedBounds { get; private set; }
    public RegionSelector()
    {
        FormBorderStyle=FormBorderStyle.None; WindowState=FormWindowState.Maximized; TopMost=true; ShowInTaskbar=false; DoubleBuffered=true;
        BackColor=Color.Black; Opacity=.30; Cursor=Cursors.Cross; KeyPreview=true;
        KeyDown+=(_,e)=>{if(e.KeyCode==Keys.Escape) DialogResult=DialogResult.Cancel;};
        MouseDown+=(_,e)=>{if(e.Button==MouseButtons.Left){start=PointToScreen(e.Location);dragging=true;}};
        MouseMove+=(_,e)=>{if(dragging){SelectedBounds=Make(start,PointToScreen(e.Location));Invalidate();}};
        MouseUp+=(_,e)=>{if(e.Button==MouseButtons.Left&&dragging){dragging=false;SelectedBounds=Make(start,PointToScreen(e.Location));DialogResult=DialogResult.OK;}};
    }
    protected override void OnPaint(PaintEventArgs e){base.OnPaint(e);using var b=new SolidBrush(Color.White);e.Graphics.DrawString("محدودهٔ بازی را با ماوس انتخاب کن — برای لغو Esc",new Font("Segoe UI",16,FontStyle.Bold),b,20,20);if(SelectedBounds.Width>0&&SelectedBounds.Height>0){var r=RectangleToClient(SelectedBounds);using var p=new Pen(Color.Lime,3);e.Graphics.DrawRectangle(p,r);}}
    static Rectangle Make(Point a,Point b)=>Rectangle.FromLTRB(Math.Min(a.X,b.X),Math.Min(a.Y,b.Y),Math.Max(a.X,b.X),Math.Max(a.Y,b.Y));
}
