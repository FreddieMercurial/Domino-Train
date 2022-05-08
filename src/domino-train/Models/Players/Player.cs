using System.Collections.Immutable;
using System.Runtime.Serialization;

namespace DominoTrain.Models.Players;

[Serializable]
[DataContract]
public abstract class Player
{
    [DataMember] private readonly LinkedList<Guid> _dominoIds;

    private readonly LinkedList<Guid> _handDominoIds;

    private readonly LinkedList<Guid> _trainDominoIds;
    [DataMember] public readonly bool AI;

    [DataMember] public readonly bool CommunityTrainVirtualPlayer;

    [DataMember] public readonly Guid GameId;
    [DataMember] public readonly bool Network;

    [DataMember] public readonly Guid PlayerId;

    public Player(Game game, string? name = null, bool ai = false, bool network = false,
        bool communityTrainVirtualPlayer = false)
    {
        this.Game = game;
        this.CommunityTrainVirtualPlayer = communityTrainVirtualPlayer;
        this.AI = ai;
        this.Network = network;
        this._dominoIds = new LinkedList<Guid>();
        this._handDominoIds = new LinkedList<Guid>();
        this._trainDominoIds = new LinkedList<Guid>();
        this.PlayerId = Guid.NewGuid();
        var defaultName = this.AI ? $"AI Player (id: {this.PlayerId})" : $"Player (id: {this.PlayerId})";
        this.Name = name ?? defaultName;
        this.Score = 0;
    }

    [DataMember] public int Wins { get; private set; }

    [DataMember] public int Losses { get; private set; }

    public Game Game { get; private init; }

    [DataMember] public string Name { get; }

    public DominoSet? DominoSet { get; private set; }

    [DataMember] public Guid DominoSetId { get; private set; }

    public IEnumerable<Guid> DominoIds => this._dominoIds.ToImmutableArray();
    public IEnumerable<Domino> Dominoes => this._dominoIds.Select(selector: id => this.Game.GetDomino(id: id)!);
    private IEnumerable<Guid> HandIds => this._handDominoIds.ToImmutableArray();
    public IEnumerable<Domino> Hand => this._handDominoIds.Select(selector: id => this.Game.GetDomino(id: id)!);
    private IEnumerable<Guid> TrainIds => this._trainDominoIds.ToImmutableArray();
    public IEnumerable<Domino> Train => this._trainDominoIds.Select(selector: id => this.Game.GetDomino(id: id)!);

    public Guid? LastTrainDominoId => this._trainDominoIds.Last?.Value;

    public Domino? LastTrainDomino => this.LastTrainDominoId is null
        ? null
        : this.DominoSet?.GetDominoById(dominoId: this.LastTrainDominoId.Value);

    public int Score { get; private set; }

    public byte HighestDouble
    {
        get
        {
            var setHighestTile = this.DominoSet?.HighestTile;
            if (setHighestTile is null)
                return 0;
            // take the highest double we have in our hand.
            // Value2 used as although Value1 == Value2, but Value2 is supposed to be always highest
            return this._dominoIds
                .Select(selector: dominoId => this.Game.GetDomino(id: dominoId)!)
                .Where(predicate: domino => domino!.IsDouble)
                .Max(selector: domino => domino!.Value2);
        }
    }

    public byte HighestDoubleInHand
    {
        get
        {
            var setHighestTile = this.DominoSet?.HighestTile;
            if (setHighestTile is null)
                return 0;
            // take the highest double we have in our hand.
            // Value2 used as although Value1 == Value2, but Value2 is supposed to be always highest
            return this._handDominoIds
                .Select(selector: dominoId => this.Game.GetDomino(id: dominoId))
                .Where(predicate: domino => domino!.IsDouble)
                .Max(selector: domino => domino!.Value2);
        }
    }

    public void ResetDominoes()
    {
        this._dominoIds.Clear();
        this._handDominoIds.Clear();
        this._trainDominoIds.Clear();
        this.DominoSetId = Guid.Empty;
        this.DominoSet = null;
    }

