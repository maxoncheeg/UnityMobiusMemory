using System;
using System.Collections.Generic;
using System.Linq;
using Model.Cards;
using Model.Cards.Positioning;

namespace Model.Decks
{
    public class Deck : IDeck
    {
        private readonly List<List<ICard>> _cards = new() { };
    
        public event Action? CardsAreOut;

        public int AreaWidth { get; }
        public IReadOnlyList<IReadOnlyList<ICard?>> Cards => _cards;

        public Deck(int width, ICardFactory factory, Random random)
        {
            if (width < 2)
                throw new ArgumentException("width must be greater than 1", nameof(width));
            if (width % 2 != 0)
                throw new ArgumentException("description width must be even");
            AreaWidth = width;
        
                //ArgumentNullException.ThrowIfNull(factory);

            string cardName = factory.GetCard();
            for (int i = 0, cardCounter = 0; i < width; i++)
            {
                List<ICard?> cardLine = new() { };
                for (int j = 0; j < width; j++)
                {
                    cardLine.Add(new Card(cardName));
                    if (++cardCounter < 2) continue;
                    if(j == width - 1 && i == width - 1)continue;
                    cardCounter = 0;
                    cardName = factory.GetCard();
                }

                _cards.Add(cardLine);
            }
        
            _cards.Shuffle(random);
        }

        public bool OpenCards(CardsSelection selection)
        {
            var (first, second) = selection;
            ICard? card = _cards[first.Y][first.X];
            bool result = false;

            if (card == null) return result;
            result = card!.Equals(_cards[second.Y][second.X]);
        
            if(result)
            {
                _cards[first.Y][first.X] = _cards[second.Y][second.X] = null;
                CheckNullableCards();
            }
        
            return result;
        }

        private void CheckNullableCards()
        {     
            if (_cards.SelectMany(line => line).OfType<ICard>().Any())
                return;
        
            CardsAreOut?.Invoke();
        }
    }
}