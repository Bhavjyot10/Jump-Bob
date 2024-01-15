using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreSystem : MonoBehaviour
{
    private int score = -1 ;
    public UIManager uiManager;
    
    public void UpdateScore()
    {
        score++;
        uiManager.UpdateScoreUI(score);
    }

    public int Score()
    {
        return score;
    }

}
