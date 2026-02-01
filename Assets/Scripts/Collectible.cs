using UnityEngine;

public class Collectible : MonoBehaviour
{
    public string itemID;
    public int uiID;
    public CollectibleUIManager uiManager;

    [Header("Linked Mask")]
    public Pickupable sourcePickupable;

    [Header("References")]
    public PlayerPickup playerPickup;

    public void Collect()
    {
        Debug.Log("Collected: " + itemID);

        // UI update
        if (uiManager != null)
            uiManager.Collect(uiID);

        // Clear mask effects if this mask is currently applied
        if (playerPickup != null && playerPickup.appliedMask != null)
        {
            if (playerPickup.appliedMask == sourcePickupable)
            {
                playerPickup.effectManager.ClearMask(playerPickup.appliedMask.overlayImage);
                playerPickup.appliedMask = null;
            }
        }

        // Permanently remove the mask object if it exists
        if (sourcePickupable != null)
            Destroy(sourcePickupable.gameObject);

        // Disable the collectible itself
        gameObject.SetActive(false);
    }



}
