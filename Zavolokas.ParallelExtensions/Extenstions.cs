using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zavolokas.Structures;

namespace Zavolokas.Arrays
{
    public static class Extenstions
    {
        private const int ProcessorAmount = 4;
        private const int MinChunckSize = 20;

        public static void ProcessChuncksInParallel<T>(this T[] data,
            Action<T[], int, int> processingToApply,
            int maxChuncksAmount = ProcessorAmount,
            int minChunckSize = MinChunckSize)
        {
            int pointsAmount = data.Length;
            // Decide on how many partitions we should divade the processing
            // of the elements.
            var chunksCount = pointsAmount > minChunckSize * maxChuncksAmount
                ? maxChuncksAmount
                : 1;

            var chunckSize = (int)(pointsAmount / chunksCount);

            Parallel.For(0, chunksCount, chunckIndex =>
            {
                var firstIndex = chunckIndex * chunckSize;
                var lastIndex = firstIndex + chunckSize - 1;
                if (chunckIndex == chunksCount - 1) lastIndex = pointsAmount - 1;
                if (lastIndex > pointsAmount) lastIndex = pointsAmount - 1;

                processingToApply(data, firstIndex, lastIndex);
            });
        }

        public static void ProcessChuncksInParallel<T>(this T[] data,
            Action<T[], int[], int, int, Func<int, bool>> processingToApply,
            int[] indeciesToProcess,
            Func<int, bool> isIndexToProcess,
            int maxChunksAmount = ProcessorAmount,
            int minChunckSize = MinChunckSize)
        {
            int pointsAmount = indeciesToProcess.Length;
            // Decide on how many partitions we should divade the processing
            // of the elements.
            var chunksCount = pointsAmount > minChunckSize * maxChunksAmount
                ? maxChunksAmount
                : 1;

            var chunckSize = (int)(pointsAmount / chunksCount);

            Parallel.For(0, chunksCount, chunckIndex =>
            {
                var firstIndex = chunckIndex * chunckSize;
                var lastIndex = firstIndex + chunckSize - 1;
                if (chunckIndex == chunksCount - 1) lastIndex = pointsAmount - 1;
                if (lastIndex > pointsAmount) lastIndex = pointsAmount - 1;

                processingToApply(data, indeciesToProcess, firstIndex, lastIndex, isIndexToProcess);
            });
        }

        public static void ProcessChuncksInParallel<T>(this T[] data,
            Action<T[], int[], int, int, Func<int, bool>> processingToApply,
            Area2D areaToProcess,
            int maxChunksAmount = ProcessorAmount,
            int minChunckSize = MinChunckSize)
        {
            int pointsAmount = areaToProcess.ElementsCount;
            var indeciesToProcess = new int[pointsAmount];
            areaToProcess.FillMappedPointsIndexes(indeciesToProcess, areaToProcess.Bound.Width);
            var indeciesSet = new HashSet<int>(indeciesToProcess);

            bool IsIndexToProcess(int pointIndex) => indeciesSet.Contains(pointIndex);

            data.ProcessChuncksInParallel(processingToApply, indeciesToProcess, IsIndexToProcess, maxChunksAmount, minChunckSize);
        }

