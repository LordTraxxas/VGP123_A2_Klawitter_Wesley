using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] private Transform startPos;
    [SerializeField] public GameObject pauseMenuPrefab;

    void Start()
    {
        GameManager.Instance.SpawnPlayer(startPos);
        
        GameObject pauseMenuInstance = Instantiate(pauseMenuPrefab);
        pauseMenuInstance.SetActive(false);

        GameManager.Instance.pauseMenuUI = pauseMenuInstance;
    }

    void Update()
    {

    }
}
