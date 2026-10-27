using UnityEngine;

public class Note : MonoBehaviour
{
    public float speed = 4f;
    public int laneID; // Identificador del carril al que pertenece
    
    public void SetSprite(Sprite newSprite)
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sprite = newSprite;
        }
    }

    void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);

        // Si pasa de la pantalla sin ser tocada
        if (transform.position.y > 6.5f)
        {
            Destroy(gameObject);
        }
    }
}