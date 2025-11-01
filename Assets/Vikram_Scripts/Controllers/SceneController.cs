using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public GameDataSO gameData;

    [Header("Scene Key Bindings")]
    public KeyCode HomeScene = KeyCode.Alpha0;
    public KeyCode DelhiScene = KeyCode.Alpha1;
    public KeyCode MilanScene = KeyCode.Alpha2;
    public KeyCode SaoPauloScene = KeyCode.Alpha3;
    public KeyCode ManilaScene = KeyCode.Alpha4;


    // Singleton instance
    public static SceneController Instance { get; private set; }


    private void Awake()
    {
        // Enforce singleton and persist across scenes
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (Input.GetKeyDown(DelhiScene))
        {
           SceneManager.LoadSceneAsync(1);
            gameData.isGameCompleted = false;
            Debug.Log("Delhi Scene Loaded");
        }
        else if (Input.GetKeyDown(MilanScene))
        {
          SceneManager.LoadSceneAsync(2);
            gameData.isGameCompleted = false;
        }
        else if (Input.GetKeyDown(SaoPauloScene))
        {
          SceneManager.LoadSceneAsync(3);
            gameData.isGameCompleted = false;
        }
        else if (Input.GetKeyDown(ManilaScene))
        {
           SceneManager.LoadSceneAsync(4);
            gameData.isGameCompleted = false;
        }

        else if (Input.GetKeyDown(HomeScene))
        {
            SceneManager.LoadSceneAsync(0);
        }
    }
}
