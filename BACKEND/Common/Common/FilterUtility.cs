namespace Common.Common
{
    public static class FilterUtility
    {
        /// <summary>
        /// Enums for filter options
        /// same sequence UI is following
        /// </summary>
        public enum FilterOptions
        {
            StartsWith = 1,
            EndsWith,
            Contains,
            DoesNotContain,
            IsEmpty,
            IsNotEmpty,
            IsGreaterThan,
            IsGreaterThanOrEqualTo,
            IsLessThan,
            IsLessThanOrEqualTo,
            IsEqualTo,
            IsNotEqualTo
        }

        /// <summary>
        /// Filter parameters Model Class
        /// </summary>
        public class FilterParams
        {
            public string ColumnName { get; set; } = string.Empty;
            public string FilterValue { get; set; } = string.Empty;
            public FilterOptions FilterOption { get; set; } = FilterOptions.Contains;
        }

    }
}