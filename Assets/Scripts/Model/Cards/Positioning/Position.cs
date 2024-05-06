using System;

namespace Model.Cards.Positioning
{
    public struct Position
    {
        public int X { get; }
        public int Y { get; }

        public Position(int x, int y)
        {
            if (x < 0)
                throw new ArgumentException($"{nameof(x)} must be greater or equals zero.");
            if (y < 0)
                throw new ArgumentException($"{nameof(y)} must be greater or equals zero.");

            X = x;
            Y = y;
        }
    }
}