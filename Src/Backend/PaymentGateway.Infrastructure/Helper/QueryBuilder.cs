using PaymentGateway.Domain.Base;
using System.Data;

namespace PaymentGateway.Infrastructure.Helper
{
    public static class QueryBuilder
    {
        public static (string? query, KeyValuePair<string, object>[]? values, bool setDbType)
            CreateFieldQuery(FilterItem filter, DbType fType, string prefix = "")
        {
            try
            {
                string query;
                var valueItems = new List<KeyValuePair<string, object>>();

                if (string.IsNullOrEmpty(filter.FieldName))
                    return (null, null, false);

                if (filter.Values == null || !filter.Values.Any())
                    return (null, null, false);

                if (string.IsNullOrEmpty(filter.Operator))
                {
                    var val = ParseString(filter.Values[0], fType);
                    if (val == null)
                        return (null, null, false);

                    query = @$"AND {prefix}""{filter.FieldName}"" = @{filter.FieldName} ";
                    valueItems.Add(new KeyValuePair<string, object>(filter.FieldName, val));

                    return (query, valueItems.ToArray(), true);
                }

                switch (filter.Operator)
                {
                    case "=":
                        var equalVal = ParseString(filter.Values[0], fType);
                        if (equalVal == null)
                            return (null, null, false);

                        query = @$"AND {prefix}""{filter.FieldName}"" = @{filter.FieldName} ";
                        valueItems.Add(new KeyValuePair<string, object>(filter.FieldName, equalVal));

                        return (query, valueItems.ToArray(), true);

                    case ">":
                        var bVal = ParseString(filter.Values[0], fType);
                        if (bVal == null)
                            return (null, null, false);

                        query = @$"AND {prefix}""{filter.FieldName}"" > @{filter.FieldName} ";
                        valueItems.Add(new KeyValuePair<string, object>(filter.FieldName, bVal));

                        return (query, valueItems.ToArray(), true);

                    case ">=":
                        var beVal = ParseString(filter.Values[0], fType);
                        if (beVal == null)
                            return (null, null, false);

                        query = @$"AND {prefix}""{filter.FieldName}"" >= @{filter.FieldName} ";
                        valueItems.Add(new KeyValuePair<string, object>(filter.FieldName, beVal));

                        return (query, valueItems.ToArray(), true);

                    case "<":
                        var kVal = ParseString(filter.Values[0], fType);
                        if (kVal == null)
                            return (null, null, false);

                        query = @$"AND {prefix}""{filter.FieldName}"" < @{filter.FieldName} ";
                        valueItems.Add(new KeyValuePair<string, object>(filter.FieldName, kVal));

                        return (query, valueItems.ToArray(), true);

                    case "<=":
                        var keVal = ParseString(filter.Values[0], fType);
                        if (keVal == null)
                            return (null, null, false);

                        query = @$"AND {prefix}""{filter.FieldName}"" <= @{filter.FieldName} ";
                        valueItems.Add(new KeyValuePair<string, object>(filter.FieldName, keVal));

                        return (query, valueItems.ToArray(), true);

                    case "like":
                        var likeVal = ParseString(filter.Values[0], fType);
                        if (likeVal == null)
                            return (null, null, false);

                        query = @$"AND LOWER({prefix}""{filter.FieldName}"") LIKE @{filter.FieldName} ";
                        valueItems.Add(new KeyValuePair<string, object>(filter.FieldName, $"%{likeVal}%"));

                        return (query, valueItems.ToArray(), true);

                    case "between":
                        if (filter.Values!.Length != 2)
                        {
                            return (null, null, false);
                        }

                        query = @$"AND {prefix}""{filter.FieldName}"" BETWEEN @From{filter.FieldName} AND @To{filter.FieldName} ";

                        var fromVal = ParseString(filter.Values![0], fType);
                        if (fromVal == null)
                            return (null, null, false);
                        valueItems.Add(new KeyValuePair<string, object>($"From{filter.FieldName}", fromVal));

                        var toVal = ParseString(filter.Values![1], fType);
                        if (toVal == null)
                            return (null, null, false);
                        valueItems.Add(new KeyValuePair<string, object>($"To{filter.FieldName}", toVal));

                        return (query, valueItems.ToArray(), true);

                    case "in":

                        var arrayVal = ParseStringToArrayObject(filter.Values, fType);
                        if (arrayVal == null)
                            return (null, null, false);

                        query = @$"AND {prefix}""{filter.FieldName}"" = ANY (@{filter.FieldName}) ";
                        valueItems.Add(new KeyValuePair<string, object>(filter.FieldName, arrayVal));
                        return (query, valueItems.ToArray(), false);

                    default:
                        var equalVal2 = ParseString(filter.Values[0], fType);
                        if (equalVal2 == null)
                            return (null, null, false);

                        query = @$"AND {prefix}""{filter.FieldName}"" = @{filter.FieldName} ";
                        valueItems.Add(new KeyValuePair<string, object>(filter.FieldName, equalVal2));

                        return (query, valueItems.ToArray(), true);
                }
            }
            catch (Exception)
            {
                return (null, null, false);
            }
        }

