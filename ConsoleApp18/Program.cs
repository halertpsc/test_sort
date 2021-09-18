using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ConsoleApp18
{
    class Program
    {
        static void Main(string[] args)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
      

            var list1 = new List<string>();
            var list2 = new List<string>();
            var list3 = new List<string>();

            var filename1 = @$"c:\test\data1.txt";
            var filename2 = @$"c:\test\data2.txt";
            var filename3 = @$"c:\test\data3.txt";

            var rnd = new Random();
            var amount =  300000000;
            var repeatableAmount =  200000;
            var randomRange = amount - repeatableAmount;
            for (long i = 0; i <= amount; i++)
            {

                var str = ProduceString(rnd);
                list1.Add($"{rnd.Next(randomRange)}. {str}");
                if (rnd.Next(0, 3) == 0)
                {
                    list2.Add($"{rnd.Next(randomRange)}. {str}");
                }
                if (rnd.Next(0, 5) == 0)
                {
                    list3.Add($"{rnd.Next(randomRange)}. {str}");
                }

              

                if (list1.Count + list2.Count + list3.Count > 10000000)
                {
                    Console.WriteLine($"flush started  {stopwatch.Elapsed}");
                    File.AppendAllLines(filename1, list1);
                    File.AppendAllLines(filename2, list2);
                    File.AppendAllLines(filename3, list3);

                    list1.Clear();
                    list2.Clear();
                    list3.Clear();
                    Console.WriteLine($"flush completed  {stopwatch.Elapsed}");
                }
            }
            Console.WriteLine($"flush started  {stopwatch.Elapsed}");
            File.AppendAllLines(filename1, list1);
            File.AppendAllLines(filename2, list2);
            File.AppendAllLines(filename3, list3);

            list1.Clear();
            list2.Clear();
            list3.Clear();
            Console.WriteLine($"flush completed  {stopwatch.Elapsed}");
            stopwatch.Stop();

            using (var targetFile = File.OpenWrite(filename1))
            {
                targetFile.Seek(0, SeekOrigin.End);
                foreach (var fileName in new List<string> { filename2, filename3 })
                {
                    using (var sourceFile = File.OpenRead(fileName))
                    {
                        sourceFile.CopyTo(targetFile);
                    }
                }
            }
            Console.WriteLine(stopwatch.Elapsed);
            Console.ReadLine();
        }

        private static string ProduceString(Random random)
        {
            var stringBuilder = new StringBuilder();
            var numberOfWords = random.Next(1, 4);
            for (int i = 0; i <= numberOfWords; i++)
            {
                var numberOfLetters = random.Next(0, 10);
                for (int j = 0; j <= numberOfLetters; j++)
                {

                    stringBuilder.Append( (char)random.Next(0x61, 0x7A));

                }
                stringBuilder.Append(' ');

            }
            return stringBuilder.ToString();
        }
    }
}
