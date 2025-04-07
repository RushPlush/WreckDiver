using UnityEngine;

public class Harpoon : MonoBehaviour
{
    private int indexValue;
    private bool isHarpoonActive = false;
    [SerializeField] private LayerMask afterImpactLayer;
    [SerializeField] private float speed = 10f;
    Rigidbody rb;
    HarpoonBehaviour harpoonBehaviour;
    Pullable pullable;
    bool Destroyed = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
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
    }

    private void FixedUpdate()
    {
        if (!isHarpoonActive) return;
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
        print(GetLayerNumber(afterImpactLayer));
        gameObject.layer = GetLayerNumber(afterImpactLayer);
        print(gameObject.layer);
        rb.isKinematic = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
        rb.interpolation = RigidbodyInterpolation.None;
        MonoBehaviour[] allScripts = transform.parent.GetComponents<MonoBehaviour>();
        if (allScripts.Length == 0) return;
        for (int i = 0; i < allScripts.Length; i++)
        {
            if (allScripts[i] is not Pullable) continue;
            pullable = (Pullable)allScripts[i];
            harpoonBehaviour.SetPullable(indexValue, pullable);
        }
    }
    //todo 
    private void OnDestroy()
    {
        print(gameObject.name + " Destroyed");
        Destroyed = true;
        if (harpoonBehaviour != null)
            harpoonBehaviour.DestroyHarpoon(indexValue);
        if (pullable != null)
            pullable.Release();
    }
}
