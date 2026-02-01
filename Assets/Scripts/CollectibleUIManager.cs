using UnityEngine;

public class CollectibleUIManager : MonoBehaviour
{
    [System.Serializable]
    public class IconPair
    {
        public GameObject greyIcon;
        public GameObject colorIcon;
        public bool collected;
    }

    public IconPair[] collectibles;

    [Header("Reward")]
    public GameObject rewardObject; // object enabled when all collected

    int collectedCount = 0;

    void Start()
    {
        // Ensure all start grey
        foreach (var c in collectibles)
        {
            c.greyIcon.SetActive(true);
            c.colorIcon.SetActive(false);
            c.collected = false;
        }

        if (rewardObject != null)
            rewardObject.SetActive(false);
    }

    public void Collect(int id)
    {
        if (id < 0 || id >= collectibles.Length) return;

        var c = collectibles[id];

        if (c.collected) return;

        c.collected = true;

        c.greyIcon.SetActive(false);
        c.colorIcon.SetActive(true);

        collectedCount++;

        Debug.Log("UI Collected: " + id);

        if (collectedCount == collectibles.Length)
        {
            Debug.Log("All collected!");
            if (rewardObject != null)
                rewardObject.SetActive(true);
        }
    }
}
