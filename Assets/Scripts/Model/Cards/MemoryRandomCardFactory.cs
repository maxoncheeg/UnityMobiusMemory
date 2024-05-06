using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace Model.Cards
{
    public class MemoryRandomCardFactory : ICardFactory
    {
        private readonly Random _random = new();
        private readonly List<int> _indexMemory = new() { };

        private readonly List<string> _cards;

        public MemoryRandomCardFactory()
        {
            _cards =
                new()
                {
                    "one", "two", "three", "four",
                    "apple", "orange", "melon", "watermelon",


                    // "n1",
                    // "n2",
                    // "n3",
                    // "n4",
                    // "n5",
                    // "n6",
                    // "n7",
                    // "n8",
                };
        }

        public MemoryRandomCardFactory(List<string> names)
        {
            _cards = new(names);
        }

        public string GetCard(int index = -1)
        {
            if (index != -1 && (index < 0 || index > _cards.Count))
                throw new IndexOutOfRangeException(nameof(index));

            int newIndex = 0;
            if (index == -1)
            {
                // do
                //     index = _random.Next(_cards.Count);
                // while (_indexMemory.IndexOf(index) != -1);

                List<string> list = _cards.Select((c, i) => (c, i)).Where(t => !_indexMemory.Contains(t.i))
                    .Select(j => j.c).ToList();
                if (list.Count <= 0) throw new ApplicationException("ГДЕ МОИ КАРТЫ!");

                var card = list[_random.Next(list.Count)];
                newIndex = _cards.IndexOf(card);
            }
            else newIndex = index;

            if (_cards.Count <= newIndex)
                throw new ApplicationException("Incorrect card factory index");
            if (_cards[newIndex] is not string value || string.IsNullOrEmpty(value))
                throw new ApplicationException("Incorrect card factory configuration ");
            
            _indexMemory.Add(newIndex);
            return value;
        }
    }
}