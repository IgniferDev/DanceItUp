using UnityEngine;

public class LaneController : MonoBehaviour
{
    public int laneID; 
    private GameObject activeNote = null; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Note"))
        {
            activeNote = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == activeNote)
        {
            Debug.Log($"¡MISS en el carril {laneID}!");
            
            // Si se pasa la nota, rompemos el combo
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.ResetCombo();
            }

            activeNote = null;
            Destroy(collision.gameObject);
        }
    }

    public void TapLane()
    {
        if (activeNote != null)
        {
            float distance = Mathf.Abs(activeNote.transform.position.y - transform.position.y);
            
            string judgment = "";

            if (distance <= 0.45f)
            {
                judgment = "PERFECT";
                Debug.Log($"¡PERFECT! 🌟 (Distancia: {distance:F2})");
            }
            else if (distance <= 0.80f)
            {
                judgment = "GOOD";
                Debug.Log($"¡GOOD! 👍 (Distancia: {distance:F2})");
            }
            else
            {
                judgment = "BAD";
                Debug.Log($"¡BAD / Fuera de tiempo! ❌ (Distancia: {distance:F2})");
            }

            // Enviamos el resultado al ScoreManager si fue acierto
            if (ScoreManager.Instance != null && (judgment == "PERFECT" || judgment == "GOOD"))
            {
                ScoreManager.Instance.AddScore(judgment);
            }
            else if (ScoreManager.Instance != null && judgment == "BAD")
            {
                ScoreManager.Instance.ResetCombo();
            }

            GameObject noteToDestroy = activeNote;
            activeNote = null; 
            Destroy(noteToDestroy);
        }
        else
        {
            Debug.Log($"¡Tap en vacío (Ghost Tap) en el carril {laneID}!");
        }
    }
}