using System;
using System.Diagnostics;

namespace Model.Cards
{

    public class Card : ICard
    {
        public string Name { get; }

        public Card(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException(nameof(Name));

            Name = name;
        }

        public bool Equals(ICard? other)
        {
            return other != null && Name == other.Name;
        }
    }
}