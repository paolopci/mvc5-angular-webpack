using System.Diagnostics.Metrics;

namespace WebCore9.Api.Telemetry;

internal static class HeroMetrics
{
    private static readonly Meter Meter = new("WebCore9.Api.Heroes", "1.0.0");

    private static readonly Counter<long> RequestsCounter = Meter.CreateCounter<long>("heroes.requests");
    private static readonly Counter<long> MutationsCounter = Meter.CreateCounter<long>("heroes.mutations");
    private static readonly Counter<long> ErrorsCounter = Meter.CreateCounter<long>("heroes.errors");

    public static void TrackRequest(string operation, string outcome)
    {
        RequestsCounter.Add(1, new("operation", operation), new("outcome", outcome));
    }

    public static void TrackMutation(string operation, string outcome)
    {
        MutationsCounter.Add(1, new("operation", operation), new("outcome", outcome));
    }

    public static void TrackError(string operation, string errorType)
    {
        ErrorsCounter.Add(1, new("operation", operation), new("error_type", errorType));
    }
}
