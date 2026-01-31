using UnityEngine;
using UnityEngine.Rendering;

public class MaskEffectManager : MonoBehaviour
{
    public Volume globalVolume;
    public VolumeProfile defaultProfile;

    public void ApplyMask(VolumeProfile profile, GameObject overlay)
    {
        globalVolume.profile = profile;

        if (overlay != null)
            overlay.SetActive(true);
    }

    public void ClearMask(GameObject overlay)
    {
        globalVolume.profile = defaultProfile;

        if (overlay != null)
            overlay.SetActive(false);
    }
}
