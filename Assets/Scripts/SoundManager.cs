using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClipRefsSO audioClipRefsSO;
    private void Start()
    {
        DeliveryManager.Instance.OnRecipeSuccess += DeliveryManagerOnRecipeSuccess;
        DeliveryManager.Instance.OnRecipeFailed += DeliveryManagerOnRecipeFailed;
        CuttingCounter.OnAnyCut += CuttingCounterOnAnyCut;
        Player.Instance.OnPickedUpSomething += PlayerOnPickedUpSomething;
    }

    private void PlayerOnPickedUpSomething(object sender, EventArgs e)
    {
        PlaySound(audioClipRefsSO.objectPickup, Player.Instance.transform.position);
        
    }

    private void CuttingCounterOnAnyCut(object sender, EventArgs e)
    {
        CuttingCounter cuttingCounter = sender as CuttingCounter;
        PlaySound(audioClipRefsSO.chop, cuttingCounter.transform.position);
        
    }

    private void DeliveryManagerOnRecipeSuccess(object sender, EventArgs e)
    {
        PlaySound(audioClipRefsSO.deliverySuccess, Camera.main.transform.position);
    }

    private void DeliveryManagerOnRecipeFailed(object sender, EventArgs e)
    {
        PlaySound(audioClipRefsSO.deliveryFail, Camera.main.transform.position);
    }

    private void PlaySound(AudioClip[] audioClips, Vector3 position, float volume = 1f)
    {
        PlaySound(audioClips[UnityEngine.Random.Range(0, audioClips.Length - 1)], position, volume);
    }
    
    private void PlaySound(AudioClip audioClip, Vector3 position, float volume = 1f)
    {
        AudioSource.PlayClipAtPoint(audioClip, position, volume);
    }
}
