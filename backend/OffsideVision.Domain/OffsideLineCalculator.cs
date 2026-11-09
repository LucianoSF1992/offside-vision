namespace OffsideVision.Domain;

public sealed class OffsideLineCalculator
{
    public AnalysisResult Calculate(AnalysisInput input)
    {
        ArgumentNullException.ThrowIfNull(input);
        if (input.Players is null || input.Players.Count == 0)
            throw new ArgumentException("Informe ao menos um jogador.", nameof(input));
        if (!double.IsFinite(input.BallX) || input.BallX is < 0 or > 1)
            throw new ArgumentOutOfRangeException(nameof(input), "ballX deve estar entre 0 e 1.");

        foreach (var p in input.Players)
            if (string.IsNullOrWhiteSpace(p.Id) || !double.IsFinite(p.X) || !double.IsFinite(p.Y)
                || p.X is < 0 or > 1 || p.Y is < 0 or > 1)
                throw new ArgumentException("Cada detecção deve ter ID e coordenadas normalizadas entre 0 e 1.");

        // Convenção do MVP: X crescente em direção ao gol adversário.
        var defenders = input.Players
            .Where(p => p.Team == TeamSide.Defending)
            .OrderByDescending(p => p.X)
            .ToList();

        if (defenders.Count < 2)
            throw new ArgumentException("São necessários pelo menos dois defensores para estimar a linha.");

        var secondLast = defenders[1];
        var lineX = Math.Max(input.BallX, secondLast.X);
        var attackers = input.Players.Where(p => p.Team == TeamSide.Attacking);
        var potentiallyOffside = attackers
            .Where(p => p.X > lineX)
            .Select(p => p.Id)
            .ToArray();

        return new AnalysisResult(
            "UNDER_REVIEW",
            "Referência geométrica experimental. Posição não equivale a infração: é necessário avaliar o momento do passe e a participação ativa.",
            lineX,
            secondLast.Id,
            secondLast.ShirtNumber,
            potentiallyOffside);
    }
}
