using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;


namespace ConsoleSort
{
    public class HandlersComparer : BaseSpanComparer,  IComparer<ItemHandler>
    {
        private char[] _data;
        public HandlersComparer(char[] data)
        {
            _data = data;
        }

        public int Compare(ItemHandler x, ItemHandler y)
        {
            var dataSpan = new Span<char>(_data);
            var spanX = dataSpan.Slice(x.Start, x.End - x.Start);
            var spanY = dataSpan.Slice(y.Start, y.End - y.Start);

            return SpanCompare(spanX, spanY);
        }
    }
}
