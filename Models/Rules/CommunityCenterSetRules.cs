using System.Collections.Immutable;
using DominoTrain.Enumerations;
using DominoTrain.Interfaces;

// ReSharper disable MemberCanBePrivate.Global

namespace DominoTrain.Models.Rules;

/// <summary>
///     Deborah's community center rules
/// </summary>
public class CommunityCenterSetRules : SetRules, ISetRules
{
    public CommunityCenterSetRules()
    {
        this.Name = "Community Center Rules";
        this.ChickenFeet = false;
        this.CallItMayhem = true;
        this.DrawsToSatisfyOwnMayhem = 0;
        this.RuleSetType = RuleSetType.CommunityCenter;
        this._playerCountToMaxDominoes =
            new Dictionary<int, SetInfo>
            {
                // For 2 to 3 players, use a double-9 (9-9) set; each player takes eight dominoes.
                {
                    2, new SetInfo(HighTile: 9,
                        DrawCount: 8,
                        SetSize: this.SetSizesByHighTile[key: 9])
                },
                {
                    3, new SetInfo(HighTile: 9,
                        DrawCount: 8,
                        SetSize: this.SetSizesByHighTile[key: 9])
                },
                // For 4 to 6 players, use a double-12( 12-12) set; each player takes 12 dominoes.
                {
                    4, new SetInfo(HighTile: 12,
                        DrawCount: 12,
                        SetSize: this.SetSizesByHighTile[key: 12])
                },
                {
                    5, new SetInfo(HighTile: 12,
                        DrawCount: 12,
                        SetSize: this.SetSizesByHighTile[key: 12])
                },
                {
                    6, new SetInfo(HighTile: 12,
                        DrawCount: 12,
                        SetSize: this.SetSizesByHighTile[key: 12])
                },
                // For 7 to 8 players, use a double-12 (12-12) set; each player takes 10 dominoes.
                {
                    7, new SetInfo(HighTile: 12,
                        DrawCount: 10,
                        SetSize: this.SetSizesByHighTile[key: 12])
                },
                {
                    8, new SetInfo(HighTile: 12,
                        DrawCount: 10,
                        SetSize: this.SetSizesByHighTile[key: 12])
                },
                // For 9 to12 players, use a double-15 (15-15) set; each player takes 11 dominoes.
                {
                    9, new SetInfo(HighTile: 15,
                        DrawCount: 11,
                        SetSize: this.SetSizesByHighTile[key: 15])
                },
                {
                    10, new SetInfo(HighTile: 15,
                        DrawCount: 11,
                        SetSize: this.SetSizesByHighTile[key: 15])
                },
                {
                    11, new SetInfo(HighTile: 15,
                        DrawCount: 11,
                        SetSize: this.SetSizesByHighTile[key: 15])
                },
                {
                    12, new SetInfo(HighTile: 15,
                        DrawCount: 11,
                        SetSize: this.SetSizesByHighTile[key: 15])
                },
                // For 13 to 14 players, use a double-18 (18-18) set; each player takes 11 dominoes.
                {
                    13, new SetInfo(HighTile: 18,
                        DrawCount: 11,
                        SetSize: this.SetSizesByHighTile[key: 18])
                },
                {
                    14, new SetInfo(HighTile: 18,
                        DrawCount: 11,
                        SetSize: this.SetSizesByHighTile[key: 18])
                },
            }.ToImmutableDictionary();
    }
}