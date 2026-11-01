using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Video; 
using UnityEngine.SceneManagement;
using UnityEngine.UI; 

public class LaneHitJudger : MonoBehaviour
{
    public SongChart currentChart;
    public AudioSource gameAudioSource;
    public VideoPlayer backgroundVideo; 

    [Header("Pausa y Transiciones")]
    public GameObject pausePanel;    
    public Image fadePanel;          

    [Header("Textos de UI")]
    public TextMeshProUGUI feedbackText;     
    public TextMeshProUGUI scoreText;        
    public TextMeshProUGUI comboText;        
    public TextMeshProUGUI countdownText;    

    [Header("Ventanas de Tiempo")]
    public float perfectWindow = 0.10f; 
    public float goodWindow = 0.18f;    
    public float badWindow = 0.28f;     

    private int score = 0, combo = 0, multiplier = 1;
    private int perfectHits = 0, goodHits = 0, badHits = 0, missHits = 0, maxCombo = 0;
    private int nextNoteToCheck = 0;
    private float lastAudioTime = 0f;
    
    private bool gameStarted = false; 
    private bool isGameOver = false; 
    private bool isPaused = false;
    private bool isFading = false;
    private bool[] processedNotes;

    void Start()
    {
        // 🌟 SEGURO: Asegurarnos de que el tiempo corra normal al cargar la escena
        Time.timeScale = 1f;

        if (GameManager.selectedSong != null)
        {
            currentChart = GameManager.selectedSong;
            if (gameAudioSource != null) gameAudioSource.clip = currentChart.audioClip;
            if (backgroundVideo != null) backgroundVideo.clip = currentChart.backgroundVideo;
        }

        if (currentChart != null) processedNotes = new bool[currentChart.notes.Count];

        UpdateUI();
        if (pausePanel != null) pausePanel.SetActive(false);
        if (gameAudioSource != null) gameAudioSource.Pause();
        if (backgroundVideo != null) backgroundVideo.Pause();

        if (fadePanel != null) StartCoroutine(FadeInAndStart());
        else StartCoroutine(StartCountdown());
    }

    IEnumerator FadeInAndStart()
    {
        fadePanel.gameObject.SetActive(true);
        Color c = fadePanel.color;
        c.a = 1f;
        while (c.a > 0f)
        {
            c.a -= Time.deltaTime * 1.5f; 
            fadePanel.color = c;
            yield return null;
        }
        fadePanel.gameObject.SetActive(false);
        StartCoroutine(StartCountdown());
    }

    IEnumerator StartCountdown()
    {
        gameStarted = false;
        if (countdownText != null) { countdownText.gameObject.SetActive(true); countdownText.text = "3"; }
        yield return new WaitForSeconds(1f);
        if (countdownText != null) countdownText.text = "2";
        yield return new WaitForSeconds(1f);
        if (countdownText != null) countdownText.text = "1";
        yield return new WaitForSeconds(1f);
        if (countdownText != null) countdownText.text = "¡YA!";
        
        if (gameAudioSource != null) gameAudioSource.Play();
        if (backgroundVideo != null) backgroundVideo.Play();

        yield return new WaitForSeconds(0.4f);
        if (countdownText != null) countdownText.gameObject.SetActive(false);
        
        gameStarted = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) TogglePause();

        if (!gameStarted || isGameOver || isPaused || isFading || currentChart == null || gameAudioSource == null) return;

        float currentTime = gameAudioSource.time;

        if (!gameAudioSource.isPlaying && lastAudioTime > 5f)
        {
            StartCoroutine(EndGameSequence());
            return;
        }

        if (currentTime < lastAudioTime || (currentTime < 0.5f && nextNoteToCheck > 0))
        {
            nextNoteToCheck = 0;
            ResetCombo();
            if (currentChart != null) processedNotes = new bool[currentChart.notes.Count];
            if (backgroundVideo != null) { backgroundVideo.Stop(); backgroundVideo.Play(); }
        }
        lastAudioTime = currentTime;

        while (nextNoteToCheck < currentChart.notes.Count && processedNotes[nextNoteToCheck])
        {
            nextNoteToCheck++;
        }

        if (currentTime > 0.5f && nextNoteToCheck < currentChart.notes.Count)
        {
            NoteEvent targetNote = currentChart.notes[nextNoteToCheck];
            if (currentTime > targetNote.time + badWindow)
            {
                ShowFeedback("MISS", Color.gray);
                missHits++; 
                ResetCombo();
                processedNotes[nextNoteToCheck] = true; 
                nextNoteToCheck++;
            }
        }

