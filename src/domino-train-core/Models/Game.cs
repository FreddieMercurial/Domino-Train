using DominoTrain.Core.Interfaces;
using DominoTrain.Core.Models.Players;
using DominoTrain.Core.Models.Rules;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace DominoTrain.Core.Models;

[Serializable]
[DataContract]
public class Game
{
    [DataMember][JsonPropertyName("gameId")] public readonly Guid GameId;

    [DataMember]
    [JsonPropertyName("ownerId")]
    private readonly Guid OwnerId;

    private Player Owner => this._players[this.OwnerId];

    [DataMember]
    [JsonPropertyName("players")]
    private readonly Dictionary<Guid, Player> _players;

    [DataMember]
    [JsonPropertyName("rules")]
    public readonly ISetRules Rules;

    [DataMember]
    [JsonPropertyName("dominoSet")]
    public DominoSet? DominoSet { get; private set; }

    [DataMember]
    [JsonPropertyName("engineHub")]
    private EngineHub EngineHub;

    private readonly CommunityTrainPlayer communityTrainPlayer;

    public Game(Player owner, List<Player>? players = null, int aiPlayerCount = 0)
    {
        if (owner is CommunityTrainPlayer)
        {
            throw new Exception("Owner cannot be CommunityTrainPlayer");
        }
        this.DateCreated = DateTime.UtcNow;
        this.GameId = Guid.NewGuid();
        this.Rules = new DefaultSetRules();
        this._players = new Dictionary<Guid, Player>();
        this.GameHistory = new GameEventLedger(game: this);
        this.EngineHub = new EngineHub(game: this);
        this.DateStarted = null;
        this.DatePaused = null;
        this.DateEnded = null;
        this.DateTurnStarted = null;
        this.Turn = 0;
        this.TurnOrder = new List<Guid>();

        bool needCommunityPlayer = true;
        if (players is not null)
        {
            foreach (var player in players)
            {
                if (player is CommunityTrainPlayer ctp)
                {
                    if (this.communityTrainPlayer is not null)
                    {
                        throw new Exception("Only one CommunityTrainPlayer is allowed");
                    }
                    this.communityTrainPlayer = ctp;
                    needCommunityPlayer = false;
                }
                this.AddPlayer(player: player);
            }
        }
        // if passed in players did not include a community player, add one
        if (needCommunityPlayer)
        {
            this.communityTrainPlayer = new CommunityTrainPlayer(game: this);
            this.AddPlayer(player: this.communityTrainPlayer);
            needCommunityPlayer = false;
        }
        if (!this._players.ContainsKey(key: owner.PlayerId))
            this.AddPlayer(player: owner);

        for (var i = 0; i < aiPlayerCount; i++)
        {
            var newPlayer = new AiPlayer(
                game: this,
                name: null);
            this.AddPlayer(player: newPlayer);
        }

        // after players are set up
        // TODO: need ability to update player count on domino set before game starts?
        this.DominoSet = new DominoSet(
                game: this,
                playerCount: this.PlayerCount,
                rules: this.Rules);
        this.DominoSetId = this.DominoSet?.SetId ?? Guid.Empty;
    }

    [DataMember][JsonPropertyName("dominoSetId")] public Guid DominoSetId { get; private set; }

    [DataMember][JsonPropertyName("turn")] public int Turn { get; private set; }

    [DataMember][JsonPropertyName("stationHubId")] public Guid StationHubId => this.EngineHub.HubId;

    private bool CanChange => !this.Ended && !this.Started;

    [DataMember]
    [JsonPropertyName("gameHistory")]
    public readonly GameEventLedger GameHistory;

    [DataMember]
    [JsonPropertyName("turnOrder")]
    public List<Guid> TurnOrder { get; }

    public int TurnIndex => this.CanChange ? this.Turn % this.PlayerCount : -1;

    public Guid CurrentPlayerId => this.TurnIndex > 0 && this.TurnIndex < this.TurnOrder.Count
        ? this.TurnOrder[index: this.TurnIndex]
        : Guid.Empty;

