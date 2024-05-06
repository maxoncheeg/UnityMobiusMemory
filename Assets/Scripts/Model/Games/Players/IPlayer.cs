using System;

namespace Model.Games.Players
{
    public interface IPlayer : IEquatable<IPlayer>
    {
        public Guid Id { get; }
        public string Name { get; }
        public int Points { get; }

        public void AddPoint();
        public void ClearPoints();
    }
}