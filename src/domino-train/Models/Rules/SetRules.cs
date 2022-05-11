using DominoTrain.Enumerations;
using DominoTrain.Interfaces;
using System.Collections.Immutable;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable InconsistentNaming

namespace DominoTrain.Models.Rules;

public abstract class SetRules : ISetRules
{
    protected ImmutableDictionary<int, int> _setSizesByHighTile { get; init; } = new Dictionary<int, int>
    {
        // Double 6 Domino Sets   Have: 28 tiles and 168 pips
        {6, 28},
        // Double 9 Domino Sets   Have: 55 tiles and 495 pips
        {9, 55},
        // Double 12 Domino Sets Have:: 91 tiles and 1092 pips
        {12, 91},
        // Double 15 Domino Sets Have: 136 tiles and 2040 pips
        {15, 136},
        // Double 18 Domino Sets Have: 190 tiles and 3420 pips
        {18, 190},
    }.ToImmutableDictionary();

    protected ImmutableDictionary<int, SetInfo>
        _playerCountToMaxDominoes
    { get; init; } = null!;

    public RuleSetType RuleSetType { get; protected init; }

    public string Name { get; protected init; }
    public bool ChickenFeet { get; protected init; }
    public bool BranchingDoubles { get; protected init; }
    public bool CallItMayhem { get; protected init; }
    public int DrawsToSatisfyOwnMayhem { get; protected init; }
    public IEnumerable<int> PlayerCounts => this.PlayerCountToMaxDominoes.Keys;
    public int MaximumPlayers => this.PlayerCountToMaxDominoes.Keys.Max();
    public int MinimumPlayers => this.PlayerCountToMaxDominoes.Keys.Min();

    public int GetSetSize(int highTile)
    {
        return this.SetSizesByHighTile.ContainsKey(key: highTile) ? this.SetSizesByHighTile[key: highTile] : -1;
    }

    public ImmutableDictionary<int, int> SetSizesByHighTile => this._setSizesByHighTile;

    public ImmutableDictionary<int, SetInfo> PlayerCountToMaxDominoes
        => this._playerCountToMaxDominoes;

    public SetInfo GetMaxDominoes(int playerCount)
    {
        return this.PlayerCountToMaxDominoes.ContainsKey(key: playerCount)
            ? this.PlayerCountToMaxDominoes[key: playerCount]
            : new SetInfo(HighTile: -1,
                DrawCount: -1,
                SetSize: -1);
    }

    public bool PlayOrderClockwise { get; private set; }
}