    public Player? CurrentPlayer
    {
        get
        {
            var currentPlayerId = this.CurrentPlayerId;
            if (currentPlayerId.Equals(g: Guid.Empty))
                return null;
            return !this._players.ContainsKey(key: currentPlayerId) ? null : this._players[key: currentPlayerId];
        }
    }
    [DataMember][JsonPropertyName("dateCreated")] public DateTime DateCreated { get; private init; }
    [DataMember][JsonPropertyName("dateStarted")] public DateTime? DateStarted { get; private set; } = null;
    [DataMember][JsonPropertyName("dateTurnStarted")] public DateTime? DateTurnStarted { get; private set; } = null;
    [DataMember][JsonPropertyName("dateEnded")] public DateTime? DateEnded { get; private set; } = null;
    [DataMember][JsonPropertyName("datePaused")] public DateTime? DatePaused { get; private set; } = null;

    public bool Started
    {
        get => this.DateStarted is not null;
        private set
        {
            if (this.DateStarted is not null)
            {
                throw new Exception("Game already started");
            }
            this.DateStarted = DateTime.UtcNow;
        }
    }

    public bool Ended
    {
        get => this.DateEnded is not null;
        private set
        {
            if (this.DateEnded is not null)
            {
                throw new Exception("Game already ended");
            }
            this.DateEnded = DateTime.UtcNow;
        }
    }

    public bool TurnStarted
    {
        get => this.DateTurnStarted is not null;
        private set
        {
            if (this.DateTurnStarted is not null)
            {
                throw new Exception("Turn already ended");
            }
            this.DateTurnStarted = DateTime.UtcNow;
        }
    }

    public bool Paused
    {
        get => this.DatePaused is not null;
        private set
        {
            if (this.DatePaused is not null && value)
            {
                throw new Exception("Already paused");
            }
            else if (this.DatePaused is null && !value)
            {
                throw new Exception("not paused");
            }
            else if (value)
            {
                this.DatePaused = DateTime.UtcNow;
            }
            else
            {
                this.DatePaused = null;
            }
        }
    }

    public int PlayerCount => this._players.Count - (this.communityTrainPlayer is not null ? 1 : 0);

    public int HumanPlayerCount => this._players.Count(predicate: player => !player.Value.AI && player.Value is not CommunityTrainPlayer);

    public int NetworkHumanPlayerCount
        => this._players.Count(predicate: player => player.Value.GetType() == typeof(HumanNetworkPlayer));

    public int AiPlayerCount => this._players.Count(predicate: player => player.Value.AI);

    /// <summary>
    ///     TODO: quote rule
    /// </summary>
    public int TileCount => this.DominoSet is not null ? this.DominoSet.Dominoes.Count : 0;

    public IEnumerable<Guid> AllPlayerIds => this._players.Keys;

    public IEnumerable<Guid> NonCommunityPlayerIds => this._players.Values
        .Where(predicate: player => !player.CommunityTrainVirtualPlayer).Select(selector: player => player.PlayerId);

    public IEnumerable<Player> Players => this._players.Values;

    public Player DetermineStartingPlayer()
    {
        var startingTile = this.DominoSet!.HighestPlayerTile;
        return this.NonCommunityPlayerIds.Select(selector: this.GetPlayer)
            .First(predicate: player => player.HighestDouble == startingTile);
    }

    private void LoadDominoSet(DominoSet set)
    {
        if (this.CanChange)
            throw new InvalidOperationException(message: "Can only load domino set when a game is not in progress");
        this.DominoSetId = set.SetId;
        this.DominoSet = set;
    }

    public Player GetPlayer(Guid id)
    {
        return this._players[key: id];
    }

    public Domino? GetDomino(Guid id)
    {
        return this.DominoSet?.GetDominoById(dominoId: id);
    }

    public void Start()
    {
        if (this.Started) throw new Exception(message: "Game already started");
        if (this.Ended) throw new Exception(message: "Game already ended");
        this.DateStarted = DateTime.UtcNow;

        var startingPlayer = this.DetermineStartingPlayer();
        this.TurnOrder.Clear();
        this.TurnOrder.Add(item: startingPlayer.PlayerId);
        // now go around the circle and add the other players
        var nextPlayer = startingPlayer;
        do
        {
            nextPlayer = this.Rules.PlayOrderClockwise
                ? this.EngineHub.PlayerToRight(player: nextPlayer)
                : this.EngineHub.PlayerToLeft(player: nextPlayer);
            // stop when we reach one we've added already
            if (this.TurnOrder.Contains(item: nextPlayer.PlayerId)) break;
            this.TurnOrder.Add(item: nextPlayer.PlayerId);
        } while (nextPlayer != startingPlayer);

        if (this.TurnOrder.Count != this.PlayerCount)
            throw new Exception(message: "Player count does not match turn order");

        this.Started = true;
    }

