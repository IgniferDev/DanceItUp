using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Paneles")]
    public GameObject tutorialPanel; // El panel oscuro de "Cómo Jugar"

    void Start()
    {
        // Nos aseguramos de que el tutorial empiece apagado y el tiempo corra normal
        Time.timeScale = 1f; 
        if (tutorialPanel != null) tutorialPanel.SetActive(false);
    }

    // Botón "Let's Dance"
    public void GoToSongSelect()
    {
        SceneManager.LoadScene("SongSelect");
    }

    // Botón "Cómo Jugar"
    public void OpenTutorial()
    {
        if (tutorialPanel != null) tutorialPanel.SetActive(true);
    }

    // Botón "Cerrar" dentro del tutorial
    public void CloseTutorial()
    {
        if (tutorialPanel != null) tutorialPanel.SetActive(false);
    }
}