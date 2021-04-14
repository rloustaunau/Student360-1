using System.Collections;
using System.Collections.Generic;

namespace SMCISD.Student360.Persistence.Grid
{
    public class GridResponse : IGridResponse
    {
        public IEnumerable<object> Data { get; set; }
        public int TotalCount { get; set; }
        public int FilteredCount { get; set; }
        public long QueryExecutionMs { get; set; }
        public IEnumerable<object> Metadata { get; set; }

    }

    public interface IGridResponse
    {
        public IEnumerable<object> Data { get; set; }
        public int TotalCount { get; set; }
        public int FilteredCount { get; set; }
        public long QueryExecutionMs { get; set; }
    }
}
