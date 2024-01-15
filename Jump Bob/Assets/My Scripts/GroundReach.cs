using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundReach : MonoBehaviour
{
    private JumpController jumpController;
    private ScoreSystem scoreSystem;
    private bool didLand = false;
    // Start is called before the first frame update
    void Start()
    {
        jumpController = GameObject.FindGameObjectWithTag("Player").GetComponent<JumpController>();
        scoreSystem = GameObject.Find("Score System").GetComponent<ScoreSystem>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            jumpController.FullHealth();
            if(!didLand)
            {
                didLand = true;
                scoreSystem.UpdateScore();
                jumpController.LatestPlatform(transform);
            }
            
        }
    }
}
