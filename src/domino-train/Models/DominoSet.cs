using System.Collections.Immutable;
using System.Runtime.Serialization;
using DominoTrain.Interfaces;
using DominoTrain.Models.Players;

namespace DominoTrain.Models;

[Serializable]
[DataContract]
public class DominoSet
{
    [DataMember] public readonly ImmutableList<Domino> Dominoes;

    private readonly Game game;
    public readonly int HighestTile;
    public readonly ISetRules Rules;

    [DataMember] public readonly Guid SetId;

    private List<Guid> _dealtDominoes;

    public DominoSet(Game game, int playerCount, ISetRules rules)
    {
        this.SetId = Guid.NewGuid();
        this.game = game;
        this._dealtDominoes = new List<Guid>();
        var (dominoes, drawCount, highestTile) = this.GenerateSet();
        this.HighestTile = highestTile;
        this.Rules = rules;
        this.Dominoes = dominoes.ToImmutableList();
        if (!this.ValidateSetDominoes()) throw new ArgumentException(message: "Invalid domino set");
    }

    public IEnumerable<Domino> Boneyard => this.Dominoes.Where(predicate: domino => domino.Owner is null);

    public IEnumerable<Guid> DealtDominoIds => this._dealtDominoes;

    public IEnumerable<Domino> DealtDominoes
        => this.Dominoes.Where(predicate: domino => this._dealtDominoes.Contains(value: domino.DominoId));

    public IEnumerable<Guid> UndealtDominoIds
        => this.Dominoes.Where(predicate: domino => !this._dealtDominoes.Contains(item: domino.DominoId))
            .Select(selector: domino => domino.DominoId);

    public int UndealtDominoCount => this.Dominoes.Count - this._dealtDominoes.Count;

    public IEnumerable<Domino> UndealtDominoes
        => this.Dominoes.Where(predicate: domino => !this._dealtDominoes.Contains(item: domino.DominoId));

    public byte HighestPlayerTile => this.game.Players.Max(selector: player => player.HighestDouble);
    public byte HighestPlayerTileInHand => this.game.Players.Max(selector: player => player.HighestDoubleInHand);

    public Domino? GetDominoById(Guid dominoId)
    {
        try
        {
            return this.Dominoes.First(predicate: domino => domino.DominoId.Equals(g: dominoId));
        }
        catch (InvalidOperationException)
        {
            return null;
        }
    }

    public bool HasDomino(Guid dominoId)
    {
        return this.Dominoes.Any(predicate: domino => domino.DominoId.Equals(g: dominoId));
    }

    /// <summary>
    ///     Generates a set of dominoes for a given player count.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private (List<Domino> Dominoes, int DrawCount, int HighTile) GenerateSet()
    {
        var playerCount = this.game.PlayerCount;
        if (!this.Rules.PlayerCountToMaxDominoes.ContainsKey(key: playerCount))
            throw new ArgumentOutOfRangeException(
                paramName: nameof(this.game.PlayerCount),
                message:
                $"Player count must be one of {string.Join(separator: ", ", values: this.Rules.PlayerCountToMaxDominoes.Keys)}");
        var maxDominoes = this.Rules.PlayerCountToMaxDominoes[key: playerCount];
        var newSet = new List<Domino>();
        for (var value1 = 0; value1 <= maxDominoes.HighTile; value1++)
        for (var value2 = value1; value2 <= maxDominoes.HighTile; value2++)
            newSet.Add(item: new Domino(
                game: this.game,
                set: this,
                owner: null,
                value1: value1,
                value2: value2));
        return (
            Dominoes: newSet,
            maxDominoes.DrawCount,
            maxDominoes.HighTile);
    }

    public int SetSize(int playerCount)
    {
        return this.Rules.PlayerCountToMaxDominoes[key: playerCount].DrawCount * playerCount;
    }

    /// <summary>
    ///     Gets the dominoes that have been dealt to the given playerId
    /// </summary>
    /// <param name="playerId"></param>
    /// <returns></returns>
    public IEnumerable<Domino> PlayerDominoes(Guid playerId)
    {
        return this.Dominoes.Where(predicate: domino
            => domino.Owner is not null && domino.Owner.PlayerId.Equals(g: playerId));
    }

    private bool ValidatePlayerDominoes(Guid playerId)
    {
        var playerDominoes = this.PlayerDominoes(playerId: playerId).ToArray();
        var player = this.game.GetPlayer(id: playerId);
        // make sure player has the correct number of dominoes and that the player has all the expected domino ids
        if (playerDominoes.Length != player.DominoIds.Count() ||
            !playerDominoes.All(predicate: domino => player.DominoIds.Contains(value: domino.DominoId)))
            return false;
        // run the player dominoes through the validation logic which checks the player's train and hand against this set
        return player.ValidateDominoes();
    }

    private bool ValidateSetDominoes()
    {
        // TODO: Validate set values
        foreach (var playerGuid in this.game.AllPlayerIds)
            if (!this.ValidatePlayerDominoes(playerId: playerGuid))
                return false;
        return true;
    }

    public void Deal(Player player, int dominoes = 1)
    {
        for (var i = 0; i < dominoes; i++)
        {
            var nextDomino = this.NextDomino();
            this._dealtDominoes.Add(item: nextDomino.DominoId);
            nextDomino.DealTo(player: player);
        }
    }

    public Guid NextDominoId()
    {
        if (this.UndealtDominoCount == 0) throw new InvalidOperationException(message: "No more dominoes to deal");
        var undealtIds = this.UndealtDominoIds.ToArray();
        var random = new Random();
        var index = random.Next(maxValue: undealtIds.Length);
        return undealtIds[index];
    }

    public Domino NextDomino()
    {
        var dominoId = this.NextDominoId();
        return this.Dominoes.First(predicate: domino => domino.DominoId.Equals(g: dominoId));
    }
}