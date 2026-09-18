using DotnetSpider.DataFlow;

namespace SpiderAutoLogo;

public sealed class LogoDownloadFlow : DataFlowBase
{
    private static readonly HttpClient HttpClient = new()
    {
        Timeout = TimeSpan.FromSeconds(30)
    };

    public override Task InitializeAsync() => Task.CompletedTask;

    public override async Task HandleAsync(DataFlowContext context, ResponseDelegate next)
    {
        if (IsDownloadEnabled() &&
            context.GetData("LogoInfoList") is List<LogoInfo> logos)
        {
            var outputDirectory = ResolveOutputDirectory();
            Directory.CreateDirectory(outputDirectory);

            foreach (var logo in logos)
            {
                await DownloadAsync(logo, outputDirectory);
            }
        }

        await next(context);
    }

    public static string CreateSafeFileName(string brandName)
    {
        var sanitized = new string(
            brandName
                .Trim()
                .Select(character => IsInvalidFileNameCharacter(character) ? '_' : character)
                .ToArray());

        sanitized = sanitized.Trim('.', ' ');
        return string.IsNullOrWhiteSpace(sanitized) ? "unknown-brand" : sanitized;
    }

    private static bool IsInvalidFileNameCharacter(char character) =>
        char.IsControl(character) ||
        Path.GetInvalidFileNameChars().Contains(character) ||
        "<>:\"/\\|?*".Contains(character, StringComparison.Ordinal);

    private static bool IsDownloadEnabled() =>
        bool.TryParse(
            Environment.GetEnvironmentVariable("SPIDERAUTOHOME_DOWNLOAD_LOGOS"),
            out var enabled) && enabled;

    private static string ResolveOutputDirectory()
    {
        var configured = Environment.GetEnvironmentVariable("SPIDERAUTOHOME_LOGO_DIR");
        return Path.GetFullPath(
            string.IsNullOrWhiteSpace(configured) ? "img" : configured);
    }

    private static async Task DownloadAsync(LogoInfo logo, string outputDirectory)
    {
        using var response = await HttpClient.GetAsync(logo.ImageUrl);
        response.EnsureSuccessStatusCode();

        var extension = Path.GetExtension(new Uri(logo.ImageUrl).AbsolutePath);
        if (string.IsNullOrWhiteSpace(extension) || extension.Length > 8)
        {
            extension = ".img";
        }

        var fileName = $"{CreateSafeFileName(logo.BrandName)}{extension}";
        var filePath = Path.Combine(outputDirectory, fileName);

        await using var file = File.Create(filePath);
        await response.Content.CopyToAsync(file);
    }
}
