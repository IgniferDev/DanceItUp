using UnityEngine;

public class SpriteButton : MonoBehaviour
{
    public int laneID; // Asigna 0, 1, 2, 3 o 4 según el carril

    // Esto se activa automáticamente al tocar o hacer clic en el sprite en pantalla
    void OnMouseDown()
    {
        LaneHitJudger judger = FindObjectOfType<LaneHitJudger>();
        if (judger != null)
        {
            judger.TryHit(laneID);
            Debug.Log(laneID);
        }
        else
        {
            Debug.LogWarning("No se encontró el LaneHitJudger en la escena.");
        }
    }
}