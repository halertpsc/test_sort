using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ConsoleSort
{
    public class BufferedFile : IDisposable
    {
        const int bufferSize = 1_000_000_000;

        public BufferedFile(string fileName)
        {
            _file = new StreamReader(fileName);
        }
     
        public Span<char> NextString(out bool endOfFile)
        {
            var startIndex = _bufferPosition;
            var conutFromStart = 0;
             endOfFile = false;
            while (_buffer[startIndex + conutFromStart] != 0x0A)
            {
                if (startIndex + conutFromStart == _actualDataSize)
                {
                    if (_actualDataSize == bufferSize)
                    {
                        _buffer.AsSpan().Slice(startIndex).CopyTo(_buffer);
                        _actualDataSize = _file.Read(_buffer, conutFromStart, bufferSize - conutFromStart) + conutFromStart;
                        startIndex = 0;
                    }
                    else
                    {
                        endOfFile = true;
                    }
                }
                conutFromStart++;
            }
            _bufferPosition = conutFromStart + 1;
            return _buffer.AsSpan().Slice(startIndex, conutFromStart);
        }

        public Span<char> GetRestFromBuffer()
        {
            return _buffer.AsSpan().Slice(_bufferPosition, _actualDataSize);
        }

        public StreamReader GetStreamReader()
        {
            return _file;
        }

        public void Dispose()
        {
            _file.Close();
            _file.Dispose();
        }

        private char[] _buffer = new char[bufferSize];

        private StreamReader _file;

        private int _bufferPosition = bufferSize;

        private int _actualDataSize = 0;
    }
}
