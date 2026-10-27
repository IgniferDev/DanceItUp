using UnityEngine;
using TMPro;
using System.Collections;

public class LaneHitJudger : MonoBehaviour
{
    public SongChart currentChart;
    public AudioSource gameAudioSource;

    [Header("Textos de UI (Arrastra tus TextMeshPro aquí)")]
    public TextMeshProUGUI feedbackText;     // Texto central de PERFECT / GOOD / MISS
    public TextMeshProUGUI scoreText;        // Texto de puntuación (ej. SCORE: 000000)
    public TextMeshProUGUI comboText;        // Texto de combo y multiplicador (ej. COMBO: 15  (x2))
    public TextMeshProUGUI countdownText;    // Texto grande en el centro para la cuenta regresiva (3, 2, 1, ¡YA!)

    [Header("Ventanas de Tiempo (Segundos)")]
    public float perfectWindow = 0.10f; 
    public float goodWindow = 0.18f;    
    public float badWindow = 0.28f;     

    [Header("Variables de Puntuación")]
    private int score = 0;
    private int combo = 0;
    private int multiplier = 1;

    private int nextNoteToCheck = 0;
    private float lastAudioTime = 0f;
    private bool gameStarted = false; // Control para pausar hasta que termine el conteo

    void Start()
    {
        nextNoteToCheck = 0;
        score = 0;
        combo = 0;
        multiplier = 1;
        UpdateUI();

        // Pausamos la música al iniciar para dar paso a la cuenta regresiva
        if (gameAudioSource != null)
        {
            gameAudioSource.Pause();
        }

        // Iniciamos el conteo de las baquetas / intro
        StartCoroutine(StartCountdown());
    }

    IEnumerator StartCountdown()
    {
        gameStarted = false;

        if (countdownText != null) countdownText.gameObject.SetActive(true);

        // Muestra "3"
        if (countdownText != null) countdownText.text = "3";
        yield return new WaitForSeconds(1f);

        // Muestra "2"
        if (countdownText != null) countdownText.text = "2";
        yield return new WaitForSeconds(1f);

        // Muestra "1"
        if (countdownText != null) countdownText.text = "1";
        yield return new WaitForSeconds(1f);

        // Muestra "¡YA!"
        if (countdownText != null) countdownText.text = "¡YA!";
        
        // Arrancamos la música oficialmente
        if (gameAudioSource != null)
        {
            gameAudioSource.Play();
        }

        yield return new WaitForSeconds(0.4f);

        // Ocultamos el texto de la cuenta regresiva
        if (countdownText != null) countdownText.gameObject.SetActive(false);
        
        // ¡Comienza el juego oficialmente!
        gameStarted = true;
    }

    void Update()
    {
        // Si la cuenta regresiva no ha terminado, no procesamos notas ni toques
        if (!gameStarted) return;

        if (currentChart == null || gameAudioSource == null || !gameAudioSource.isPlaying)
            return;

        float currentTime = gameAudioSource.time;

        // Reinicio automático si la canción vuelve a empezar
        if (currentTime < lastAudioTime || (currentTime < 0.5f && nextNoteToCheck > 0))
        {
            nextNoteToCheck = 0;
            ResetCombo();
        }
        lastAudioTime = currentTime;

        // Miss automático si la nota pasa de la ventana permitida
        if (currentTime > 0.5f && nextNoteToCheck < currentChart.notes.Count)
        {
            NoteEvent targetNote = currentChart.notes[nextNoteToCheck];
            if (currentTime > targetNote.time + badWindow)
            {
                ShowFeedback("MISS", Color.gray);
                ResetCombo();
                nextNoteToCheck++;
            }
        }

        // Respaldo con teclado numérico de PC
        if (Input.GetKeyDown(KeyCode.Keypad7) || Input.GetKeyDown(KeyCode.Alpha7)) HitLane1();
        if (Input.GetKeyDown(KeyCode.Keypad9) || Input.GetKeyDown(KeyCode.Alpha9)) HitLane3();
        if (Input.GetKeyDown(KeyCode.Keypad5) || Input.GetKeyDown(KeyCode.Alpha5)) HitLane2();
        if (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1)) HitLane0();
        if (Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.Alpha3)) HitLane4();
    }

    public void TryHit(int laneID)
    {
        // Si no ha empezado el juego o ya no hay notas, ignoramos el toque
        if (!gameStarted || currentChart == null || nextNoteToCheck >= currentChart.notes.Count) return;

        float currentTime = gameAudioSource.time;

        for (int i = nextNoteToCheck; i < Mathf.Min(nextNoteToCheck + 8, currentChart.notes.Count); i++)
        {
            NoteEvent note = currentChart.notes[i];

            if (note.laneID == laneID)
            {
                float timeDifference = currentTime - note.time; 
                float absDiff = Mathf.Abs(timeDifference);

                if (absDiff <= badWindow)
                {
                    string result = "BAD";
                    Color color = Color.yellow;
                    int basePoints = 0;

                    if (absDiff <= perfectWindow)
                    {
                        result = "PERFECT!";
                        color = Color.cyan;
                        basePoints = 100;
                        combo++;
                    }
                    else if (absDiff <= goodWindow)
                    {
                        result = "GOOD";
                        color = Color.green;
                        basePoints = 50;
                        combo++;
                    }
                    else
                    {
                        combo = 0; // Si es BAD, rompe el combo
                    }

                    // Multiplicador (sube 1x cada 10 notas)
                    multiplier = 1 + (combo / 10);
                    score += basePoints * multiplier;

                    ShowFeedback(result, color);
                    UpdateUI();
                    
                    // Destruir la nota visual en pantalla
                    DestroyClosestNoteInLane(laneID);

                    if (i == nextNoteToCheck)
                    {
                        nextNoteToCheck++;
                    }
                    return;
                }
            }
        }
    }

    public void ResetCombo()
    {
        combo = 0;
        multiplier = 1;
        UpdateUI();
    }

    // Métodos directos para los botones táctiles y el teclado
    public void HitLane0() { TryHit(0); }
    public void HitLane1() { TryHit(1); }
    public void HitLane2() { TryHit(2); }
    public void HitLane3() { TryHit(3); }
    public void HitLane4() { TryHit(4); }

    void DestroyClosestNoteInLane(int laneID)
    {
        Note[] allNotes = Object.FindObjectsByType<Note>(FindObjectsSortMode.None);
        Note closestNote = null;
        float closestDistance = float.MaxValue;

        foreach (Note note in allNotes)
        {
            if (note.laneID == laneID)
            {
                float distance = Mathf.Abs(note.transform.position.y - 4f);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestNote = note;
                }
            }
        }

        if (closestNote != null)
        {
            Destroy(closestNote.gameObject);
        }
    }

    void ShowFeedback(string message, Color color)
    {
        if (feedbackText != null)
        {
            feedbackText.text = message;
            feedbackText.color = color;
            StopAllCoroutines();
            StartCoroutine(ClearText());
        }
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
                comboText.text = "COMBO: " + combo + "  (x" + multiplier + ")";
            }
            else
            {
                comboText.text = "";
            }
        }
    }

    IEnumerator ClearText()
    {
        yield return new WaitForSeconds(0.35f);
        feedbackText.text = "";
    }
}
