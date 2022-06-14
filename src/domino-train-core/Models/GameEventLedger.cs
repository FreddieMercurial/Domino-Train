using DominoTrain.Core.Enumerations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace DominoTrain.Core.Models;

[Serializable]
[DataContract]
public class GameEventLedger
{
    [DataMember][JsonPropertyName("ledger")] private readonly List<HistoryRecord> Ledger;

    private Game game;

    [DataMember]
    [JsonPropertyName("historyClosed")]
    private bool HistoryClosed;

    public GameEventLedger(Game game)
    {
        this.game = game;
        this.Ledger = new List<HistoryRecord>();
        // TODO: import remainder of history?
    }

    public int Turns => this.Ledger.Count;

    public IEnumerable<HistoryRecord> History()
    {
        return this.Ledger;
    }

    public HistoryRecord GetTurn(int index)
    {
        if (index < 1 || index > this.Ledger.Count) throw new Exception(message: "Invalid index");

        return this.Ledger[index: index - 1];
    }

    private void ValidateRecord(HistoryRecord record)
    {
        // TODO: Add validation, which is time/current position in history responsive
        // or throw
        if (this.game.DateEnded is not null)
        {
            throw new Exception(message: "Game is already ended");
        }
        switch (record.EventType)
        {
            /// The first event in history should be the creation of the game, GameCreated in any other circumstance is an error
            case GameEventType.GameCreated when this.game.TurnIndex == 0:
                break;
            /// pause when game hasn't been ended and we're not already paused
            case GameEventType.Pause when this.game.DateEnded is null && this.game.DateStarted is null:
                break;
            /// unpause when game hasn't been ended and the last event is a pause event/pause start time isnt null
            case GameEventType.Unpause when this.game.DateEnded is null && this.Ledger.Last()!.EventType.Equals(GameEventType.Pause) && this.game.DatePaused is not null:
                break;
            /// We can only start a game from GameCreated and we haven't already started before
            /// since a GameCreated event must precede a GameStarted event, any valid game will have a populated lastEvent when encountering a GameStarted
            case GameEventType.GameStarted when this.game.DateStarted is null:
                break;
            /// Start a new turn but only when a game has been started, hasn't ended, isn't paused and another turn isn't in progress
            case GameEventType.TurnStarted when this.game.DateStarted is not null && this.game.DateEnded is null && this.game.DatePaused is null && this.game.DateTurnStarted is null:
                break;
            case GameEventType.TurnEnded when this.game.DateStarted is not null && this.game.DateEnded is null && this.game.DatePaused is null && this.game.DateTurnStarted is not null:
                break;
            case GameEventType.Deal when this.game.DateStarted is not null && this.game.DateEnded is null && this.game.DatePaused is null:
                /* TODO */
                break;
            case GameEventType.DrawTile when this.game.DateStarted is not null && this.game.DateEnded is null && this.game.DatePaused is null && this.game.DateTurnStarted is not null:
                /* TODO */
                break;
            case GameEventType.PlayTile when this.game.DateStarted is not null && this.game.DateEnded is null && this.game.DatePaused is null && this.game.DateTurnStarted is not null:
                /* TODO */
                break;
            case GameEventType.Knock when this.game.DateStarted is not null && this.game.DateEnded is null && this.game.DatePaused is null && this.game.DateTurnStarted is not null:
                /* TODO */
                break;
            /// When session ends before a game has actually begun/before a single move is played
            case GameEventType.GameEnded when this.game.DateStarted is null && this.game.DateEnded is null:
                break;
            /// When game ends after a game has actually begun
            case GameEventType.GameEnded when this.game.DateStarted is not null && this.game.DateEnded is null:
                break;
            default:
                break;
        }
    }

