using System;
using System.IO;
using System.Linq;
using System.Net;
using HtmlAgilityPack;

namespace Cognizer {
    public class IT_eBooks {
        public const string baseSite = "http://it-ebooks.info/book/";
        public const string dlDir = @"I:\Books\";
        public const int numberOfBooks = 1000; // Max 5686

        private static void Main(string[] args) {
            Console.Write("So it begins.");
            var web = new HtmlWeb();

            for (int i = 1; i < numberOfBooks; i++) {
                var baseURI = new Uri(baseSite + i + "/");
                var document = web.Load(baseURI.AbsoluteUri);
                var xpath = document.DocumentNode.SelectNodes("//a");
                var book = xpath.Select(link => link.Attributes["href"].Value).FirstOrDefault(path => path.Contains("filepi"));
                var bookName = xpath.First(link => link.Attributes["href"].Value.Contains("filepi")).InnerText;

                using (WebClient client = new WebClient()) {
                    if (!string.IsNullOrEmpty(book) && !string.IsNullOrEmpty(bookName)) {
                        bookName = bookName + ".pdf";
                        var bookSite = new Uri(book).AbsoluteUri;
                        client.Headers.Add(HttpRequestHeader.Host, "filepi.com");
                        client.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (Windows NT 6.3; WOW64; rv:37.0) Gecko/20100101 Firefox/37.0");
                        client.Headers.Add(HttpRequestHeader.Accept, "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
                        client.Headers.Add(HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.5");
                        client.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
                        client.Headers.Add(HttpRequestHeader.Referer, baseSite + i);
                        client.DownloadFile(bookSite, dlDir + bookName);
                        Console.Write(i + " : Downloaded Book : " + bookName + "\n");
                        using (StreamWriter file = new StreamWriter(@"I:\Books\index.csv", true)) {
                            file.WriteLine(i + "," + bookName + "," + DateTime.Now);
                        }
                    }
                }
            }
        }
    }
}
