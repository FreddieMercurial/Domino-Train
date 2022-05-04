using System.Runtime.Serialization;

namespace DominoTrain.Models;

[Serializable]
[DataContract]
public record SetInfo(int HighTile, int DrawCount, int SetSize);