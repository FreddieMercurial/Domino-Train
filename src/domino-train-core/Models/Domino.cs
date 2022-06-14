using DominoTrain.Core.Models.Players;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace DominoTrain.Core.Models;

[Serializable]
[DataContract]
public class Domino
{
    [DataMember][JsonPropertyName("dominoId")] public readonly Guid DominoId;

    public readonly DominoSet DominoSet;

    [DataMember][JsonPropertyName("dominoSetId")] public readonly Guid DominoSetId;

    public readonly Game Game;

    [DataMember][JsonPropertyName("gameId")] public readonly Guid GameId;

    /// <summary>
    ///     Value for the first side of the domino.
    ///     Value1 is always less than or equal to Value2
    /// </summary>
    public readonly byte Value1;

    /// <summary>
    ///     Value for the second side of the domino
    ///     Value2 is always greater than or equal to Value1
    /// </summary>
    public readonly byte Value2;

    public Domino(Game game, DominoSet set, int value1, int value2, Player? owner = null)
    {
        this.DominoId = Guid.NewGuid();
        this.Game = game;
        this.GameId = game.GameId;
        this.DominoSetId = set.SetId;
        this.DominoSet = set;
        var swapValues = value1 <= value2;
        // Value1 is always less than or equal to Value2
        this.Value1 = (byte)(swapValues ? value1 : value2);
        this.Value2 = (byte)(swapValues ? value2 : value1);
        this.Owner = owner;
    }

    public bool IsDouble => this.Value1 == this.Value2;

    [DataMember][JsonPropertyName("orientationFlipped")] public bool? OrientationFlipped { get; private set; }

    [DataMember][JsonPropertyName("hubId")] public Guid HubId { get; private set; }

    [DataMember][JsonPropertyName("hubAttachmentIndex")] public int HubAttachmentIndex { get; private set; }

    [DataMember][JsonPropertyName("ownerId")] public Guid? OwnerId { get; private set; }

    public Player? Owner { get; private set; }

    public bool Owned => this.Owner?.HasDomino(dominoId: this.DominoId) ?? false;

    public bool InHand => this.Owner?.DominoInHand(dominoId: this.DominoId) ?? false;

    public bool InTrain => this.Owner?.DominoInTrain(dominoId: this.DominoId) ?? false;

    /// <summary>
    ///     Returns whether the current domino matches the proposed next domino.
    ///     Normally Value1 is on the left and Value2 is on the right
    ///     New dominoes Value1 must match the current domino Value2.
    /// </summary>
    /// <param name="nextDomino"></param>
    /// <returns></returns>
    public bool? NextDominoOrientation(Domino nextDomino)
    {
        //                                   This Domino         Next Domino     Next Domino Flipped
        // if orientation is not flipped [ Value1 | Value2 ] [ Value1 | Value2 ] [ Value2 | Value1 ]
        // if orientation is flipped     [ Value2 | Value1 ] [ Value1 | Value2 ] [ Value2 | Value1 ]
        // current domino orientation should be specified, but if it is not, it is assumed to be not flipped
        // if nextDomino orientation is not specified, it is assumed to be not flipped

        var thisRightSide = this.OrientationFlipped is not null && this.OrientationFlipped.Value
            ? this.Value1
            : this.Value2;
        var nextDominoFlipped = nextDomino.OrientationFlipped is not null && nextDomino.OrientationFlipped.Value;
        var nextLeftSideValue = nextDominoFlipped
            ? nextDomino.Value2
            : nextDomino.Value1;
        if (thisRightSide == nextLeftSideValue)
            return false; // not flipped

        // we failed to match with the next domino in the default orientation, try flipping it (without changing OrientationFlipped)
        // if next domino orientation is not specified, it is assumed to be not flipped, so [ Value1 | Value2 ]
        // if next domino orientation is marked flipped, so [ Value2 | Value1 ]
        nextDominoFlipped = !nextDominoFlipped;
        nextLeftSideValue = nextDominoFlipped
            ? nextDomino.Value2
            : nextDomino.Value1;

        if (thisRightSide == nextLeftSideValue)
            return true; // we matched with the next domino in the flipped orientation

        // we failed to match with the next domino in the flipped orientation, so we can't match
        return null;
    }

    internal void SetOrientationFlipped(bool? newValue)
    {
        this.OrientationFlipped = newValue;
    }

    public void DealTo(Player player)
    {
        this.Owner = player;
        this.OwnerId = player.PlayerId;
        player.DealDomino(domino: this);
    }

    public bool MoveToHand()
    {
        if (this.Owner is null)
            throw new Exception(message: "Domino is not dealt");

        return this.Owner.MoveDominoToHand(dominoId: this.DominoId);
    }

    public bool MoveToTrain()
    {
        if (this.Owner is null)
            throw new Exception(message: "Domino is not dealt");

        return this.Owner.MoveDominoToTrain(dominoId: this.DominoId);
    }

    public bool BelongsTo(Player player)
    {
        return this.Owner?.PlayerId.Equals(g: player.PlayerId) ?? false;
    }

    /// <summary>
    ///     Checks if dominoes are in the same set, equal in terms of having the same left/right values
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj)
    {
        if (obj is not Domino domino)
            return false;

        if (this.DominoSetId != domino.DominoSetId)
            return false;

        return this.Value1 == domino.Value1 && this.Value2 == domino.Value2 ||
               this.Value2 == domino.Value1 && this.Value1 == domino.Value2;
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(value: this.DominoSetId);
        hash.Add(value: this.DominoSet.HighestTile);
        hash.Add(value: this.Value1);
        hash.Add(value: this.Value2);
        return hash.ToHashCode();
    }
}