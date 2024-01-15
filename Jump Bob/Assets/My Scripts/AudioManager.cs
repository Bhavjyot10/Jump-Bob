using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource Jump;
    public AudioSource Dash;
    public AudioSource LevelFailed;
    public AudioSource LevelSuccess;
    public AudioSource JumpPickup;
    public AudioSource DashPickup;
    public AudioSource JumpImpact;

    public void PlayJumpAudio()
    {
        Jump.Play();
    }

    public void PlayDashAudio()
    {
        Dash.Play();
    }

    public void PlayFailedAudio()
    {
        LevelFailed.Play();
    }

    public void PlaySuccessAudio()
    {
        LevelSuccess.Play();
    }

    public void PlayJumpPickupAudio()
    {
        JumpPickup.Play();
    }

    public void PlayDashPickupAudio()
    {
        DashPickup.Play();
    }

    public void PlayJumpImpactAudio()
    {
        JumpImpact.Play();
    }



}
