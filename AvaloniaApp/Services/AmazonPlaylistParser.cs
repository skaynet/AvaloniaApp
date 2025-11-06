using AvaloniaApp.Models;
using HtmlAgilityPack;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaApp.Services
{
    public static class AmazonPlaylistParser
    {
        public static async Task GetPlaylistAsync(Playlist playlist)
        {
            try
            {
                using var playwright = await Playwright.CreateAsync();

                await using var browser = await playwright.Chromium.LaunchAsync(new()
                {
                    Headless = true,
                    Args = new[] { "--disable-blink-features=AutomationControlled" }
                });

                string url = playlist.Url;

                var context = await browser.NewContextAsync(new BrowserNewContextOptions
                {
                    UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) " +
                                "AppleWebKit/537.36 (KHTML, like Gecko) " +
                                "Chrome/120.0 Safari/537.36"
                });

                var page = await context.NewPageAsync();

                // Загружаем страницу и ждём пока всё догрузится
                await page.GotoAsync(url, new() { WaitUntil = WaitUntilState.NetworkIdle });
                //await page.WaitForTimeoutAsync(5000); // ждем рендера web components

                string html = await page.ContentAsync();

                // Парсим с помощью HtmlAgilityPack
                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                // Ищем <music-detail-header>
                var headerNode = doc.DocumentNode.SelectSingleNode("//music-detail-header[@image-src and string-length(@image-src) > 0]");

                if (headerNode != null)
                {
                    string? result = headerNode.GetAttributeValue("image-src", null);

                    if (!string.IsNullOrEmpty(result))
                    {
                        //Найдено изображение: {imageUrl}
                        //return imageUrl;
                        playlist.AvatarUrl = result;
                    }

                    result = headerNode.GetAttributeValue("headline", null);

                    if (!string.IsNullOrEmpty(result))
                    {
                        playlist.Name = result;
                    }

                    result = headerNode.GetAttributeValue("secondary-text", "");

                    playlist.Description = result;
                }

                // Получаем все <music-image-row> с атрибутом primary-text
                var headerNodes = doc.DocumentNode.SelectNodes("//music-image-row[@primary-text]");

                if (headerNodes != null)
                {
                    foreach (var header in headerNodes)
                    {
                        Song song = new Song();
                        song.ArtistName = header.GetAttributeValue("secondary-text-1", "");
                        song.SongName = header.GetAttributeValue("primary-text", "");
                        song.AlbumName = header.GetAttributeValue("secondary-text-2", "");

                        var timeSpanNode = header.SelectSingleNode(".//div[@class='col4']//span");
                        if (timeSpanNode != null)
                            song.Duration = timeSpanNode.InnerText.Trim();

                        playlist.Songs.Add(song);
                    }
                }
                playlist.IsLoaded = true;
                //Не удалось найти тег <music-detail-header> или атрибут image-src.
                //return null;
            }
            catch (TimeoutException)
            {
                //Таймаут: страница не успела догрузиться
                //return null;
            }
            catch (Exception ex)
            {
                //Ошибка Playwright/HAP: {ex.Message}
                //return null;
            }
        }
    }
}
