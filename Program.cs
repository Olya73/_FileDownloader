using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileLoaderConsoleApp
{
    class Program
    {
        public static ConcurrentDictionary<string, bool> files = new ConcurrentDictionary<string, bool>();
        // public static int countDownloaded = 0;
        
        static async Task Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            FileDownloader fileDownloader = FileDownloader.GetInstance();
            fileDownloader.SetDegreeOfParallelism(12);
            fileDownloader.OnDownloaded += OnDownload;
            fileDownloader.OnFailed += OnFail;
            string fileName = @"C:\Users\User\Downloads\file\urls-list.txt";
            int id = 0;
            try
            {
                string line;
                using (StreamReader sr = new StreamReader(fileName))
                {
                    while ((line = await sr.ReadLineAsync()) != null)
                    {
                        fileDownloader.AddFileToDownloadingQueue(id.ToString(), line, "");
                        files.GetOrAdd(id.ToString(), false);
                        id++;
                    }
                }
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
           
            //ThreadPool.SetMaxThreads(12, 12);
            //ThreadPool.GetMaxThreads(out i, out j);
            //Console.WriteLine(i + " "+j);
          /*  files.GetOrAdd("1", false);
            files.GetOrAdd("2", false);
            files.GetOrAdd("3", false);
            files.GetOrAdd("4", false);
            files.GetOrAdd("5", false);
            files.GetOrAdd("6", false);
            files.GetOrAdd("7", false);
            files.GetOrAdd("8", false);
            fileDownloader.AddFileToDownloadingQueue("1", "https://www.soundhelix.com/examples/mp3/SoundHelix-Song-1.mp3", "");
            fileDownloader.AddFileToDownloadingQueue("2", "https://www.soundhelix.com/examples/mp3/SoundHelix-Song-2.mp3", "");
            fileDownloader.AddFileToDownloadingQueue("3", "https://sun9-65.userapi.com/9xopop1IzxomT2OxvBgOdLKRgL6j2S-SKo_PZm1XQA/jijvQwiDDXQ.jpg", "");
            fileDownloader.AddFileToDownloadingQueue("4", "https://sun9-65.userapi.com/cDjwGmNpJCPQQmAIP_kUyR5PwjZRbotsKoMxiQ/zOKzI6VjgJk.jpg", "");
            fileDownloader.AddFileToDownloadingQueue("5", "https://sun9-29.userapi.com/uqNtAZl9Q7CyZb9F4pUIAdfyXimXxPlC-CZHoA/Ta1ejaR_d24.jpg", "");
            fileDownloader.AddFileToDownloadingQueue("6", "https://sun9-37.userapi.com/_B2ns0HjE5mLIi-mzfq1x3HZzSjDi0DXELY7eA/97eL_Gqz_Yk.jpg", "");
            fileDownloader.AddFileToDownloadingQueue("7", "https://sun9-32.userapi.com/c854324/v854324801/2554d0/1W3Jkc7LjqI.jpg", "");
            fileDownloader.AddFileToDownloadingQueue("8", "https://sun9-7.userapi.com/R0GxzIJyXKaHfyopBoM3ZC30fr8aFnRWrF8DJQ/N3fUF0lTgO8.jpg", "");
         */
            await Task.WhenAll(fileDownloader.tasks.ToArray());
            Console.WriteLine($"{files.Values.Where(v => v == true).Count()} downloaded successfully");
            Console.WriteLine($"{files.Values.Where(v => v == false).Count()} was failed");
            Console.WriteLine(stopwatch.ElapsedTicks);
            Console.ReadKey();


        }
        public static void OnDownload(string id)
        {
            
            files[id] = true;
            int count = files.Values.Where(v => v == true).Count();
            Console.WriteLine($"File {id} is downloaded. Downloaded { count * 100 / files.Count}% outta 100%");
        }
        public static void OnFail(string id, Exception exception)
        {
            File.AppendAllText("log.txt",
                $"{DateTime.Now} \n" +
                $"File {id} is failed with an exception:\n " +
                $"{ exception.Message} \n" +
                $"{exception.StackTrace} \n\n");
        }
        
    }
}
