using UnityEngine;

public class SceneManagement : MonoBehaviour
{

public void LoadGame()
    {
        SceneManagement.LoadScene(1);
    }


    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Quit"); // Only shows in Editor
    }

}
