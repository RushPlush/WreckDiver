using System.Linq;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public static class DeviceListener
{
    private static InputDevice _lastDevice;
    public static event System.Action<DeviceType> OnDeviceChange;
    public static DeviceType CurrentDevice => _lastDevice?.GetDeviceType() ?? DeviceType.Keyboard;

    public static void SetupEvents()
    {
        InputSystem.onEvent += OnEvent;
        InputSystem.onDeviceChange += InputSystemOnonDeviceChange;
    }

    private static void InputSystemOnonDeviceChange(InputDevice device, InputDeviceChange deviceChange)
    {
        if (_lastDevice == device) return;
        _lastDevice = device;
        OnDeviceChange?.Invoke(device.GetDeviceType());
    }

    private static void OnEvent(InputEventPtr eventPtr, InputDevice device)
    {
        if (device == _lastDevice) return;
        var eventType = eventPtr.type;
        if (eventType == StateEvent.Type)
            // PS4 controller likes to spam events so this filters them out
            if (!eventPtr.EnumerateChangedControls(device, 0.0001f).Any())
                return;

        _lastDevice = device;
        OnDeviceChange?.Invoke(device.GetDeviceType());
    }
}

public enum DeviceType
{
    Keyboard,
    Gamepad
}

