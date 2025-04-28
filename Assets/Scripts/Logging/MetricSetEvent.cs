using Data;
using Unity.Services.Analytics;

public class MetricSetEvent : Event
{
    public MetricSetEvent(UserMetricsType metricsType, object value) : base("metricSetEvent")
    {
        var type = metricsType.ToString();
        SetParameter("metricType", type);
        switch (value)
        {
            case long i:
                SetParameter(type, (int)i);
                break;
            case string s:
                SetParameter(type, s);
                break;
        }
    }
}