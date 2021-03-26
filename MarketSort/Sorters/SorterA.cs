using System.Collections.Generic;
using System.Linq;

namespace MarketSort.Solutions
{
    class SorterA : ISorter
    {
        /// <summary>
        /// I tried to implement the entire process in a single linq statement without using anonymous functions.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public IEnumerable<string> Sort(IEnumerable<string> values)
        {
            return values
                .GroupBy(x => x.Last() != '-' && x.Split('-').All(y => int.TryParse(y, out _)))
                .OrderBy(x => x.Key)
                .SelectMany(x => x.Key
                    ? x
                        .Select(y => (n: y, c: y.Split('-').Select(z => int.Parse(z))))
                        .OrderBy(y => y.c.First())
                        .ThenBy(y => y.c.ElementAt(1))
                        .Select(x => x.n)
                    : x.
                        OrderBy(y => y));
        }
    }
}
