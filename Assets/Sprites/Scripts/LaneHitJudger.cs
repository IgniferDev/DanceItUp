using UnityEngine;
using TMPro;
using System.Collections;

public class HitJudger : MonoBehaviour
{
    public SongChart currentChart;
    public AudioSource gameAudioSource;
    public TextMeshProUGUI feedbackText; // Arrastra aquí tu texto del Canvas

    [Header("Ventanas de Tiempo (en segundos)")]
    public float perfectWindow = 0.08f; // ±80 milisegundos
    public float goodWindow = 0.15f;    // ±150 milisegundos
    public float badWindow = 0.25f;     // ±250 milisegundos

    private int nextNoteToCheck = 0;
    private Coroutine hideTextRoutine;

    void Update()
    {
        if (currentChart == null || gameAudioSource == null || !gameAudioSource.isPlaying)
            return;

        float currentTime = gameAudioSource.time;

        // 1. Detectar si una nota se pasó de largo sin ser tocada (MISS automático)
        if (nextNoteToCheck < currentChart.notes.Count)
        {
            NoteEvent targetNote = currentChart.notes[nextNoteToCheck];
            // Si el tiempo actual superó la ventana de Bad, cuenta como Miss
            if (currentTime > targetNote.time + badWindow)
            {
                ShowFeedback("MISS", Color.gray);
                nextNoteToCheck++;
            }
        }

        // 2. Leer las teclas del pad numérico para juzgar el golpe del jugador
        if (Input.GetKeyDown(KeyCode.Keypad7) || Input.GetKeyDown(KeyCode.Alpha7)) TryHit(1);
        if (Input.GetKeyDown(KeyCode.Keypad9) || Input.GetKeyDown(KeyCode.Alpha9)) TryHit(3);
        if (Input.GetKeyDown(KeyCode.Keypad5) || Input.GetKeyDown(KeyCode.Alpha5)) TryHit(2);
        if (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1)) TryHit(0);
        if (Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.Alpha3)) TryHit(4);
    }

    public void TryHit(int laneID)
    {
        if (currentChart == null || nextNoteToCheck >= currentChart.notes.Count) return;

        float currentTime = gameAudioSource.time;

        // Buscamos en una ventana cercana la nota que coincida con este carril
        // (Revisamos unas cuantas notas adelante si es necesario)
        for (int i = nextNoteToCheck; i < Mathf.Min(nextNoteToCheck + 5, currentChart.notes.Count); i++)
        {
            NoteEvent note = currentChart.notes[i];

            if (note.laneID == laneID)
            {
                float timeDifference = Mathf.Abs(currentTime - note.time);

                // Evaluamos la precisión según los segundos de diferencia
                if (timeDifference <= perfectWindow)
                {
                    ShowFeedback("PERFECT!", Color.cyan);
                    nextNoteToCheck = i + 1; // Marcamos esta nota como superada
                    return;
                }
                else if (timeDifference <= goodWindow)
                {
                    ShowFeedback("GOOD", Color.green);
                    nextNoteToCheck = i + 1;
                    return;
                }
                else if (timeDifference <= badWindow)
                {
                    ShowFeedback("BAD", Color.yellow);
                    nextNoteToCheck = i + 1;
                    return;
                }
            }
        }
    }
    
    // Método público para recibir los toques táctiles del botón
    public void ProcessTouchHit(int laneID)
    {
        TryHit(laneID);
    }

    void ShowFeedback(string message, Color color)
    {
        if (feedbackText == null) return;

        feedbackText.text = message;
        feedbackText.color = color;

        // Reiniciamos la animación del texto para que parpadee limpio
        if (hideTextRoutine != null)
        {
            StopCoroutine(hideTextRoutine);
        }
        hideTextRoutine = StartCoroutine(FadeOutText());
    }

    IEnumerator FadeOutText()
    {
        feedbackText.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.4f); // El texto se muestra medio segundo
        feedbackText.text = ""; // Se limpia
    }
}