    public bool MoveDominoToHand(Guid dominoId)
    {
        if (!this._dominoIds.Contains(value: dominoId)) return false;
        var handContains = this._handDominoIds.Contains(value: dominoId);
        var trainContains = this._trainDominoIds.Contains(value: dominoId);
        if (!handContains)
            this._handDominoIds.AddLast(value: dominoId);

        if (trainContains)
            this._trainDominoIds.Remove(value: dominoId);

        return handContains || trainContains;
    }

    public bool MoveDominoToTrain(Guid dominoId)
    {
        if (!this._dominoIds.Contains(value: dominoId)) return false;

        var handContains = this._handDominoIds.Contains(value: dominoId);
        if (!handContains)
            return false;

        var trainContains = this._trainDominoIds.Contains(value: dominoId);
        if (trainContains)
            return false;

        // check if domino is valid for the train
        var domino = this.Game.GetDomino(id: dominoId);
        if (domino is null) return false;
        if (this._trainDominoIds.Count == 0)
        {
            if (domino.Value1 != this.Game.HighestTile || domino.Value2 != this.Game.HighestTile)
                return false;
            domino.SetOrientationFlipped(newValue: false);
        }
        else
        {
            var nextDominoOrientation = this.LastTrainDomino!.NextDominoOrientation(nextDomino: domino);

            if (nextDominoOrientation is null)
                return false;

            domino.SetOrientationFlipped(newValue: nextDominoOrientation.Value);
        }

        // move from the hand to the train
        this._handDominoIds.Remove(value: dominoId);
        this._trainDominoIds.AddLast(value: dominoId);


        return handContains || trainContains;
    }

    public bool DealDomino(Domino domino)
    {
        if (!this._dominoIds.Contains(value: domino.DominoId)) return false;

        if (this.DominoSetId.Equals(g: Guid.Empty))
            // first domino domino dealt determines the active domino set
            this.DominoSetId = domino.DominoSetId;
        else if (this.DominoSetId != domino.DominoSetId)
            // can't deal dominoes from different sets
            return false;

        this.DominoSet = domino.DominoSet;

        this._dominoIds.AddLast(value: domino.DominoId);
        return this.MoveDominoToHand(dominoId: domino.DominoId);
    }

    public bool ValidateDominoes()
    {
        foreach (var dominoId in this._dominoIds)
        {
            // ensure game/set has domino
            var domino = this.Game.GetDomino(id: dominoId);
            if (domino is null)
                return false;

            // ensure domino is in the current domino set
            if (!this.Game.DominoSetId.Equals(g: domino.DominoSetId))
                return false;

            // ensure domino is either in hand or on a train
            var handContains = this._handDominoIds.Contains(value: dominoId);
            var trainContains = this._trainDominoIds.Contains(value: dominoId);
            // not in either set
            if (!handContains && !trainContains)
                return false;
            // in both sets
            if (handContains && trainContains) return false;
        }

        return true;
    }

    public bool HasDomino(Guid dominoId)
    {
        return this._dominoIds.Contains(value: dominoId);
    }

    public bool CanMatchInHand(Domino dominoOnBoard)
    {
        return this.Hand.Any(predicate: handDomino => handDomino.Equals(obj: dominoOnBoard));
    }

    public bool DominoInHand(Guid dominoId)
    {
        return this._handDominoIds.Contains(value: dominoId);
    }

    public bool DominoInTrain(Guid dominoId)
    {
        return this._trainDominoIds.Contains(value: dominoId);
    }

    public Domino GetHighestDouble()
    {
        var highestDouble = this.HighestDouble;
        return this._dominoIds.Select(selector: dominoId => this.Game.GetDomino(id: dominoId)!)
            .First(predicate: domino => domino!.IsDouble && domino.Value2 == highestDouble);
    }
}