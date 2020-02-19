using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Utility;
using FileInfo = Utility.FileInfo;

namespace Mix
{
    class Mix
    {
        static void Main(string[] args)
        {
            if (args.Length % 2 != 0 || args.Length < 4)
            {
                Console.WriteLine("Invalid arguments.  There must be an even number of arguments and there must be at least 4 arguments.");
                Console.WriteLine("Usage: mix.exe <file1> <gain1> <file2> <gain2> [<file N> <gain N> ...]");
                return;
            }

            var files = new List<Stream>();
            var infos = new List<FileInfo>();
            var gains = new List<double>();

            for (int i = 0; i < args.Length; i++)
            {
                if (i % 2 == 0)
                {
                    files.Add(new FileStream(args[i], FileMode.Open));
                    infos.Add(FileHeaderUtil.GetHeaderInfo(files.Last()));
                }
                else
                {
                    gains.Add(double.Parse(args[i]));
                }
            }

            foreach (var info in infos)
            {
                if (info.Channels != infos[0].Channels ||
                    info.SampleBits != infos[0].SampleBits ||
                    info.SampleCount != infos[0].SampleCount)
                {
                    Console.WriteLine("Files incompatible for mixing.");
                    return;
                }
            }

            var outputStream = Console.OpenStandardOutput();
            FileHeaderUtil.WriteHeaderInfo(outputStream, infos.First());

            var fileInfo = infos.First();
            uint maxAmplitude = (uint)(Math.Pow(2, fileInfo.SampleBits) - 1);
            unchecked
            {
                for (int i = 0; i < fileInfo.SampleCount; i++)
                {
                    uint sample = 0;

                    for (int j = 0; j < files.Count; j++)
                    {
                        sample += (uint)(SampleUtils.ReadSample(files[j], fileInfo) * gains[j]);
                    }

                    if (sample >= maxAmplitude)
                    {
                        sample = maxAmplitude;
                    }

                    SampleUtils.WriteSample(sample, outputStream, fileInfo);
                }
            }
        }
    }
}
