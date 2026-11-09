using OffsideVision.Domain;

namespace OffsideVision.Domain.Tests;

public class OffsideLineCalculatorTests
{
    private readonly OffsideLineCalculator _sut = new();

    [Fact]
    public void UsesTheMoreAdvancedOfBallAndSecondLastDefender()
    {
        var input = new AnalysisInput(.62, new[] {
            new PlayerDetection("A17", TeamSide.Attacking, 17, .78, .4),
            new PlayerDetection("D4", TeamSide.Defending, 4, .35, .3),
            new PlayerDetection("D23", TeamSide.Defending, 23, .55, .6),
            new PlayerDetection("D8", TeamSide.Defending, 8, .25, .7)
        });
        var result = _sut.Calculate(input);
        Assert.Equal(.62, result.OffsideLineX, 3);
        Assert.Equal("D23", result.SecondLastDefenderId);
        Assert.Contains("A17", result.PotentiallyOffsidePlayers);
        Assert.Equal("UNDER_REVIEW", result.Status);
    }

    [Fact]
    public void RejectsFewerThanTwoDefenders()
    {
        var input = new AnalysisInput(.5, new[] {
            new PlayerDetection("A", TeamSide.Attacking, 9, .7, .5),
            new PlayerDetection("D", TeamSide.Defending, 4, .3, .5)
        });
        Assert.Throws<ArgumentException>(() => _sut.Calculate(input));
    }

    [Fact]
    public void RejectsCoordinatesOutsideNormalizedRange()
    {
        var input = new AnalysisInput(.5, new[] {
            new PlayerDetection("A", TeamSide.Attacking, 9, 1.2, .5),
            new PlayerDetection("D1", TeamSide.Defending, 4, .4, .5),
            new PlayerDetection("D2", TeamSide.Defending, 5, .3, .5)
        });
        Assert.Throws<ArgumentException>(() => _sut.Calculate(input));
    }
}
