using UnityEngine;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    private AudioSource gameManagerAudioSource;
    private AudioSource playerAudioSource;

    void Start()
    {
        GameObject gameManager = GameObject.FindWithTag("GameManager");
        if (gameManager != null)
        {
            gameManagerAudioSource = gameManager.GetComponent<AudioSource>();
        }

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerAudioSource = player.GetComponent<AudioSource>();
        }

        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.AddListener(OnVolumeChange);
        }

        if (gameManagerAudioSource != null)
        {
            volumeSlider.value = gameManagerAudioSource.volume;
        }
    }

    void OnVolumeChange(float volume)
    {
        if (gameManagerAudioSource != null)
        {
            gameManagerAudioSource.volume = volume;
        }

        if (playerAudioSource != null)
        {
            playerAudioSource.volume = volume;
        }
    }
}

