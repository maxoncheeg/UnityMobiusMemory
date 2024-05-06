using System;

namespace Model.Cards
{
    public interface ICard : IEquatable<ICard>
    {
        public string Name { get; }
    }
}
