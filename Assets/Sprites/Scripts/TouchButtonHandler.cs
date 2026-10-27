using UnityEngine;

public class TouchButtonHandler : MonoBehaviour
{
    public int laneID; // Asigna 0, 1, 2, 3 o 4 en el Inspector según el carril
    private LaneHitJudger hitJudger;

    void Start()
    {
        // Buscamos el componente LaneHitJudger de forma directa en la escena
        hitJudger = FindObjectOfType<LaneHitJudger>();

        if (hitJudger == null)
        {
            Debug.LogWarning("⚠️ No se encontró LaneHitJudger en la escena. Asegúrate de que esté en un GameObject activo.");
        }
    }

    // Este método se conecta al evento "On Click" de tu Botón UI en Unity
    public void OnButtonTouch()
    {
        if (hitJudger != null)
        {
            // Llamamos directamente al método del juez
            hitJudger.ProcessTouchHit(laneID);
            Debug.Log($"👆 ¡Toque enviado al carril {laneID}!");
        }
        else
        {
            Debug.LogWarning("El botón fue presionado, pero el LaneHitJudger no está vinculado.");
        }
    }
}