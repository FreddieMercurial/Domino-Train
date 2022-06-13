namespace DominoTrain.Core.Models.Players;

public sealed class HumanNetworkPlayer : HumanPlayer
{
    public HumanNetworkPlayer(Game game, string? name = null) : base(game: game,
        name: name,
        network: true)
    {
    }
}