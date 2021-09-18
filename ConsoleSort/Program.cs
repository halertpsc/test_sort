using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ConsoleSort
{
    class Program
    {
        static void Main(string[] args)
        {
            var stopWatch = System.Diagnostics.Stopwatch.StartNew();
            var sourceFileName = @$"c:\test\data1.txt";
            var destinationFileName = @"c:\test\result.txt";
            var maxThreadsFactor = 8;
            var chunkSize = 2_000_000_000;
            var chunk = new char[chunkSize];
            var index = new List<ItemHandler>();
            var chunkPositions = new List<long>();
            var nextChunkStartPositinInFIle = 0;
            var processes = new List<SortProcess>(maxThreadsFactor);


            //read chunk
            using (var file = new StreamReader(sourceFileName, System.Text.Encoding.UTF8))
            {
                file.Read(chunk);
            }
            //build index
            for(int i = 0; i< chunk.Length; i++)
            {
                if(chunk[i] == 0x0A)
                {
                    var range = new ItemHandler { Start = index.Count == 0 ? 0 : index[index.Count - 1].End, End = i };
                    index.Add(range);
                }
            }

            var averageLength = index.Count / maxThreadsFactor;
            var previousPosition = 0;
            for(int i = 0; i<maxThreadsFactor;i++)
            {
                var count = previousPosition + averageLength < index.Count ? averageLength : index.Count - previousPosition;
                processes.Add(new SortProcess(index.GetRange(previousPosition, count)));
                previousPosition = previousPosition + count;
            }

            Console.WriteLine($"start sorting {stopWatch.Elapsed}");
            var tasks = new List<Task>();
            foreach(var process in processes)
            {
                tasks.Add(Task.Run(() => process.Sort(chunk)));
            }

            Task.WaitAll(tasks.ToArray());
            Console.WriteLine($"end sorting {stopWatch.Elapsed}");

            index.Clear();
            Console.WriteLine($"start meging {stopWatch.Elapsed}");
            var merger = new Merger(new IndexEqualityComparer(chunk));
            foreach(var process in processes)
            {
                index = merger.Merge(index, process.Index);
            }
            Console.WriteLine($"end merging {stopWatch.Elapsed}");


            Console.WriteLine($"start writing {stopWatch.Elapsed}");
            using (var file = new StreamWriter(destinationFileName))
            {
                index.ForEach(idx => file.Write(chunk, idx.Start, idx.End - idx.Start));
            }
            Console.WriteLine($"end writing {stopWatch.Elapsed}");

            stopWatch.Stop();
            Console.WriteLine(stopWatch.Elapsed);
            Console.ReadLine();
        }
    }
}

