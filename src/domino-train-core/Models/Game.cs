using DominoTrain.Core.Interfaces;
using DominoTrain.Core.Models.Players;
using DominoTrain.Models;
using DominoTrain.Models.Rules;
using System.Runtime.Serialization;

namespace DominoTrain.Core.Models;

[Serializable]
[DataContract]
public class Game
{
    [DataMember] public readonly Guid GameId;

    private readonly Dictionary<Guid, Player> players;

    public readonly ISetRules Rules;
    private DominoSet? dominoSet;

    private EngineHub engineHub;

    public Game(List<Player>? players = null, int aiPlayerCount = 0)
    {
        this.GameId = Guid.NewGuid();
        this.Rules = new DefaultSetRules();
        this.players = new Dictionary<Guid, Player>();
        this.GameHistory = new GameHistory();
        this.engineHub = new EngineHub(game: this);
        this.TurnOrder = new List<Guid>();
        this.Turn = 0;
        this.dominoSet = players is null
            ? null
            : new DominoSet(
                game: this,
                playerCount: this.PlayerCount,
                rules: this.Rules);
        this.DominoSetId = this.dominoSet?.SetId ?? Guid.Empty;
        this.Started = false;
        this.Ended = false;
        this.Init();
        if (players is not null)
            foreach (var player in players)
                this.AddPlayer(player: player);

        for (var i = 0; i < aiPlayerCount; i++)
        {
            var newPlayer = new AiPlayer(
                game: this,
                name: null);
            this.AddPlayer(player: newPlayer);
        }

        // if passed in players did not include a community player, add one
        if (!this.players.Values.Any(predicate: player => player.CommunityTrainVirtualPlayer))
            this.AddPlayer(player: new CommunityTrainPlayer(game: this));
    }

    [DataMember] public Guid DominoSetId { get; private set; }

    public int HighestTile => this.dominoSet!.HighestTile;

    [DataMember] public int Turn { get; private set; }

    [DataMember] public Guid StationHubId => this.engineHub.HubId;

    private bool CanChange => !this.Ended && !this.Started;

