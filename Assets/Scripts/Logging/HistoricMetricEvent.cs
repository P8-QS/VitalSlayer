using Data;

public class HistoricMetricEvent : Unity.Services.Analytics.Event
{
	public HistoricMetricEvent(UserMetricsType metricsType, object value) : base("historicMetricEvent")
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