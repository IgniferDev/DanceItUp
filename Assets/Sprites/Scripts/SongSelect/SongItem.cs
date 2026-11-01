using UnityEngine;
using TMPro;
using UnityEngine.UI; // Necesario para modificar imágenes

public class SongItem : MonoBehaviour
{
    public SongChart myChart; 
    public SongSelectMenu menuManager; 
    
    public TextMeshProUGUI buttonText;
    public Image buttonBackgroundImage; // Arrastra el componente Image de tu botón aquí

    void Start()
    {
        if (myChart != null)
        {
            if (buttonText != null) buttonText.text = myChart.songName;
            
            // 🌟 Cargamos la imagen automáticamente
            if (buttonBackgroundImage != null && myChart.coverImage != null)
            {
                buttonBackgroundImage.sprite = myChart.coverImage;
            }
        }
    }

    public void Clicked()
    {
        if (menuManager != null) menuManager.OnSongButtonClicked(myChart);
    }
}