namespace DominoTrain.Models.Players;

public sealed class CommunityTrainPlayer : Player
{
    public CommunityTrainPlayer(Game game) : base(game: game,
        name: "Community Train",
        ai: false,
        network: false,
        communityTrainVirtualPlayer: true)
    {
    }
}