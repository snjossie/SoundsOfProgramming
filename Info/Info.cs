using System;
using System.IO;
using Utility;

namespace Info
{
    class Info
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Invalid arguments.  Usage is info.exe <file name>");
                return;
            }

            try
            {
                using (var stream = new FileStream(args[0], FileMode.Open))
                {
                    var headerInfo = FileHeaderUtil.GetHeaderInfo(stream);

                    Console.WriteLine("Channels      {0}", headerInfo.Channels);
                    Console.WriteLine("Frequency     {0}", headerInfo.Frequency);
                    Console.WriteLine("Sample Bits   {0}", headerInfo.SampleBits);
                    Console.WriteLine("Sample Count  {0}", headerInfo.SampleCount);


                    Console.ReadLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Encountered an error processing the file.");
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
