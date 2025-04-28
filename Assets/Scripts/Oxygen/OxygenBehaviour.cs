using System;
using UnityEngine;
using UnityEngine.UI;

public class OxygenBehaviour : MonoBehaviour
{
    [SerializeField] public bool InOxygenPocket;
    [SerializeField] private double oxygen;
    // ReSharper disable FieldCanBeMadeReadOnly.Local
    // ReSharper disable ConvertToConstant.Local
    [SerializeField] private double maxOxygen = 100;
    [SerializeField] private double oxygenRegenerationRate = 10;
    [SerializeField] private double oxygenDepletionRate = .5f;
    // ReSharper restore ConvertToConstant.Local
    // ReSharper restore FieldCanBeMadeReadOnly.Local

    [SerializeField] private Image oxygenBar;
    [SerializeField] private RectTransform oxygenBarParent;
    private float maxSize = 0;

    private void Start()
    {
        oxygen = maxOxygen;
        maxSize = oxygenBar.rectTransform.sizeDelta.x;
    }

    private void FixedUpdate()
    {
        if (InOxygenPocket)
        {
            oxygen += oxygenRegenerationRate * Time.fixedDeltaTime;
        }
        else
        {
            oxygen -= oxygenDepletionRate * Time.fixedDeltaTime;
        }
        oxygen = Math.Clamp(oxygen, 0, maxOxygen);

        var barSize = (float)(oxygen / maxOxygen * maxSize);
        oxygenBar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, barSize);
        oxygenBar.rectTransform.SetLocalPositionAndRotation(new Vector3(barSize / 2 - maxSize / 2, 0, 0), Quaternion.identity);
    }

    public double GetOxygen() => oxygen;
}
