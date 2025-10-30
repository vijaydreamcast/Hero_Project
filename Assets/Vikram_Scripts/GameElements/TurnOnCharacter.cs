using UnityEngine;

public class TurnOnCharacter : MonoBehaviour
{
    public GameObject character;
    public GameObject head;
    public GameObject CurvedCanvas;

    private void Show()
    {
        character.SetActive(true);   
        head.SetActive(true);
        CurvedCanvas.SetActive(true);
    }
}
