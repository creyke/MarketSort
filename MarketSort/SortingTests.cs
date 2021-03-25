using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace MarketSort
{
    public class SortingTests
    {
        private readonly ITestOutputHelper output;

        public SortingTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        public static IEnumerable<object[]> Data => new List<object[]>
        {
            new object []
            {
                new [] { "1-0", "0-0", "3-2", "10-1", "Any Other Score", "1-10", "5-5", "0-1", "1-1", "1-2", "1-3", "2-0", "2-1", "1-4", "4-0", "4-1", "6-1", "7-0", "7-1", "8-0", "8-1", "9-0", "0-10" },
                new [] { "Any Other Score", "0-0", "0-1", "0-10", "1-0", "1-1", "1-2", "1-3", "1-4", "1-10", "2-0", "2-1", "3-2", "4-0", "4-1", "5-5", "6-1", "7-0", "7-1", "8-0", "8-1", "9-0", "10-1" }
            }
        };

        [Theory]
        [MemberData(nameof(Data))]
        public void Sort(string[] values, string[] expected)
        {
            values = values
                .GroupBy(x => x.Last() != '-' && x.Split('-').All(y => int.TryParse(y, out _)))
                .OrderBy(x => x.Key)
                .SelectMany(x => x.Key
                    ? x
                        .Select(y => (n: y, c: y.Split('-').Select(z => int.Parse(z))))
                        .OrderBy(y => y.c.First())
                        .ThenBy(y => y.c.ElementAt(1))
                        .Select(x => x.n)
                    : x
                        .OrderBy(y => y))
                .ToArray();

            output.WriteLine(string.Join(",", expected));
            output.WriteLine(string.Join(",", values));

            Assert.Equal(expected, values);
        }
    }
}
