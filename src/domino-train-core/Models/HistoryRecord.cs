using DominoTrain.Core.Enumerations;
using System.Runtime.Serialization;

namespace DominoTrain.Core.Models;

[Serializable]
[DataContract]
public record HistoryRecord(int Turn, DateTime EventTime, GameEventType EventType, Guid PlayerId, Guid ExistingDominoId, Guid AddedDominoId);