using FSP.Domain.Models;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using System.Net;
using System.Text;

namespace FSP.Infrastructure.Helpers
{
    public class PdfGeneratorHelper : IPdfGeneratorHelper
    {
        private static readonly SemaphoreSlim _browserSemaphore = new(1, 1);
        private static IBrowser? _sharedBrowser;
        private static bool _chromiumDownloaded = false;

        public async Task<byte[]> GenerateBibliographicPdfAsync(Catalogbibliographic model, CancellationToken cancellationToken)
        {
            try
            {
                var html = await GenerateHtmlContentAsync(model, cancellationToken);
                return await GeneratePdfFromHtmlAsync(html, cancellationToken);
            }
            catch (Exception ex)
            {
                return Array.Empty<byte>();
            }
        }

        private async Task<string> GenerateHtmlContentAsync(Catalogbibliographic model, CancellationToken cancellationToken)
        {
            var BaseUrl = Environment.GetEnvironmentVariable("BASE_URL");
            var templatePath = Path.Combine(AppContext.BaseDirectory, "Templates", "Template_ficha.html");
            var templateContent = await File.ReadAllTextAsync(templatePath, Encoding.UTF8, cancellationToken);

            var html = new StringBuilder(templateContent);

            var replacements = new Dictionary<string, string>
            {
                {"{{commonNoun}}", WebUtility.HtmlEncode(model.Catalog.CommonNoun ?? string.Empty)},
                {"{{specie}}", WebUtility.HtmlEncode(model.Catalog.Specie ?? string.Empty)},
                {"{{description}}", WebUtility.HtmlEncode(model.Catalog.Description ?? string.Empty)},
                {"{{habits}}", WebUtility.HtmlEncode(model.Catalog.Habits ?? string.Empty)},
                {"{{habitat}}", WebUtility.HtmlEncode(model.Catalog.Habitat ?? string.Empty)},
                {"{{reproduction}}", WebUtility.HtmlEncode(model.Catalog.Reproduction ?? string.Empty)},
                {"{{feeding}}", WebUtility.HtmlEncode(model.Catalog.Feeding ?? string.Empty)},
                {"{{distribution}}", WebUtility.HtmlEncode(model.Catalog.Distribution ?? string.Empty)},
                {"{{category}}", WebUtility.HtmlEncode(model.Catalog.Category ?? string.Empty)},
                {"{{image}}", $"{BaseUrl}/catalog/{model.Catalog.Image}" ?? string.Empty}
            };

            foreach (var (placeholder, value) in replacements)
            {
                html.Replace(placeholder, value);
            }

            return html.ToString();
        }

        private async Task<byte[]> GeneratePdfFromHtmlAsync(string html, CancellationToken cancellationToken)
        {
            await _browserSemaphore.WaitAsync(cancellationToken);
            
            try
            {
                await EnsureBrowserInitializedAsync();

                await using var page = await _sharedBrowser!.NewPageAsync();
                
                try
                {
                    await page.SetContentAsync(html, new NavigationOptions 
                    { 
                        WaitUntil = new[] { WaitUntilNavigation.Load }
                    });

                    return await page.PdfDataAsync(new PdfOptions
                    {
                        Format = PaperFormat.Letter,
                        PrintBackground = true,
                        MarginOptions = new MarginOptions
                        {
                            Top = "20mm",
                            Bottom = "20mm",
                            Left = "15mm",
                            Right = "15mm"
                        }
                    });
                }
                finally
                {
                    await page.CloseAsync();
                }
            }
            finally
            {
                _browserSemaphore.Release();
            }
        }

        private static async Task EnsureBrowserInitializedAsync()
        {
            if (!_chromiumDownloaded)
            {
                await new BrowserFetcher().DownloadAsync();
                _chromiumDownloaded = true;
            }

            if (_sharedBrowser == null || _sharedBrowser.IsClosed)
            {
                _sharedBrowser = await Puppeteer.LaunchAsync(new LaunchOptions 
                { 
                    Headless = true,
                    Args = new[] { "--no-sandbox", "--disable-setuid-sandbox", "--disable-dev-shm-usage" }
                });
            }
        }

        public static async Task DisposeSharedResourcesAsync()
        {
            if (_sharedBrowser != null && !_sharedBrowser.IsClosed)
            {
                await _sharedBrowser.CloseAsync();
                _sharedBrowser.Dispose();
            }
            _browserSemaphore.Dispose();
        }
    }
}
