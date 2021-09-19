using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleSort
{
    public interface IMerger<T>
    {
        T Merge(T first, T second);
    }
}
