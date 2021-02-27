using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StartView : MonoBehaviour
{
    public ScoreManager scoreManager;
    public TMP_Text leaderboardNames, leaderboardScores;

    private int page;
    
    // Start is called before the first frame update
    void Start()
    {
        scoreManager.onLoaded += ScoresLoaded;
        scoreManager.LoadLeaderBoards(page);
    }

    private void ScoresLoaded()
    {
        leaderboardNames.text = scoreManager.leaderBoardPositionsString;
        leaderboardScores.text = scoreManager.leaderBoardScoresString;
    }

    public void StartGame()
    {
        var scene = PlayerPrefs.HasKey("PlayerName") ? "Main" : "Name";
        SceneChanger.Instance.ChangeScene(scene);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}