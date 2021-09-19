using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ConsoleSort
{
    public abstract class FileMerger : BaseSpanComparer, IMerger<string>
    {
        private readonly string _dirName;

        protected FileMerger(string dirName)
        {
            _dirName = dirName ?? throw new ArgumentNullException(nameof(dirName));
        }

        public string Merge(string first, string second)
        {
            var outputFileNmae = @$"{_dirName}/{Guid.NewGuid().ToString("N")}";

            using (var firstFile = new BufferedFile(first))
            using (var secondFile = new BufferedFile(second))
            using (var outputFile = new StreamWriter(outputFileNmae))
            {
                var firstSapn = firstFile.NextString(out var firsFileEnded);
                var secondSapn = secondFile.NextString(out var secondFileEnded);
                while (firsFileEnded || secondFileEnded)
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
                if(firsFileEnded)
                {
                    outputFile.Write(secondFile.GetRestFromBuffer());
                    var reader = secondFile.GetStreamReader();
                    if(!reader.EndOfStream)
                    {
                        reader.BaseStream.CopyTo(outputFile.BaseStream);
                    }
                }
                else
                {
                    outputFile.Write(firstFile.GetRestFromBuffer());
                    var reader = firstFile.GetStreamReader();
                    if(!reader.EndOfStream)
                    {
                        reader.BaseStream.CopyTo(outputFile.BaseStream);
                    }
                }
                outputFile.Close();
            }
            return outputFileNmae;
        }
    }
}
