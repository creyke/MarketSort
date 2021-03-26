using System;
using System.Collections.Generic;
using System.Linq;

namespace MarketSort.Solutions
{
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
}
