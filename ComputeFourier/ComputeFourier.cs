using System;
using System.IO;
using Utility;

namespace ComputeFourier
{
    class ComputeFourier
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: computeFourier <filename>");
                return;
            }

            using (var stream = new FileStream(args[0], FileMode.Open))
            {
                var info = FileHeaderUtil.GetHeaderInfo(stream);
                uint[] samples = Fourier.ReadSamples(stream, 0, 256, info);

                var vals = Fourier.CalculateComplex(samples, samples.Length);
                Console.WriteLine("Lowest Frequency Energy:   {0} + {1}i",
                                  vals[0].Real,
                                  vals[0].Imaginary);
                Console.WriteLine("Highest Frequency Energy:  {0} + {1}i",
                                  vals[samples.Length / 2].Real,
                                  vals[samples.Length / 2].Imaginary);

                //foreach (var mag in vals)
                //    Console.WriteLine(mag);

                Console.ReadLine();
            }
        }
    }
}
