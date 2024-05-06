using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Model.Cards;
using Model.Cards.Positioning;
using Model.Decks;
using Model.Games.Players;

namespace Model.Games
{
    public enum BotDifficulty
    {
        Easy,
        Medium,
        Hard,
        Impossible
    }

    public class BotGame : IGame
    {
        private readonly Random _random = new();
        private readonly IDeck _deck;
        private readonly Dictionary<IGameBot, BotDifficulty>? _bots;
        private readonly List<IPlayer> _players;
        private readonly IPlayer _player;

        public IPlayer CurrentTurnPlayer { get; private set; }

        public IReadOnlyCollection<IPlayer> Players => _players;
        public IDictionary<IGameBot, BotDifficulty>? Bots => _bots;
        
        public event Action<IPlayer>? NextStep; 
        public event EventHandler<GameEventArgs>? GameFinished;
        public event Action<string>? MessageReceived;

        public BotGame(IDeck deck, List<IPlayer>? players, Dictionary<string, BotDifficulty>? botDifficulties = null)
        {
            //ArgumentNullException.ThrowIfNull(player);
            //ArgumentNullException.ThrowIfNull(deck);

            _deck = deck;
            _deck.CardsAreOut += DeckOnCardsAreOut;

            if (players != null)
            {
                _players = new(players);
                // _player = players[0];
            }
            else _players = new();

            if (botDifficulties != null && botDifficulties.Count != 0)
            {
                _bots = new Dictionary<IGameBot, BotDifficulty>();
                foreach (var pair in botDifficulties)
                    _bots.Add(new GameBot(_deck.AreaWidth, pair.Key), pair.Value);
                _players.AddRange(_bots.Select(bot => bot.Key.Player));
            }

            _players.Shuffle(_random);
            CurrentTurnPlayer = _players.Last();
        }

        private void DeckOnCardsAreOut()
        {
            CurrentTurnPlayer.AddPoint();
            var points = (from p in _players
                select p.Points).Max();
            var player = (from p in _players where p.Points == points select p).First();

            
            GameFinished?.Invoke(this, new GameEventArgs() {PlayerName = player.Name, Players = new(_players)});
        }

        public OpenStatus OpenCard(IPlayer player, CardsSelection selection)
        {
            if (selection.First.Equals(selection.Second)) return OpenStatus.Failure;
            if (!_players.Contains(player))
            {
                MessageReceived?.Invoke("playerDoesntExist");
                return OpenStatus.WrongTurn;
            }

            if (player != CurrentTurnPlayer)
            {
                MessageReceived?.Invoke("wrongTurn");
                return OpenStatus.WrongTurn;
            }

            bool openSuccess = _deck.OpenCards(selection);
            Debug.WriteLine(player.Name + Environment.NewLine + openSuccess);
            MemoriseSelectionForBots(selection, openSuccess);

            if (!openSuccess)
                CurrentTurnPlayer = _players[(_players.IndexOf(CurrentTurnPlayer) + 1) % _players.Count];
            else
                CurrentTurnPlayer.AddPoint();
            NextStep?.Invoke(CurrentTurnPlayer);

            return openSuccess ? OpenStatus.Success : OpenStatus.Failure;
        }

        public IReadOnlyList<IReadOnlyList<ICard?>> GetCards() => _deck.Cards;

