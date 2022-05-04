using System.Runtime.Serialization;

namespace DominoTrain.Models;

[Serializable]
[DataContract]
public record HistoryRecord(int Turn, DateTime Time, Guid PlayerId, Guid DominoId, Guid NextDominoId);