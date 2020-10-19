using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class VolumeControls : MonoBehaviour
{
    public AudioMixer masterMixer;

    public void SetMasterVol(float masterLvl)
    {
        masterMixer.SetFloat("volMaster", masterLvl);
    }

    public void SetSfxLv(float sfxLvl)
    {
        masterMixer.SetFloat("volSFXs", sfxLvl);
    }

    public void SetMusicLv(float musicLvl)
    {
        masterMixer.SetFloat("volSDTs", musicLvl);
    }

    public void SetVoiceContLv(float voiceContLvl)
    {
        masterMixer.SetFloat("voiceContVol", voiceContLvl);
    }

}
