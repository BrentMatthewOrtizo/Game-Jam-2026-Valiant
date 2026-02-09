using UnityEngine;

public class PlayButton : MonoBehaviour
{
    [SerializeField] private SceneLoader sceneLoader;

    public void OnClickPlay()
    {
        Debug.Log("PlayButton: OnClickPlay fired!");

        if (sceneLoader != null)
            sceneLoader.LoadScene();
        else
            Debug.LogError("PlayButton: sceneLoader is not assigned.");
    }
}