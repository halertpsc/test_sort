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
           
            var tempFileDir = @$"c:\test";
            var sourceFileName = @$"{tempFileDir}\data1.txt";
            var destinationFileName = $@"{tempFileDir}\result.txt";
            var tempFileName = @$"{tempFileDir}\temp";
            var maxThreadsFactor = 8;
            var chunkSize = 2_000_000_000;
            var chunk = new char[chunkSize];
            var actualChunkSize = 0;
            var endOfStringPositionForPreviousChunk = chunkSize;
            var tempFileNumber = 0;
            var tempFilesList = new List<string>();

            Directory.CreateDirectory(tempFileDir);

            //read chunk
            using (var file = new StreamReader(sourceFileName, System.Text.Encoding.UTF8))
            {
                while (true)
                {
                    actualChunkSize = file.Read(chunk, chunkSize - endOfStringPositionForPreviousChunk, endOfStringPositionForPreviousChunk) + chunkSize - endOfStringPositionForPreviousChunk ;
                    if (actualChunkSize == 0)
                    {
                        break;
                    }
                    tempFileNumber++;
                    //build index
                    //each thread will have its own index to deal with
                    var indexes = new List<ItemHandler>[maxThreadsFactor];
                    var activeIndex = 0;
                    ItemHandler? previousHandler = null;
                    InitIndices(indexes, maxThreadsFactor);
                  
                    for (int i = 0; i < actualChunkSize; i++)
                    {

                        if (chunk[i] == 0x0A)
                        {
                            var handler = new ItemHandler { Start = previousHandler == null ? 0 : previousHandler.Value.End + 1, End = i };
                            previousHandler = handler;
                            indexes[activeIndex].Add(handler);
                            if (activeIndex == maxThreadsFactor - 1)
                            {
                                activeIndex = 0;
                            }
                            else
                            {
                                activeIndex++;
                            }
                        }
                    }

                    if (previousHandler != null)
                    {
                        endOfStringPositionForPreviousChunk = previousHandler.Value.End + 1;
                    }

              
                    Console.WriteLine($"start sorting {stopWatch.Elapsed}");
                    var tasks = new List<Task>();
                    foreach (var index in indexes)
                    {
                        tasks.Add(Task.Run(() =>
                        {
                            var comparer = new HandlersComparer(chunk);
                            index.Sort(comparer);
                        }));
                    }

                    Task.WaitAll(tasks.ToArray());
                    Console.WriteLine($"end sorting {stopWatch.Elapsed}");


                    Console.WriteLine($"start meging {stopWatch.Elapsed}");

                    var merger = new HandlerMerger(new HandlersComparer(chunk));
                    var commonIndex = Merge(indexes, merger);

                    Console.WriteLine($"end merging {stopWatch.Elapsed}");


                    Console.WriteLine($"start writing {stopWatch.Elapsed}");
                    using (var targetFile = new StreamWriter($"{tempFileName}{tempFileNumber}"))
                    {
                        commonIndex.ForEach(idx => targetFile.Write(chunk, idx.Start, idx.End - idx.Start +1));
                        targetFile.Flush();
                        targetFile.Close();
                    }
                    tempFilesList.Add($"{tempFileName}{tempFileNumber}");
                    Console.WriteLine($"end writing {stopWatch.Elapsed}");

                    var span = chunk.AsSpan().Slice(endOfStringPositionForPreviousChunk);
                    span.CopyTo(chunk);

                }
                chunk = null;
            }

            Console.WriteLine($"start file merge {stopWatch.Elapsed}");
            var fileMerger = new FileMerger(tempFileDir);
            Merge(tempFilesList, fileMerger);
            Console.WriteLine($"end file merge {stopWatch.Elapsed}");
            stopWatch.Stop();
            Console.WriteLine(stopWatch.Elapsed);
            Console.ReadLine();
        }

        private static void InitIndices(List<ItemHandler>[] lists, int maxThread)
        {
            for (int i = 0; i < maxThread; i++)
            {
                lists[i] = new List<ItemHandler>();
            }
        }

     
        private static T Merge<T>(IEnumerable<T> lists, IMerger<T> merger)
        {
            
            var mergeQueue = new Queue<T>();
            foreach (var list in lists)
            {
                mergeQueue.Enqueue(list);
            }

            var mergeTasks = new List<Task<T>>();

            while (mergeQueue.Count > 1)
            {
                while (mergeQueue.Count > 1)
                {
                    var first = mergeQueue.Dequeue();
                    var second = mergeQueue.Dequeue();
                    mergeTasks.Add(Task.Run(() => merger.Merge(first, second)));
                }
                Task.WaitAll(mergeTasks.ToArray());
                foreach (var task in mergeTasks)
                {
                    mergeQueue.Enqueue(task.Result);
                }
                mergeTasks.Clear();
            }

            return mergeQueue.Dequeue();
        }
    }
}

