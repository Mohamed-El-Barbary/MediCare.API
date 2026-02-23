using System;
using System.Collections.Generic;
using System.Text;

namespace Health.Shared
{
    public class DoctorSpecParams
    {
        public string? Specialization { get; set; }
        public string? ClinicLocation { get; set; }
        public int? MinExperience { get; set; }
        public int? MaxExperience { get; set; }
        public double? MinRating { get; set; }
        public string? Search { get; set; }
        public DoctorSortingOptions Sort { get; set; }


        private const int _defaultPageIndex = 1;
        private const int _maxPageSize = 10;
        private const int _defaultPageSize = 5;


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
                _pageSize = (value > 10 ? _maxPageSize : value);
            }
        }
    }
}
