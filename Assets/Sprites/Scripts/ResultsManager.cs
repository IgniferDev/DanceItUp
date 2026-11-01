using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ResultsManager : MonoBehaviour
{
    [Header("Textos de Resultados")]
    public TextMeshProUGUI songNameText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI maxComboText;
    public TextMeshProUGUI perfectText;
    public TextMeshProUGUI goodText;
    public TextMeshProUGUI badText;
    public TextMeshProUGUI missText;

    void Start()
    {
        // Traemos los datos de la memoria mágica (GameManager) y los pegamos en la pantalla
        if (GameManager.selectedSong != null) songNameText.text = GameManager.selectedSong.songName;
        
        if (scoreText != null) scoreText.text = "PUNTOS: " + GameManager.lastScore.ToString("D6");
        if (maxComboText != null) maxComboText.text = "COMBO MÁXIMO: " + GameManager.lastMaxCombo;
        
        if (perfectText != null) perfectText.text = "PERFECT: " + GameManager.lastPerfects;
        if (goodText != null) goodText.text = "GOOD: " + GameManager.lastGoods;
        if (badText != null) badText.text = "BAD: " + GameManager.lastBads;
        if (missText != null) missText.text = "MISS: " + GameManager.lastMisses;
    }

    // Esta función la vas a conectar al botón "Continuar" (On Click)
    public void GoBackToMenu()
    {
        SceneManager.LoadScene("SongSelect");
    }
}