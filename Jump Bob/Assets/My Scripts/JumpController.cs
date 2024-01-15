using CodeMonkey.HealthSystemCM;
using SupanthaPaul;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpController : MonoBehaviour
{
    private HealthSystem healthSystem;
    private MyPlayerController playerController;
    private Vector3 lastPlatformPos;
    private UIManager uiManager;
    private AudioManager audioManager;
    public int lives = 3;

    // Start is called before the first frame update
    void Start()
    {
        healthSystem = GetComponent<HealthSystemComponent>().GetHealthSystem();

        healthSystem.OnDead += HealthSystem_OnDead;
        healthSystem.OnDamaged += HealthSystem_OnDamaged;
        healthSystem.OnHealed += HealthSystem_OnHealed;
        healthSystem.OnHealthMaxChanged += HealthSystem_OnHealthMaxChanged;

        uiManager = GameObject.Find("UI Manager").GetComponent<UIManager>();
        playerController = GetComponent<MyPlayerController>();
        audioManager = GameObject.Find("Audio Manager").GetComponent <AudioManager>();
    }

    private void Update()
    {
        if (transform.position.y < -25f)
        {
            if (!LevelGenerator.isInfiniteLevel)
            {
                if (lives > 1)
                {
                    GetComponent<MyPlayerController>().StopJumpPhysics();
                    transform.position = lastPlatformPos;
                    lives--;
                    uiManager.UpdateLivesUI();
                }

                else
                {
                    uiManager.UpdateLivesUI();
                    uiManager.UpdatePauseBtn(false);
                    uiManager.UpdateGameOverPanel(true, false);
                    audioManager.PlayFailedAudio();
                    enabled = false;
                    GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
                }
            }

            else
            {
                uiManager.UpdatePauseBtn(false);
                uiManager.UpdateGameOverPanel(true, false);
                audioManager.PlayFailedAudio();
                enabled = false;
                GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            }
        }
    }

    private void HealthSystem_OnHealed(object sender, System.EventArgs e)
    {
        playerController.JumpDashStatus(true);
    }

    private void HealthSystem_OnDamaged(object sender, System.EventArgs e)
    {

    }

    private void HealthSystem_OnHealthMaxChanged(object sender, System.EventArgs e)
    {
        playerController.JumpDashStatus(true);
    }

    private void HealthSystem_OnDead(object sender, System.EventArgs e)
    {
        playerController.JumpDashStatus(false);
    }

    public void JumpExhaustion(int jumpUsed)
    {
        healthSystem.Damage(20);
    }

    public void DashExhaustion(int dashUsed)
    {
        healthSystem.Damage(25);
    }
    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            if(collision.gameObject.CompareTag("Jump Energy"))
            {
                healthSystem.Heal(40);
                Destroy(collision.gameObject);
                audioManager.PlayJumpPickupAudio();
            }

            if (collision.gameObject.CompareTag("Dash Energy"))
            {
                playerController.DashStatus(true);
                Destroy(collision.gameObject);
                audioManager.PlayDashPickupAudio();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision != null)
        {
            if(collision.gameObject.layer == 8)
            {
                audioManager.PlayJumpImpactAudio();
            }

        }
    }

    public void FullHealth()
    {
        healthSystem.HealComplete();
    }

    public void LatestPlatform(Transform t)
    {
        lastPlatformPos = t.position;
        lastPlatformPos.y += 4f;        
    }
}
