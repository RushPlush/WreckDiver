using UnityEngine;

namespace Inventory
{
    public interface IItem
    {
        public void PrimaryUse(bool pressed);
        public void SecondaryUse(bool pressed);
        public void TertiaryUse(bool pressed);
    }
}