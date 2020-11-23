using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ParticleSystemJobs;

public class ThiefPaticles : MonoBehaviour
{

    public ParticleSystem exitParticle;
    public ParticleSystem enterParticle;
    public GameObject pirateGeo;

    [ContextMenu("ExitParticle")]
    public void PlayExitParticle()
    {
        exitParticle.Play();
        StartCoroutine(WaitForExit());
    }

    [ContextMenu("EnterParticle")]
    public void PlayEnterParticle()
    {
        enterParticle.Play();
        StartCoroutine(WaitForEnter());
    }

    IEnumerator WaitForExit()
    {
        pirateGeo.SetActive(false);
        yield return new WaitUntil(() => exitParticle.isStopped);
        TurnManager.instance.thiefWinPanel.SetActive(true);
    }

    IEnumerator WaitForEnter()
    {
        yield return new WaitUntil(() => enterParticle.isStopped);
        pirateGeo.SetActive(true);
    }
}
