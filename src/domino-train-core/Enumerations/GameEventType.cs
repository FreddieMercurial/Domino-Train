namespace DominoTrain.Core.Enumerations
{
    public enum GameEventType
    {
        GameCreated,
        GameStarted,
        Deal,
        TurnStarted,
        Knock,
        DrawTile,
        PlayTile,
        TurnEnded, // aka "Pass"
        GameEnded,
        Pause,
        Unpause,
    }
}