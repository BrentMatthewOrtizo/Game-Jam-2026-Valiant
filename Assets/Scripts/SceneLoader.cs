using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private string sceneToLoad = "TestScene";

    public void LoadScene()
    {
        Debug.Log("SceneLoader: Loading scene -> " + sceneToLoad);
        SceneManager.LoadScene(sceneToLoad);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}