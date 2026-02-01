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
    //public Vector3 dropOffset = new Vector3(0f, -0.5f, 1.5f);
    public float minDropY = 1f;
    public Transform dropPoint;

    [Header("Mask Effects")]
    public MaskEffectManager effectManager;

    [Header("Pickup Audio")]
    public AudioSource sfxSource;
    public AudioClip maskPickupClip;

    // Currently held pickupable (for moving around / dropping)
    private Pickupable heldObject;
    private Collider playerCollider;
    private GameObject currentTarget;


    // Currently applied mask for effects
    [HideInInspector] // optional, hides it in Inspector
    public Pickupable appliedMask;

    void Awake()
    {
        playerCollider = GetComponent<Collider>();
    }

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

        Vector3 dropPos = dropPoint.position;

        // Safety clamp
        if (dropPos.y < minDropY)
            dropPos.y = minDropY;


        if (appliedMask != null)
        {
            effectManager.ClearMask(appliedMask.overlayImage);
            appliedMask = null;
        }

        heldObject.OnDrop(dropPos);

        heldObject.IgnorePlayerCollision(playerCollider, 0.25f);

        Debug.Log("Dropped: " + heldObject.name);
        heldObject = null;
    }

    void CheckForInteractable()
    {
        GameObject newTarget = null;

        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, pickupDistance))
        {
            if (hit.collider.GetComponent<Pickupable>() != null ||
                hit.collider.GetComponent<Collectible>() != null)
            {
                newTarget = hit.collider.gameObject;
            }
        }

        // Only update UI if target changed
        if (newTarget == currentTarget)
            return;

        currentTarget = newTarget;

        // Hide all prompts
        pickupPrompt.SetActive(false);
        holdingPrompt.SetActive(false);
        collectiblePrompt.SetActive(false);
        removePrompt.SetActive(false);

        if (currentTarget == null)
        {
            if (heldObject != null)
                removePrompt.SetActive(true);
            return;
        }

        Pickupable pickupable = currentTarget.GetComponent<Pickupable>();
        if (pickupable != null)
        {
            if (heldObject != null)
                holdingPrompt.SetActive(true);
            else
                pickupPrompt.SetActive(true);
            return;
        }

        Collectible collectible = currentTarget.GetComponent<Collectible>();
        if (collectible != null)
        {
            collectiblePrompt.SetActive(true);
            return;
        }
    }


    void OnDrawGizmos()
    {
        if (dropPoint == null) return;

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(dropPoint.position, 0.1f);
    }

}
