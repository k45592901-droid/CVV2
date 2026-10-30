using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PersianScreenTranslator;

public sealed class MainForm : Form
{
    private readonly Button selectButton = new() { Text = "انتخاب محدوده صفحه", AutoSize = true };
    private readonly Button fullButton = new() { Text = "خواندن کل صفحه", AutoSize = true };
    private readonly Label status = new() { AutoSize = true, Text = "آماده — OCR آفلاین ویندوز" };
    private readonly FloatingForm floating = new();

    public MainForm()
    {
        Text = "مترجم صفحه فارسی — نسخه شروع";
        Width = 430;
        Height = 170;
        StartPosition = FormStartPosition.CenterScreen;
        RightToLeft = RightToLeft.Yes;
        RightToLeftLayout = true;

        var panel = new FlowLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(14), FlowDirection = FlowDirection.RightToLeft };
        panel.Controls.Add(selectButton);
        panel.Controls.Add(fullButton);
        panel.Controls.Add(status);
        Controls.Add(panel);

        selectButton.Click += async (_, _) => await SelectRegionAsync();
        fullButton.Click += async (_, _) => await ReadScreenAsync();
        floating.Show();
    }

    private async Task SelectRegionAsync()
    {
        Hide();
        await Task.Delay(250);
        using var selector = new RegionSelector();
        if (selector.ShowDialog() == DialogResult.OK && selector.SelectedBounds is Rectangle r && r.Width > 5 && r.Height > 5)
            await CaptureAndReadAsync(r);
        Show();
    }

    private async Task ReadScreenAsync()
    {
        Hide();
        await Task.Delay(200);
        var bounds = Screen.PrimaryScreen?.Bounds ?? Rectangle.Empty;
        Show();
        if (bounds != Rectangle.Empty) await CaptureAndReadAsync(bounds);
    }

    private async Task CaptureAndReadAsync(Rectangle bounds)
    {
        try
        {
            status.Text = "در حال تشخیص نوشته...";
            using var bmp = new Bitmap(bounds.Width, bounds.Height);
            using (var g = Graphics.FromImage(bmp)) g.CopyFromScreen(bounds.Location, Point.Empty, bounds.Size);
            string recognized = await WindowsOcr.ReadAsync(bmp);
            floating.SetText(string.IsNullOrWhiteSpace(recognized)
                ? "متنی پیدا نشد."
                : "متن شناسایی‌شده (هنوز ترجمه نشده):\r\n\r\n" + recognized);
            status.Text = "OCR تمام شد";
        }
        catch (Exception ex)
        {
            status.Text = "خطا در OCR";
            MessageBox.Show(ex.Message, "خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}

public sealed class FloatingForm : Form
{
    private readonly TextBox output = new()
    {
        Multiline = true, ReadOnly = true, Dock = DockStyle.Fill,
        ScrollBars = ScrollBars.Vertical, Font = new Font("Segoe UI", 13),
        RightToLeft = RightToLeft.Yes
    };

    public FloatingForm()
    {
        Text = "زیرنویس / متن شناور";
        Width = 620; Height = 230;
        TopMost = true;
        StartPosition = FormStartPosition.Manual;
        Location = new Point(40, 650);
        Controls.Add(output);
        Show();
    }

    public void SetText(string text) => output.Text = text;
}
