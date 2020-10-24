using FileLoaderConsoleApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Http;
using System.Collections.Concurrent;
using System.IO;
using System.Diagnostics;

namespace FileLoaderConsoleApp
{
    public class FileDownloader : IFileDownloader
    {
        static readonly FileDownloader instance = new FileDownloader();
        static HttpClient _httpClient = new HttpClient();

        SemaphoreSlim semaphore;
        int _maxDegree = 4;
        bool _isWorking;
        public List<Task> tasks;
        

        public event Action<string> OnDownloaded = delegate { };
        public event Action<string, Exception> OnFailed = delegate { };

        private FileDownloader()
        {
            _isWorking = false;
            tasks = new List<Task>();
        }
        public static FileDownloader GetInstance()
        {
            return instance;
        }
        public void AddFileToDownloadingQueue(string fileId, string url, string pathToSave)
        {
            if (semaphore == null) semaphore = new SemaphoreSlim(_maxDegree, _maxDegree);
            Task task = Task.Run(
                () => proccessUrlAsync(url, fileId));
            tasks.Add(task);
        }
        private async Task proccessUrlAsync(string url, string id)
        {
           // Stopwatch stopwatch = new Stopwatch();
           // stopwatch.Start();
            await semaphore.WaitAsync();
            //Console.WriteLine(semaphore.CurrentCount + "count");
            // Console.WriteLine($"{id} starts at {DateTime.Now.Minute}:{DateTime.Now.Second}:{DateTime.Now.Millisecond}......");
            try
            {
                byte[] imageBytes = await _httpClient.GetByteArrayAsync(url);
                string documentsPath = System.Environment.GetFolderPath(
                    System.Environment.SpecialFolder.Personal);
                DirectoryInfo dirInfo = new DirectoryInfo(Path.Combine(documentsPath, "images"));
                if (!dirInfo.Exists)
                {
                    dirInfo.Create();
                }
                string localFilename = Path.GetFileName(url);
                // Console.WriteLine($"Write to file {url} ......");
                string localPath = Path.Combine(documentsPath, "images", localFilename);
                
                File.WriteAllBytes(localPath, imageBytes);
                OnDownloaded(id);
            }
            catch(Exception ex)
            {
                OnFailed(id.ToString(), ex);
            }      
            finally
            {
                semaphore.Release();
            }
           // Console.WriteLine($"{url} added ++++ {id} {stopwatch.Elapsed.TotalSeconds}");
        }
        public void SetDegreeOfParallelism(int degreeOfParallelism)
        {
            if (_isWorking) throw new InvalidOperationException();
            if (degreeOfParallelism <= 0) throw new ArgumentOutOfRangeException();
            _maxDegree = degreeOfParallelism;
            _isWorking = true;
        }

        
    }
    

}
