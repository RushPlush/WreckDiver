using System;
using UnityEngine;

public class Breath : MonoBehaviour
{
    private bool isInAir=false;
    public float breath { get; private set; }
    [SerializeField] private float maxBreath = 100;
    [SerializeField] private float breathRegenerationRate = 10;
    [SerializeField] private float breathDepletionRate = .5f;

    private void Start()
    {
    }

    public void ReportAirState(bool isInsideOfAir)
    {
    }
    private void FixedUpdate()
    {
    }

    private void RegainBreath()
    {
    }

    private void DepleteBreathPerSec()
    {
    }
    public void DepleteBreath(float depletionAmount)
    {
    }
    private void BreathDepleted()
    {
    }
}
