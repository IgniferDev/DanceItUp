using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SongSelectMenu : MonoBehaviour
{
    [Header("UI del Menú")]
    public TextMeshProUGUI songTitleText;
    public TextMeshProUGUI artistText;
    public Button playButton;

    [Header("Audio Preview")]
    public AudioSource previewAudioSource; // Un AudioSource que pondremos en la escena

    private SongChart currentSelectedChart;

    void Start()
    {
        // Desactivamos el botón de Jugar hasta que elija una canción
        playButton.interactable = false;
        songTitleText.text = "Selecciona una pista";
        artistText.text = "";
    }

    // Esta función la llamarán los botones de cada canción
    public void OnSongButtonClicked(SongChart chartClicked)
    {
        currentSelectedChart = chartClicked;
        
        // Actualizamos los textos
        songTitleText.text = currentSelectedChart.songName;
        artistText.text = currentSelectedChart.artist;

        // Reproducimos la vista previa de audio
        previewAudioSource.Stop();
        previewAudioSource.clip = currentSelectedChart.audioClip;
        previewAudioSource.time = currentSelectedChart.previewStartTime;
        previewAudioSource.Play();

        // Activamos el botón de jugar
        playButton.interactable = true;
    }

    // Esta función la llamará el gran botón de "JUGAR"
    public void GoToGameScene()
    {
        if (currentSelectedChart != null)
        {
            // Guardamos la canción en el puente
            GameManager.selectedSong = currentSelectedChart;
            // Cargamos la escena del juego (Asegúrate de que se llame exactamente así)
            SceneManager.LoadScene("GameScene"); 
        }
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}