using UnityEngine;

public class Note : MonoBehaviour
{
    public float speed = 4f;
    
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
        // La nota sube constantemente
        transform.Translate(Vector3.up * speed * Time.deltaTime);

        // Si la nota sube demasiado y pasa de la pantalla sin ser tocada (Miss automático)
        if (transform.position.y > 6.5f)
        {
            Destroy(gameObject);
        }
    }
}