        private void MemoriseSelectionForBots(CardsSelection selection, bool openSuccess)
        {
            if (_bots == null) return;
            var (first, second) = selection;

            foreach (var bot in _bots)
            {
                if (openSuccess)
                {
                    bot.Key.MemoriseEmptyPlace(first);
                    bot.Key.MemoriseEmptyPlace(second);
                    continue;
                }

                List<Position> firstPositions = new() { }, secondPositions = new() { };

                if (bot.Value > BotDifficulty.Easy)
                {
                    if (_deck.Cards[first.Y][first.X] != null)
                        firstPositions.Add(first);
                    if (_deck.Cards[first.Y][first.X] != null)
                        secondPositions.Add(second);
                }

                switch (bot.Value)
                {
                    case BotDifficulty.Hard:
                        if (_deck.Cards[first.Y][first.X] != null)
                            MemoriseAroundCards(firstPositions, first);
                        if (_deck.Cards[second.Y][second.X] != null)
                            MemoriseAroundCards(secondPositions, second);
                        break;
                    case BotDifficulty.Medium:
                        if (_deck.Cards[first.Y][first.X] != null)
                            MemoriseAllAroundCards(firstPositions, first);
                        if (_deck.Cards[second.Y][second.X] != null)
                            MemoriseAllAroundCards(secondPositions, second);
                        break;
                    case BotDifficulty.Impossible:
                    case BotDifficulty.Easy:
                    default:
                        break;
                }

                if (firstPositions.Count > 0 && _deck.Cards[first.Y][first.X] is { } firstCard)
                    bot.Key.MemoriseCard(firstCard, firstPositions);
                if (secondPositions.Count > 0 && _deck.Cards[second.Y][second.X] is { } secondCard)
                    bot.Key.MemoriseCard(secondCard, secondPositions);
            }
        }

        private void MemoriseAllAroundCards(ICollection<Position> positions, Position cardPosition)
        {
            MemoriseAroundCards(positions, cardPosition);
            if (_random.Next(3) >= 1
                && cardPosition.Y - 1 >= 0
                && cardPosition.X - 1 >= 0 && _deck.Cards[cardPosition.Y - 1][cardPosition.X - 1] != null)
                positions.Add(new Position(cardPosition.X - 1, cardPosition.Y - 1));
            if (_random.Next(3) >= 1
                && cardPosition.Y + 1 < _deck.AreaWidth
                && cardPosition.X + 1 < _deck.AreaWidth && _deck.Cards[cardPosition.Y + 1][cardPosition.X + 1] != null)
                positions.Add(new Position(cardPosition.X + 1, cardPosition.Y + 1));
            if (_random.Next(3) >= 1
                && cardPosition.Y + 1 < _deck.AreaWidth
                && cardPosition.X - 1 >= 0 && _deck.Cards[cardPosition.Y + 1][cardPosition.X - 1] != null)
                positions.Add(new Position(cardPosition.X - 1, cardPosition.Y + 1));
            if (_random.Next(3) >= 1
                && cardPosition.Y - 1 >= 0
                && cardPosition.X + 1 < _deck.AreaWidth && _deck.Cards[cardPosition.Y - 1][cardPosition.X + 1] != null)
                positions.Add(new Position(cardPosition.X + 1, cardPosition.Y - 1));
        }

        private void MemoriseAroundCards(ICollection<Position> positions, Position cardPosition)
        {
            if (_random.Next(3) >= 1 && cardPosition.Y - 1 >= 0 && _deck.Cards[cardPosition.Y - 1][cardPosition.X] != null)
                positions.Add(new Position(cardPosition.X, cardPosition.Y - 1));
            if (_random.Next(3) >= 1 && cardPosition.Y + 1 < _deck.AreaWidth &&
                _deck.Cards[cardPosition.Y + 1][cardPosition.X] != null)
                positions.Add(new Position(cardPosition.X, cardPosition.Y + 1));
            if (_random.Next(3) >= 1 && cardPosition.X - 1 >= 0 && _deck.Cards[cardPosition.Y][cardPosition.X - 1] != null)
                positions.Add(new Position(cardPosition.X - 1, cardPosition.Y));
            if (_random.Next(3) >= 1 && cardPosition.X + 1 < _deck.AreaWidth &&
                _deck.Cards[cardPosition.Y][cardPosition.X + 1] != null)
                positions.Add(new Position(cardPosition.X + 1, cardPosition.Y));
        }
    }
}