using UnityEngine;
using System.Collections.Generic;

public class ChartRecorder : MonoBehaviour
{
    public SongChart targetChart; 
    public AudioSource audioSource;
    private bool isRecording = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) StartRecording();
        if (Input.GetKeyDown(KeyCode.Space) && isRecording) TogglePause();
        if (Input.GetKeyDown(KeyCode.Backspace) && isRecording) RewindAndClearRecentNotes();

        if (isRecording && audioSource.isPlaying)
        {
            if (Input.GetKeyDown(KeyCode.Keypad7) || Input.GetKeyDown(KeyCode.Alpha7)) RecordHit(1);
            if (Input.GetKeyDown(KeyCode.Keypad9) || Input.GetKeyDown(KeyCode.Alpha9)) RecordHit(3);
            if (Input.GetKeyDown(KeyCode.Keypad5) || Input.GetKeyDown(KeyCode.Alpha5)) RecordHit(2);
            if (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1)) RecordHit(0);
            if (Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.Alpha3)) RecordHit(4);

            if (!audioSource.isPlaying) StopAndSave();
        }
    }

    void StartRecording()
    {
        if (targetChart == null || audioSource == null) return;
        targetChart.notes.Clear(); 
        audioSource.time = 0f;
        audioSource.Play();
        isRecording = true;
        Debug.Log("🔴 ¡GRABACIÓN INICIADA (Modo Exacto)!");
    }

    void TogglePause()
    {
        if (audioSource.isPlaying) audioSource.Pause();
        else audioSource.UnPause();
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
        float currentTime = audioSource.time; // Guardamos el tiempo exacto del golpe
        
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
        isRecording = false;
        if (audioSource.isPlaying) audioSource.Stop();

        #if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(targetChart);
        UnityEditor.AssetDatabase.SaveAssets();
        #endif

        Debug.Log($"💾 ¡GRABACIÓN FINALIZADA! Total de notas: {targetChart.notes.Count}");
    }
}