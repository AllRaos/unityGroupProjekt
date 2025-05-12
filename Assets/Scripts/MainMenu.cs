using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Slider soundSlider; 

    private void Start()
    {      
        soundSlider.value = PlayerPrefs.GetFloat("GameVolume", 0.5f);
        UpdateVolume(soundSlider.value);
    }

    public void StartNewGame()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ExitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; 
#endif
    }

    public void UpdateVolume(float volume)
    {
        PlayerPrefs.SetFloat("GameVolume", volume);
        PlayerPrefs.Save();
        AudioListener.volume = volume;
    }
}