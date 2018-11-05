using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FileReceiver
{
    class Program : IDisposable
    {
        static void Main(string[] args)
        {
            try
            {
                using (var program = new Program(args))
                {
                    program.Run().Wait();
                }
            }
            catch (Exception x)
            {
                Console.WriteLine(x.Message);
            }

            Console.ReadKey();
        }
        
        private readonly HttpClient _client;
        private readonly string _targetDirectory;

        struct FileData
        {
            public string Name;
            public long Length;
        }

        public Program(string[] args)
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri(args.FirstOrDefault() ?? "https://localhost:5001")
            };
            _targetDirectory = Directory.GetCurrentDirectory();
        }

        private async Task Run()
        {
            // List all files
            var files = await GetFiles();
            foreach (var file in files)
            {
                Console.WriteLine($"[{file.Length}] {file.Name}");
            }

            // Download files
            foreach (var file in files)
            {
                await DownloadFile(file);
            }
        }

        private async Task<IList<FileData>> GetFiles()
        {
            var response = await _client.GetAsync("/files");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<IEnumerable<FileData>>(json).ToList();
        }

        private async Task DownloadFile(FileData file)
        {
            var sourceStream = await _client.GetStreamAsync("/files/" + file.Name);
            var target = Path.Combine(_targetDirectory, file.Name);
            using (var destinationStreamStream = File.Create(target))
            {
                await sourceStream.CopyToAsync(destinationStreamStream);
            }
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}
