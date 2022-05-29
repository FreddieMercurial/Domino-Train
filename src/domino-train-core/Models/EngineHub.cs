using DominoTrain.Core.Models.Players;
using System.Runtime.Serialization;

namespace DominoTrain.Core.Models;

[Serializable]
[DataContract]
public class EngineHub
{
    private readonly Game game;

    [DataMember] public readonly Guid GameId;

    [DataMember] public readonly Guid HubId;

    [DataMember] private readonly Guid?[] hubPlayerAttachments;

    [DataMember] private int communityIndex = -1;

    [DataMember] private Guid enginePlayerId;

    public EngineHub(Game game, int hubSize = 8)
    {
        this.HubId = Guid.NewGuid();
        this.game = game;
        this.GameId = this.game.GameId;
        this.enginePlayerId = Guid.Empty;
        this.CommunityTrainPlayer = new CommunityTrainPlayer(game: this.game);
        this.CommunityTrainPlacedBy = null;
        this.CommunityTrainPlacedById = Guid.Empty;
        this.hubPlayerAttachments = new Guid?[hubSize];
        for (var i = 0; i < hubSize; i++) this.hubPlayerAttachments[i] = null;
    }

    public CommunityTrainPlayer CommunityTrainPlayer { get; private init; }

    public Player? EnginePlayer => this.game.HasPlayer(id: this.enginePlayerId)
        ? this.game.GetPlayer(id: this.enginePlayerId)
        : null;

    [DataMember] public Guid CommunityTrainPlacedById { get; set; }

    public Player? CommunityTrainPlacedBy { get; private set; }

    public int HubSize => this.hubPlayerAttachments.Length;

    private bool CommunityTrainAttached => this.communityIndex >= 0;

    public int PlayerAttachmentIndex(Player player)
    {
        return Array.IndexOf(array: this.hubPlayerAttachments,
            value: player.PlayerId);
    }

    public Player PlayerToLeft(Player player)
    {
        var playerArrayIndex = this.PlayerAttachmentIndex(player: player);
        var looped = false;
        var index = playerArrayIndex;
        do
        {
            index = index == 0 ? this.HubSize - 1 : index - 1;
            if (this.hubPlayerAttachments[index] is null) continue;
            if (index == playerArrayIndex)
            {
                if (looped) throw new Exception(message: "Could not find player to left of player " + player.PlayerId);
                looped = true;
                continue;
            }

            var playerId = this.hubPlayerAttachments[index]!.Value;
            var nextPlayer = this.game.GetPlayer(id: playerId);
            // skip the community train
            if (nextPlayer.CommunityTrainVirtualPlayer) continue;
            return nextPlayer;
        } while (true);
    }

    public Player PlayerToRight(Player player)
    {
        var playerArrayIndex = this.PlayerAttachmentIndex(player: player);
        var looped = false;
        var index = playerArrayIndex;
        do
        {
            index = (index + 1) % this.HubSize;
            if (this.hubPlayerAttachments[index] is null) continue;
            if (index == playerArrayIndex)
            {
                if (looped) throw new Exception(message: "Could not find player to right of player " + player.PlayerId);
                looped = true;
                continue;
            }

            var playerId = this.hubPlayerAttachments[index]!.Value;
            var nextPlayer = this.game.GetPlayer(id: playerId);
            // skip the community train
            if (nextPlayer.CommunityTrainVirtualPlayer) continue;
            return nextPlayer;
        } while (true);
    }

    public bool HasAttachmentAt(int index)
    {
        return this.hubPlayerAttachments[index] != null;
    }

    public EngineHub Resize(int newSize)
    {
        var attachedPlayerCount = this.hubPlayerAttachments
            .Where(predicate: (playerGuid, attachmentIndex) => this.communityIndex != attachmentIndex)
            .Count();
        if (newSize < attachedPlayerCount)
            throw new ArgumentException(
                message:
                $"Cannot resize station to {newSize} players, because there are already {attachedPlayerCount} players attached.");

        var newStation = new EngineHub(game: this.game,
            hubSize: newSize);
        var usedSlots = new bool[newSize];
        for (var i = 0; i < this.hubPlayerAttachments.Length; i++)
        {
            if (this.hubPlayerAttachments[i] is null) continue;

            var playerGuid = this.hubPlayerAttachments[i]!.Value;
            var player = this.game.GetPlayer(id: playerGuid);
            var playerSlot = i;
            if (i >= newSize || newStation.HasAttachmentAt(index: i))
            {
                // preferred slot not available due to size change
                // find a free slot
                for (var j = 0; j < newSize; j++)
                {
                    if (usedSlots[j]) continue;
                    playerSlot = j;
                    break;
                }

                if (playerSlot == i)
                    throw new Exception(message: "Could not find a free slot for player " + playerGuid);
            }

            if (i == this.communityIndex || player.CommunityTrainVirtualPlayer)
            {
                if (player is CommunityTrainPlayer communityTrainPlayer)
                    newStation.AttachCommunityTrain(
                        index: playerSlot,
                        player: communityTrainPlayer,
                        placedBy: this.CommunityTrainPlacedBy!);
                else
                    throw new Exception(message: "Community train player expected, but player " + playerGuid +
                                                 " is not.");
            }

            else
            {
                newStation.AttachPlayer(
                    index: playerSlot,
                    player: player);
            }
        }

        return newStation;
    }

    public bool PlayerAttachedToHub(Player player)
    {
        // community train "owner" is only who placed it
        // the user can still start a train on the hub
        return this.hubPlayerAttachments
            .Where(predicate: (playerGuid, attachmentIndex)
                => player.PlayerId.Equals(o: playerGuid) && attachmentIndex != this.communityIndex)
            .Any();
    }

    public void AttachPlayer(int index, Player player)
    {
        if (this.game.Started) throw new InvalidOperationException(message: "Game already started");

        if (this.hubPlayerAttachments[index] is not null)
            throw new InvalidOperationException(message: "Player already attached at this location");

        if (!this.game.HasPlayer(player: player)) throw new Exception(message: "Player not in game");

        if (this.PlayerAttachedToHub(player: player))
            throw new Exception(message: "Player already attached to another location");

        this.hubPlayerAttachments[index] = player.PlayerId;
    }

    public void AttachCommunityTrain(int index, CommunityTrainPlayer player, Player? placedBy = null)
    {
        if (this.communityIndex == -1) throw new Exception(message: "Community train already added");

        if (this.hubPlayerAttachments[this.communityIndex] != null)
            throw new Exception(message: "Player train already attached at this location");

        this.communityIndex = index;
        this.CommunityTrainPlacedBy = placedBy;
        this.CommunityTrainPlacedById = placedBy is null ? Guid.Empty : placedBy.PlayerId;
        this.hubPlayerAttachments[this.communityIndex] = player.PlayerId;
    }

    public void SetEnginePlayer(Player player)
    {
        this.enginePlayerId = player.PlayerId;
    }
}