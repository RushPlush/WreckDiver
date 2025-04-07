using System;
using UnityEngine;
using UnityEngine.InputSystem;

//[RequireComponent(typeof(Camera))] //As in it needs a child with it
public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private Vector2 mouseSensitivity = new Vector2(.05f, .03f);
    [SerializeField] private Vector2 contollerSensitivity = new Vector2(1f, 1f);

    private Camera cam;

    private Vector2 sensitivity = new Vector2(5f, 5f);

    private Vector2 rot;

    private void Awake()
    {
        try
        {
            cam = GetComponentInChildren<Camera>();
        }
        catch (Exception e)
        {
            Console.WriteLine(e + " camera in child not found");
            throw;
        }
        DeviceListener.OnDeviceChange += OnDeviceChange;
    }

    private void OnDestroy()
    {
        DeviceListener.OnDeviceChange -= OnDeviceChange;
    }

    private void OnDeviceChange(DeviceType obj)
    {
        sensitivity = obj == DeviceType.Gamepad ? contollerSensitivity : mouseSensitivity;
    }

    public void Look(Vector2 input)
    {
        input.y = -input.y;
        rot += input * sensitivity;
        rot.y = Mathf.Clamp(rot.y, -80, 80);

        cam.transform.localRotation = Quaternion.Euler(rot.y, 0, 0);
        transform.localRotation = Quaternion.Euler(0, rot.x, 0);
    }
}
