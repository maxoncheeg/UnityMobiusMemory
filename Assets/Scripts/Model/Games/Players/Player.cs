using System;

namespace Model.Games.Players
{
    public class Player : IPlayer
    {
        public Guid Id { get; }
        public string Name { get; }
        public int Points { get; private set; } = 0;

        public Player(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public void AddPoint()
        {
            Points++;
        }

        public void ClearPoints()
        {
            Points = 0;
        }

        public bool Equals(IPlayer? other)
        {
            return other is { } player && player.Id == Id;
        }
    }
}