    public GameHistory GameHistory { get; private set; }
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
            return !this.players.ContainsKey(key: currentPlayerId) ? null : this.players[key: currentPlayerId];
        }
    }

    [DataMember] public bool Started { get; private set; }

    [DataMember] public bool Ended { get; private set; }

    public int PlayerCount => this.players.Count - this.CommunityPlayerCount;

    public int CommunityPlayerCount
        => this.players.Count(predicate: player => player.Value.CommunityTrainVirtualPlayer);

    public int HumanPlayerCount => this.players.Count(predicate: player => !player.Value.AI);

    public int NetworkHumanPlayerCount
        => this.players.Count(predicate: player => player.Value.GetType() == typeof(HumanNetworkPlayer));

    public int AiPlayerCount => this.players.Count(predicate: player => player.Value.AI);

    /// <summary>
    ///     TODO: quote rule
    /// </summary>
    public int TileCount => this.dominoSet is not null ? this.dominoSet.Dominoes.Count : 0;

    public IEnumerable<Guid> AllPlayerIds => this.players.Keys;

    public IEnumerable<Guid> NonCommunityPlayerIds => this.players.Values
        .Where(predicate: player => !player.CommunityTrainVirtualPlayer).Select(selector: player => player.PlayerId);

    public IEnumerable<Player> Players => this.players.Values;

    public Player DetermineStartingPlayer()
    {
        var startingTile = this.dominoSet!.HighestPlayerTile;
        return this.NonCommunityPlayerIds.Select(selector: this.GetPlayer)
            .First(predicate: player => player.HighestDouble == startingTile);
    }

    private void LoadDominoSet(DominoSet set)
    {
        if (this.CanChange)
            throw new InvalidOperationException(message: "Can only load domino set when a game is not in progress");
        this.DominoSetId = set.SetId;
        this.dominoSet = set;
    }

    public Player GetPlayer(Guid id)
    {
        return this.players[key: id];
    }

    public Domino? GetDomino(Guid id)
    {
        return this.dominoSet?.GetDominoById(dominoId: id);
    }

    public void Start()
    {
        if (this.Started) throw new Exception(message: "Game already started");
        if (this.Ended) throw new Exception(message: "Game already ended");

        var startingPlayer = this.DetermineStartingPlayer();
        this.TurnOrder.Clear();
        this.TurnOrder.Add(item: startingPlayer.PlayerId);
        // now go around the circle and add the other players
        var nextPlayer = startingPlayer;
        do
        {
            nextPlayer = this.Rules.PlayOrderClockwise
                ? this.engineHub.PlayerToRight(player: nextPlayer)
                : this.engineHub.PlayerToLeft(player: nextPlayer);
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
        if (this.CurrentPlayer is null)
            throw new Exception(message: "No current player");

        this.GameHistory.AddRecord(record: new HistoryRecord(
            Turn: this.Turn,
            Time: DateTime.Now,
            PlayerId: this.CurrentPlayer.PlayerId,
            DominoId: Guid.Empty,
            NextDominoId: Guid.Empty));
        this.Turn++;
    }

    public bool HasPlayer(Player player)
    {
        return this.players.ContainsKey(key: player.PlayerId);
    }

    public bool HasPlayer(Guid id)
    {
        return this.players.ContainsKey(key: id);
    }

    public bool HasDomino(Domino domino)
    {
        return this.dominoSet is not null && this.dominoSet.HasDomino(dominoId: domino.DominoId);
    }

    public bool HasDomino(Guid dominoId)
    {
        return this.dominoSet is not null && this.dominoSet.HasDomino(dominoId: dominoId);
    }

    public void AddPlayer(Player player)
    {
        if (!this.CanChange) throw new Exception(message: "Game already in progress");
        if (player is CommunityTrainPlayer communityTrainPlayer)
        {
            if (this.players.Values.Any(predicate: p => p.CommunityTrainVirtualPlayer))
                throw new Exception(message: "Community train player already exists");
            this.players[key: player.PlayerId] = communityTrainPlayer;
        }
        else
        {
            var newPlayerCount = this.PlayerCount + 1;
            if (newPlayerCount >= this.Rules.MaximumPlayers)
                throw new Exception(message: $"Game already has max players ({this.Rules.MaximumPlayers})");
            if (this.HasPlayer(player: player)) throw new Exception(message: "Player already in game");
            this.players[key: player.PlayerId] = player;
        }

        this.ResizeHub();
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
        if (this.players.ContainsKey(key: player.PlayerId))
            this.players.Remove(key: player.PlayerId);
        if (this.TurnOrder.Contains(item: player.PlayerId))
            this.TurnOrder.Remove(item: player.PlayerId);
        this.ResizeHub();
    }

    /// <summary>
    /// </summary>
    /// <returns>Returns whether resized</returns>
    private bool ResizeHub()
    {
        var playerCount = this.players.Count;
        // need number of players plus one for the community train
        var hubSize = playerCount + 1;
        var oldHubId = this.engineHub.HubId;
        this.engineHub = this.engineHub.Resize(newSize: hubSize);
        return oldHubId == this.engineHub.HubId;
    }

    private void Init()
    {
        if (this.Started) throw new Exception(message: "Game already started");

        this.Ended = false;
        foreach (var player in this.players.Values) player.ResetDominoes();

        // clear history
        this.GameHistory = new GameHistory();
    }


    private void Deal(Player player, int tileCount)
    {
        if (!this.CanChange) throw new Exception(message: "Game already in progress");
        if (this.dominoSet is null)
            throw new Exception(message: "No domino set");
        this.dominoSet.Deal(player: player,
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


    public bool PlayDomino(Domino dominoInHand)
    {
        if (!this.CanChange) return false;

        if (this.CurrentPlayer is null) return false;

        if (!this.CurrentPlayer.HasDomino(dominoId: dominoInHand.DominoId)) return false;

        this.dominoSet ??= new DominoSet(
            game: this,
            playerCount: this.PlayerCount,
            rules: this.Rules);

        switch (this.Turn)
        {
            // if it is the first turn, the tile played must be the double with the value matching the HighestTile
            case 0 when dominoInHand.Value1 != this.dominoSet.HighestTile ||
                        dominoInHand.Value2 != this.dominoSet.HighestTile:
                return false;
            case 0:
                this.engineHub.SetEnginePlayer(player: this.CurrentPlayer);
                this.CurrentPlayer.MoveDominoToTrain(dominoId: dominoInHand.DominoId);
                this.GameHistory.AddRecord(record: new HistoryRecord(
                    Turn: this.Turn,
                    Time: DateTime.Now,
                    PlayerId: this.CurrentPlayer.PlayerId,
                    DominoId: Guid.Empty,
                    NextDominoId: dominoInHand.DominoId));
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
                Time: DateTime.Now,
                PlayerId: this.CurrentPlayer.PlayerId,
                DominoId: dominoOnBoard.DominoId,
                NextDominoId: dominoInHand.DominoId));
        else
            // domino in hand matches domino on board, attached by its left side
            this.GameHistory.AddRecord(record: new HistoryRecord(
                Turn: this.Turn,
                Time: DateTime.Now,
                PlayerId: this.CurrentPlayer.PlayerId,
                DominoId: dominoOnBoard.DominoId,
                NextDominoId: dominoInHand.DominoId));

        this.CurrentPlayer.MoveDominoToTrain(dominoId: dominoInHand.DominoId);

        this.Pass();

        return true;
    }
}