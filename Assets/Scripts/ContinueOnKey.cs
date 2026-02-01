using UnityEngine;
using UnityEngine.SceneManagement;

public class ContinueOnKey : MonoBehaviour
{
    public string sceneToLoad;
    public KeyCode continueKey = KeyCode.X;

    void Update()
    {
        if (Input.GetKeyDown(continueKey))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
