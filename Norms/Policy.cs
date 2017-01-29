using System;

namespace Assets.Scripts.Norms
{
    [Serializable]
    public class Policy
    {
        public Option Option;
        public Operator ComparisonOperator;
        public float Value;
        public float Reward;
        public RewardType RewardType;

        public Policy()
        {

        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", Option, ComparisonOperator, Value);
        }
    }

    public enum Operator
    {
        Smaller, SmallerOrEqual, Equals, BiggerOrEqual, Bigger
    }

    public enum Option
    {
        TrustRating, Consistency, Speedup, Workload
    }

    public enum RewardType
    {
        Sanction, Incentive
    }
}
