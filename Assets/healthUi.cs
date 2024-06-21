using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class healthUi : MonoBehaviour
{
    public int health = 7;
    public GameObject[] healthPanel;
    public TextMeshProUGUI livesCounter;
    public TextMeshProUGUI scoreCounter;
    public static healthUi Instance { get; private set; }

    void Start()
    {
        Instance = this;
        UpdateHealthPanel();
    }

    void Update()
    {
    }

    public void healthBar(int val)
    {
        if (health > 0 & val == 1)
        {
            --health;
            UpdateHealthPanel();
        }
        else if (health > 0 & val == 2)
        {
            ++health;
            UpdateHealthPanel();
        }
    }

    public void resetBar()
    {
        health = 7;
        UpdateHealthPanel();
    }

    private void UpdateHealthPanel()
    {
        for (int i = 0; i < healthPanel.Length; i++)
        {
            if (i < health)
            {
                healthPanel[i].SetActive(true);
            }
            else
            {
                healthPanel[i].SetActive(false);
            }
        }
    }

    public void UpdateLivesDisplay(int lives)
    {
        livesCounter.text = lives.ToString();
    }

    public void UpdateScoreDisplay(int score)
    {
        if (score <= 9)
        {
            scoreCounter.text = "0" + score.ToString();
        }
        else
        {
            scoreCounter.text = score.ToString();
        }
    }
}

