using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Shared
{
    public class PaginatedResult<T>
    {
        public PaginatedResult(int pageIndex, int pageSize, int countOfAllResult, IEnumerable<T> data)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            CountOfAllResult = countOfAllResult;
            Data = data;
        }

        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        public int CountOfAllResult { get; set; }

        public IEnumerable<T> Data { get; set; }
    }
}
