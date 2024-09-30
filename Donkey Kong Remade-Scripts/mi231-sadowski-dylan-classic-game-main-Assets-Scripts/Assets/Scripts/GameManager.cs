using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private bool spacebarPressed = false;
    private float spacebarPressedTime = 0f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "StartScreen")
        {
            spacebarPressed = false;
        }
        else if (scene.name == "GameOver" || scene.name == "LevelComplete")
        {
            Invoke("LoadStartScreenDelayed", 3f); 
        }
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "StartScreen")
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                spacebarPressed = true;
                spacebarPressedTime = Time.time;
            }
            if (spacebarPressed && Time.time - spacebarPressedTime >= 3f)
            {
                LoadLevel0();
            }
        }
    }

    public void LoadLevel0()
    {
        SceneManager.LoadScene("Level0");
    }

    public void LevelComplete()
    {
        SceneManager.LoadScene("LevelComplete");
    }

    private void LoadStartScreenDelayed()
    {
        SceneManager.LoadScene("StartScreen");
    }
}
