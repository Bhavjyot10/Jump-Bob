using SupanthaPaul;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalGameOver : MonoBehaviour
{
    private bool GameOver = false;
    private UIManager UIManager;

    private void Start()
    {
        UIManager = GameObject.Find("UI Manager").GetComponent<UIManager>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && !GameOver)
        {
            GameOver = true;
            UIManager.UpdatePauseBtn(false);
            UIManager.UpdateGameOverPanel(true, true);
            GameObject.FindGameObjectWithTag("Player").GetComponent<MyPlayerController>().enabled = false;
        }
    }
}
