using System;
using System.Collections.Generic;
using Model.Cards;
using Model.Cards.Positioning;
using Model.Games.Players;

namespace Model.Games
{
    public enum OpenStatus
    {
        Success,
        Failure,
        WrongTurn
    }
    public interface IGame
    {
        public IPlayer CurrentTurnPlayer { get; }
        public IReadOnlyCollection<IPlayer> Players { get; }
        public IDictionary<IGameBot, BotDifficulty>? Bots { get; }

        public event Action<IPlayer>? NextStep; 
        public event EventHandler<GameEventArgs>? GameFinished;
        public event Action<string>? MessageReceived;
    
        public OpenStatus OpenCard(IPlayer player, CardsSelection selection);
    
        public IReadOnlyList<IReadOnlyList<ICard?>> GetCards();
    }
}