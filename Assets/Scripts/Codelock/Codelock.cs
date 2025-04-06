using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Outline))]
[DisallowMultipleComponent]
public class Codelock : MonoBehaviour, IInteractableWithPlayer
{
    [SerializeField] private Lockwheel[] lockwheels;
    [SerializeField] private int[] code;
    [SerializeField] private int[] hardCode;
    [SerializeField] private MonoBehaviour unlockable;
    [SerializeField] private int highlighted = -1;
    [SerializeField] private bool flipIncrementDecrement;
    [SerializeField] private Vector2[] codePositions;
    [SerializeField] private GameObject[] codeObjects;

    private WreckDiverInputActions diverInputActions;
    private WreckDiverInputActions.InteractActions interact;
    private Outline outline;

    void Awake()
    {
        diverInputActions = new WreckDiverInputActions();
        interact = diverInputActions.Interact;
        interact.Left.performed += InteractOnLeft;
        interact.Right.performed += InteractOnRight;
        interact.Down.performed += InteractOnDown;
        interact.Up.performed += InteractOnUp;

        outline = GetComponent<Outline>();
        outline.enabled = false;
    }

    private void InteractOnLeft(InputAction.CallbackContext obj)
    {
        highlighted--;
        if (highlighted < 0) highlighted = lockwheels.Length - 1;
        lockwheels[highlighted].Decrement();
    }

    private void InteractOnRight(InputAction.CallbackContext obj)
    {
        highlighted = (highlighted + 1) % lockwheels.Length;
        lockwheels[highlighted].Increment();
    }

    private void InteractOnDown(InputAction.CallbackContext obj)
    {
        if (flipIncrementDecrement)
            lockwheels[highlighted].Decrement();
        else
            lockwheels[highlighted].Increment();
    }

    private void InteractOnUp(InputAction.CallbackContext obj)
    {
        if (flipIncrementDecrement)
            lockwheels[highlighted].Increment();
        else
            lockwheels[highlighted].Decrement();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // if (unlockable is not IUnlockable _)
        //     throw new System.Exception("Unlockable must implement IUnlockable");

        // get all lockwheels for this component
        lockwheels = GetComponentsInChildren<Lockwheel>();
        // generate random code
        code = new int[lockwheels.Length];
        if (hardCode.Length == code.Length)
        {
            code = hardCode;

            for (int i = 0; i < lockwheels.Length; i++)
            {
                SetCodeFacesAndMaterial(i);
            }
        }
        else
            for (int i = 0; i < lockwheels.Length; i++)
            {
                code[i] = Random.Range(0, 9);
                SetCodeFacesAndMaterial(i);
            }
    }

    private void SetCodeFacesAndMaterial(int i)
    {
        // Generate random face for each lockwheel that is not the code face required
        var randomFace = Random.Range(0, 9);
        while (randomFace == code[i])
        {
            randomFace = Random.Range(0, 9);
        }

        lockwheels[i].SetCodeFace(randomFace);

        // Set the material uv offset to the code face required
        foreach (var material in codeObjects[i].GetComponent<Renderer>().materials) material.SetVector("_CodeOffset", codePositions[code[i]]);
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var lockwheel in lockwheels)
        {
            lockwheel.Unhighlight();
        }
        if (highlighted >= 0)
        {
            lockwheels[highlighted].Highlight();
        }

        // check if code face matches code for all lockwheels
        for (int i = 0; i < lockwheels.Length; i++)
        {
            if (lockwheels[i].GetCodeFace() != code[i])
            {
                return;
            }
        }

        // if code matches, open lock if unlockable is assigned else log to console
        if (unlockable is IUnlockable unlockableInterface) unlockableInterface.Unlock();
        else if (unlockable != null) Debug.Log($"Unlockable {unlockable.name} must implement IUnlockable to unlock");
        else return;

        // stop updating
        enabled = false;
    }

    bool IInteractable.IsDestroyed => !enabled;

    public void Select(GameObject player)
    {
        player.GetComponent<PlayerController>().DisableMovement();
        interact.Enable();
        outline.enabled = false;
    }

    public void Select()
    {
        throw new System.NotImplementedException();
    }

    public void Deselect()
    {
        throw new System.NotImplementedException();
    }

    public void Deselect(GameObject player)
    {
        player.GetComponent<PlayerController>().EnableMovement();
        interact.Disable();
        outline.enabled = true;
    }

    public void Highlight()
    {
        outline.enabled = true;
    }

    public void Unhighlight()
    {
        outline.enabled = false;
    }
}
