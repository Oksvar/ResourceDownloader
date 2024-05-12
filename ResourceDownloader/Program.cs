using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LinkDownloader
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Base URLs
            string baseUrlOnline = "https://sapui5.hana.ondemand.com/";
            string baseUrlLocal = @"C:\Users\HP\Desktop\ui5mixed\UI5\sap.ui.core.tutorial.navigation.01\webapp";

            // Sample text with links
            string text = File.ReadAllText(@"C:\Users\HP\Desktop\todown.txt");

            // Collect links
            List<string> links = ExtractLinks(text);

            // Download files
            await DownloadFiles(links, baseUrlOnline, baseUrlLocal);
        }

        static List<string> ExtractLinks(string text)
        {
            List<string> links = new List<string>();

            // Regular expression to find URLs
            string pattern = @"(http|https):\/\/[^\s$.?#].[^\s]*";
            Regex regex = new Regex(pattern);

            // Find matches
            MatchCollection matches = regex.Matches(text);
            foreach (Match match in matches)
            {
                links.Add(match.Value);
            }

            return links;
        }

        static async Task DownloadFiles(List<string> links, string baseUrlOnline, string baseUrlLocal)
        {
            using (HttpClient client = new HttpClient())
            {
                foreach (string link1 in links)
                {
                    var link = link1.Replace("http://localhost:1234/webapp/", baseUrlOnline).TrimEnd(':');
                    // Determine file path
                    string filePathOnline = link.Replace(baseUrlOnline, "");
                    string filePathLocal = Path.Combine(baseUrlLocal, filePathOnline.Replace("/", "\\"));

                    // Create directory if it doesn't exist
                    string directoryPath = Path.GetDirectoryName(filePathLocal);
                    Directory.CreateDirectory(directoryPath);

                    // Download file
                    HttpResponseMessage response = await client.GetAsync(link);
                    if (response.IsSuccessStatusCode)
                    {
                        using (Stream contentStream = await response.Content.ReadAsStreamAsync())
                        {
                            using (Stream fileStream = new FileStream(filePathLocal, FileMode.Create))
                            {
                                await contentStream.CopyToAsync(fileStream);
                            }
                        }
                        Console.WriteLine($"Downloaded: {filePathLocal}");
                    }
                    else
                    {
                        Console.WriteLine($"Failed to download: {filePathLocal}");
                    }
                }
            }
        }
    }
}
