using System;
using UnityEngine;
//[RequireComponent(typeof(Camera))] //As in it needs a child with it
public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Vector2 mouseSensitivity = new Vector2(0.5f, 5f);

    private Camera cam;

    private float multiplier = 0.01f;

    private float xRot;
    private float yRot;
    
    private void Awake() {
        try
        {
            cam = GetComponentInChildren<Camera>();
        }
        catch (Exception e)
        {
            Console.WriteLine(e + " camera in child not found");
            throw;
        }
        
    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public void Look(Vector2 input)
    {
        xRot += input.x * mouseSensitivity.x * multiplier;

        yRot -= input.y * mouseSensitivity.y * multiplier;
        yRot = Mathf.Clamp(yRot, -80, 80);
        
        cam.transform.localRotation = Quaternion.Euler(yRot, 0, 0 );
        transform.localRotation = Quaternion.Euler(0, xRot, 0);
    }
}
