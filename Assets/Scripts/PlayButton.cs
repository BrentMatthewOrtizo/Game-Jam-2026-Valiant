using UnityEngine;

public class PlayButton : MonoBehaviour
{
    [SerializeField] private SceneLoader sceneLoader;

    public void OnClick()
    {
        Debug.Log("PlayButton: OnClickPlay fired!");

        if (sceneLoader != null)
            sceneLoader.LoadScene();
        else
            Debug.LogError("PlayButton: sceneLoader is not assigned.");
    }
}