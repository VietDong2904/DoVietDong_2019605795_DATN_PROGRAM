namespace Common.Common
{
    public static class SortingUtility
    {
        public enum SortOrders
        {
            Asc = 1,
            Desc = 2
        }

        public  class SortingParams
        {
            public SortOrders SortOrder { get; set; } = SortOrders.Asc;
            public string ColumnName { get; set; }
        }
    }
}