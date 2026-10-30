using System.Drawing;
namespace PersianScreenTranslator;
public sealed class MainForm : Form
{
    readonly Button choose=new(){Text="انتخاب محدودهٔ بازی",AutoSize=true};
    readonly Button full=new(){Text="انتخاب کل صفحه",AutoSize=true};
    readonly Button start=new(){Text="شروع خواندن خودکار",AutoSize=true};
    readonly Button stop=new(){Text="توقف",AutoSize=true,Enabled=false};
    readonly Button color=new(){Text="رنگ متن",AutoSize=true};
    readonly NumericUpDown size=new(){Minimum=14,Maximum=64,Value=26,Width=70};
    readonly TrackBar opacity=new(){Minimum=0,Maximum=200,Value=0,TickFrequency=50,Width=130};
    readonly Label status=new(){AutoSize=true,Text="ابتدا محدودهٔ بازی را انتخاب کن."};
    readonly System.Windows.Forms.Timer timer=new(){Interval=1200};
    readonly FloatingForm overlay=new();
    Rectangle captureBounds; bool busy; string previous="";
    public MainForm()
    {
        Text="مترجم خودکار بازی — ترجمهٔ آنلاین آزمایشی";Width=590;Height=245;StartPosition=FormStartPosition.CenterScreen;RightToLeft=RightToLeft.Yes;RightToLeftLayout=true;
        var p=new FlowLayoutPanel{Dock=DockStyle.Fill,Padding=new Padding(14),FlowDirection=FlowDirection.RightToLeft,WrapContents=true};
        p.Controls.AddRange(new Control[]{choose,full,start,stop,new Label{Text="اندازه متن:",AutoSize=true,Padding=new Padding(4,8,0,0)},size,color,new Label{Text="شفافیت زمینه:",AutoSize=true,Padding=new Padding(4,8,0,0)},opacity,status});Controls.Add(p);
        choose.Click+=async(_,_)=>await PickRegion(); full.Click+=(_,_)=>{captureBounds=Screen.PrimaryScreen?.Bounds??Rectangle.Empty;status.Text="کل صفحه انتخاب شد.";};
        start.Click+=(_,_)=>{if(captureBounds.IsEmpty){status.Text="اول محدوده یا کل صفحه را انتخاب کن.";return;}timer.Start();start.Enabled=false;stop.Enabled=true;status.Text="خواندن خودکار فعال است (هر ۱٫۲ ثانیه).";overlay.Show();};
        stop.Click+=(_,_)=>{timer.Stop();start.Enabled=true;stop.Enabled=false;status.Text="متوقف شد.";};
        color.Click+=(_,_)=>{using var d=new ColorDialog{Color=overlay.TextColor,FullOpen=true};if(d.ShowDialog()==DialogResult.OK){overlay.TextColor=d.Color;overlay.Invalidate();}};
        size.ValueChanged+=(_,_)=>{overlay.TextSize=(float)size.Value;overlay.Invalidate();};
        opacity.ValueChanged+=(_,_)=>{overlay.BackgroundOpacity=opacity.Value;overlay.Invalidate();};
        timer.Tick+=async(_,_)=>await Scan(); overlay.Show(); overlay.SetCaption("زیرنویس شناور — متنِ شناسایی‌شده اینجا می‌آید");
        FormClosing+=(_,_)=>{timer.Stop();overlay.Close();};
    }
    async Task PickRegion(){Hide();await Task.Delay(250);using var s=new RegionSelector();if(s.ShowDialog()==DialogResult.OK&&s.SelectedBounds.Width>10&&s.SelectedBounds.Height>10){captureBounds=s.SelectedBounds;status.Text="محدوده انتخاب شد. شروع خواندن خودکار را بزن.";}Show();}
    async Task Scan(){if(busy||captureBounds.IsEmpty)return;busy=true;try{using var bmp=new Bitmap(captureBounds.Width,captureBounds.Height);using(var g=Graphics.FromImage(bmp))g.CopyFromScreen(captureBounds.Location,Point.Empty,captureBounds.Size);var text=(await WindowsOcr.ReadAsync(bmp)).Trim();
            if(!string.IsNullOrWhiteSpace(text)&&text!=previous){
                previous=text;
                status.Text="متن پیدا شد؛ در حال ترجمه…";
                try { var translated=await Translator.EnglishToPersianAsync(text); overlay.SetCaption(translated); status.Text="ترجمهٔ فارسی نمایش داده شد (نیازمند اینترنت)."; }
                catch { overlay.SetCaption(text); status.Text="ترجمه در دسترس نیست؛ متن انگلیسی نمایش داده شد. اتصال اینترنت را بررسی کن."; }
            }}catch(Exception ex){timer.Stop();start.Enabled=true;stop.Enabled=false;status.Text="خطا: "+ex.Message;}finally{busy=false;}}
}
