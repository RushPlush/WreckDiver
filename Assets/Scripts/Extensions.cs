using OpenCover.Framework.Model;using UnityEngine;
using UnityEngine.InputSystem;

public static class QuaternionExtensions
{
    /// <summary>
    /// Calculates the distance squared between two quaternions
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static float Distance2(this Quaternion a, Quaternion b)
    {
        var sum = a.eulerAngles - b.eulerAngles;
        sum.x *= sum.x;
        sum.y *= sum.y;
        sum.z *= sum.z;
        sum.x += sum.y;
        sum.x += sum.z;
        return sum.x;
    }

    /// <summary>
    /// Calculates the distance between two quaternions
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static float Distance(this Quaternion a, Quaternion b)
    {
        return Mathf.Sqrt(a.Distance2(b));
    }
}

public static class TransformExtensions
{
    /// <summary>
    /// Linearly interpolates between two transforms
    /// </summary>
    /// <param name="a">The start transform</param>
    /// <param name="b">The end transform</param>
    /// <param name="t">Delta</param>
    /// <returns><see cref="Transform"/></returns>
    public static Transform Lerp(this Transform a, Transform b, float t)
    {
        a.localPosition = Vector3.Lerp(a.localPosition, b.localPosition, t);
        a.localRotation = Quaternion.Lerp(a.localRotation, b.localRotation, t);
        a.localScale = Vector3.Lerp(a.localScale, b.localScale, t);
        return a;
    }

    /// <summary>
    /// Spherically interpolates between two transforms
    /// </summary>
    /// <param name="a">The start transform</param>
    /// <param name="b">The end transform</param>
    /// <param name="t">Delta</param>
    /// <returns><see cref="Transform"/></returns>
    public static Transform Slerp(this Transform a, Transform b, float t)
    {
        a.localPosition = Vector3.Slerp(a.localPosition, b.localPosition, t);
        a.localRotation = Quaternion.Slerp(a.localRotation, b.localRotation, t);
        a.localScale = Vector3.Slerp(a.localScale, b.localScale, t);
        return a;
    }
}

public static class InputDeviceExtensions
{
    public static DeviceType GetDeviceType(this InputDevice device) => device switch
    {
        Gamepad => DeviceType.Gamepad,
        Keyboard => DeviceType.Keyboard,
        _ => DeviceType.Keyboard
    };
}