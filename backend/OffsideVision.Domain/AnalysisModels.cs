namespace OffsideVision.Domain;

public enum TeamSide { Attacking, Defending }

public sealed record PlayerDetection(
    string Id,
    TeamSide Team,
    int ShirtNumber,
    double X,
    double Y);

public sealed record AnalysisInput(double BallX, IReadOnlyList<PlayerDetection> Players);

public sealed record AnalysisResult(
    string Status,
    string Explanation,
    double OffsideLineX,
    string? SecondLastDefenderId,
    int? SecondLastDefenderNumber,
    IReadOnlyList<string> PotentiallyOffsidePlayers);
