using UnityEngine;

public class InputManager : MonoBehaviour
{
    public LaneController[] lanes; // Arreglo con nuestros 5 carriles (0 al 4)

    void Update()
    {
        // ----------------------------------------------------
        // MAPEO DE CONTROLES SEGÚN TU DISTRIBUCIÓN DE PAD NUMÉRICO:
        // ----------------------------------------------------

        // 7 = Arriba Izquierda -> Lane 1
        if (Input.GetKeyDown(KeyCode.Keypad7) || Input.GetKeyDown(KeyCode.Alpha7))
        {
            TriggerLane(1);
        }
        
        // 9 = Arriba Derecha -> Lane 3
        if (Input.GetKeyDown(KeyCode.Keypad9) || Input.GetKeyDown(KeyCode.Alpha9))
        {
            TriggerLane(3);
        }
        
        // 5 = Centro -> Lane 2
        if (Input.GetKeyDown(KeyCode.Keypad5) || Input.GetKeyDown(KeyCode.Alpha5))
        {
            TriggerLane(2);
        }
        
        // 1 = Abajo Izquierda -> Lane 0
        if (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1))
        {
            TriggerLane(0);
        }
        
        // 3 = Abajo Derecha -> Lane 4
        if (Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.Alpha3))
        {
            TriggerLane(4);
        }

        // ----------------------------------------------------
        // SOPORTE TÁCTIL Y MOUSE (Para pantallas táctiles / PC)
        // ----------------------------------------------------
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                CheckTouchInput(touch.position);
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            CheckTouchInput(Input.mousePosition);
        }
    }

    void TriggerLane(int laneIndex)
    {
        if (lanes != null && laneIndex >= 0 && laneIndex < lanes.Length && lanes[laneIndex] != null)
        {
            lanes[laneIndex].TapLane();
        }
    }

    void CheckTouchInput(Vector2 screenPosition)
    {
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(screenPosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

        if (hit.collider != null)
        {
            LaneController lane = hit.collider.GetComponent<LaneController>();
            if (lane != null)
            {
                lane.TapLane();
            }
        }
    }
}