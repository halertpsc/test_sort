using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ConsoleSort
{
    public class BufferedFile : IDisposable
    {
        private readonly int _bufferSize;;;

        private char[] _buffer;

        private StreamReader _file;

        private int _currentStringBegin = 0;
        private int _currentStringEnd = -1;
        private int _currentStringSearchPosition = 0;
        private int _dataEndPosition = 0;

        bool stringProviderInvalid = false;
        bool partialBufferReturned = false;

        public BufferedFile(string fileName, int size)
        {
            _file = new StreamReader(fileName);
            _bufferSize = size;
            _buffer = new char[_bufferSize];
        }

   
        public Span<char> NextString(out bool endOfFile)
        {
            if(stringProviderInvalid)
            {
                throw new InvalidOperationException();
            }
            endOfFile = false;
            if (_currentStringEnd == _dataEndPosition && _file.EndOfStream)
            {
                endOfFile = true;
                return _buffer.AsSpan().Slice(0, 0);
            }
            _currentStringBegin = _currentStringEnd + 1;
            _currentStringSearchPosition = _currentStringBegin;

            if (_dataEndPosition == 0 || _currentStringSearchPosition == _bufferSize)
            {
                _dataEndPosition = _file.Read(_buffer) - 1;
                _currentStringBegin = 0;
                _currentStringSearchPosition = 0;
                if(_dataEndPosition == -1)
                {
                    endOfFile = false;
                    return _buffer.AsSpan().Slice(0, 0);
                }
            }

            while (_buffer[_currentStringSearchPosition] != 0x0A)
            {
               
                if (_currentStringSearchPosition == _bufferSize -1)
                {
                    _buffer.AsSpan().Slice(_currentStringBegin).CopyTo(_buffer);
                    _dataEndPosition = _file.Read(_buffer, _bufferSize - _currentStringBegin, _currentStringBegin) + _bufferSize - _currentStringBegin - 1;
                    _currentStringSearchPosition = _currentStringSearchPosition - _currentStringBegin;
                    _currentStringBegin = 0;
                }
               
                _currentStringSearchPosition++;
            }
             _currentStringEnd = _currentStringSearchPosition;            
            return _buffer.AsSpan().Slice(_currentStringBegin, _currentStringEnd - _currentStringBegin + 1);
        }

       

        public Span<char> NextBuffer(out bool endOfFile)
        {
            endOfFile = false;
            if (!partialBufferReturned)
            {
                return GetFromBuffer();
            }
            var readed = _file.Read(_buffer);
            if(readed == 0 )
            {
                endOfFile = true;
                return _buffer.AsSpan().Slice(0, 0);
            }
            return _buffer.AsSpan().Slice(0, readed);
        }

        public Span<char> GetFromBuffer()
        {
            stringProviderInvalid = true;
            partialBufferReturned = true;
            if(_dataEndPosition - _currentStringEnd == 0)
            {
                return _buffer.AsSpan().Slice(0, 0);
            }
            return _buffer.AsSpan().Slice(_currentStringEnd + 1, _dataEndPosition - _currentStringEnd);
        }

        public Stream GetBaseStream()
        {
            return _file.BaseStream;
        }

        public void Dispose()
        {
            _file.Close();
            _file.Dispose();
        }

     
    }
}
