using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;
using Windows.Storage.Streams;

namespace PersianScreenTranslator;

internal static class WindowsOcr
{
    public static async Task<string> ReadAsync(Bitmap bitmap)
    {
        using var stream = new MemoryStream();
        bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
        stream.Position = 0;

        using IRandomAccessStream ras = new InMemoryRandomAccessStream();
        using (var writer = new DataWriter(ras))
        {
            writer.WriteBytes(stream.ToArray());
            await writer.StoreAsync();
            await writer.FlushAsync();
            writer.DetachStream();
        }
        ras.Seek(0);

        var decoder = await BitmapDecoder.CreateAsync(ras);
        var softwareBitmap = await decoder.GetSoftwareBitmapAsync();
        var engine = OcrEngine.TryCreateFromLanguage(new Windows.Globalization.Language("en"));
        if (engine is null)
            throw new InvalidOperationException("بسته زبان انگلیسی OCR در ویندوز در دسترس نیست. زبان English را در تنظیمات ویندوز نصب کن.");
        var result = await engine.RecognizeAsync(softwareBitmap);
        return result.Text;
    }
}
