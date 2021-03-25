using System.Collections.Generic;

namespace MarketSort
{
    interface ISorter
    {
        IEnumerable<string> Sort(IEnumerable<string> values);
    }
}
