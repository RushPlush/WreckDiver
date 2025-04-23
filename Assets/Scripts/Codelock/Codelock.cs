using System.Collections;
using UnityEditor;
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

    [SerializeField] private GameObject currentInteractor;

    private WreckDiverInputActions diverInputActions;
    private WreckDiverInputActions.InteractActions interact;
    private Outline outline;
    private (Vector3 position, Vector3 eulerAngle) oldCameraOffsets;
    private (Vector3 position, Vector3 eulerAngle) cameraOffset = (new(-0.175f, 0, -0.5f), new (0, 15, 0));

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
        cameraOffset.position += transform.position;
    }

    private void InteractOnLeft(InputAction.CallbackContext obj)
    {
        highlighted--;
        if (highlighted < 0) highlighted = lockwheels.Length - 1;
    }

    private void InteractOnRight(InputAction.CallbackContext obj)
    {
        highlighted = (highlighted + 1) % lockwheels.Length;
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
        if (unlockable is not IUnlockable _)
            throw new System.Exception("Unlockable must implement IUnlockable");

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
        else if (unlockable) Debug.Log($"Unlockable {unlockable.name} must implement IUnlockable to unlock");
        else return;

        Deselect(currentInteractor);
        StartCoroutine(DelayedUnhighlight());

        // stop updating
        enabled = false;
    }

    /// <summary>
    /// This is only used from the editor and should not be used in the game
    /// </summary>
    internal void Unlock()
    {
        if (unlockable is IUnlockable unlockableInterface) unlockableInterface.Unlock();
        enabled = false;
    }

    bool IInteractable.IsDestroyed => !enabled;

    public bool Select(GameObject player)
    {
        currentInteractor = player;
        player.GetComponent<PlayerController>().DisableMovement();
        oldCameraOffsets.position = Camera.main.transform.position;
        oldCameraOffsets.eulerAngle = Camera.main.transform.eulerAngles;
        Camera.main.GetComponentInChildren<Light>().enabled = false;
        StartCoroutine(Selected());
        return true;
    }

    private float elapsedTime = 0;

    IEnumerator Selected()
    {
        yield return new WaitForFixedUpdate();
        while (elapsedTime < 0.5f)
        {
            elapsedTime += Time.deltaTime;
            Camera.main.transform.position = Vector3.Lerp(oldCameraOffsets.position, cameraOffset.position, elapsedTime / 0.5f);
            Camera.main.transform.rotation = Quaternion.Lerp(Quaternion.Euler(oldCameraOffsets.eulerAngle), Quaternion.Euler(cameraOffset.eulerAngle), elapsedTime / 0.5f);
            yield return new WaitForFixedUpdate();
        }
        elapsedTime = 0;
        Camera.main.transform.position = cameraOffset.position;
        Camera.main.transform.rotation = Quaternion.Euler(cameraOffset.eulerAngle);
        interact.Enable();
        outline.enabled = false;
        highlighted = 0;
        lockwheels[0].Highlight();
        currentInteractor.GetComponentInChildren<MappingHandler>().ToggleInteractionGuide();
    }

    public bool Select()
    {
        return false;
    }

    public void Deselect()
    {
    }

    public void Deselect(GameObject player)
    {
        currentInteractor = null;
        Camera.main.GetComponentInChildren<Light>().enabled = true;
        player.GetComponentInChildren<MappingHandler>().ToggleInteractionGuide();
        StartCoroutine(Deselected(player));
    }

    IEnumerator Deselected(GameObject player)
    {
        yield return new WaitForFixedUpdate();
        while (elapsedTime < 0.5f)
        {
            elapsedTime += Time.deltaTime;
            Camera.main.transform.position = Vector3.Lerp(cameraOffset.position, oldCameraOffsets.position, elapsedTime / 0.5f);
            Camera.main.transform.rotation = Quaternion.Lerp(Quaternion.Euler(cameraOffset.eulerAngle), Quaternion.Euler(oldCameraOffsets.eulerAngle), elapsedTime / 0.5f);
            yield return new WaitForFixedUpdate();
        }
        elapsedTime = 0;
        Camera.main.transform.position = oldCameraOffsets.position;
        Camera.main.transform.rotation = Quaternion.Euler(oldCameraOffsets.eulerAngle);
        interact.Disable();
        outline.enabled = true;
        lockwheels[highlighted].Unhighlight();
        highlighted = -1;
        var playerController = player.GetComponent<PlayerController>();
        playerController.EnableMovement();
        playerController.interactor.isInteracting = false;
    }

    public void Highlight()
    {
        if (highlighted >= 0) return;
        outline.enabled = true;
    }

    public void Unhighlight()
    {
        outline.enabled = false;
    }

    IEnumerator DelayedUnhighlight()
    {
        while (elapsedTime < 0.5f) yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        outline.enabled = false;
    }
}

[CustomEditor(typeof(Codelock))]
public class CodelockEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Open Codelock")) (target as Codelock).Unlock();
    }
}