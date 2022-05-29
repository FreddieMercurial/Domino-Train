using System.Runtime.Serialization;

namespace DominoTrain.Core.Models;

[Serializable]
[DataContract]
public record HistoryRecord(int Turn, DateTime Time, Guid PlayerId, Guid DominoId, Guid NextDominoId);