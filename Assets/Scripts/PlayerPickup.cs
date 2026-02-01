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

    [Header("Drop Settings")]
    public Vector3 dropOffset = new Vector3(0f, -0.5f, 1.5f);

    [Header("Mask Effects")]
    public MaskEffectManager effectManager;

    [Header("Pickup Audio")]
    public AudioSource sfxSource;
    public AudioClip maskPickupClip;

    // Currently held pickupable (for moving around / dropping)
    private Pickupable heldObject;
    // Currently applied mask for effects
    [HideInInspector] // optional, hides it in Inspector
    public Pickupable appliedMask;

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
            // Normal pickup object (mask)
            if (heldObject == null &&
                hit.collider.TryGetComponent(out Pickupable pickupable))
            {
                //play audio
                if (maskPickupClip)
                {
                    sfxSource.PlayOneShot(maskPickupClip);
                }

                heldObject = pickupable;
                pickupable.OnPickup();

                // Apply mask effects and store appliedMask separately
                appliedMask = pickupable;
                effectManager.ApplyMask(
                    appliedMask.maskProfile,
                    appliedMask.overlayImage
                );

                Debug.Log("Picked up: " + pickupable.name);
            }

            // Collectible object
            else if (hit.collider.TryGetComponent(out Collectible collectible))
            {
                // play audio
                if (collectible.pickupClip)
                {
                    sfxSource.PlayOneShot(collectible.pickupClip);
                }


                collectible.Collect();

                // If collectible is linked to a mask, clear its effects
                if (appliedMask != null && appliedMask == collectible.sourcePickupable)
                {
                    effectManager.ClearMask(appliedMask.overlayImage);
                    appliedMask = null;
                }
            }
        }
    }



    void Drop()
    {
        if (heldObject == null) return;

        // Drop relative to player
        Vector3 dropPos =
            transform.position +
            transform.right * dropOffset.x +
            transform.up * dropOffset.y +
            transform.forward * dropOffset.z;

        // Remove mask effects using appliedMask
        if (appliedMask != null)
        {
            effectManager.ClearMask(appliedMask.overlayImage);
            appliedMask = null;
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
