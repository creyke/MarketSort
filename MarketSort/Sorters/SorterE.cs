using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace MarketSort.Solutions
{
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
}
