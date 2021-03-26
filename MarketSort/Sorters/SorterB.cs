using System.Collections.Generic;
using System.Linq;

namespace MarketSort.Solutions
{
    class SorterB : ISorter
    {
        /// <summary>
        /// The general idea was to create a tuple to have ordering of processed values without losing the original values.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public IEnumerable<string> Sort(IEnumerable<string> values)
        {
            var ordered = new List<string>();
            var processed = values
              .Select(v => (splitted: v.Split("-"), key: v))
              .OrderBy(v => {
                  if (v.splitted.Length <= 1 || v.key.Any(char.IsLetter)) return -1;
                  int.TryParse(v.splitted[1], out var converted);
                  return converted;
              })
              .OrderBy(v => {
                  if (v.splitted.Length <= 1 || v.key.Any(char.IsLetter)) return -1;
                  int.TryParse(v.splitted[0], out var converted);
                  return converted;
              });
            foreach (var p in processed)
            {
                ordered.Add(p.key);
            }

            return ordered;
        }
    }
}
