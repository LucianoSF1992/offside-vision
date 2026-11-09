using Microsoft.ML.OnnxRuntime;

namespace OffsideVision.ComputerVision;

/// <summary>
/// Verifica e mantém a sessão ONNX. O mapeamento de tensores depende do modelo escolhido.
/// </summary>
public sealed class OnnxModelReadiness : IDisposable
{
    public InferenceSession? Session { get; }

    public OnnxModelReadiness(string? modelPath)
    {
        if (!string.IsNullOrWhiteSpace(modelPath) && File.Exists(modelPath))
            Session = new InferenceSession(modelPath);
    }

    public bool IsModelLoaded => Session is not null;
    public void Dispose() => Session?.Dispose();
}
