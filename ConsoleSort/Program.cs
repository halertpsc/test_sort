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
            var chunkSize = 2000000000;
            var chunk = new char[chunkSize];
            var index = new List<Range>();
            var chunkPositions = new List<long>();
            var nextChunkStartPositinInFIle = 0;


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
                    var range = new Range { Start = index.Count == 0 ? 0 : index[index.Count - 1].End, End = i };
                    index.Add(range);
                }
            }

            Console.WriteLine($"start sort {stopWatch.Elapsed}");
            // index.Sort(new IndexEqualityComparer(chunk));
            var indexArray = index.ToArray();
            Array.Sort(indexArray, new IndexEqualityComparer(chunk));
            Console.WriteLine($"end sort {stopWatch.Elapsed}");


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

