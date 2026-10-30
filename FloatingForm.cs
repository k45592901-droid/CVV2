using System.Drawing;
namespace PersianScreenTranslator;
public sealed class FloatingForm : Form
{
    const int WM_NCHITTEST=0x84, HTCAPTION=2;
    string caption="متن شناسایی‌شده اینجا نمایش داده می‌شود";
    public Color TextColor {get;set;}=Color.White;
    public float TextSize {get;set;}=22;
    public int BackgroundOpacity {get;set;}=0;
    public FloatingForm()
    {
        Text="زیرنویس شناور"; FormBorderStyle=FormBorderStyle.None; ShowInTaskbar=false; TopMost=true;
        StartPosition=FormStartPosition.Manual; Bounds=new Rectangle(100,650,900,150);
        BackColor=Color.Magenta; TransparencyKey=Color.Magenta; DoubleBuffered=true;
    }
    public void SetCaption(string text){if(IsDisposed)return; if(InvokeRequired){BeginInvoke(()=>SetCaption(text));return;} caption=text;Invalidate();}
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e); if(BackgroundOpacity>0){using var bg=new SolidBrush(Color.FromArgb(Math.Clamp(BackgroundOpacity,0,220),Color.Black));e.Graphics.FillRectangle(bg,ClientRectangle);}
        using var font=new Font("Segoe UI",TextSize,FontStyle.Bold,GraphicsUnit.Pixel);
        using var brush=new SolidBrush(TextColor); using var shadow=new SolidBrush(Color.FromArgb(210,0,0,0));
        var rect=new RectangleF(14,10,ClientSize.Width-28,ClientSize.Height-20);
        var sf=new StringFormat{Alignment=StringAlignment.Center,LineAlignment=StringAlignment.Center,Trimming=StringTrimming.EllipsisWord};
        var shadowRect=rect; shadowRect.Offset(2,2); e.Graphics.DrawString(caption,font,shadow,shadowRect,sf); e.Graphics.DrawString(caption,font,brush,rect,sf);
    }
    protected override void WndProc(ref Message m){if(m.Msg==WM_NCHITTEST){base.WndProc(ref m);m.Result=(IntPtr)HTCAPTION;return;}base.WndProc(ref m);}
}
