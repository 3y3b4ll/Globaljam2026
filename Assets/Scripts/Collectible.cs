using UnityEngine;

public class Collectible : MonoBehaviour
{
    public string itemID;
    public Pickupable sourcePickupable;

    public void Collect()
    {
        Debug.Log("Collected: " + itemID);

        if (sourcePickupable != null)
            sourcePickupable.PermanentlyRemove();

        gameObject.SetActive(false);
    }
}
