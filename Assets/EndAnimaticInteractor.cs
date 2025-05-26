using UnityEngine;
using UnityEngine.Video;

public class EndAnimaticInteractor : MonoBehaviour, IInteractable
{
    private VideoPlayer videoPlayer;
    private Movement playerMovement;
    private PlayerCamera playerCamera;
    private Helmet helmet;
    private Outline outline;
    private MappingHandler mappingHandler;

    [SerializeField]private Vector3 targetRotation;

    [SerializeField]private float rotationSpeed = 50f;
    [SerializeField] private Color highlightColor = Color.yellow;
    [SerializeField] private Color unhighlightColor = Color.white;
    private bool hasStarted = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerMovement = FindFirstObjectByType<Movement>();
        playerCamera = playerMovement.GetComponent<PlayerCamera>();
        helmet = FindFirstObjectByType<Helmet>();
        mappingHandler = FindFirstObjectByType<MappingHandler>();
        videoPlayer = playerMovement.gameObject.GetComponent<VideoPlayer>();
        outline = GetComponent<Outline>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!hasStarted) return;
        var handle = transform.parent;
        if (handle.rotation.eulerAngles.z < -0.47f)
        {
            var rotation = handle.rotation;
            var curRotation = rotation.eulerAngles;
            curRotation += new Vector3(-rotationSpeed * Time.deltaTime, 0, 0);
            handle.rotation = Quaternion.Euler(curRotation);
        }
        else
        {
            //playerMovement.transform.rotation = Quaternion.identity; // Reset rotation when video ends
            //playerMovement.StartMoving(); // Re-enable player movement after animatic
            //hasStarted = false; // Reset the state
            helmet.gameObject.SetActive(false);
            playerCamera.enabled = false;
            mappingHandler.gameObject.SetActive(false);
            videoPlayer.Play();
            hasStarted = false; // Reset the state
        }
    }

    public void Highlight()
    {
        outline.OutlineColor = highlightColor;
        outline.OutlineWidth = 5;
    }

    public void Unhighlight()
    {
        outline.OutlineColor = unhighlightColor;
        outline.OutlineWidth = 2;
    }

    public bool IsDestroyed { get; }
    public bool Select()
    {
        hasStarted = true;
        playerMovement.StopMoving(); // Disable player movement during the animatic
        return false;
    }

    public void Deselect()
    {
        throw new System.NotImplementedException();
    }
}
