using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ParticleSystemJobs;

public class TestPaticles : MonoBehaviour
{

    public ParticleSystem exitParticle;
    public ParticleSystem enterParticle;
    public GameObject pirate;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    [ContextMenu("ExitParticle")]
    public void ExitParticle()
    {
        exitParticle.Play();
        StartCoroutine("WaitForExit");
    }

    [ContextMenu("EnterParticle")]
    public void EnterParticle()
    {
        enterParticle.Play();
        StartCoroutine("WaitForEnter");
    }

    IEnumerator WaitForExit()
    {
        yield return new WaitForSeconds(1);
        pirate.SetActive(false);
    }

    IEnumerator WaitForEnter()
    {
        yield return new WaitForSeconds(1);
        pirate.SetActive(true);
    }
}
