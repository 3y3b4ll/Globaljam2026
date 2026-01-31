using UnityEngine.UI; // for Text
using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    public float pickupDistance = 3f;
    public Transform cameraTransform;

    [Header("UI Prompts")]
    public GameObject pickupPrompt;    // "Press E to pick up"
    public GameObject holdingPrompt;   // "It's hard to wear multiple masks at once"
    public GameObject collectiblePrompt; // "Press E to collect"
    public GameObject removePrompt;      // "Press Q to remove/drop the mask"


    private Pickupable heldObject;

    void Update()
    {
        CheckForInteractable();

        if (Input.GetKeyDown(KeyCode.E))
            TryPickup();

        if (Input.GetKeyDown(KeyCode.Q))
            Drop();
    }

    void TryPickup()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, pickupDistance))
        {
            // Normal pickup object
            if (heldObject == null &&
                hit.collider.TryGetComponent(out Pickupable pickupable))
            {
                heldObject = pickupable;
                pickupable.OnPickup();
                Debug.Log("Picked up: " + pickupable.name);
            }

            // Collectible object
            else if (hit.collider.TryGetComponent(out Collectible collectible))
            {
                collectible.Collect();
            }
        }
    }


    void Drop()
    {
        if (heldObject == null) return;

        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);

        Vector3 dropPos;

        // If we hit something, drop at hit point
        if (Physics.Raycast(ray, out RaycastHit hit, pickupDistance))
        {
            dropPos = hit.point;
        }
        else
        {
            // Otherwise drop at max distance
            dropPos = cameraTransform.position +
                      cameraTransform.forward * pickupDistance;
        }

        heldObject.OnDrop(dropPos);

        Debug.Log("Dropped: " + heldObject.name);
        heldObject = null;
    }

    void CheckForInteractable()
    {
        // Hide all prompts by default
        pickupPrompt.SetActive(false);
        holdingPrompt.SetActive(false);
        collectiblePrompt.SetActive(false);
        removePrompt.SetActive(false);

        // Optionally, remove any highlights here if using glow/halo

        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, pickupDistance))
        {
            Pickupable pickupable = hit.collider.GetComponent<Pickupable>();
            if (pickupable != null)
            {
                if (heldObject != null) // already holding a pickupable
                {
                    holdingPrompt.SetActive(true);
                }
                else
                {
                    pickupPrompt.SetActive(true);
                }
                return;
            }

            Collectible collectible = hit.collider.GetComponent<Collectible>();
            if (collectible != null)
            {
                collectiblePrompt.SetActive(true);
                return;
            }
        }

        // Show RemovePrompt whenever holding a pickupable, even if looking at nothing
        if (heldObject != null)
        {
            removePrompt.SetActive(true);
        }
    }


}