        #region Comment
        /// <summary>
        /// Processes data blocks in parallel. Blocks are result of horisontal splitting of the data array that is treated as a 2-demensional.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">The data to process.</param>
        /// <param name="width">The width of the array to process.</param>
        /// <param name="height">The height of the array to process.</param>
        /// <param name="processingToApply">
        /// The processing action to apply.<br />
        /// <list type="bullet">
        ///     <item>
        ///         <term>T[] data</term>
        ///         <description> - an array to process</description>
        ///     </item>
        ///     <item>
        ///         <term>int width</term>
        ///         <description> - a width of the array</description>
        ///     </item>
        ///     <item>
        ///         <term>int height</term>
        ///         <description> - a height of the array</description>
        ///     </item>
        ///     <item>
        ///         <term>int[] indeciesToProcess</term>
        ///         <description> 
        ///             - indecies within the block that need to be processed. 
        ///             Note that not all the data needs to be processed because of passed Area2D.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>int firstY</term>
        ///         <description> - first Y coordinate to process. Where the block starts.</description>
        ///     </item>
        ///     <item>
        ///         <term>int lastY</term>
        ///         <description> - last Y coordinate to process. Where the block ends.</description>
        ///     </item>
        ///     <item>
        ///         <term>Func&lt;int, bool&gt; isValidIndex</term>
        ///         <description> - function to validate indecies whether they are reside within the provided Area2D.</description>
        ///     </item>
        /// </list>
        /// </param>
        /// <param name="indeciesToProcess">The indecies to process.</param>
        /// <param name="isIndexToProcess">Function to validate indecies whether they are reside within the provided indecies array.</param>
        /// <param name="maxBlocksAmount">The maximum blocks amount.</param>
        /// <param name="minBlockSize">Minimum size of the block.</param>
        #endregion
        public static void ProcessBlocksInParallel<T>(this T[] data,
            int width,
            int height,
            Action<T[], int, int, int[], int, int, Func<int, bool>> processingToApply,
            int[] indeciesToProcess,
            Func<int, bool> isIndexToProcess,
            int maxBlocksAmount = ProcessorAmount,
            int minBlockSize = MinChunckSize)
        {
            int pointsAmount = indeciesToProcess.Length;
            // Decide on how many partitions we should divade the processing
            // of the elements.
            var blocksCount = pointsAmount > minBlockSize * maxBlocksAmount
                ? maxBlocksAmount
                : 1;

            var blockHeight = (int)(height / blocksCount);

            Parallel.For(0, blocksCount, blockIndex =>
            {
                var firstY = blockIndex * blockHeight;
                var lastY = firstY + blockHeight - 1;
                if (blockIndex == blocksCount - 1) lastY = height - 1;

                processingToApply(data, width, height, indeciesToProcess, firstY, lastY, isIndexToProcess);
            });
        }

        #region Comment
        /// <summary>
        /// Processes data blocks in parallel. Blocks are result of horisontal splitting of the data array that is treated as a 2-demensional.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">The data to process.</param>
        /// <param name="width">The width of the array to process.</param>
        /// <param name="height">The height of the array to process.</param>
        /// <param name="processingToApply">
        /// The processing action to apply.<br />
        /// <list type="bullet">
        ///     <item>
        ///         <term>T[] data</term>
        ///         <description> - an array to process</description>
        ///     </item>
        ///     <item>
        ///         <term>int width</term>
        ///         <description> - a width of the array</description>
        ///     </item>
        ///     <item>
        ///         <term>int height</term>
        ///         <description> - a height of the array</description>
        ///     </item>
        ///     <item>
        ///         <term>int[] indeciesToProcess</term>
        ///         <description> 
        ///             - indecies within the block that need to be processed. 
        ///             Note that not all the data needs to be processed because of passed Area2D.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>int firstY</term>
        ///         <description> - first Y coordinate to process. Where the block starts.</description>
        ///     </item>
        ///     <item>
        ///         <term>int lastY</term>
        ///         <description> - last Y coordinate to process. Where the block ends.</description>
        ///     </item>
        ///     <item>
        ///         <term>Func&lt;int, bool&gt; isValidIndex</term>
        ///         <description> - function to validate indecies whether they are reside within the provided Area2D.</description>
        ///     </item>
        /// </list>
        /// </param>
        /// <param name="areaToProcess">The area to process.</param>
        /// <param name="maxBlocksAmount">The maximum blocks amount.</param>
        /// <param name="minBlockSize">Minimum size of the block.</param>
        #endregion
        public static void ProcessBlocksInParallel<T>(this T[] data,
            int width,
            int height,
            Action<T[], int, int, int[], int, int, Func<int, bool>> processingToApply,
            Area2D areaToProcess,
            int maxBlocksAmount = ProcessorAmount,
            int minBlockSize = MinChunckSize)
        {
            int pointsAmount = areaToProcess.ElementsCount;
            var indeciesToProcess = new int[pointsAmount];
            areaToProcess.FillMappedPointsIndexes(indeciesToProcess, areaToProcess.Bound.Width);
            var indeciesSet = new HashSet<int>(indeciesToProcess);

            bool IsIndexToProcess(int pointIndex) => indeciesSet.Contains(pointIndex);
            data.ProcessBlocksInParallel(width, height, processingToApply, indeciesToProcess, IsIndexToProcess, maxBlocksAmount, minBlockSize);
        }
    }
}