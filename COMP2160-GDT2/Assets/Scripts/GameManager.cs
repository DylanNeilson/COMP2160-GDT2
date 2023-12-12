using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int kickCount;
    public int levelOneKick;
    public int levelTwoKick;
    public int levelThreeKick;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
         
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void LoadLevel(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex);
    }

    public void LoadNextLevel()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        int currentIndex = currentScene.buildIndex;
        SceneManager.LoadScene(currentIndex + 1);
    }

}
