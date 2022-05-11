using DominoTrain.Enumerations;
using DominoTrain.Interfaces;
using System.Collections.Immutable;

// ReSharper disable MemberCanBePrivate.Global

namespace DominoTrain.Models.Rules;

/// <summary>
///     https://www.mexicantrainfun.com/questions-answers/
/// </summary>
public class AlternateSetRules : SetRules, ISetRules
{
    public AlternateSetRules()
    {
        this.Name = "Alternate Rules";
        this.ChickenFeet = false;
        this.CallItMayhem = false;
        this.DrawsToSatisfyOwnMayhem = -1;
        this.RuleSetType = RuleSetType.Alternate;
        this._playerCountToMaxDominoes =
            new Dictionary<int, SetInfo>
            {
                // Double-Nine
                // For 2 to 4 players, use a double-9 (9-9) set.
                {
                    2, new SetInfo(HighTile: 9,
                        DrawCount: 12,
                        SetSize: this.SetSizesByHighTile[key: 9])
                },
                {
                    3, new SetInfo(HighTile: 9,
                        DrawCount: 11,
                        SetSize: this.SetSizesByHighTile[key: 9])
                },
                {
                    4, new SetInfo(HighTile: 9,
                        DrawCount: 10,
                        SetSize: this.SetSizesByHighTile[key: 9])
                },

                // Double-Twelve
                // For 5 to 8 players, use a double-12( 12-12) set.
                {
                    5, new SetInfo(HighTile: 12,
                        DrawCount: 12,
                        SetSize: this.SetSizesByHighTile[key: 12])
                },
                {
                    6, new SetInfo(HighTile: 12,
                        DrawCount: 11,
                        SetSize: this.SetSizesByHighTile[key: 12])
                },
                {
                    7, new SetInfo(HighTile: 12,
                        DrawCount: 10,
                        SetSize: this.SetSizesByHighTile[key: 12])
                },
                {
                    8, new SetInfo(HighTile: 12,
                        DrawCount: 9,
                        SetSize: this.SetSizesByHighTile[key: 12])
                },

                // Double-Fifteen
                // For 9 to 12 players, use a double-15 (15-15) set.
                {
                    9, new SetInfo(HighTile: 15,
                        DrawCount: 12,
                        SetSize: this.SetSizesByHighTile[key: 15])
                },
                {
                    10, new SetInfo(HighTile: 15,
                        DrawCount: 11,
                        SetSize: this.SetSizesByHighTile[key: 15])
                },
                {
                    11, new SetInfo(HighTile: 15,
                        DrawCount: 10,
                        SetSize: this.SetSizesByHighTile[key: 15])
                },
                {
                    12, new SetInfo(HighTile: 15,
                        DrawCount: 9,
                        SetSize: this.SetSizesByHighTile[key: 15])
                },

                // Double-Eighteen
                // For 13 to 16 players, use a double-18 (18-18) set.
                {
                    13, new SetInfo(HighTile: 18,
                        DrawCount: 12,
                        SetSize: this.SetSizesByHighTile[key: 18])
                },
                {
                    14, new SetInfo(HighTile: 18,
                        DrawCount: 11,
                        SetSize: this.SetSizesByHighTile[key: 18])
                },
                {
                    15, new SetInfo(HighTile: 18,
                        DrawCount: 10,
                        SetSize: this.SetSizesByHighTile[key: 18])
                },
                {
                    16, new SetInfo(HighTile: 18,
                        DrawCount: 9,
                        SetSize: this.SetSizesByHighTile[key: 18])
                },
            }.ToImmutableDictionary();
    }
}