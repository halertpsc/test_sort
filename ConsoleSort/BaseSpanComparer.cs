using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleSort
{
    public abstract class BaseSpanComparer
    {
        protected int SpanCompare(Span<char> spanX, Span<char> spanY)
        {
            var pointPositionX = spanX.IndexOf('.');
            var pointPositionY = spanY.IndexOf('.');
            var stringPartX = spanX.Slice(pointPositionX);
            var stringPartY = spanY.Slice(pointPositionY);

            var stringComparisonResult = stringPartX.SequenceCompareTo(stringPartY);
            if (stringComparisonResult != 0)
            {
                return stringComparisonResult;
            }

            var numerPartX = spanX.Slice(0, pointPositionX);
            var numerPartY = spanY.Slice(0, pointPositionX);

            var trimmedPartX = numerPartX.TrimStart('0');
            var trimmedPartY = numerPartY.TrimStart('0');

            if (trimmedPartX.Length < trimmedPartY.Length) return 1;
            if (trimmedPartX.Length > trimmedPartY.Length) return -1;

            return trimmedPartX.SequenceCompareTo(trimmedPartY);
        }
    }
}
