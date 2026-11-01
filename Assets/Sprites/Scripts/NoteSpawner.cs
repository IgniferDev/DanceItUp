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
        // 🌟 AUTOMATIZACIÓN: Carga los datos de la canción seleccionada
        if (GameManager.selectedSong != null)
        {
            currentChart = GameManager.selectedSong;
            if (gameAudioSource != null) gameAudioSource.clip = currentChart.audioClip;
        }
        
        noteIndex = 0;

        float spawnY = -6.0f;
        float targetY = (lanes != null && lanes.Length > 0 && lanes[0] != null) ? lanes[0].position.y : 4.0f;
        float distance = Mathf.Abs(targetY - spawnY);

        if (notePrefabs != null && notePrefabs.Length > 0 && notePrefabs[0] != null)
        {
            Note noteComponent = notePrefabs[0].GetComponent<Note>();
            if (noteComponent != null)
            {
                calculatedTravelTime = distance / noteComponent.speed;
                Debug.Log($"[NoteSpawner] Distancia: {distance} | Velocidad: {noteComponent.speed} | Tiempo de viaje: {calculatedTravelTime}s");
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
            
            float spawnSongTime = noteData.time - calculatedTravelTime - globalLatencyOffset;

            // Si ya es el momento (o la nota debió spawnear antes), la lanzamos sin restricciones
            if (songTime >= spawnSongTime)
            {
                SpawnNote(noteData.laneID);
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
        spawnPosition.y = -6f; 

        if (notePrefabs.Length > laneID && notePrefabs[laneID] != null)
        {
            GameObject newNoteObj = Instantiate(notePrefabs[laneID], spawnPosition, Quaternion.identity);
            
            // Asignamos el carril a la nota
            Note noteScript = newNoteObj.GetComponent<Note>();
            if (noteScript != null)
            {
                noteScript.laneID = laneID;
            }
        }
    }
}