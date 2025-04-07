namespace Inventory
{
    public interface IItem
    {
        public void PrimaryUse();
        public void SecondaryUse(bool pressed);
        public void TertiaryUse(bool pressed);
    }
}