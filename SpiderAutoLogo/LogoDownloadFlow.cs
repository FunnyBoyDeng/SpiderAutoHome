using DotnetSpider.DataFlow;

namespace SpiderAutoLogo;

public sealed class LogoDownloadFlow : DataFlowBase
{
    public const long MaxDownloadBytes = 10 * 1024 * 1024;

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

    public static async Task CopyWithLimitAsync(
        Stream source,
        Stream destination,
        long maxBytes,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxBytes);

        var buffer = new byte[81920];
        long totalBytes = 0;

        while (true)
        {
            var bytesRead = await source.ReadAsync(buffer, cancellationToken);
            if (bytesRead == 0)
            {
                break;
            }

            totalBytes += bytesRead;
            if (totalBytes > maxBytes)
            {
                throw new InvalidDataException(
                    $"Logo download exceeded the {maxBytes}-byte limit.");
            }

            await destination.WriteAsync(
                buffer.AsMemory(0, bytesRead),
                cancellationToken);
        }
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
        using var response = await HttpClient.GetAsync(
            logo.ImageUrl,
            HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        if (response.Content.Headers.ContentLength > MaxDownloadBytes)
        {
            throw new InvalidDataException(
                $"Logo download exceeded the {MaxDownloadBytes}-byte limit.");
        }

        var extension = Path.GetExtension(new Uri(logo.ImageUrl).AbsolutePath);
        if (string.IsNullOrWhiteSpace(extension) || extension.Length > 8)
        {
            extension = ".img";
        }

        var fileName = $"{CreateSafeFileName(logo.BrandName)}{extension}";
        var filePath = Path.Combine(outputDirectory, fileName);
        var temporaryFilePath = $"{filePath}.tmp";

        try
        {
            await using (var source = await response.Content.ReadAsStreamAsync())
            await using (var file = File.Create(temporaryFilePath))
            {
                await CopyWithLimitAsync(source, file, MaxDownloadBytes);
            }

            File.Move(temporaryFilePath, filePath, true);
        }
        finally
        {
            if (File.Exists(temporaryFilePath))
            {
                File.Delete(temporaryFilePath);
            }
        }
    }
}
