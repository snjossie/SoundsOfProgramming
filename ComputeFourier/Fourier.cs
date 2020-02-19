using System;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;
using Utility;
using FileInfo = Utility.FileInfo;

namespace ComputeFourier
{
    public class Fourier
    {
        public static double[] Calculate(uint[] samples, int numSamples)
        {
            Complex[] complexTransform = CalculateComplex(samples, numSamples);
            double[] computed = new double[numSamples];

            for (int i = 0; i < numSamples; i++)
            {
                computed[i] = complexTransform[i].Magnitude;
            }

            return computed;
        }

        public static uint[] CalculateInt(uint[] samples, int numSamples)
        {

            Complex[] complexTransform = CalculateComplex(samples, numSamples);
            uint[] computed = new uint[numSamples];

            for (int i = 0; i < numSamples; i++)
            {
                computed[i] = (uint)complexTransform[i].Magnitude;
            }

            return computed;
        }

        public static Complex[] CalculateComplex(uint[] samples, int numSamples)
        {
            Complex[] complexTransform = new Complex[numSamples];
            // e^(it) => cos(t) + i * sin(t)
            double partialCos = 0;
            double partialSin = 0;

            object lockObject = new object();

            for (int k = 0; k < numSamples; k++)
            {
                //for (int n = 0; n < numSamples; n++)
                //{
                //    partialCos += Math.Cos(Math.PI * 2 * n * k / numSamples) * samples[n];
                //    partialSin += Math.Sin(Math.PI * 2 * n * k / numSamples) * samples[n];
                //}

                Parallel.For<double[]>(0, numSamples, () => new double[2], (n, loop, subtotal) =>
                    {
                        subtotal[0] += Math.Cos(Math.PI * 2 * n * k / numSamples) * samples[n];
                        subtotal[1] += Math.Sin(Math.PI * 2 * n * k / numSamples) * samples[n];
                        return subtotal;
                    },
                    (x) =>
                    {
                        lock (lockObject)
                        {
                            partialCos += x[0];
                            partialSin += x[1];
                        }
                    });

                complexTransform[k] = new Complex(partialCos, partialSin);

                partialSin = 0;
                partialCos = 0;
            }

            return complexTransform;
        }

        public static uint[] ReadSamples(Stream readFrom, int startAt, int windowSize, FileInfo info)
        {
            readFrom.Seek(-((info.SampleBits / 8) * (info.SampleCount - startAt)), SeekOrigin.End);
            uint[] samples = new uint[windowSize];

            int i = 0;

            for (; i < windowSize; i++)
            {
                samples[i] = SampleUtils.ReadSample(readFrom, info);
                if (info.Channels == Channel.Stereo)
                {
                    SampleUtils.ReadSample(readFrom, info);
                }
            }

            return samples;
        }
    }
}