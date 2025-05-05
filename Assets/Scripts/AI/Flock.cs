using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;

public class Flock : MonoBehaviour 
{
    float speed;
    bool turning = false;
    public FlockManager manager;
    
    void Start ()
    {
        speed = Random.Range(manager.minSpeed, manager.maxSpeed);
    }

    void Update ()
    {
        Bounds b = new Bounds(manager.transform.position, manager.swimLimits * 2);

        if (!b.Contains(transform.position))
        {
            turning = true;
        }
        else
            turning = false;

        if (turning)
        {
            Vector3 direction = manager.transform.position - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction),
                manager.rotationSpeed * Time.deltaTime);
        }
        else
        {
            if (Random.Range(0, 100) < 10)
            {
                speed = Random.Range(manager.minSpeed, manager.maxSpeed);
            }

            if (Random.Range(0, 100) < 10)
            {
                ApplyRules();
            }
        }

        this.transform.Translate(0, 0, speed * Time.deltaTime);
    }

    void ApplyRules()
    {
        GameObject[] gos;
        gos = manager.allFish;

        Vector3 vCenter = Vector3.zero;
        Vector3 vAvoid = Vector3.zero;
        float gSpeed = 0.01f;
        float nDistance;
        int groupSize = 0;

        foreach (GameObject go in gos)
        {
            if (go != this.gameObject)
            {
                nDistance = Vector3.Distance(go.transform.position, this.transform.position);
                if (nDistance <= manager.neighbourDistance)
                {
                    vCenter += go.transform.position;
                    groupSize++;

                    if (nDistance < 1.0f)
                    {
                        vAvoid += vAvoid + (this.transform.position - go.transform.position);
                    }

                    Flock anotherFLock = go.GetComponent< Flock>();
                    gSpeed = gSpeed + anotherFLock.speed;
                }
            }
        }

        if (groupSize > 0)
        {
            vCenter = vCenter / groupSize + (manager.goalPos - this.transform.position);
            speed = gSpeed / groupSize;
            if (speed > manager.maxSpeed)
            {
                speed = manager.maxSpeed;
            }

            Vector3 direction = (vCenter + vAvoid) - transform.position;
            if (direction != Vector3.zero) 
                transform.rotation = Quaternion.Slerp(transform.rotation,
                    Quaternion.LookRotation(direction),
                    manager.rotationSpeed * Time.deltaTime);
        }
    }
}
