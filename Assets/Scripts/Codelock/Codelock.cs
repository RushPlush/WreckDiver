using UnityEngine;

public class Codelock : MonoBehaviour
{
    [SerializeField] private Lockwheel[] lockwheels;
    [SerializeField] private int[] code;
    [SerializeField] private int[] hardCode;
    [SerializeField] private MonoBehaviour unlockable;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (unlockable is not IUnlockable _)
            throw new System.Exception("Unlockable must implement IUnlockable");

        // get all lockwheels for this component
        lockwheels = GetComponentsInChildren<Lockwheel>();
        // generate random code
        code = new int[lockwheels.Length];
        if (hardCode.Length == code.Length)
        {
            code = hardCode;

            // Generate random face for each lockwheel that is not the code face required
            for (int i = 0; i < lockwheels.Length; i++)
            {
                var randomFace = Random.Range(0, 9);
                while (randomFace == code[i])
                {
                    randomFace = Random.Range(0, 9);
                }

                lockwheels[i].SetCodeFace(randomFace);
            }
        }
        else
            for (int i = 0; i < lockwheels.Length; i++)
            {
                code[i] = Random.Range(0, 9);

                // Generate random face for each lockwheel that is not the code face required
                var randomFace = Random.Range(0, 9);
                while (randomFace == code[i])
                {
                    randomFace = Random.Range(0, 9);
                }
                lockwheels[i].SetCodeFace(randomFace);
            }
    }

    // Update is called once per frame
    void Update()
    {
        // check if code face matches code for all lockwheels
        for (int i = 0; i < lockwheels.Length; i++)
        {
            if (lockwheels[i].GetCodeFace() != code[i])
            {
                return;
            }
        }
        // if code matches, open lock
        (unlockable as IUnlockable)!.Unlock();

        // stop updating
        enabled = false;
    }
}