    /// <summary>
    ///     Current player is done with the turn.
    /// </summary>
    public void Pass()
    {
        var dateTurnEnded = DateTime.UtcNow;
        if (this.CurrentPlayer is null)
            throw new Exception(message: "No current player");
        if (this.DateTurnStarted is null)
            throw new Exception(message: "Cannot end turn when turn has not begun");
        this.GameHistory.AddRecord(record: new HistoryRecord(
            Turn: this.Turn++,
            EventTime: dateTurnEnded,
            EventType: Enumerations.GameEventType.TurnEnded,
            PlayerId: this.CurrentPlayer.PlayerId,
            ExistingDominoId: Guid.Empty,
            AddedDominoId: Guid.Empty));
        this.DateTurnStarted = null;
    }

    private void EndGame(Player requestingPlayer)
    {
        var dateEnded = DateTime.UtcNow;
        if (this.DateStarted is null)
            throw new Exception(message: "Game not started");
        if (this.DateEnded is not null)
            throw new Exception(message: "Game already ended");
        this.GameHistory.AddRecord(record: new HistoryRecord(
            Turn: this.Turn,
            EventTime: dateEnded,
            EventType: Enumerations.GameEventType.GameEnded,
            PlayerId: requestingPlayer.PlayerId,
            ExistingDominoId: Guid.Empty,
            AddedDominoId: Guid.Empty));
        this.DateEnded = dateEnded;
    }

    public bool HasPlayer(Player player)
    {
        return this._players.ContainsKey(key: player.PlayerId);
    }

    public bool HasPlayer(Guid id)
    {
        return this._players.ContainsKey(key: id);
    }

    public bool HasDomino(Domino domino)
    {
        return this.DominoSet is not null && this.DominoSet.HasDomino(dominoId: domino.DominoId);
    }

    public bool HasDomino(Guid dominoId)
    {
        return this.DominoSet is not null && this.DominoSet.HasDomino(dominoId: dominoId);
    }

    public void AddPlayer(Player player)
    {
        if (!this.CanChange) throw new Exception(message: "Game already in progress");
        if (player is CommunityTrainPlayer communityTrainPlayer)
        {
            if (this._players.Values.Any(predicate: p => p.CommunityTrainVirtualPlayer))
                throw new Exception(message: "Community train player already exists");
            this._players[key: player.PlayerId] = communityTrainPlayer;
        }
        else
        {
            var newPlayerCount = this.PlayerCount + 1;
            if (newPlayerCount >= this.Rules.MaximumPlayers)
                throw new Exception(message: $"Game already has max players ({this.Rules.MaximumPlayers})");
            if (this.HasPlayer(player: player)) throw new Exception(message: "Player already in game");
            this._players[key: player.PlayerId] = player;
        }

        this.ResizeHub();
    }

    public void DealInitialHand(Player player)
    {
        this.Deal(
            player: player,
            tileCount: this.TileCount);
    }

    public void RemovePlayer(Player player)
    {
        if (!this.CanChange)
            throw new InvalidOperationException(message: "Game is in progress");

        if (player is CommunityTrainPlayer communityTrainPlayer)
            throw new Exception(message: "Cannot remove virtual community train player");

        var newPlayerCount = this.PlayerCount - 1;
        if (newPlayerCount <= this.Rules.MinimumPlayers)
            throw new Exception(message: $"Game already has minimum players ({this.Rules.MinimumPlayers})");
        if (this._players.ContainsKey(key: player.PlayerId))
            this._players.Remove(key: player.PlayerId);
        if (this.TurnOrder.Contains(item: player.PlayerId))
            this.TurnOrder.Remove(item: player.PlayerId);
        this.ResizeHub();
    }

