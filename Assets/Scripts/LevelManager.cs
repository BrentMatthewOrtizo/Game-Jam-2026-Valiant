using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GameObject[] levels;

    private GameObject _currentLevel;
    private int _currentIndex = 0;

    void Start()
    {
        LoadLevel(0);
    }

    public void LoadNextLevel()
    {
        _currentIndex++;

        if (_currentIndex >= levels.Length)
        {
            _currentIndex = 0; // or win screen
        }

        LoadLevel(_currentIndex);
    }

    void LoadLevel(int index)
    {
        if (_currentLevel != null)
        {
            Destroy(_currentLevel);
        }

        _currentLevel = Instantiate(levels[index]);
    }
}