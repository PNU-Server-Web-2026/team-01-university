using Serilog.Core;
using Serilog.Events;

namespace UniversityManagement.Api.Tests.Features.Logging;

/// <summary>
/// In-memory sink, який збирає всі події Serilog для перевірки в тестах.
/// Реєструється в DI, тому підхоплюється через ReadFrom.Services у Program.cs.
/// </summary>
public sealed class CollectingSink : ILogEventSink
{
    private readonly List<LogEvent> _events = new();

    public void Emit(LogEvent logEvent)
    {
        lock (_events)
        {
            _events.Add(logEvent);
        }
    }

    public IReadOnlyList<LogEvent> Snapshot()
    {
        lock (_events)
        {
            return _events.ToArray();
        }
    }

    public void Clear()
    {
        lock (_events)
        {
            _events.Clear();
        }
    }
}
