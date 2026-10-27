using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    public SongChart currentChart;      
    public GameObject[] notePrefabs;    
    public Transform[] lanes;           
    public AudioSource gameAudioSource; 

    [Header("Calibración Fina (Ajuste de Latencia)")]
    public float globalLatencyOffset = 0.0f; 

    private int noteIndex = 0;
    private float calculatedTravelTime = 2.5f; 

    void Start()
    {
        noteIndex = 0;

        // CÁLCULO OBJETIVO Y DINÁMICO:
        // Distancia = Posición de la lane (ej. 4) menos la altura donde nacen (-6) = 10 unidades.
        float spawnY = -6.0f;
        float targetY = (lanes != null && lanes.Length > 0 && lanes[0] != null) ? lanes[0].position.y : 4.0f;
        float distance = Mathf.Abs(targetY - spawnY);

        if (notePrefabs != null && notePrefabs.Length > 0 && notePrefabs[0] != null)
        {
            Note noteComponent = notePrefabs[0].GetComponent<Note>();
            if (noteComponent != null)
            {
                calculatedTravelTime = distance / noteComponent.speed;
                Debug.Log($"[NoteSpawner] Distancia: {distance} | Velocidad: {noteComponent.speed} | Tiempo de viaje calculado: {calculatedTravelTime}s");
            }
        }
    }

    void Update()
    {
        if (currentChart == null || gameAudioSource == null || !gameAudioSource.isPlaying)
            return;

        float songTime = gameAudioSource.time;

        while (noteIndex < currentChart.notes.Count)
        {
            NoteEvent noteData = currentChart.notes[noteIndex];
            
            // Tiempo de viaje dinámico + latencia opcional
            float spawnSongTime = noteData.time - calculatedTravelTime - globalLatencyOffset;

            if (songTime >= spawnSongTime)
            {
                if (spawnSongTime >= 0f)
                {
                    SpawnNote(noteData.laneID);
                }
                noteIndex++;
            }
            else
            {
                break;
            }
        }
    }

    void SpawnNote(int laneID)
    {
        if (laneID < 0 || laneID >= lanes.Length) return;

        Vector3 spawnPosition = lanes[laneID].position;
        spawnPosition.y = -6f; // Altura donde nacen abajo

        if (notePrefabs.Length > laneID && notePrefabs[laneID] != null)
        {
            Instantiate(notePrefabs[laneID], spawnPosition, Quaternion.identity);
        }
    }
}