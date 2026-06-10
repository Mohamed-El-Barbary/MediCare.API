using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Shared.ReviewSpecParams
{
    public class ReviewSpecParam
    {
        private const int _defaultPageIndex = 1;
        private const int _maxPageSize = 6;
        private const int _defaultPageSize = 6;


        private int _pageIndex = _defaultPageIndex;
        public int PageIndex
        {
            get { return _pageIndex; }
            set
            {
                _pageIndex = (value <= 0 ? 1 : value);
            }
        }

        private int _pageSize = _defaultPageSize;
        public int PageSize
        {
            get { return _pageSize; }
            set
            {
                _pageSize = (value > 6 ? _maxPageSize : value);
            }
        }
    }
}
