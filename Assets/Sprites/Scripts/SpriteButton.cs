using UnityEngine;

public class SpriteButton : MonoBehaviour
{
    public int laneID; // Asigna 0, 1, 2, 3 o 4 según el carril
    
    private LaneHitJudger judger;
    private Collider2D col;

    void Start()
    {
        // Guardamos las referencias desde el inicio (es mucho mejor para el rendimiento)
        judger = FindFirstObjectByType<LaneHitJudger>();
        col = GetComponent<Collider2D>();
    }

    void Update()
    {
        // 1. SISTEMA MULTITOUCH PARA ANDROID
        // Revisa cada dedo que esté tocando la pantalla en este momento
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);

            // Solo nos interesa el milisegundo exacto en el que el dedo TOCA el cristal
            if (touch.phase == TouchPhase.Began)
            {
                CheckHit(touch.position);
            }
        }

        // 2. SISTEMA PARA PC (Para que no pierdas la capacidad de probar en el editor)
        if (Input.GetMouseButtonDown(0))
        {
            CheckHit(Input.mousePosition);
        }
    }

    // Esta función traduce el toque en la pantalla a tu mundo 2D
    void CheckHit(Vector3 screenPosition)
    {
        // Convertimos el toque a coordenadas de la cámara
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPosition);
        Vector2 touchPos2D = new Vector2(worldPos.x, worldPos.y);

        // Si el punto que tocamos está dentro del BoxCollider2D de este sprite...
        if (col.OverlapPoint(touchPos2D))
        {
            if (judger != null)
            {
                judger.TryHit(laneID);
            }
            else
            {
                Debug.LogWarning("No se encontró el LaneHitJudger en la escena.");
            }
        }
    }
}