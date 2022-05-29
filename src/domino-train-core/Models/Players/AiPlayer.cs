using DominoTrain.Core.Models;

namespace DominoTrain.Core.Models.Players;

public class AiPlayer : Player
{
    public AiPlayer(Game game, string? name = null) : base(game: game,
        name: name,
        ai: true,
        network: false,
        communityTrainVirtualPlayer: false)
    {
    }

    protected AiPlayer(Game game, string? name = null, bool network = false) : base(game: game,
        name: name,
        ai: true,
        network: network,
        communityTrainVirtualPlayer: false)
    {
    }
}