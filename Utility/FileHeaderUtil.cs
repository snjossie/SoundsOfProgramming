using System;
using System.IO;
using System.Text;

namespace Utility
{
    public static class FileHeaderUtil
    {
        public static FileInfo GetHeaderInfo(Stream fileStream)
        {
            if (fileStream == null)
            {
                throw new ArgumentException();
            }

            var reader = new StreamReader(fileStream);
            string contents = reader.ReadToEnd();

            int index = contents.IndexOf("HEADER");
            int indexOfEndHeader = contents.IndexOf("ENDHEADER") + "ENDHEADER".Length + 1;
            var parameters = new FileInfo() { SampleCount = -1 };

            if (index != -1)
            {
                fileStream.Seek(index, SeekOrigin.Begin);
                bool endHeaderFound = false;
                while (!endHeaderFound)
                {
                    string headerInfo = reader.ReadLine();

                    string value = null;
                    int indexOfSpace = headerInfo.IndexOf(' ');
                    if (indexOfSpace != -1)
                    {
                        value = headerInfo.Substring(indexOfSpace);
                        headerInfo = headerInfo.Replace(value, string.Empty);
                    }

                    switch (headerInfo)
                    {
                        case "HEADER":
                            continue;
                        case "FREQUENCY":
                            parameters.Frequency = int.Parse(value);
                            break;
                        case "CHANNELS":
                            parameters.Channels = (Channel)Enum.Parse(typeof(Channel), value, true);
                            break;
                        case "SAMPLE":
                            parameters.SampleCount = int.Parse(value);
                            break;
                        case "SAMPLEBITS":
                            parameters.SampleBits = int.Parse(value);
                            break;
                        case "ENDHEADER":
                            endHeaderFound = true;
                            break;
                    }
                }

                fileStream.Seek(indexOfEndHeader, SeekOrigin.Begin);
                CountSamples(fileStream, parameters);
                fileStream.Seek(indexOfEndHeader, SeekOrigin.Begin);
            }

            return parameters;
        }

        public static void WriteHeaderInfo(Stream outputStream, FileInfo fileInfo)
        {
            var header = string.Format("HEADER{0}FREQUENCY {1}{0}CHANNELS {2}{0}SAMPLE {3}{0}SAMPLEBITS {4}{0}ENDHEADER{0}",
                                       "\n",
                                       fileInfo.Frequency,
                                       fileInfo.Channels.ToString().ToUpper(),
                                       fileInfo.SampleCount,
                                       fileInfo.SampleBits);

            var bytes = ASCIIEncoding.ASCII.GetBytes(header);
            outputStream.Write(bytes, 0, bytes.Length);
        }

        private static void CountSamples(Stream fileStream, FileInfo fileInfo)
        {
            if (fileInfo.SampleBits % 8 != 0)
            {
                throw new InvalidDataException("Sample bits is not a multiple of 8");
            }

            int sampleCount = 0;
            while (fileStream.Length != fileStream.Position)
            {
                SampleUtils.ReadSample(fileStream, fileInfo);
                sampleCount++;
            }

            if (fileInfo.SampleCount == -1)
            {
                fileInfo.SampleCount = sampleCount;
            }
            else if (fileInfo.SampleCount != sampleCount)
            {
                throw new InvalidDataException();
            }
        }
    }
}
