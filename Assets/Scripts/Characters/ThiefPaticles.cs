using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ParticleSystemJobs;

public class ThiefPaticles : MonoBehaviour
{

    public ParticleSystem exitParticle;
    public ParticleSystem enterParticle;
    public GameObject pirateGeo;
    public AudioSource audioSource;
    public AudioClip clipEntrance;
    public AudioClip clipExit;

    /// <summary>
    /// Plays the exit particle and then starts a wait coroutine
    /// </summary>
    [ContextMenu("ExitParticle")]
    public void PlayExitParticle()
    {
        exitParticle.Play();
        GameUtilities.PlayAudioClip(ref clipExit, ref audioSource);
        StartCoroutine(WaitForExit());
    }

    /// <summary>
    /// Plays the enter particle and then starts a wait coroutine
    /// </summary>
    [ContextMenu("EnterParticle")]
    public void PlayEnterParticle()
    {
        enterParticle.Play();
        GameUtilities.PlayAudioClip(ref clipEntrance, ref audioSource);
        StartCoroutine(WaitForEnter());
    }

    /// <summary>
    /// Sets the pirate's geometry inactive and then delays until the the particle effect is over
    /// then it turns on the win panel
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitForExit()
    {
        pirateGeo.SetActive(false);
        yield return new WaitUntil(() => exitParticle.isStopped);
        TurnManager.instance.thiefWinPanel.SetActive(true);
    }

    /// <summary>
    /// Waits until the end of the particle effect and then sets the parite's geometry to active.
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitForEnter()
    {
        yield return new WaitUntil(() => enterParticle.isStopped);
        pirateGeo.SetActive(true);
    }
}
