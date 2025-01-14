using UnityEngine;

public class Breath : MonoBehaviour
{
    public float breath { get; private set; }
    [SerializeField] private float maxBreath = 100;
    [SerializeField] private float breathRegenerationRate = 10;
    [SerializeField] private float breathDepletionRate = .5f;

    public void RegainBreath()
    {
        
    }
}
