using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorSFX : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    public void PlaySFX(AudioClip audioClip)
    {
        audioSource.PlayOneShot(audioClip, 0.13f);
    }
}
