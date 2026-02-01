using UnityEngine.Rendering;
using UnityEngine;

public class Pickupable : MonoBehaviour
{
    [HideInInspector] public Rigidbody rb;

    public VolumeProfile maskProfile;
    public GameObject overlayImage;

    [Header("Linked Collectible")]
    public GameObject linkedCollectible;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void OnPickup()
    {
        if (rb) rb.isKinematic = true;
        gameObject.SetActive(false);

        if (linkedCollectible != null)
            linkedCollectible.SetActive(true);
    }

    public void OnDrop(Vector3 position)
    {
        transform.position = position;
        gameObject.SetActive(true);

        if (rb)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (linkedCollectible != null)
            linkedCollectible.SetActive(false);
    }

    public void PermanentlyRemove()
    {
        Destroy(gameObject);
    }
}
