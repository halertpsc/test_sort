using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ConsoleSort
{
    public class FileMerger : BaseSpanComparer, IMerger<string>
    {
        private readonly string _dirName;
        private const int bufferSize = 200_000_000;

        public FileMerger(string dirName)
        {
            _dirName = dirName ?? throw new ArgumentNullException(nameof(dirName));
        }

        public string Merge(string first, string second)
        {
            var outputFileNmae = @$"{_dirName}/{Guid.NewGuid().ToString("N")}.txt";

            using (var firstFile = new BufferedFile(first, bufferSize))
            using (var secondFile = new BufferedFile(second, bufferSize))
            using (var outputFile = new StreamWriter(outputFileNmae))
            {
                var firstSapn = firstFile.NextString(out var firsFileEnded);
                var secondSapn = secondFile.NextString(out var secondFileEnded);
                while(!firsFileEnded && !secondFileEnded)
                {
                    if (SpanCompare(firstSapn, secondSapn) < 0)
                    {
                        outputFile.Write(firstSapn);
                        firstSapn = firstFile.NextString(out firsFileEnded);
                    }
                    else
                    {
                        outputFile.Write(secondSapn);
                        secondSapn = secondFile.NextString(out secondFileEnded);
                    }
                }
                if (firsFileEnded)
                {
                    outputFile.Write(secondSapn);
                    outputFile.Write(secondFile.GetFromBuffer());
                    secondFile.GetBaseStream().CopyTo(outputFile.BaseStream);

                }
                else
                {
                    outputFile.Write(firstSapn);
                    outputFile.Write(firstFile.GetFromBuffer());
                    firstFile.GetBaseStream().CopyTo(outputFile.BaseStream);

                }
                outputFile.Flush();
                outputFile.Close();
            }
            File.Delete(first);
            File.Delete(second);
            return outputFileNmae;
        }
    }
}
