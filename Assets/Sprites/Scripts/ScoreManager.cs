using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance; 

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI comboText;

    private int score = 0;
    private int combo = 0;
    private int multiplier = 1; // <--- NUEVO: Multiplicador inicial

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateUI();
    }

    public void AddScore(string judgment)
    {
        // Base de puntos según el juicio
        int basePoints = 0;

        if (judgment == "PERFECT")
        {
            basePoints = 100;
            combo++;
        }
        else if (judgment == "GOOD")
        {
            basePoints = 50;
            combo++;
        }

        // Calculamos el multiplicador basado en el combo (ej: sube 1x cada 10 notas)
        // 0-9 combo = x1, 10-19 combo = x2, 20-29 combo = x3, etc.
        multiplier = 1 + (combo / 10);

        // Sumamos el puntaje aplicando el multiplicador
        score += basePoints * multiplier;

        UpdateUI();
    }

    public void ResetCombo()
    {
        combo = 0;
        multiplier = 1; // Reiniciamos el multiplicador al fallar
        UpdateUI();
    }

    void UpdateUI()
    {
        if (scoreText != null) 
        {
            scoreText.text = "SCORE: " + score.ToString("D6");
        }

        if (comboText != null) 
        {
            if (combo > 1)
            {
                // Mostramos el combo y el multiplicador actual en pantalla
                comboText.text = "COMBO: " + combo + "  (x" + multiplier + ")";
            }
            else
            {
                comboText.text = "";
            }
        }
    }
}