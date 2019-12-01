using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace alman
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            // debug
            args = new[] {"convert"};
            // end debug

            switch (args.Length)
            {
                case 0:
                    WriteOptions();
                    break;
                case 1:
                    switch (args[0])
                    {
                        case "sort":
                            await FeatureSort.Sort();
                            break;
                        case "convert":
                            await FeatureConvert.Convert();
                            break;
                    }
                    break;
            }
        }

        private static void WriteOptions()
        {
            Console.WriteLine("Please run alman with one of the following options:");
            Console.WriteLine("> sort");
            Console.WriteLine("| Sort out a raw import of images/videos from an iPhone (in the current directory). This expects that you have `high efficiency` selected as your camera format settings.");
            Console.WriteLine();
            Console.WriteLine("> convert");
            Console.WriteLine("| Converts .heic files to max quality .jpg files, places them in the `alman-converted` folder.");
        }
    }
}
