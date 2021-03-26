using System;
using System.Collections.Generic;
using System.Linq;

namespace MarketSort.Solutions
{
    class SorterC : ISorter
    {
        /// <summary>
        /// I started off by *duplicating* the values so I can preserve the original value of one, but also manipulate the other, without having to reconstitute it back into it's original form.
        /// I did this by creating a dictionary, keyed by a Tuple(Home and Away scores) by splitting on "-".
        /// Any values that don't return a successful split are given a default *no-order* that appear at the start of the list.
        /// Once I've got those home and away keyed tuples, It's simply a case of doing a linq orderby Home team, then by the Away team.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public IEnumerable<string> Sort(IEnumerable<string> values)
        {
            return values
                .ToDictionary(key =>
                {
                    var split = key.Split('-');
                    return split.Length == 1
                    ? (-1, -1)
                    : (
                        Convert.ToInt32(split[0]),
                        Convert.ToInt32(split[1])
                    );
                }, val => val)
                .OrderBy(x => x.Key.Item1)
                .ThenBy(x => x.Key.Item2)
                .Select(x => x.Value);
        }
    }
}
