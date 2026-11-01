using UnityEngine;
using System.Collections.Generic;

public class ChartRecorder : MonoBehaviour
{
    public SongChart targetChart; 
    public AudioSource audioSource;
    private bool isRecording = false;

    void Update()
    {
        // Iniciar grabación
        if (Input.GetKeyDown(KeyCode.R) && !isRecording) StartRecording();

        if (isRecording)
        {
            // Tecla S para detener y guardar manualmente en cualquier momento
            if (Input.GetKeyDown(KeyCode.S)) 
            {
                StopAndSave();
                return; 
            }

            if (Input.GetKeyDown(KeyCode.Space)) TogglePause();
            if (Input.GetKeyDown(KeyCode.Backspace)) RewindAndClearRecentNotes();

            // Solo registramos notas si la música está sonando
            if (audioSource.isPlaying)
            {
                if (Input.GetKeyDown(KeyCode.Keypad7) || Input.GetKeyDown(KeyCode.Alpha7)) RecordHit(1);
                if (Input.GetKeyDown(KeyCode.Keypad9) || Input.GetKeyDown(KeyCode.Alpha9)) RecordHit(3);
                if (Input.GetKeyDown(KeyCode.Keypad5) || Input.GetKeyDown(KeyCode.Alpha5)) RecordHit(2);
                if (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1)) RecordHit(0);
                if (Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.Alpha3)) RecordHit(4);
            }
            else
            {
                // Si la música se detuvo porque llegó al final de la canción, autoguarda
                if (audioSource.clip != null && audioSource.time >= audioSource.clip.length - 0.1f)
                {
                    StopAndSave();
                }
            }
        }
    }

    void StartRecording()
    {
        if (targetChart == null || audioSource == null) return;
        targetChart.notes.Clear(); 
        audioSource.time = 0f;
        audioSource.Play();
        isRecording = true;
        Debug.Log("🔴 ¡GRABACIÓN INICIADA! (Presiona 'S' para terminar y guardar)");
    }

    void TogglePause()
    {
        if (audioSource.isPlaying) audioSource.Pause();
        else audioSource.Play(); // Corregido: UnPause no siempre es fiable con AudioSource
    }

    void RewindAndClearRecentNotes()
    {
        float targetTime = Mathf.Max(0f, audioSource.time - 5f);
        audioSource.time = targetTime;
        targetChart.notes.RemoveAll(note => note.time > targetTime);
        Debug.Log("↩️ ¡REBOBINADO! Notas recientes borradas.");
    }

    void RecordHit(int laneID)
    {
        float currentTime = audioSource.time; 
        
        NoteEvent newNote = new NoteEvent
        {
            time = currentTime,
            laneID = laneID
        };

        targetChart.notes.Add(newNote);
        targetChart.notes.Sort((a, b) => a.time.CompareTo(b.time));

        Debug.Log($"Grabado golpe en Carril {laneID} en el segundo {currentTime:F2}s");
    }

    public void StopAndSave()
    {
        if (!isRecording) return; // Evitamos que se ejecute dos veces

        isRecording = false;
        if (audioSource.isPlaying) audioSource.Stop();

        // Esto le avisa a Unity que modifique el archivo físico en el disco duro
        #if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(targetChart);
        UnityEditor.AssetDatabase.SaveAssets();
        #endif

        Debug.Log($"💾 ¡GRABACIÓN FINALIZADA Y GUARDADA! Total de notas: {targetChart.notes.Count}");
    }

    // 🛡️ SEGURO DE VIDA: Si presionas el botón de "Play" en Unity para salir de golpe, 
    // este método se dispara un milisegundo antes de cerrar y guarda tu archivo.
    void OnDisable()
    {
        if (isRecording)
        {
            Debug.Log("⚠️ Cierre abrupto detectado. Auto-guardando chart...");
            StopAndSave();
        }
    }
}