    /// <summary>
    /// </summary>
    /// <returns>Returns whether resized</returns>
    private bool ResizeHub()
    {
        var playerCount = this._players.Count;
        // need number of players plus one for the community train
        var hubSize = playerCount + 1;
        var oldHubId = this.EngineHub.HubId;
        this.EngineHub = this.EngineHub.Resize(newSize: hubSize);
        return oldHubId == this.EngineHub.HubId;
    }

    private void Deal(Player player, int tileCount)
    {
        if (!this.CanChange) throw new Exception(message: "Game already in progress");
        if (this.DominoSet is null)
            throw new Exception(message: "No domino set");
        this.DominoSet.Deal(player: player,
            dominoes: tileCount);
    }

    /// <summary>
    ///     Unclear what this function was for in the old code
    /// </summary>
    /// <param name="player"></param>
    /// <param name="domino"></param>
    /// <returns></returns>
    public bool DrawCardFromTile(Player player, Domino domino)
    {
        if (this.CurrentPlayer!.PlayerId != player.PlayerId || domino.Owner is not null) return false;
        domino.DealTo(player: player);
        return true;
    }


    public bool PlayDomino(Domino dominoInHand, Player destinationPlayer)
    {
        if (!this.CanChange) return false;

        if (this.CurrentPlayer is null) return false;

        if (!this.CurrentPlayer.HasDomino(dominoId: dominoInHand.DominoId)) return false;

        if (destinationPlayer is CommunityTrainPlayer dctp && this.communityTrainPlayer is not null)
        {
            if (dctp.PlayerId.Equals(this.communityTrainPlayer.PlayerId))
            {

            }
        }

        this.DominoSet ??= new DominoSet(
            game: this,
            playerCount: this.PlayerCount,
            rules: this.Rules);

        switch (this.Turn)
        {
            // if it is the first turn, the tile played must be the double with the value matching the HighestTile
            case 0 when dominoInHand.Value1 != this.DominoSet.HighestTile ||
                        dominoInHand.Value2 != this.DominoSet.HighestTile:
                return false;
            case 0:
                this.EngineHub.SetEnginePlayer(player: this.CurrentPlayer);
                this.CurrentPlayer.MoveDominoToTrain(dominoId: dominoInHand.DominoId);
                this.GameHistory.AddRecord(record: new HistoryRecord(
                    Turn: this.Turn,
                    EventTime: DateTime.UtcNow,
                    EventType: Enumerations.GameEventType.PlayTile,
                    PlayerId: this.CurrentPlayer.PlayerId,
                    ExistingDominoId: Guid.Empty,
                    AddedDominoId: dominoInHand.DominoId));
                this.Pass();
                return true;
        }

        // make sure the domino on the board is the next domino
        var dominoOnBoard = this.CurrentPlayer.LastTrainDomino;

        // ReSharper disable once UseNullPropagation
        if (dominoOnBoard is null) return false;

        var nextDominoOrientation = dominoOnBoard.NextDominoOrientation(nextDomino: dominoInHand);
        if (nextDominoOrientation is null)
            return false;

        if (!nextDominoOrientation.Value)
            // domino in hand matches domino on board, attached to our right side
            this.GameHistory.AddRecord(record: new HistoryRecord(
                Turn: this.Turn,
                EventTime: DateTime.UtcNow,
                EventType: Enumerations.GameEventType.PlayTile,
                PlayerId: this.CurrentPlayer.PlayerId,
                ExistingDominoId: dominoOnBoard.DominoId,
                AddedDominoId: dominoInHand.DominoId));
        else
            // domino in hand matches domino on board, attached by its left side
            this.GameHistory.AddRecord(record: new HistoryRecord(
                Turn: this.Turn,
                EventTime: DateTime.UtcNow,
                EventType: Enumerations.GameEventType.PlayTile,
                PlayerId: this.CurrentPlayer.PlayerId,
                ExistingDominoId: dominoOnBoard.DominoId,
                AddedDominoId: dominoInHand.DominoId));

        this.CurrentPlayer.MoveDominoToTrain(dominoId: dominoInHand.DominoId);

        this.Pass();

        return true;
    }
}