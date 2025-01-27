using System;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class Interactor : MonoBehaviour
    {
        List<IInteractable> interactables      = new List<IInteractable>();
        List<Transform> interactableTransforms = new List<Transform>();
        List<Collider> interactableColliders   = new List<Collider>();
        [SerializeField] private Transform raySource;
        private void OnTriggerEnter(Collider other)
        {
            MonoBehaviour[] allScripts = other.GetComponents<MonoBehaviour>();
            if(allScripts.Length == 0) return;
            //List<IInteractable> interactable = new List<IInteractable>();
            for (int i = 0; i < allScripts.Length; i++)
            {
                if (allScripts[i] is IInteractable)
                {
                    interactables.Add((IInteractable)allScripts[i]);
                    interactableTransforms.Add(allScripts[i].transform);
                    interactableColliders.Add(other);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var indexes = interactableColliders.FindAll(coll => coll == other);
            interactableColliders.Remove(other);
        }

        private void FixedUpdate()
        {
            if(interactables.Count == 0) return;
            RaycastHit hit;
            for (int i = 0; i < interactables.Count; i++)
            {
                var direction = interactableTransforms[i].position - raySource.position;
                if (Physics.Raycast(raySource.position, direction, out hit, 5))
                {
                    if (hit.collider == interactableColliders[i])
                    {
                        interactables[i].Highlight();
                    }
                    else
                    {
                        interactables[i].Unhighlight();
                    }
                }
            }
        }
        public void Interact()
        {
            
        }
        private void OnTriggerStay(Collider other)
        {
            throw new NotImplementedException();
        }
    }
}