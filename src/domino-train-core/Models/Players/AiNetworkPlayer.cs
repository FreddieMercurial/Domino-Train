namespace DominoTrain.Core.Models.Players;

public sealed class AiNetworkPlayer : Player
{
    public AiNetworkPlayer(Game game, string? name = null) : base(game: game,
        name: name,
        ai: true,
        network: true,
        communityTrainVirtualPlayer: false)
    {
    }
}