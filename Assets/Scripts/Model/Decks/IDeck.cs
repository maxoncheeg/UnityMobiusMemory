using System;
using System.Collections.Generic;
using Model.Cards;
using Model.Cards.Positioning;

namespace Model.Decks
{
    public interface IDeck
    {
        public int AreaWidth { get; }
        public IReadOnlyList<IReadOnlyList<ICard>> Cards { get; }
    
        public event Action CardsAreOut;

        public bool OpenCards(CardsSelection selection);
    }
}