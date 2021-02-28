using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MainMenu : MonoBehaviour
{
    //シーン内のプレハブの数
    static int aliveAgent= 9;
    static int nowStageNumber;
    //ステージ番号と、ステージ名の辞書
    Dictionary<int, string> sceneName = new Dictionary<int, string> { { 1, "TrainingArea1" },{ 2, "TrainingArea2" },{ 3, "TrainingArea3" },{ 4, "TrainingArea4" } };
    void Start()
    {
        //MainMenuを放棄しないよう(常に裏で動き続けるよう)にする。
        DontDestroyOnLoad(this);
        //newGame関数を2秒後に開始
        Invoke("newGame", 2);
    }
    void newGame()
    {
        //yamlから学習の進捗に応じたステージ番号を取得
        int newStageNumber = (int)Academy.Instance.EnvironmentParameters.GetWithDefault("stage_number", -1.0f);
        nowStageNumber= newStageNumber;
        //ステージ番号からステージをロードする。
        SceneManager.LoadScene(sceneName[newStageNumber]);
    }

    void Update()
    {
        if (aliveAgent == 0)
        {
            aliveAgent = 9;
            Invoke("newGame", 5);
        }
    }
    //Agentから呼び出される関数
    public static bool stageContinue()
    {
        int newStageNumber = (int)Academy.Instance.EnvironmentParameters.GetWithDefault("stage_number", -1.0f);
        if (nowStageNumber == newStageNumber) return true;
        return false;
    }
    //Agentから呼び出される関数
    public static void agentDestroyed()
    {
        aliveAgent--;
    }
}