        private static (DbType type, object obj) ParseString(string str)
        {
            if (bool.TryParse(str, out var boolValue))
                return (DbType.Boolean, boolValue);

            if (short.TryParse(str, out var shortValue))
                return (DbType.Int16, shortValue);

            if (int.TryParse(str, out var intValue))
                return (DbType.Int32, intValue);

            if (long.TryParse(str, out var bigintValue))
                return (DbType.Int64, bigintValue);

            if (decimal.TryParse(str, out var decimalValue))
                return (DbType.Currency, decimalValue);

            if (DateTime.TryParse(str, out var dateValue))
                return (DbType.Decimal, dateValue);

            return (DbType.String, str);
        }

        private static object? ParseString(string str, DbType type)
        {
            str = str.Trim();

            switch (type)
            {
                case DbType.Boolean:
                    return bool.TryParse(str, out var boolVal) ? boolVal : null;

                case DbType.Currency:
                    return decimal.TryParse(str, out var currencyVal) ? currencyVal : null;

                case DbType.DateTime:
                    return DateTime.TryParse(str, out var dateVal) ? dateVal : null;

                case DbType.Decimal:
                    return decimal.TryParse(str, out var desVal) ? desVal : null;

                case DbType.Double:
                    return double.TryParse(str, out var doVal) ? doVal : null;

                case DbType.Int16:
                    return short.TryParse(str, out var shortVal) ? shortVal : null;

                case DbType.Int32:
                    return int.TryParse(str, out var intVal) ? intVal : null;

                case DbType.Int64:
                    return long.TryParse(str, out var longVal) ? longVal : null;

                case DbType.String:
                    return str.ToLower();

                default:
                    return str.ToLower();
            }
        }

        public static object? ParseStringToArrayObject(string[] str, DbType type)
        {
            if (!str.Any())
                return null;

            try
            {
                switch (type)
                {
                    case DbType.Boolean:
                        return str.Select(bool.Parse).ToArray();

                    case DbType.Currency:
                        return str.Select(decimal.Parse).ToArray();

                    case DbType.DateTime:
                        return str.Select(DateTime.Parse).ToArray();

                    case DbType.Decimal:
                        return str.Select(DateTime.Parse).ToArray();

                    case DbType.Double:
                        return str.Select(double.Parse).ToArray();

                    case DbType.Int16:
                        return str.Select(short.Parse).ToArray();

                    case DbType.Int32:
                        return str.Select(int.Parse).ToArray();

                    case DbType.Int64:
                        return str.Select(long.Parse).ToArray();

                    case DbType.String:
                        return str;

                    default:
                        return str;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
