using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileLoaderConsoleApp
{
    public class TextFile
    {
        public async Task ReadFileAsync(string fileName)
        {
            try
            {
                string line;
                using (StreamReader sr = new StreamReader(fileName))
                {
                    while ((line = await sr.ReadLineAsync()) != null)
                    {
                        
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
        }
    }
}
