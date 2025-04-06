using System;
using UnityEngine;

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

    private void Start()
    {
        oxygen = maxOxygen;
    }

    private void FixedUpdate()
    {
        if (InOxygenPocket)
        {
            oxygen += oxygenRegenerationRate * Time.deltaTime;
        }
        else
        {
            oxygen -= oxygenDepletionRate * Time.deltaTime;
        }
        oxygen = Math.Clamp(oxygen, 0, maxOxygen);
        Debug.Log($"Oxygen: {oxygen}");
    }

    public double GetOxygen() => oxygen;
}
