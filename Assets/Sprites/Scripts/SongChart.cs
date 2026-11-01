using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Video; // <-- Necesario para el video

[System.Serializable]
public struct NoteEvent
{
    public float time;
    public int laneID;
}

[CreateAssetMenu(fileName = "NewSongChart", menuName = "RhythmGame/SongChart")]
public class SongChart : ScriptableObject
{
    [Header("Información de la Canción")]
    public string songName = "Nombre de la Canción";
    public string artist = "Artista";
    public Sprite coverImage; // La imagen en miniatura de la canción
    
    [Header("Archivos Multimedia")]
    public AudioClip audioClip;          // El MP3
    public VideoClip backgroundVideo;    // El MP4
    
    [Header("Menú de Selección")]
    public float previewStartTime = 30f; // En qué segundo arranca la vista previa en el menú

    [Header("Notas del Nivel")]
    public List<NoteEvent> notes = new List<NoteEvent>();
}