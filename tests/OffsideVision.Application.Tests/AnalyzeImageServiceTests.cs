using OffsideVision.Application;
using OffsideVision.Domain;

namespace OffsideVision.Application.Tests;

public class AnalyzeImageServiceTests
{
    private sealed class FakeAnnotator : IImageAnnotator
    {
        public double? ReceivedX { get; private set; }
        public byte[] DrawOffsideLine(byte[] image, double normalizedX) { ReceivedX = normalizedX; return [1, 2, 3]; }
    }

    [Fact]
    public void CalculatesAndAnnotatesImage()
    {
        var annotator = new FakeAnnotator();
        var service = new AnalyzeImageService(new OffsideLineCalculator(), annotator);
        var input = new AnalysisInput(.6, new[] {
            new PlayerDetection("A", TeamSide.Attacking, 9, .8, .5),
            new PlayerDetection("D1", TeamSide.Defending, 4, .5, .4),
            new PlayerDetection("D2", TeamSide.Defending, 5, .4, .6)
        });
        var (result, bytes) = service.Execute([10], input);
        Assert.Equal(.6, result.OffsideLineX, 3);
        Assert.Equal(.6, annotator.ReceivedX);
        Assert.Equal(new byte[] {1,2,3}, bytes);
    }
}
