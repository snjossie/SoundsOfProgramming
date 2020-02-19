using System;
using Utility;

namespace GenSine
{
    class GenSine
    {
        static void Main(string[] args)
        {
            if (args.Length != 6)
            {
                Console.WriteLine("Invalid number of parameters.");
                Console.WriteLine("Usage is gensine.exe <channels> <frequency> <sample size> <sample rate> <amplitude> <duration>");
                Console.WriteLine("<channels> := Mono, Stereo");
                Console.WriteLine("<frequency> := int (Hz)");
                Console.WriteLine("<sample size> := 8, 16, 24, or 32");
                Console.WriteLine("<sample rate> := int (samples/sec)");
                Console.WriteLine("<amplitude> := int");
                Console.WriteLine("<duration> := double (seconds)");
                return;
            }

            Channel channels = (Channel)Enum.Parse(typeof(Channel), args[0], true);
            int frequency = int.Parse(args[1]);
            int sampleSize = int.Parse(args[2]);
            int sampleRate = int.Parse(args[3]);
            int amplitude = int.Parse(args[4]);
            double duration = double.Parse(args[5]);

            var info = new FileInfo
            {
                Channels = channels,
                Frequency = sampleRate,
                SampleBits = sampleSize,
                SampleCount = (int)(sampleRate * duration)
            };

            double samplesPerCycle = sampleRate / (double)frequency;
            double interval = 2 * Math.PI / samplesPerCycle;
            double offset = 0;

            var outputStream = Console.OpenStandardOutput();
            FileHeaderUtil.WriteHeaderInfo(outputStream, info);

            for (int i = 0; i < info.SampleCount; i++)
            {
                if (offset > 2 * Math.PI)
                {
                    offset -= 2 * Math.PI;
                }

                uint sample = (uint)(((Math.Sin(offset) + 1) * amplitude / 2.0) + (amplitude / 2.0));

                SampleUtils.WriteSample(sample, outputStream, info);
                if (info.Channels == Channel.Stereo)
                {
                    SampleUtils.WriteSample(sample, outputStream, info);
                }

                offset += interval;
            }
        }
    }
}
