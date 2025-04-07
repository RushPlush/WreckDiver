using UnityEngine.InputSystem;

namespace Inventory
{
    public interface IItem
    {
        public void PrimaryUse(InputAction.CallbackContext context);
        public void SecondaryUse(InputAction.CallbackContext context);
        public void TertiaryUse(InputAction.CallbackContext context);
    }
}