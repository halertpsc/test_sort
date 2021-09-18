using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleSort
{
    public class Merger
    {
        private readonly IndexEqualityComparer _indexEqualityComparer;

        public Merger(IndexEqualityComparer comparer)
        {
            _indexEqualityComparer = comparer;
        }

        public List<ItemHandler> Merge(List<ItemHandler> first, List<ItemHandler> second)
        {
            var result = new List<ItemHandler>();
            var firstPosition = 0;
            var secondPosition = 0;
            while (firstPosition < first.Count && secondPosition < second.Count)
            {
                if (_indexEqualityComparer.Compare(first[firstPosition], second[secondPosition]) < 0)
                {
                    result.Add(first[firstPosition]);
                    firstPosition++;
                }
                else
                {
                    result.Add(second[secondPosition]);
                    secondPosition++;
                }
            }

            if(firstPosition == first.Count)
            {
                result.AddRange(second.GetRange(secondPosition, second.Count - secondPosition));
            }
            else
            {
                result.AddRange(first.GetRange(firstPosition, first.Count - firstPosition));
            }

            return result;
        }
    }
}
