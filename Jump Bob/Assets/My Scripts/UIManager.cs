using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    public TMP_Text scoreText;
    public GameObject DashImage;
    public GameObject DashBtn;
    public GameObject pauseBtn;
    public GameObject gameOverPanel;
    public TMP_Text gameOverPlatformText;
    public TMP_Text gameOverText;
    public LevelGenerator levelGenerator;
    public AudioManager audioManager;
    public ScoreSystem scoreSystem;
    public Image livesImage;
    public List<Sprite> livesSprites;
    int livesLeft;

    private void Awake()
    {
        if(levelGenerator.IsLevelInfinite())
        {
            livesImage.gameObject.SetActive(false);
        }
    }
    private void Start()
    {
        livesLeft = livesSprites.Count - 1;
    }
    public void UpdateScoreUI(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }
    }

    public void UpdateDashImg(bool DashImg)
    {
        DashImage.SetActive(DashImg);
        DashBtn.SetActive(DashImg);
    }

    public void UpdateLivesUI()
    {
        livesLeft--;
        livesImage.sprite = livesSprites[livesLeft];
    }

    public void UpdatePauseBtn(bool status)
    {
        pauseBtn.SetActive(status);
    }

    public void UpdateGameOverPanel(bool status, bool result)
    {
        gameOverPlatformText.text = scoreSystem.Score().ToString() + "/" + levelGenerator.TotalPlatforms();
        gameOverPanel.SetActive(status);
        if (result)
        {
            audioManager.PlaySuccessAudio();
            gameOverText.text = "success!!";
        }

        else
        {
            gameOverText.text = "failed!!";
            gameOverPlatformText.text = scoreSystem.Score().ToString();
        }
    }

    public bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
