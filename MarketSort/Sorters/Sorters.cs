using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;

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
        /// <summary>
        /// I implemented my solution this way to play around with the IComparer interface, which was unfamiliar to me but which made the call signature quite clean:
        /// It was written in an overly verbose style with heavy usage of variables in an attempt to increase readability, and makes use of a switch expression with a tuple to determine the result of comparing the two scores.This tuple switch expression is perhaps not the most obvious way of comparing the results, but overall I believe the code to be quite clean and extensible(if required).
        /// (Since originally sharing with the group I've added extra validation to the IsScore function and increased the re-usability of the function by allowing a string to split on to be specified in the constructor).
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public IEnumerable<string> Sort(IEnumerable<string> values)
        {
            return values.OrderBy(x => x, new ScoreComparer("-")).ToList();
        }

        class ScoreComparer : IComparer<string>
        {
            private readonly string scoreSeparator;

            private const int LessThan = -1;
            private const int EqualTo = 0;
            private const int GreaterThan = 1;

            public ScoreComparer(string scoreSeparator)
            {
                this.scoreSeparator = scoreSeparator;
            }

            public int Compare([AllowNull] string firstScore, [AllowNull] string secondScore)
            {
                if (!IsScore(firstScore, scoreSeparator))
                    return LessThan;

                if (!IsScore(secondScore, scoreSeparator))
                    return GreaterThan;

                var firstScoreSplit = SplitStringToIntArray(firstScore, scoreSeparator);
                var secondScoreSplit = SplitStringToIntArray(secondScore, scoreSeparator);

                var firstHomeScore = GetHomeScore(firstScoreSplit);
                var secondHomeScore = GetHomeScore(secondScoreSplit);

                var firstAwayScore = GetAwayScore(firstScoreSplit);
                var secondAwayScore = GetAwayScore(secondScoreSplit);

                var homeScoreComparison = firstHomeScore.CompareTo(secondHomeScore);
                var awayScoreComparison = firstAwayScore.CompareTo(secondAwayScore);

                return (homeScoreComparison, awayScoreComparison) switch
                {
                    (EqualTo, EqualTo) => EqualTo,
                    (EqualTo, LessThan) => LessThan,
                    (EqualTo, GreaterThan) => GreaterThan,
                    (LessThan, _) => LessThan,
                    (GreaterThan, _) => GreaterThan,
                    (_, _) => GreaterThan
                };
            }

            private static bool IsScore(string input, string scoreSeparator)
            {
                var splitInput = input.Split(scoreSeparator);

                return splitInput.Length == 2
                    && int.TryParse(splitInput.First(), out _)
                    && int.TryParse(splitInput.Last(), out _);
            }

            private static int[] SplitStringToIntArray(string input, string splitPattern) => input.Split(splitPattern).Select(x => int.Parse(x)).ToArray();

            private static int GetHomeScore(int[] splitScore) => splitScore.First();

            private static int GetAwayScore(int[] splitScore) => splitScore.Last();
        }
    }

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
