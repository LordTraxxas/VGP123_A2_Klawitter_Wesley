using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static GameManager _instance;
    public static GameManager Instance => _instance;

    private int _lives;

    public GameObject pauseMenuUI;
    private bool isPaused = false;
    
    [SerializeField] private PlayerController playerPrefab;
    [SerializeField] public AudioSource audioSource;
    [SerializeField] public AudioClip titleTheme;
    [SerializeField] public AudioClip gameTheme;
    [SerializeField] public AudioClip gameoverTheme;
    [SerializeField] private AudioSource playerAudioSource;

    [HideInInspector] public PlayerController PlayerInstance => _playerInstance;
    PlayerController _playerInstance = null;
    Transform currentCheckpoint;


    // Start is called before the first frame update
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }
        else
        {
            Destroy(gameObject);
        }

        Destroy(gameObject);
    }

    private void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
        PlayMusicForCurrentScene();
        if (playerPrefab != null)
        {
            playerAudioSource = playerPrefab.GetComponent<AudioSource>();
        }
        else
        {
            Debug.LogError("Player GameObject is not assigned in the GameManager.");
        }
        pauseMenuUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().name == "GameOver")
        {
            SceneManager.LoadScene(0);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                Pause();
            }
        }
    }
    void Pause()
    {
        Time.timeScale = 0;
        isPaused = true;
        pauseMenuUI.SetActive(true);
    }

    void ResumeGame()
    {
        Time.timeScale = 1;
        isPaused = false;
        pauseMenuUI.SetActive(false);
    }

    private void GameOver()
    {
        SceneManager.LoadScene(2);
    }

    private void Respawn()
    {
        _playerInstance.transform.position = currentCheckpoint.position;
    }

    public void SpawnPlayer(Transform spawnLocation)
    {
        _playerInstance = Instantiate(playerPrefab, spawnLocation.position, spawnLocation.rotation);
        currentCheckpoint = spawnLocation;
    }

    public void UpdateCheckpoint(Transform updatedCheckpoint)
    {
        currentCheckpoint = updatedCheckpoint;
    }
    private void PlayPlayerClipByIndex(int index)
    {
        if (playerPrefab != null)
        {
            playerPrefab.PlaySpecificClip(index);
        }
        else
        {
            Debug.LogError("PlayerController is not assigned in the GameManager.");
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForCurrentScene();
        if (SceneManager.GetActiveScene().name == "TitleScreen")
        {
            pauseMenuUI.SetActive(true);
            isPaused = true;
        }
        else
        {
            pauseMenuUI.SetActive(false);
            isPaused = false;
        }
    }

    private void PlayMusicForCurrentScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        audioSource.Stop();

        switch (sceneName)
        {
            case "TitleScreen":
                audioSource.clip = titleTheme;
                break;
            case "Game":
                audioSource.clip = gameTheme;
                break;
            case "GameOver":
                audioSource.clip = gameoverTheme;
                break;
            default:
                audioSource.clip = null;
                break;
        }

        if (audioSource.clip != null)
        {
            audioSource.Play();
        }
    }

    public void startGameButton()
    {
        SceneManager.LoadScene(1);
    }
}

