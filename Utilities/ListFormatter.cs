using System.Collections.Generic;
using System.Linq;

namespace RecruitmentApp.Utilities
{
    public class ListFormatter
    {
        public static List<List<T>> SplitList<T>(List<T> source, int chunkSize)
        {
            return source
                .Select((item, index) => new { item, index })
                .GroupBy(x => x.index / chunkSize)
                .Select(group => group.Select(x => x.item).ToList())
                .ToList();
        }
    }
}
