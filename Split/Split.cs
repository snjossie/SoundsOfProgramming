using System;
using System.IO;
using Utility;

namespace Split
{
    class Split
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Invalid parameters.  Usage is split.exe <filename>");
                Console.WriteLine("You can only split STEREO files.");
                return;
            }

            using (var streamIn = new FileStream(args[0], FileMode.Open))
            {
                var info = FileHeaderUtil.GetHeaderInfo(streamIn);

                if (info.Channels != Channel.Mono)
                {
                    throw new InvalidDataException();
                }

                info.SampleCount *= 2;
                info.Channels = Channel.Stereo;

                var outputStream = Console.OpenStandardOutput();
                FileHeaderUtil.WriteHeaderInfo(outputStream, info);

                while (streamIn.Length != streamIn.Position)
                {
                    var sample = SampleUtils.ReadSample(streamIn, info);
                    SampleUtils.WriteSample(sample, outputStream, info);
                    SampleUtils.WriteSample(sample, outputStream, info);
                }
            }
        }
    }
}
