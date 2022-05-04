using System.Runtime.Serialization;

namespace DominoTrain.Models;

[Serializable]
[DataContract]
public class GameHistory
{
    [DataMember] private readonly DateTime GameCreatedTime;

    [DataMember] private readonly List<HistoryRecord> RecordedHistory;

    [DataMember] private DateTime? GameEndedTime;

    [DataMember] private DateTime? GameStartedTime;


    public GameHistory()
    {
        this.RecordedHistory = new List<HistoryRecord>();
        this.GameCreatedTime = DateTime.Now;
        this.GameStartedTime = null;
        this.GameEndedTime = null;
    }

    public int Turns => this.RecordedHistory.Count;

    public IEnumerable<HistoryRecord> History()
    {
        return this.RecordedHistory;
    }

    public HistoryRecord GetTurn(int index)
    {
        if (index < 1 || index > this.RecordedHistory.Count) throw new Exception(message: "Invalid index");

        return this.RecordedHistory[index: index - 1];
    }

    private void ValidateRecord(HistoryRecord record)
    {
        // TODO: Add validation
        // or throw
    }

    public void AddRecord(HistoryRecord record)
    {
        if (this.GameEndedTime is not null) throw new Exception(message: "Game is already ended");
        if (this.GameStartedTime == null) this.GameStartedTime = DateTime.Now;
        this.ValidateRecord(record: record);
        this.RecordedHistory.Add(item: record);
    }

    public void EndHistory()
    {
        if (this.GameEndedTime is not null) throw new Exception(message: "Game is already ended");
        var gameEndedTime = DateTime.Now;
        this.GameEndedTime = gameEndedTime;
    }
}