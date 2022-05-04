namespace DominoTrain.Models.Players;

public class HumanPlayer : Player
{
    public HumanPlayer(Game game, string? name = null) : base(game: game,
        name: name,
        network: false,
        ai: false,
        communityTrainVirtualPlayer: false)
    {
    }

    protected HumanPlayer(Game game, string? name = null, bool network = false) : base(game: game,
        name: name,
        ai: false,
        network: network,
        communityTrainVirtualPlayer: false)
    {
    }
}