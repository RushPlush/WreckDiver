using System;
using UnityEngine;
using UnityEngine.UI;

public class OxygenBehaviour : MonoBehaviour
{
    [SerializeField] public bool InOxygenPocket;
    [SerializeField] private double oxygen;
    // ReSharper disable FieldCanBeMadeReadOnly.Local
    // ReSharper disable ConvertToConstant.Local
    [SerializeField][Tooltip("In seconds")] private double maxOxygen = 300;
    [SerializeField] private double oxygenRegenerationRate = 30;
    [SerializeField][Tooltip("Multiplier for how fast oxygen runs out, Default/neutral = 1")] private double oxygenDepletionRate = 1f;
    // ReSharper restore ConvertToConstant.Local
    // ReSharper restore FieldCanBeMadeReadOnly.Local

    [SerializeField] private Image oxygenBar;
    [SerializeField] private RectTransform oxygenBarParent;
    private float maxSize = 0;

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
        maxSize = oxygenBar.rectTransform.sizeDelta.x;
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
            oxygen -= oxygenDepletionRate * Time.fixedDeltaTime;
        }
        oxygen = Math.Clamp(oxygen, 0, maxOxygen);
        if (oxygen < 0.01)
        {
            timeToDieCounter += Time.fixedDeltaTime;
            // progress 0 to 1
            var progress = timeToDieCounter / timeToDie;
            //todo add screen effect, sound effects etc here, can use progress as a multiplier
            
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

        var barSize = (float)(oxygen / maxOxygen * maxSize);
        oxygenBar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, barSize);
        oxygenBar.rectTransform.SetLocalPositionAndRotation(new Vector3(barSize / 2 - maxSize / 2, 0, 0), Quaternion.identity);
    }
    public void LoseOxygen(double amount) 
    {
        oxygen = Math.Clamp(oxygen - amount, 0, maxOxygen);
    }
    public void SetDepletionRate(double depletionRate)
    {
        oxygenDepletionRate = depletionRate;
    }
    public double GetOxygen() => oxygen;
    public void Reset()
    {
        oxygen = maxOxygen;
    }
}
