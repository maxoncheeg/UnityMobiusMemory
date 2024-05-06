using System.Collections.Generic;
using Model.Cards;
using Model.Cards.Positioning;

namespace Model.Games.Players
{
    public interface IGameBot
    {
        public IPlayer Player { get; }

        public void MemoriseEmptyPlace(Position position);
        public void MemoriseCard(ICard card, IList<Position> positions);
        public Position SelectCard();
        public Position SelectCard(ICard card, Position position);
    }
}