        if (Input.GetKeyDown(KeyCode.Keypad7) || Input.GetKeyDown(KeyCode.Alpha7)) HitLane1();
        if (Input.GetKeyDown(KeyCode.Keypad9) || Input.GetKeyDown(KeyCode.Alpha9)) HitLane3();
        if (Input.GetKeyDown(KeyCode.Keypad5) || Input.GetKeyDown(KeyCode.Alpha5)) HitLane2();
        if (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1)) HitLane0();
        if (Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.Alpha3)) HitLane4();
    }

    public void TryHit(int laneID)
    {
        if (!gameStarted || isGameOver || isPaused || isFading || currentChart == null) return;
        float currentTime = gameAudioSource.time;
        for (int i = nextNoteToCheck; i < Mathf.Min(nextNoteToCheck + 12, currentChart.notes.Count); i++)
        {
            if (processedNotes[i]) continue; 
            NoteEvent note = currentChart.notes[i];
            if (note.laneID == laneID)
            {
                float absDiff = Mathf.Abs(currentTime - note.time);
                if (absDiff <= badWindow)
                {
                    string result = "BAD"; Color color = Color.yellow; int basePoints = 0;
                    if (absDiff <= perfectWindow) { result = "PERFECT!"; color = Color.cyan; basePoints = 100; combo++; perfectHits++; if (combo > maxCombo) maxCombo = combo; }
                    else if (absDiff <= goodWindow) { result = "GOOD"; color = Color.green; basePoints = 50; combo++; goodHits++; if (combo > maxCombo) maxCombo = combo; }
                    else { combo = 0; badHits++; }

                    multiplier = 1 + (combo / 10);
                    score += basePoints * multiplier;
                    ShowFeedback(result, color); UpdateUI(); DestroyClosestNoteInLane(laneID);
                    processedNotes[i] = true; 
                    return;
                }
            }
        }
    }

    public void TogglePause()
    {
        if (!gameStarted || isGameOver || isFading) return;
        if (isPaused) ResumeGame();
        else PauseGame();
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f; // 🌟 LA MAGIA: Congelamos el movimiento de todo el universo
        
        if (pausePanel != null) pausePanel.SetActive(true);
        if (gameAudioSource != null) gameAudioSource.Pause();
        if (backgroundVideo != null) backgroundVideo.Pause();
    }

    public void ResumeGame()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        StartCoroutine(ResumeCountdown());
    }

    IEnumerator ResumeCountdown()
    {
        // 🌟 USAMOS Realtime: Así la cuenta regresiva funciona aunque el juego esté congelado
        if (countdownText != null) { countdownText.gameObject.SetActive(true); countdownText.text = "3"; }
        yield return new WaitForSecondsRealtime(0.6f);
        if (countdownText != null) countdownText.text = "2";
        yield return new WaitForSecondsRealtime(0.6f);
        if (countdownText != null) countdownText.text = "1";
        yield return new WaitForSecondsRealtime(0.6f);
        if (countdownText != null) countdownText.text = "¡GO!";
        
        Time.timeScale = 1f; // 🌟 Descongelamos el mundo un milisegundo antes de que suene la música

        if (gameAudioSource != null) gameAudioSource.Play();
        if (backgroundVideo != null) backgroundVideo.Play();
        
        yield return new WaitForSeconds(0.4f);
        if (countdownText != null) countdownText.gameObject.SetActive(false);
        
        isPaused = false;
    }

    public void RestartGame() { StartCoroutine(FadeAndLoadScene(SceneManager.GetActiveScene().name)); }
    public void BackToMenu() { StartCoroutine(FadeAndLoadScene("SongSelect")); }

    IEnumerator FadeAndLoadScene(string sceneName)
    {
        isFading = true;
        Time.timeScale = 1f; // 🌟 CRÍTICO: Descongelamos el mundo para poder cambiar de escena
        
        if (pausePanel != null) pausePanel.SetActive(false);
        if (fadePanel != null)
        {
            fadePanel.gameObject.SetActive(true);
            Color c = fadePanel.color;
            c.a = 0f;
            while (c.a < 1f)
            {
                c.a += Time.deltaTime * 2f;
                fadePanel.color = c;
                yield return null;
            }
        }
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator EndGameSequence() { isGameOver = true; if (backgroundVideo != null) backgroundVideo.Stop(); if (feedbackText != null) { feedbackText.text = "¡CANCIÓN TERMINADA!"; feedbackText.color = Color.white; } yield return new WaitForSeconds(2.5f); GameManager.lastScore = score; GameManager.lastMaxCombo = maxCombo; GameManager.lastPerfects = perfectHits; GameManager.lastGoods = goodHits; GameManager.lastBads = badHits; GameManager.lastMisses = missHits; StartCoroutine(FadeAndLoadScene("ResultsScene")); }
    public void ResetCombo() { combo = 0; multiplier = 1; UpdateUI(); }
    public void HitLane0() { TryHit(0); } public void HitLane1() { TryHit(1); } public void HitLane2() { TryHit(2); } public void HitLane3() { TryHit(3); } public void HitLane4() { TryHit(4); }
    void DestroyClosestNoteInLane(int laneID) { Note[] allNotes = Object.FindObjectsByType<Note>(FindObjectsSortMode.None); Note closestNote = null; float closestDistance = float.MaxValue; foreach (Note note in allNotes) { if (note.laneID == laneID) { float distance = Mathf.Abs(note.transform.position.y - 4f); if (distance < closestDistance) { closestDistance = distance; closestNote = note; } } } if (closestNote != null) Destroy(closestNote.gameObject); }
    void ShowFeedback(string message, Color color) { if (feedbackText != null) { feedbackText.text = message; feedbackText.color = color; StopAllCoroutines(); StartCoroutine(ClearText()); } }
    void UpdateUI() { if (scoreText != null) scoreText.text = "SCORE: " + score.ToString("D6"); if (comboText != null) { if (combo > 1) comboText.text = "COMBO: " + combo + "  (x" + multiplier + ")"; else comboText.text = ""; } }
    IEnumerator ClearText() { yield return new WaitForSeconds(0.35f); if (!isGameOver) feedbackText.text = ""; }
}