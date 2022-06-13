using DominoTrain.Core.Enumerations;
using System.Runtime.Serialization;

namespace DominoTrain.Core.Models;

[Serializable]
[DataContract]
public record HistoryRecordWithSpans : HistoryRecord
{

    public HistoryRecordWithSpans() : this(GameSpans: null, Turn: default, EventTime: default, EventType: default, PlayerId: Guid.Empty, ExistingDominoId: Guid.Empty, AddedDominoId: Guid.Empty)
    {
    }

    public HistoryRecordWithSpans(GameSpans? GameSpans, HistoryRecord historyRecord) : base(Turn: historyRecord.Turn, EventTime: historyRecord.EventTime, EventType: historyRecord.EventType, PlayerId: historyRecord.PlayerId, ExistingDominoId: historyRecord.ExistingDominoId, AddedDominoId: historyRecord.AddedDominoId)
    {

    }

    public HistoryRecordWithSpans(GameSpans? GameSpans, int Turn, DateTime EventTime, GameEventType EventType, Guid PlayerId, Guid ExistingDominoId, Guid AddedDominoId) : base(Turn: Turn, EventTime: EventTime, EventType: EventType, PlayerId: PlayerId, ExistingDominoId: ExistingDominoId, AddedDominoId: AddedDominoId)
    {

    }
}