using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FileReceiver
{
    struct FileData
    {
        public string Name;
        public long Length;
    }

    class Program
    {
        private readonly HttpClient _client;

        public Program()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri("https://localhost:5001");
        }

        static void Main(string[] args)
        {
            var program = new Program();
            if (!args.Any())
                program.GetFiles().Wait();
            else
                program.GetFile(args.First());
        }

        private async Task GetFiles()
        {
            var response = await _client.GetAsync("/files");
            response.EnsureSuccessStatusCode();
            var jsonString = await response.Content.ReadAsStringAsync();
            var files = JsonConvert.DeserializeObject<IEnumerable<FileData>>(jsonString);
            foreach (var f in files)
            {
                Console.WriteLine(f.Name);
            }

            //string yourPrompt = (string)s["dialog"]["prompt"];
            //var buf = await response.Content.ReadAsBufferAsync();
            //return buf.ToArray();

        }

        private void GetFile(string fileName)
        {
            throw new NotImplementedException();
        }

    }
}
