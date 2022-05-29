using DominoTrain.Core.Enumerations;
using DominoTrain.Core.Models;
using System.Collections.Immutable;

namespace DominoTrain.Core.Interfaces;

public interface ISetRules
{
    public RuleSetType RuleSetType { get; }

    public string Name { get; }
    public bool ChickenFeet { get; }
    public bool BranchingDoubles { get; }
    public bool CallItMayhem { get; }
    public int DrawsToSatisfyOwnMayhem { get; }
    public IEnumerable<int> PlayerCounts => this.PlayerCountToMaxDominoes.Keys;
    public int MaximumPlayers => this.PlayerCountToMaxDominoes.Keys.Max();
    public int MinimumPlayers => this.PlayerCountToMaxDominoes.Keys.Min();

    public ImmutableDictionary<int, int> SetSizesByHighTile { get; }

    public ImmutableDictionary<int, SetInfo>
        PlayerCountToMaxDominoes
    { get; }

    public bool PlayOrderClockwise { get; }

    public int GetSetSize(int highTile);

    public SetInfo GetMaxDominoes(int playerCount);
}