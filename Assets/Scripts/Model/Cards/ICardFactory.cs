namespace Model.Cards
{
    public interface ICardFactory
    {
        public string GetCard(int index = -1);
    }
}