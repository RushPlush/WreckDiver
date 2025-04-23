using System;
using UnityEngine;

public class OxygenBehaviour : MonoBehaviour
{
    [SerializeField] public bool InOxygenPocket;
    [SerializeField] private double oxygen;
    // ReSharper disable FieldCanBeMadeReadOnly.Local
    // ReSharper disable ConvertToConstant.Local
    [SerializeField][Tooltip("In seconds")] private double maxOxygen = 300;
    [SerializeField] private double oxygenRegenerationRate = 30;
    //[SerializeField] private double oxygenDepletionRate = .5f;
    // ReSharper restore ConvertToConstant.Local
    // ReSharper restore FieldCanBeMadeReadOnly.Local
    [SerializeField] private float timeToDie = 10f;
    private float timeToDieCounter = 0f;
    SaveAndLoad saveAndLoad;
    Movement movement;
    private void Awake()
    {
        saveAndLoad = GetComponent<SaveAndLoad>();
        movement = GetComponent<Movement>();
    }
    private void Start()
    {
        oxygen = maxOxygen;
    }

    private void FixedUpdate()
    {
        if(!movement.enabled) return;
        if (InOxygenPocket)
        {
            oxygen += oxygenRegenerationRate * Time.fixedDeltaTime;
        }
        else
        {
            oxygen -= Time.fixedDeltaTime;
        }
        oxygen = Math.Clamp(oxygen, 0, maxOxygen);
        if (oxygen < 0.01)
        {
            timeToDieCounter += Time.fixedDeltaTime;
            // progress 0 to 1
            var progress = timeToDieCounter / timeToDie;
            //todo add screen effect, sound effects etc here
            
            if (timeToDieCounter >= timeToDie)
            {
                if (saveAndLoad != null)
                {
                    saveAndLoad.Death();
                }
            }
        }
        else
        {
            timeToDieCounter = 0f;
        }
        // Debug.Log($"Oxygen: {oxygen}");
    }

    public double GetOxygen() => oxygen;
    public void Reset()
    {
        oxygen = maxOxygen;
    }
}
