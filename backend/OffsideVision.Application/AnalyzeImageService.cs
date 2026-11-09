using OffsideVision.Domain;

namespace OffsideVision.Application;

public interface IImageAnnotator
{
    byte[] DrawOffsideLine(byte[] image, double normalizedX);
}

public sealed class AnalyzeImageService(OffsideLineCalculator calculator, IImageAnnotator annotator)
{
    public (AnalysisResult Result, byte[] AnnotatedImage) Execute(byte[] image, AnalysisInput input)
    {
        if (image.Length == 0) throw new ArgumentException("Imagem vazia.");
        var result = calculator.Calculate(input);
        return (result, annotator.DrawOffsideLine(image, result.OffsideLineX));
    }
}
