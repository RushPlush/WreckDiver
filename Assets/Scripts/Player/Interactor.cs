using System;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class Interactor : MonoBehaviour
    {
        [SerializeField]List<IInteractable> interactables      = new List<IInteractable>();
        List<Transform> interactableTransforms = new List<Transform>();
        List<Collider> interactableColliders   = new List<Collider>();
        private int interactionIndex = -1;
        [SerializeField][Tooltip("If empty, will use it's own transform")] private Transform raySource;
        [SerializeField] private float maxInteractionDistance = 5;
        [SerializeField] private LayerMask interactableLayer;
        [SerializeField] private bool useClosest = true; 
        [SerializeField][Tooltip("How much it weighs centered interactable objects")] private float DotWeight = 1;
        private float dotMultiplier = 10;
        [SerializeField][Tooltip("How much it weighs close interactable objects")] private float DistanceWeight = 1;

        private void Start()
        {
            if(raySource == null)
            {
                raySource = transform;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            MonoBehaviour[] allScripts = other.GetComponents<MonoBehaviour>();
            if(allScripts.Length == 0) return;
            //List<IInteractable> interactable = new List<IInteractable>();
            for (int i = 0; i < allScripts.Length; i++)
            {
                if (allScripts[i] is not IInteractable) continue;
                interactables.Add((IInteractable)allScripts[i]);
                interactableTransforms.Add(allScripts[i].transform);
                interactableColliders.Add(other);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            // find the index of the collider in the list
            if(interactableColliders.Count == 0) return;
            var index = interactableColliders.FindIndex(collider => collider == other);
            interactionIndex = -1;
            interactables.Remove(interactables[index]);
            interactableColliders.Remove(other);
            interactableTransforms.Remove(other.transform);
        }

        private void FixedUpdate()
        {
            interactionIndex = -1;
            if(interactables.Count == 0) return;
            RaycastHit hit;
            List<float> dots = new List<float>();
            List<float> distances = new List<float>();
            for (int i = 0; i < interactables.Count; i++)
            {
                var direction = interactableTransforms[i].position - raySource.position;

                if (!Physics.Raycast(raySource.position, direction, out hit, maxInteractionDistance, interactableLayer)) continue;
                //Get the dot product of the direction and the forward vector of the player, then keep the interactable that is most in front of the player
                if (hit.collider != interactableColliders[i]) continue;
                distances.Add(hit.distance);
                dots.Add(Vector3.Dot(hit.point, raySource.position));
            }
            if(dots.Count == 0) return;
            int closestIndex = -1;
            int lowestDotIndex = -1;
            float lowestDot = 1;
            float lowestDistance = maxInteractionDistance;
            for (int i = 0; i < dots.Count; i++) // dots and distances have the same length
            {
                //check for the closest interactable
                if(dots[i] < lowestDot)
                {
                    lowestDot = dots[i];
                    lowestDotIndex = i;
                }
                if(distances[i] < lowestDistance)
                {
                    lowestDistance = distances[i];
                    closestIndex = i;
                }
            }
            if(useClosest)
            {
                interactionIndex = closestIndex;
            }
            else
            {
                interactionIndex = lowestDotIndex;
            }

            for (int i = 0; i < interactables.Count; i++)
            {
                if(i == interactionIndex)
                {
                    interactables[i].Highlight();
                }
                else
                {
                    interactables[i].Unhighlight();
                }
            }
        }
        public void Interact()
        {
            if(interactionIndex == -1) return;
            interactables[interactionIndex].Interact();
        }
        private void OnTriggerStay(Collider other)
        {
            //throw new NotImplementedException();
        }
    }
}