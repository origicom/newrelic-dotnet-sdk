using System;
using System.Collections.Generic;
using System.Text;

namespace NewRelic.DotNetSDK.Binding
{
    public class MetricData
    {
        //// ----------------------------------------------------------------------------------------------------------
		
        public string Name { get; set; }

        public int Count { get; set; }

        public float Value { get; set; }

        public float MinValue { get; set; }

        public float MaxValue { get; set; }

        public float SumOfSquares { get; set; }
 
        //// ----------------------------------------------------------------------------------------------------------
		 
        public MetricData(string name, float value)
            : this(name, 1, value, value, value, value)
        {
        }

        //// ----------------------------------------------------------------------------------------------------------

        public MetricData(string name, int count, float value, float minValue, float maxValue, float sumOfSquares)
        {
            Name = name;
            Count = count;
            Value = value;
            MinValue = minValue;
            MaxValue = maxValue;
            SumOfSquares = sumOfSquares;
        }

        //// ---------------------------------------------------------------------------------------------------------- 
        
        public void AggregateWith(MetricData other)
        {
            Count += other.Count;
            Value += other.Value;
            MinValue = Math.Min(MinValue, other.MinValue);
            MaxValue = Math.Max(MaxValue, other.MaxValue);
            SumOfSquares += other.SumOfSquares;
        }

        //// ---------------------------------------------------------------------------------------------------------- 
        
        public void Serialize(Dictionary<string, object> metricsOutput)
        {
            metricsOutput.Add(Name, new List<float> { Value, Count, MinValue, MaxValue, SumOfSquares });
        }

        //// ----------------------------------------------------------------------------------------------------------

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendFormat("Metric: {0}, ", Name);
            sb.AppendFormat("count: {0}, ", Count);
            sb.AppendFormat("value: {0}, ", Value);
            sb.AppendFormat("minValue: {0}, ", MinValue);
            sb.AppendFormat("maxValue: {0}, ", MaxValue);
            sb.AppendFormat("sumOfSquares: {0}", SumOfSquares);

            return sb.ToString();
        }

        //// ----------------------------------------------------------------------------------------------------------

        public override int GetHashCode()
        {
            const int Prime = 31;
            var result = 1;

            result = Prime * (result + ((Name == null) ? 0 : Name.GetHashCode()));

            return result;
        }

        //// ----------------------------------------------------------------------------------------------------------

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;

            if (obj == null)
                return false;

            if (GetType() != obj.GetType())
                return false;

            var other = (MetricData)obj;

            if (Name == null)
            {
                if (other.Name != null)
                    return false;
            }
            else if (!Name.Equals(other.Name))
                return false;

            return true;
        }

        //// ---------------------------------------------------------------------------------------------------------- 
    }
}