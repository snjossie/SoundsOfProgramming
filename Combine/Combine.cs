using System;
using System.IO;
using Utility;

namespace Combine
{
    class Combine
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Invalid parameters.  Usage is combine.exe <filename 1> <filename 2>");
                Console.WriteLine("You can only combine 2 mono files.");
                return;
            }

            using (var streamIn = new FileStream(args[0], FileMode.Open))
            using (var stream2In = new FileStream(args[1], FileMode.Open))
            {
                var info = FileHeaderUtil.GetHeaderInfo(streamIn);
                var info2 = FileHeaderUtil.GetHeaderInfo(stream2In);

                if (info.Channels != Channel.Mono || info.Channels != Channel.Mono ||
                    info.SampleCount != info2.SampleCount ||
                    info.SampleBits != info2.SampleCount ||
                    info.Frequency != info2.Frequency)
                {
                    throw new InvalidDataException();
                }

                info.SampleCount *= 2;
                info.Channels = Channel.Stereo;

                var outputStream = Console.OpenStandardOutput();
                FileHeaderUtil.WriteHeaderInfo(outputStream, info);

                while (streamIn.Length != streamIn.Position)
                {
                    long sample1 = SampleUtils.ReadSample(streamIn, info);
                    long sample2 = SampleUtils.ReadSample(stream2In, info);

                    uint combinedSample = (uint)((sample1 + sample2) / 2);

                    SampleUtils.WriteSample(combinedSample, outputStream, info);
                }
            }
        }
    }
}
