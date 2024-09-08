using System;

namespace Conditions
{
    public class ChannelCondition
    {
        public int Channel { get; set; }
        public double ConditionValue { get; set; }

        public ComparisonOperator Operator { get; set; }

        public ChannelCondition(int channel, double value, ComparisonOperator op)
        {
            Channel = channel;
            ConditionValue = value;
            Operator = op;
        }

        public bool IsConditionMet(double value)
        {
            switch (Operator)
            {
                case ComparisonOperator.LessThan:
                    return value < ConditionValue;
                case ComparisonOperator.LessThanOrEqualTo:
                    return value <= ConditionValue;
                case ComparisonOperator.GreaterThan:
                    return value > ConditionValue;
                case ComparisonOperator.GreaterThanOrEqualTo:
                    return value >= ConditionValue;
                case ComparisonOperator.EqualTo:
                    return value == ConditionValue;
                case ComparisonOperator.NotEqualTo:
                    return value != ConditionValue;
                default:
                    throw new Exception("Invalid operator");
            }
        }
    }
}