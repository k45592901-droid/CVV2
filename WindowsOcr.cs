using System.Drawing;
using System.IO;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;
using Windows.Storage.Streams;
namespace PersianScreenTranslator;
internal static class WindowsOcr
{
    public static async Task<string> ReadAsync(Bitmap bitmap)
    {
        using var ms = new MemoryStream(); bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png); ms.Position = 0;
        using IRandomAccessStream ras = new InMemoryRandomAccessStream();
        using (var writer = new DataWriter(ras)) { writer.WriteBytes(ms.ToArray()); await writer.StoreAsync(); await writer.FlushAsync(); writer.DetachStream(); }
        ras.Seek(0); var decoder = await BitmapDecoder.CreateAsync(ras); var sb = await decoder.GetSoftwareBitmapAsync();
        var engine = OcrEngine.TryCreateFromLanguage(new Windows.Globalization.Language("en"));
        if (engine is null) throw new InvalidOperationException("بستهٔ زبان انگلیسی OCR در ویندوز نصب نیست. از Settings > Time & language > Language & region زبان English را اضافه کن.");
        var result = await engine.RecognizeAsync(sb); return result.Text;
    }
}
