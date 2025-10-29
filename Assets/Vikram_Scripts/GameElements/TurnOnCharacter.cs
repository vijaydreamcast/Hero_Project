using UnityEngine;

public class TurnOnCharacter : MonoBehaviour
{
   public GameObject character;
    public GameObject head;

    private void Show()
    {
        character.SetActive(true);   
        head.SetActive(true);
    }
}
