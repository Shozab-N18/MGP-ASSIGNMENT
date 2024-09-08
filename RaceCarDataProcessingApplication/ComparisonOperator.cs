public enum ComparisonOperator
{
    LessThan,
    LessThanOrEqualTo,
    GreaterThan,
    GreaterThanOrEqualTo,
    EqualTo,
    NotEqualTo
}

public static class ComparisonOperatorHelper
{
    public static string GetSymbol(ComparisonOperator comparisonOperator)
    {
        switch (comparisonOperator)
        {
            case ComparisonOperator.LessThan:
                return "<";
            case ComparisonOperator.LessThanOrEqualTo:
                return "<=";
            case ComparisonOperator.GreaterThan:
                return ">";
            case ComparisonOperator.GreaterThanOrEqualTo:
                return ">=";
            case ComparisonOperator.EqualTo:
                return "==";
            case ComparisonOperator.NotEqualTo:
                return "!=";
            default:
                throw new ArgumentOutOfRangeException(nameof(comparisonOperator), comparisonOperator, null);
        }
    }
}