using OffsideVision.Application;
using OpenCvSharp;

namespace OffsideVision.Infrastructure;

public sealed class OpenCvImageAnnotator : IImageAnnotator
{
    public byte[] DrawOffsideLine(byte[] image, double normalizedX)
    {
        using var source = Cv2.ImDecode(image, ImreadModes.Color);
        if (source.Empty()) throw new ArgumentException("Formato de imagem não reconhecido.");
        var x = Math.Clamp((int)Math.Round(normalizedX * (source.Width - 1)), 0, source.Width - 1);
        Cv2.Line(source, new Point(x, 0), new Point(x, source.Height - 1), new Scalar(0, 0, 255), 3);
        Cv2.PutText(source, "OFFSIDE REFERENCE", new Point(Math.Min(x + 8, Math.Max(8, source.Width - 220)), 30),
            HersheyFonts.HersheySimplex, 0.7, new Scalar(0, 0, 255), 2);
        return source.ToBytes(".jpg");
    }
}
