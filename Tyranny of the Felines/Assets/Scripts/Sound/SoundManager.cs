using System;
using System.Collections;
using System.Collections.Generic;
using Matryoshka.UI;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

public class SoundManager : NetworkBehaviour
{
    private static SoundManager _instance;
    
    public static SoundManager Singleton => _instance;
    private AudioSource MusicSoundPlayer;
    private AudioSource SFXSoundPlayer;
    private float SFXVolume;
    private float MusicVolume;

    private bool bigWalking;
    private bool salumonWalking;
    private bool wazoWalking;

    private bool mittensSpeaking;

    private List<AudioClip> BigOverheadSlash = new List<AudioClip>();
    private List<AudioClip> BigShieldWall = new List<AudioClip>();
    private List<AudioClip> BigSlash = new List<AudioClip>();
    private List<AudioClip> BigWalk = new List<AudioClip>();

    private List<AudioClip> Furball = new List<AudioClip>();

    private List<AudioClip> Click = new List<AudioClip>();

    private List<AudioClip> SalumonWalk = new List<AudioClip>();

    private List<AudioClip> TowerDamaged = new List<AudioClip>();

    public void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);

        MusicSoundPlayer = this.gameObject.AddComponent<AudioSource>();
        MusicSoundPlayer.loop = true;
        MusicSoundPlayer.volume = 0.25f;
        
        SFXSoundPlayer = this.gameObject.AddComponent<AudioSource>();
        SFXSoundPlayer.loop = true;
        SFXSoundPlayer.volume = 0.25f;

        bigWalking = false;
        salumonWalking = false;
        wazoWalking = false;

        mittensSpeaking = false;
        
        // create lists for sound groups
        BigOverheadSlash.AddRange(new List<AudioClip>
        {
            SoundClips.BigOverheadSlash1, SoundClips.BigOverheadSlash2
        });
        BigShieldWall.AddRange(new List<AudioClip>
        {
            SoundClips.BigShieldWall1, SoundClips.BigShieldWall2, SoundClips.BigShieldWall3
        });
        BigSlash.AddRange(new List<AudioClip>
        {
            SoundClips.BigSlash1, SoundClips.BigSlash2, SoundClips.BigSlash3
        });
        BigWalk.AddRange(new List<AudioClip>
        {
            SoundClips.BigWalk1, SoundClips.BigWalk2, SoundClips.BigWalk3, SoundClips.BigWalk4,
            SoundClips.BigWalk5, SoundClips.BigWalk6, SoundClips.BigWalk7, SoundClips.BigWalk8,
            SoundClips.BigWalk9, SoundClips.BigWalk10
        });
        Furball.AddRange(new List<AudioClip>
        {
            SoundClips.Furball1, SoundClips.Furball2, SoundClips.Furball3, SoundClips.Furball4,
            SoundClips.Furball5, SoundClips.Furball6, SoundClips.Furball7, SoundClips.Furball8,
            SoundClips.Furball9, SoundClips.Furball10, SoundClips.Furball11, SoundClips.Furball12,
            SoundClips.Furball13, SoundClips.Furball14, SoundClips.Furball15
        });
        Click.AddRange(new List<AudioClip>
        {
            SoundClips.Click1, SoundClips.Click2, SoundClips.Click3, SoundClips.Click4,
            SoundClips.Click5
        });
        SalumonWalk.AddRange(new List<AudioClip>
        {
            SoundClips.SalumonWalking1, SoundClips.SalumonWalking2
        });
        TowerDamaged.AddRange(new List<AudioClip>
        {
            SoundClips.TowerDamaged1, SoundClips.TowerDamaged2, SoundClips.TowerDamaged3
        });

        // start playing the menu theme
        PlayMenuMusic();
    }
    
    private void FixedUpdate()
    {
        SFXVolume = (float) OptionsUI.Singleton.SFXVolumeSlider.value / 100;
        MusicVolume = (float) OptionsUI.Singleton.MusicVolumeSlider.value / 100;
        MusicSoundPlayer.volume = 0.25f * MusicVolume;
        SFXSoundPlayer.volume = 0.25f * SFXVolume;
    }

    public void PlayBossMusic()
    {
        MusicSoundPlayer.Stop();
        MusicSoundPlayer.clip = SoundClips.BossMusic;
        MusicSoundPlayer.PlayDelayed(0.5f);
    }

    public void PlayMenuMusic()
    {
        MusicSoundPlayer.Stop();
        MusicSoundPlayer.clip = SoundClips.MenuMusic;
        MusicSoundPlayer.PlayDelayed(0.5f);
    }

    public void PlayCharacterMusic()
    {
        MusicSoundPlayer.Stop();
        MusicSoundPlayer.clip = SoundClips.CharacterMusic;
        MusicSoundPlayer.PlayDelayed(0.5f);
    }
    
    public void PlayBigDeath()
    {
        SFXSoundPlayer.PlayOneShot(SoundClips.BigDeath, 1.0f*SFXVolume);
    }
    
    // unsure where to play this
    public void PlayBigWhistle()
    {
        SFXSoundPlayer.PlayOneShot(SoundClips.BigWhistle, 1.0f * SFXVolume);
    }
    
    public void PlayBigWalking()
    {
        if (!bigWalking)
        {
            SFXSoundPlayer.PlayOneShot(BigWalk[UnityEngine.Random.Range(0,10)], 0.3f*SFXVolume);
            StartCoroutine(BigWalkWait());
        }
    }
    
    private IEnumerator BigWalkWait()
    {
        bigWalking = true;
        yield return new WaitForSeconds(0.5f);
        bigWalking = false;
    }
    
    public void PlayBigSlash()
    {
        SFXSoundPlayer.PlayOneShot(BigSlash[Random.Range(0,3)], 1.0f * SFXVolume);
    }
    
    public void PlayBigOverheadSlash()
    {
        SFXSoundPlayer.PlayOneShot(BigOverheadSlash[Random.Range(0,2)], 1.0f * SFXVolume);
    }
    
    public void PlayFurballSound()
    {
        // rare chance to play furball growls
        if (Random.Range(0, 10000) == 0)
        {
            SFXSoundPlayer.PlayOneShot(Furball[Random.Range(0, 15)], 1.0f * SFXVolume);
        }
    }
    
    public void PlayMenuClick()
    {
        SFXSoundPlayer.PlayOneShot(Click[Random.Range(0,5)], 1.0f*SFXVolume);
    }
    
    public void PlayBigShieldWall()
    {
        SFXSoundPlayer.PlayOneShot(BigShieldWall[Random.Range(0,3)], 1.0f * SFXVolume);
    }

    public void PlayMittens()
    {
        // rare chance for mittens to meow
        if (Random.Range(0, 5) == 0)
        {
            if (!mittensSpeaking)
            {
                mittensSpeaking = true;
                StartCoroutine(Meows());
            }
        }
    }

    private IEnumerator Meows()
    {
        if (Random.Range(0, 2) == 0)
        {
            PlayMittensMeow();
        }
        else
        {
            PlayMittensFakeDeath();
        }

        yield return new WaitForSeconds(1.0f);
        mittensSpeaking = false;
    }
    
    public void PlayMittensMeow()
    {
        SFXSoundPlayer.PlayOneShot(SoundClips.MittensMeow, 1.0f * SFXVolume);
    }
    
    // similar to big swing so I elected to not play it
    public void PlayMittensClawSwipe()
    {
        SFXSoundPlayer.PlayOneShot(SoundClips.MittensClawSwipe, 1.0f * SFXVolume);
    }
    
    public void PlayMittensTailSwipe()
    {
        SFXSoundPlayer.PlayOneShot(SoundClips.MittensTailSwipe, 1.0f * SFXVolume);
    }
    
    public void PlayMittensSpit()
    {
        SFXSoundPlayer.PlayOneShot(SoundClips.MittensSpit, 1.0f * SFXVolume);
    }

    public void PlayMittensSpikes()
    {
        StartCoroutine(Spikes());
    }

    private IEnumerator Spikes()
    {
        PlayMittensTailSwipe();
        yield return new WaitForSeconds(1.5f);
        PlayMittensImpale();
    }
    
    public void PlayMittensImpale()
    {
        SFXSoundPlayer.PlayOneShot(SoundClips.MittensImpale, 1.0f * SFXVolume);
    }
    
    public void PlayMittensScream()
    {
        SFXSoundPlayer.PlayOneShot(SoundClips.MittensScream, 1.0f * SFXVolume);
    }
    
    public void PlayMittensFakeDeath()
    {
        SFXSoundPlayer.PlayOneShot(SoundClips.MittensFakeDeath, 1.0f * SFXVolume);
    }
    
    public void PlayMittensTeleport()
    {
        SFXSoundPlayer.PlayOneShot(SoundClips.MittensTeleport, 1.0f * SFXVolume);
    }
    
    // dont know where to play this
    public void PlaySalumonBubbles()
    {
        SFXSoundPlayer.PlayOneShot(SoundClips.SalumonBubbles, 1.0f * SFXVolume);
    }
    
    public void PlaySalumonDeath()
    {
        SFXSoundPlayer.PlayOneShot(SoundClips.SalumonDeath, 1.0f*SFXVolume);
    }
    
    public void PlaySalumonWalking()
    {
        if (!salumonWalking)
        {
            SFXSoundPlayer.PlayOneShot(SalumonWalk[Random.Range(0,2)], 0.3f*SFXVolume);
            StartCoroutine(SalumonWalkWait());
        }
    }
    
    private IEnumerator SalumonWalkWait()
    {
        salumonWalking = true;
        yield return new WaitForSeconds(0.5f);
        salumonWalking = false;
    }
    
    public void PlaySalumonHeal()
    {
        SFXSoundPlayer.PlayOneShot(SoundClips.SalumonHeal, 1.0f * SFXVolume);
    }
    
    public void PlaySalumonMagicMissile()
    {
        SFXSoundPlayer.PlayOneShot(SoundClips.SalumonMagicMissile, 1.0f * SFXVolume);
    }
    
    public void PlaySalumonBigHeal()
    {
        SFXSoundPlayer.PlayOneShot(SoundClips.SalumonBigHeal, 1.0f * SFXVolume);
    }
    
    // dont know where to play this
    public void PlayTowerDamaged()
    {
        SFXSoundPlayer.PlayOneShot(TowerDamaged[Random.Range(0,3)], 1.0f * SFXVolume);
    }
    
    public void PlayWazoDeath()
    {
        SFXSoundPlayer.PlayOneShot(SoundClips.WazoDeath, 1.0f*SFXVolume);
    }
    
    // dont know where to play this
    public void PlayWazoWhistle()
    {
        SFXSoundPlayer.PlayOneShot(SoundClips.WazoWhistle, 1.0f * SFXVolume);
    }
    
    public void PlayWazoWalking()
    {
        if (!wazoWalking)
        {
            SFXSoundPlayer.PlayOneShot(SoundClips.WazoWalking, 0.5f * SFXVolume);
            StartCoroutine(WazoWalkWait());
        }
    }

    private IEnumerator WazoWalkWait()
    {
        wazoWalking = true;
        yield return new WaitForSeconds(0.3f);
        wazoWalking = false;
    }
    
    public void PlayWazoDash()
    {
        SFXSoundPlayer.PlayOneShot(SoundClips.WazoDash, 1.0f * SFXVolume);
    }
    
    public void PlayWazoFeatherThrow()
    {
        SFXSoundPlayer.PlayOneShot(SoundClips.WazoFeatherThrow, 1.0f * SFXVolume);
    }
    
    public void PlayWazoHailOfFeathers()
    {
        SFXSoundPlayer.PlayOneShot(SoundClips.WazoHailOfFeathers, 1.0f * SFXVolume);
    }

}
