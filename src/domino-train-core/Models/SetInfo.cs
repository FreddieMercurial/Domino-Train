using System.Runtime.Serialization;

namespace DominoTrain.Core.Models;

[Serializable]
[DataContract]
public record SetInfo(int HighTile, int DrawCount, int SetSize);