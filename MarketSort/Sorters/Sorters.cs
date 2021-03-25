using System;
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

    class SorterD : ISorter
    {
        /// <summary>
        /// The issue with ordering strings containing numbers is that 10 comes before 2
        /// However in hexadecimal 2 comes before A, so I converted the numbers to hex, did a straight sort, then converted back to integers.
        /// Note!  This only works up to scores 15-15
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public IEnumerable<string> Sort(IEnumerable<string> values)
        {
            var converted = values.Select(x => ConvertToHex(x)).OrderBy(x => x).OrderByDescending(x => x.Length);

            return converted.Select(x => ConvertBack(x));
        }

        private static string ConvertToHex(string score)
        {
            if (score.Length > 7)
            {
                return score;
            }

            var scores = score.Split("-");

            var home = Int32.Parse(scores[0]);
            var away = Int32.Parse(scores[1]);

            var result = $"{home:X}-{away:X}";
            return result;
        }

        private static string ConvertBack(string score)
        {
            if (score.Length > 7)
            {
                return score;
            }

            var scores = score.Split("-");
            var home = Convert.ToInt32(scores[0], 16);
            var away = Convert.ToInt32(scores[1], 16);


            return $"{home}-{away}";
        }
    }

    class SorterE : ISorter
    {
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
                        OrderBy(y => y))
                .ToArray();
        }
    }

    class SorterF : ISorter
    {
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
                        OrderBy(y => y))
                .ToArray();
        }
    }
}
