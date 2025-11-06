using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaApp.Services
{
    public static class PlaywrightInitializer
    {
        private static readonly string MarkerFile = Path.Combine(
            Directory.GetCurrentDirectory(), ".playwright_installed");

        public static async Task EnsureInstalledAsync()
        {
            if (File.Exists(MarkerFile))
                return;

            // Playwright.Program.Main возвращает int, поэтому оборачиваем в Task.Run
            await Task.Run(() => Microsoft.Playwright.Program.Main(new[] { "install" }));

            File.WriteAllText(MarkerFile, "ok");
        }
    }
}
