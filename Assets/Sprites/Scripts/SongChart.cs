using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct NoteEvent
{
    public float time;    // Segundo exacto de la canción
    public int laneID;    // Carril (0 al 4)
}

[CreateAssetMenu(fileName = "NewSongChart", menuName = "RhythmGame/SongChart")]
public class SongChart : ScriptableObject
{
    public AudioClip audioClip;
    public List<NoteEvent> notes = new List<NoteEvent>();
}