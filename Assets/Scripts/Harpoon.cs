using System;
using UnityEngine;

public class Harpoon : MonoBehaviour
{
    private int indexValue;
    public bool isHarpoonActive { get; private set; } = false;
    [SerializeField] private LayerMask afterImpactLayer;
    [SerializeField] private float speed = 10f;
    [SerializeField] private Vector3 initialScale;
    public GameObject connectionPoint { get; private set; }
    public bool canPull { get; private set; } = true;
    Rigidbody rb;
    HarpoonBehaviour harpoonBehaviour;
    Pullable pullable;
    ParticleSystem particleSystem;
    bool Destroyed = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        particleSystem = GetComponentInChildren<ParticleSystem>();
        transform.localScale = initialScale;
    }

    private void Start()
    {
        connectionPoint = transform.GetChild(1).gameObject;
    }

    public void SetHarpoonBehaviour(HarpoonBehaviour harpoonBehaviour)
    {
        this.harpoonBehaviour = harpoonBehaviour;
    }

    public void SetIndexValue(int index)
    {
        indexValue = index;
    }

    public void Shoot()
    {
        isHarpoonActive = true;
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.AddForce(-transform.up * speed, ForceMode.Impulse);
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        particleSystem.Play();
    }

    private void FixedUpdate()
    {
        if (!isHarpoonActive) return;
        particleSystem.emissionRate = rb.linearVelocity.magnitude * 10f;
        //transform.Translate(Vector3.forward * speed);
    }

    int GetLayerNumber(LayerMask layerMaskIn)
    {
        int layerNumber = -1;
        int layer = layerMaskIn.value;
        while (layer > 0)
        {
            layer = layer >> 1;
            layerNumber++;
        }
        return layerNumber;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!isHarpoonActive) return;
        transform.parent = other.transform;
        isHarpoonActive = false;
        gameObject.layer = GetLayerNumber(afterImpactLayer);
        rb.isKinematic = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
        rb.interpolation = RigidbodyInterpolation.None;
        
        particleSystem.emissionRate = 4f;

        MonoBehaviour[] allScripts = transform.parent.GetComponents<MonoBehaviour>();
        if (allScripts.Length == 0) return;
        for (int i = 0; i < allScripts.Length; i++)
        {
            if (allScripts[i] is not Pullable) continue;
            pullable = (Pullable)allScripts[i];
            harpoonBehaviour.SetPullable(indexValue, pullable);
        }
    }
    
    private void OnDestroy()
    {
        print(gameObject.name + " Destroyed");
        Destroyed = true;
        if (harpoonBehaviour != null)
            harpoonBehaviour.DestroyHarpoon(indexValue);
        if (pullable != null)
            pullable.Release();
    }
    public void Reset()
    {
        if(!isHarpoonActive) return;
        Destroy(this.gameObject);
    }
    public void SetCanPull(bool canPull)
    {
        
        this.canPull = canPull;
    }
    
}
