﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;


namespace ConsoleSort
{
    public class IndexEqualityComparer : IComparer<Range>
    {

        char[] _data;
        public IndexEqualityComparer(char[] data)
        {
            _data = data;
        }

        public int Compare(Range x, Range y)
        {
            var dataSpan = new Span<char>(_data);
            var spanX = dataSpan.Slice(x.Start, x.End - x.Start);
            var spanY = dataSpan.Slice(y.Start, y.End - y.Start);

            var pointPositionX = spanX.IndexOf('.');
            var pointPositionY = spanY.IndexOf('.');
            var stringPartX = spanX.Slice(pointPositionX);
            var stringPartY = spanY.Slice(pointPositionY);

            var stringComparisonResult = stringPartX.SequenceCompareTo(stringPartY);
            if(stringComparisonResult != 0)
            {
                return stringComparisonResult;
            }

            var numerPartX = spanX.Slice(0, pointPositionX);
            var numerPartY = spanY.Slice(0, pointPositionX);

            var trimmedPartX  = numerPartX.TrimStart('0');
            var trimmedPartY = numerPartY.TrimStart('0');

            if (trimmedPartX.Length < trimmedPartY.Length) return 1;
            if (trimmedPartX.Length > trimmedPartY.Length) return -1;

            return trimmedPartX.SequenceCompareTo(trimmedPartY);

        }

        //private int CompareArraysAsStrings(Span<char> x, Span<char> y)
        //{
        //    var length = Math.Min(x.Length, y.Length);
        //    for(int i = 0; i<length; i++)
        //    {
        //        if (x[i] > y[i]) return 1;
        //        if (x[i] < y[i]) return -1;
        //    }
        //    if (x.Length < y.Length) return 1;
        //    if (x.Length > y.Length) return -1;
        //    return 0;
        //}

        //private int CompareArraysAsNumbers(Span<char> x, Span<char> y)
        //{
        //    x.SequenceCompareTo
        //}
    }
}
