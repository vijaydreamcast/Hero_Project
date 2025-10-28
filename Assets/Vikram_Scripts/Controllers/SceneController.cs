using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public KeyCode DelhiScene = KeyCode.Alpha1;
    public KeyCode MilanScene = KeyCode.Alpha2;
    public KeyCode SaoPauloScene = KeyCode.Alpha3;
    public KeyCode ManilaScene = KeyCode.Alpha4;




    void Update()
    {
        if (Input.GetKeyDown(DelhiScene))
        {
           SceneManager.LoadSceneAsync(1);

            Debug.Log("Delhi Scene Loaded");
        }
        else if (Input.GetKeyDown(MilanScene))
        {
          SceneManager.LoadSceneAsync(2);
        }
        else if (Input.GetKeyDown(SaoPauloScene))
        {
          SceneManager.LoadSceneAsync(3);
        }
        else if (Input.GetKeyDown(ManilaScene))
        {
           SceneManager.LoadSceneAsync(4);
        }
    }
}
