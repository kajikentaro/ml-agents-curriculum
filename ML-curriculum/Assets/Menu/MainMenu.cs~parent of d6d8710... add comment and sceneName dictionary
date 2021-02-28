using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    public static int aliveCount = 9;
    public static int nextStage = -1;
    public static int nowStage = -1;
    void Start()
    {
        DontDestroyOnLoad(this);
        Invoke("newGame", 2);
    }
    void newGame()
    {
        Debug.Log("start new game");
        int nextStageNumber = (int)Academy.Instance.EnvironmentParameters.GetWithDefault("stage_number", -1.0f);
        nowStage = nextStageNumber;
        SceneManager.LoadScene(nextStageNumber);
    }

    // Update is called once per frame
    void Update()
    {
        if (aliveCount == 0)
        {
            aliveCount = 9;
            Invoke("newGame", 5);
        }
    }
}

