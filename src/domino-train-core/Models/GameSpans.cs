namespace DominoTrain.Core.Models
{
    public record GameSpans(TimeSpan TimeSinceCreation, TimeSpan TimeSinceCreationPaused, TimeSpan? TimeInGame, TimeSpan? TimeInGamePaused, TimeSpan? TimeSinceLastEvent);
}
