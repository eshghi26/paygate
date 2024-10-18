namespace PaymentGateway.Domain.Base
{
    public class FetchOption
    {
        public FilterItem[]? Filters { get; set; }
        public SortItem[]? SortOptions { get; set; }
        public int? PageSize { get; set; }
        public int? Page { get; set; }
    }

    public class FilterItem
    {
        public string? FieldName { get; set; }

        public string? Operator { get; set; }
        public string[]? Values { get; set; }
    }

    public class SortItem
    {
        public string? FieldName { get; set; }
        public string? Direction { get; set; }
    }
}
