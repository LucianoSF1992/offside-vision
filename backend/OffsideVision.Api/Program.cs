using OpenCvSharp;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using OffsideVision.Application;
using OffsideVision.Domain;
using OffsideVision.Infrastructure;
using OffsideVision.ComputerVision;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
    policy.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod()));
builder.Services.AddSingleton<OffsideLineCalculator>();
builder.Services.AddSingleton<IImageAnnotator, OpenCvImageAnnotator>();
builder.Services.AddScoped<AnalyzeImageService>();
builder.Services.AddSingleton(new OnnxModelReadiness(builder.Configuration["Onnx:ModelPath"]));

var app = builder.Build();
app.UseCors();
app.UseSwagger();
app.UseSwaggerUI();
app.MapGet("/", () => Results.Ok(new { name = "Offside Vision API", status = "running" }));
app.MapGet("/api/health", (OnnxModelReadiness model) => Results.Ok(new
{
    status = "healthy",
    onnxModelLoaded = model.IsModelLoaded,
    message = model.IsModelLoaded ? "Modelo ONNX carregado; adaptador de inferência ainda deve ser configurado." : "Modo demonstração: envie detecções estruturadas."
}));

app.MapPost("/api/analyses/image", async (HttpRequest request, AnalyzeImageService service) =>
{
    if (!request.HasFormContentType)
        return Results.BadRequest(new { error = "Envie os dados como multipart/form-data." });

    var form = await request.ReadFormAsync();

    var image = form.Files.GetFile("image");
    var detections = form["detections"].ToString();

    if (image is null || image.Length == 0)
        return Results.BadRequest(new { error = "Envie uma imagem." });

    if (image.Length > 10 * 1024 * 1024)
        return Results.BadRequest(new { error = "Limite de imagem: 10 MB." });

    AnalysisInput? input;

    try
    {
        Console.WriteLine("JSON RECEBIDO:");
        Console.WriteLine(detections);

        input = JsonSerializer.Deserialize<AnalysisInput>(
            detections,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters =
                {
                new System.Text.Json.Serialization.JsonStringEnumConverter()
                }
            });

        Console.WriteLine("JSON CONVERTIDO COM SUCESSO!");
    }
    catch (JsonException ex)
    {
        Console.WriteLine("ERRO NA DESSERIALIZACAO:");
        Console.WriteLine(ex.ToString());

        return Results.BadRequest(new
        {
            error = "JSON de detecções inválido.",
            details = ex.Message
        });
    }

    if (input is null)
        return Results.BadRequest(new { error = "Detecções não informadas." });

    try
    {
        await using var stream = new MemoryStream();
        await image.CopyToAsync(stream);

        var (result, annotated) = service.Execute(stream.ToArray(), input);

        return Results.Ok(new
        {
            result,
            annotatedImage = $"data:image/jpeg;base64,{Convert.ToBase64String(annotated)}"
        });
    }
    catch (Exception ex) when (
        ex is ArgumentException || ex is OpenCvSharpException)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
})
.Accepts<ImageAnalysisForm>("multipart/form-data")
.DisableAntiforgery();

app.Run();

public sealed class ImageAnalysisForm
{
    public IFormFile? Image { get; set; }
    public string? Detections { get; set; }
}