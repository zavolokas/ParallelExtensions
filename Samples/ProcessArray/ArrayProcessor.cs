using System;
using BenchmarkDotNet.Attributes;
using Zavolokas.Arrays;
using Zavolokas.Structures;

namespace ArrayProcessing
{
    public class ArrayProcessor
    {
        [Params(80)]
        public int NotDividableMinAmountElements { get; set; } = 80;

        //Environment.ProcessorCount
        [Params(4)]
        public byte ProcessorCount { get; set; }

        byte ComponentsAmount;
        Area2D ImageArea;

        public ArrayProcessor()
        {
            ComponentsAmount = 1;
            ImageArea = Area2D.Create(0, 0, 480, 360);
        }

        [Benchmark]
        public unsafe double[] NewMethod()
        {
            void PowPixels(double[] pixelsData, int[] indecies, int chunckStartIndex, int chunckLastIndex, Func<int, bool> isValidIndex)
            {
                fixed (double* pixelsDataP = pixelsData)
                {
                    for (int j = chunckLastIndex; j >= chunckStartIndex; j--)
                    {
                        int absIndex = indecies[j] * ComponentsAmount;
                        if (!isValidIndex(absIndex))
                            continue;
                        // components should be in the range [0.0 , 1.0]
                        double g = *(pixelsDataP + absIndex + 0);
                        g = g * g;
                        *(pixelsDataP + absIndex + 0) = g;
                    }
                }
            }

            double[] pixelsData1 = new double[ImageArea.Bound.Width * ImageArea.Bound.Height * ComponentsAmount];
            pixelsData1.ProcessChuncksInParallel(PowPixels, ImageArea);
            return pixelsData1;
        }

        //[Benchmark]
        //public unsafe double[] OldMethod()
        //{
        //    double[] pixelsData1 = new double[ImageArea.Bound.Width * ImageArea.Bound.Height * ComponentsAmount];

        //    int pointsAmount = ImageArea.ElementsCount;
        //    var pointIndecies = new int[pointsAmount];
        //    ImageArea.FillMappedPointsIndexes(pointIndecies, ImageArea.Bound.Width);

        //    Action<double[], int[], int, int> processing = (pixelsData, indecies, startIndex, lastIndex) =>
        //    {
        //        fixed (double* pixelsDataP = pixelsData)
        //        {
        //            for (int j = lastIndex; j >= startIndex; j--)
        //            {
        //                int absIndex = indecies[j] * ComponentsAmount;
        //                // components should be in the range [0.0 , 1.0]
        //                double g = *(pixelsDataP + absIndex + 0);
        //                g = g * g;
        //                *(pixelsDataP + absIndex + 0) = g;
        //            }
        //        }
        //    };

        //    pixelsData1.ApplyInParallel(pointIndecies, ProcessorCount, NotDividableMinAmountElements, processing);
        //    return pixelsData1;
        //}
    }
}