    public IEnumerable<HistoryRecordWithSpans> AuditHistoryTimeline()
    {
        List<HistoryRecordWithSpans> timeSpans = new List<HistoryRecordWithSpans>();
        /// total time paused since game created
        TimeSpan totalPausedTime = new TimeSpan();
        /// total time paused since game started
        TimeSpan totalPausedTimeInGame = new TimeSpan();
        DateTime? creationTime = null;
        DateTime? startTime = null;
        DateTime? endTime = null;
        DateTime? pauseStartTime = null;
        DateTime? turnStartTime = null;
        HistoryRecord? lastEvent = null;
        var historyIndex = 0;
        foreach (var history in this.Ledger)
        {
            switch (history.EventType)
            {
                /// The first event in history should be the creation of the game, GameCreated in any other circumstance is an error
                case GameEventType.GameCreated when historyIndex == 0:
                    creationTime = history.EventTime;
                    break;
                /// pause when game hasn't been ended and we're not already paused
                case GameEventType.Pause when endTime is null && pauseStartTime is null:
                    pauseStartTime = history.EventTime;
                    var timeCreationToThisPause = history.EventTime.Subtract(creationTime!.Value);
                    TimeSpan? timeStartToThisPause = startTime is null ? null : history.EventTime.Subtract(startTime.Value);
                    timeSpans.Add(new HistoryRecordWithSpans(
                        GameSpans: new GameSpans(
                            TimeSinceCreation: timeCreationToThisPause,
                            TimeSinceCreationPaused: totalPausedTime,
                            TimeInGame: timeStartToThisPause,
                            TimeInGamePaused: startTime is null ? null : totalPausedTimeInGame,
                            TimeSinceLastEvent: history.EventTime.Subtract(lastEvent!.EventTime)
                        ),
                        historyRecord: history));
                    break;
                /// unpause when game hasn't been ended and the last event is a pause event/pause start time isnt null
                case GameEventType.Unpause when endTime is null && lastEvent!.EventType.Equals(GameEventType.Pause) && pauseStartTime is not null:
                    var pauseDuration = history.EventTime.Subtract(pauseStartTime.Value);

                    totalPausedTime += pauseDuration;
                    if (startTime is not null)
                    {
                        totalPausedTimeInGame += pauseDuration;
                    }

                    var timeCreationToThisUnpause = history.EventTime.Subtract(creationTime!.Value);
                    TimeSpan? timeStartToThisUnpause = startTime is null ? null : history.EventTime.Subtract(startTime.Value);
                    timeSpans.Add(new HistoryRecordWithSpans(
                        GameSpans: new GameSpans(
                            TimeSinceCreation: timeCreationToThisUnpause,
                            TimeSinceCreationPaused: totalPausedTime,
                            TimeInGame: timeStartToThisUnpause,
                            TimeInGamePaused: startTime is null ? null : totalPausedTime.Subtract(totalPausedTimeInGame),
                            TimeSinceLastEvent: history.EventTime.Subtract(lastEvent!.EventTime)
                        ),
                        historyRecord: history));
                    pauseStartTime = null;
                    break;
                /// We can only start a game from GameCreated and we haven't already started before
                /// since a GameCreated event must precede a GameStarted event, any valid game will have a populated lastEvent when encountering a GameStarted
                case GameEventType.GameStarted when startTime is null:
                    startTime = history.EventTime;
                    var totalLobbyTimeWithPauses = history.EventTime.Subtract(creationTime!.Value);
                    var totalLobbyTimeWithoutPauses = totalLobbyTimeWithPauses.Subtract(totalPausedTime);
                    timeSpans.Add(new HistoryRecordWithSpans(
                        GameSpans: new GameSpans(
                            TimeSinceCreation: totalLobbyTimeWithPauses,
                            TimeSinceCreationPaused: totalLobbyTimeWithoutPauses,
                            TimeInGame: new TimeSpan() /* since now */,
                            TimeInGamePaused: new TimeSpan() /* since now */,
                            TimeSinceLastEvent: history.EventTime.Subtract(lastEvent!.EventTime) /* only possible events are created, pause/unpause */
                        ),
                        historyRecord: history));
                    break;
                /// Start a new turn but only when a game has been started, hasn't ended, isn't paused and another turn isn't in progress
                case GameEventType.TurnStarted when startTime is not null && endTime is null && pauseStartTime is null && turnStartTime is null:
                    turnStartTime = history.EventTime;
                    /*
                    var totalGameTimeWithPauses = history.EventTime.Subtract(creationTime!.Value);
                    var totalGameTimeWithoutPasuses = totalGameTimeWithPauses.Subtract(totalPausedTimeInGame);
                    timeSpans.Add(new HistoryRecordWithSpans(
                         GameSpans: new GameSpans(
                             TimeSinceCreation: totalGameTimeWithPauses,
                             TimeSinceCreationPaused: totalGameTimeWithoutPasuses,
                             TimeInGame: new TimeSpan(),
                             TimeInGamePaused: new TimeSpan(),
                             TimeSinceLastEvent: history.EventTime.Subtract(lastEvent!.EventTime)
                         ),
                         historyRecord: history));
                    */
                    break;
                case GameEventType.TurnEnded when startTime is not null && endTime is null && pauseStartTime is null && turnStartTime is not null:
                    turnStartTime = null;
                    break;
                case GameEventType.Deal when startTime is not null && endTime is null && pauseStartTime is null:
                    /* TODO */
                    break;
                case GameEventType.DrawTile when startTime is not null && endTime is null && pauseStartTime is null && turnStartTime is not null:
                    /* TODO */
                    break;
                case GameEventType.PlayTile when startTime is not null && endTime is null && pauseStartTime is null && turnStartTime is not null:
                    /* TODO */
                    break;
                case GameEventType.Knock when startTime is not null && endTime is null && pauseStartTime is null && turnStartTime is not null:
                    /* TODO */
                    break;
                /// When session ends before a game has actually begun/before a single move is played
                case GameEventType.GameEnded when startTime is null && endTime is null:
                    endTime = history.EventTime;
                    var timeCreationToGameEnd = history.EventTime.Subtract(creationTime!.Value);
                    var timeCreationToGameEndUnpaused = timeCreationToGameEnd.Subtract(totalPausedTimeInGame);
                    timeSpans.Add(new HistoryRecordWithSpans(
                         GameSpans: new GameSpans(
                             TimeSinceCreation: timeCreationToGameEnd,
                             TimeSinceCreationPaused: totalPausedTime,
                             TimeInGame: new TimeSpan(),
                             TimeInGamePaused: new TimeSpan(),
                             TimeSinceLastEvent: history.EventTime.Subtract(lastEvent!.EventTime)
                         ),
                         historyRecord: history));
                    pauseStartTime = null;
                    break;
                /// When game ends after a game has actually begun
                case GameEventType.GameEnded when startTime is not null && endTime is null:
                    endTime = history.EventTime;
                    var timeCreationToGameEndStarted = history.EventTime.Subtract(creationTime!.Value);
                    var timeGameStartToEnd = history.EventTime.Subtract(startTime.Value);
                    var totalGameTimeWithoutPasuses = timeCreationToGameEndStarted.Subtract(totalPausedTimeInGame);
                    timeSpans.Add(new HistoryRecordWithSpans(
                         GameSpans: new GameSpans(
                             TimeSinceCreation: timeCreationToGameEndStarted,
                             TimeSinceCreationPaused: totalPausedTime,
                             TimeInGame: timeGameStartToEnd,
                             TimeInGamePaused: totalPausedTimeInGame,
                             TimeSinceLastEvent: history.EventTime.Subtract(lastEvent!.EventTime)
                         ),
                         historyRecord: history));
                    pauseStartTime = null;
                    break;
                /// Unexpected condition
                default:
                    throw new Exception("Game validation error");
            } // end switch

            // before moving on
            lastEvent = history;
            historyIndex++;
        }
        return timeSpans;
    }

    public TimeSpan TimeSinceGameCreation => DateTime.UtcNow.Subtract(this.game.DateCreated);
    public TimeSpan? TimeSinceGameStarted
    {
        get
        {
            if (this.game.DateStarted is null)
                return null;
            throw new NotImplementedException();

        }
    }

    public void AddRecord(HistoryRecord record)
    {
        if (this.HistoryClosed) throw new Exception("History is closed");
        if (this.game.DateEnded is not null) throw new Exception(message: "Game is already ended");
        this.ValidateRecord(record: record);
        this.Ledger.Add(item: record);
    }

    public void EndHistory(HistoryRecord record)
    {
        if (record.EventType != GameEventType.GameEnded) throw new Exception(message: "Record is not a GameEnded event");
        this.AddRecord(record);
        this.HistoryClosed = true;
    }
}