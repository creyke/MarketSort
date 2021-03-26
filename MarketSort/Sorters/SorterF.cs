using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MarketSort.Solutions
{
    class SorterF : ISorter
    {
        /// <summary>
        /// Goal: Minimal code
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public IEnumerable<string> Sort(IEnumerable<string> values)
        {
            return values
                .OrderBy(s => Regex.IsMatch(s, "[A-Z]"))
                .ThenByDescending(
                    s => Regex.Replace(s, "[0-9]+",
                    s => s.Value.Split("-")[0].PadLeft(10, '0')))
                .Reverse();
        }
    }
}
