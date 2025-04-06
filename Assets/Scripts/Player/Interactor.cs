using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Player
{
    public class Interactor : MonoBehaviour
    {
        [SerializeField] private GameObject player;
        List<IInteractable> interactables      = new List<IInteractable>();
        List<Transform> interactableTransforms = new List<Transform>();
        List<Collider> interactableColliders   = new List<Collider>();
        //int interactableSize = 0;
        private int interactionIndex = -1;
        [SerializeField][Tooltip("If empty, will use it's own transform")] private Transform raySource;
        [SerializeField] private float maxInteractionDistance = 5;
        [SerializeField][Tooltip("All layers that the raycast can hit. Has to include Interactables or will not work. And all layers that should stop the player from interacting with them, such as walls etc.")] 
        private LayerMask raycastLayers;
        [SerializeField] private bool useClosest = true; 
        [SerializeField][Tooltip("How much it weighs centered interactable objects")] private float DotWeight = 1;
        private float dotMultiplier = 10;
        [SerializeField][Tooltip("How much it weighs close interactable objects")] private float DistanceWeight = 1;
        [SerializeField]private bool ShowGizmos = true;

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
            //interactableSize = interactables.Count;
        }

        private void OnTriggerExit(Collider other)
        {
            // find the index of the collider in the list
            if(interactableColliders.Count == 0) return;
            var index = interactableColliders.FindIndex(collider => collider == other);
            if(index == -1) return;
            interactionIndex = -1;
            interactables[index].Unhighlight();
            interactables.Remove(other.GetComponent<IInteractable>());
            interactableColliders.Remove(other);
            interactableTransforms.Remove(other.transform);
            
            //interactableSize = interactables.Count;
        }
        
        List<float> dots = new List<float>();
        List<Vector3> hitPoints = new List<Vector3>();
        List<float> distances = new List<float>();
        private void FixedUpdate()
        {
            
            interactionIndex = -1;
            for (int i = interactables.Count() - 1; i >= 0; i--)
            {
                if(interactables[i].destroyed)
                {
                    print("destroyed");
                    interactables.RemoveAt(i);
                    interactableTransforms.RemoveAt(i);
                    interactableColliders.RemoveAt(i);
                }
            }
            
            if(interactables.Count == 0) return;
            RaycastHit hit;
            dots.Clear();
            hitPoints.Clear();
            distances.Clear();
            for (int i = 0; i < interactables.Count; i++)
            {
                var direction = interactableTransforms[i].position - raySource.position;
                //print($"ray{i} cast");
                if (!Physics.Raycast(raySource.position, direction, out hit, maxInteractionDistance, raycastLayers)) continue;
                //print($"ray{i} hit {hit.collider.name}");
                //Get the dot product of the direction and the forward vector of the player, then keep the interactable that is most in front of the player
                
                if (hit.collider != interactableColliders[i]) continue;
                //print($"ray{i} hit correct collider {hit.collider.name}");
                distances.Add(hit.distance);
                hitPoints.Add(hit.point);
                dots.Add(Vector3.Dot(hit.point - raySource.position, raySource.forward));
                //print(dots[i]);
            }
            if (Physics.Raycast(raySource.position, transform.forward, out hit, maxInteractionDistance, raycastLayers))
            {
                
            }
            if(dots.Count == 0) return;
            int closestIndex = -1;
            int dotIndex = -1;
            float highestDot = -1;
            float lowestDistance = maxInteractionDistance;
            for (int i = 0; i < dots.Count; i++) // dots and distances have the same length
            {
                //check for the closest interactable
                if(dots[i] > highestDot)
                {
                    highestDot = dots[i];
                    dotIndex = i;
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
                interactionIndex = dotIndex;
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
            if(interactables.Count == 0) return;
            var interactable    = interactables[interactionIndex];
            var interactionType = interactable.interactionType;
            switch (interactionType)
            {
                case IInteractable.InteractionType.FreeInteraction:
                    interactable.Interact();
                    break;
                case IInteractable.InteractionType.PlayerInteraction:
                    interactable.Interact(player); // todo not implemented
                    break;
                case IInteractable.InteractionType.ItemInteraction:
                    interactable.Interact(new Item()); // todo not implemented
                    break;
                case IInteractable.InteractionType.ItemAndPlayerInteraction:
                    interactable.Interact(player, new Item()); // todo not implemented
                    break;
                case IInteractable.InteractionType.NoInteraction:
                    interactable.InteractionFailed();
                    break;
            }
        }
        private void OnTriggerStay(Collider other)
        {
            //throw new NotImplementedException();
        }

        private void OnDrawGizmos()
        {
            if(!ShowGizmos) return;
            Gizmos.color = Color.red;
            //Gizmos.DrawRay(raySource.position, raySource.forward * maxInteractionDistance);
            //if(hitPoints.Count == 0) return;
            for (int i = 0; i < interactableTransforms.Count; i++)
            {
                Gizmos.DrawRay(raySource.position, interactableTransforms[i].position - raySource.position);
            }
        }
    }
}