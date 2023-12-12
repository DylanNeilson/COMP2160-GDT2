using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UserInterface : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private TextMeshProUGUI kickText;
    [SerializeField] private TextMeshProUGUI endText;
    [SerializeField] private Goal goalPost;
    [SerializeField] private GameObject endScreen;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button nextLevel;

    private int iKicks;

    // Start is called before the first frame update
    void Start()
    {
        restartButton.onClick.AddListener(OnRetry);
        nextLevel.onClick.AddListener(OnNextLevel);
        OnEndDisable();
    }

    // Update is called once per frame
    void Update()
    {
        iKicks = gameManager.kickCount;

        kickText.text = string.Format("Kicks: {0} / 3", iKicks);

        if (goalPost.goal)
        {
            OnEndEnable();
        }

    }

    private void OnRetry()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    private void OnNextLevel()
    {
        gameManager.LoadNextLevel();
    }

    private void OnEndEnable()
    {
        endScreen.SetActive(true);
        endText.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);
        nextLevel.gameObject.SetActive(true);
    }

    private void OnEndDisable()
    {
        endScreen.SetActive(false);
        endText.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        nextLevel.gameObject.SetActive(false);
    }
}