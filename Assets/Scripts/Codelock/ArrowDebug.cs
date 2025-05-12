using System;
using UnityEditor;
using UnityEngine;

public class ArrowDebug : MonoBehaviour
{
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.color = Color.red;
        Handles.ArrowHandleCap(0, transform.position, Quaternion.LookRotation(transform.forward), 0.1f, EventType.Repaint);
        Handles.ArrowHandleCap(0, transform.position, Quaternion.LookRotation(transform.up), 0.1f, EventType.Repaint);
    }
    #endif
}