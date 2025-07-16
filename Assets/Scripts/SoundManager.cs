using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;


    [Header("Audio Sources")]
    public AudioSource sfxSource;
    public AudioSource chaseMusicSource;
    public AudioSource backgroundMusicSource;

    private float SFXvolume;

    [Header("Audio Clips")]
    public AudioClip playerFootstepClip;
    public AudioClip reloadSoundClip;
    public AudioClip playerShootClip;
    public AudioClip enemyShootClip;
    public AudioClip enemyChaseMusicClip;
    public AudioClip keyCollectClip;
    public AudioClip keyAllCollectedClip;
    public AudioClip playerDeathClip;
    public AudioClip enemyDeathClip;
    public AudioClip boxDestroyClip;
    public AudioClip bigButtonsClickClip;
    public AudioClip littleButtonsClickClip;
    public AudioClip backgroundMusicClip;
    public AudioClip arrowShootClip;
    public AudioClip healingSoundClip;
    public AudioClip teleportSoundClip;
    public AudioClip buttonHoverClip;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Play a random footstep sound
    public void PlayFootstep()
    {
        sfxSource.PlayOneShot(playerFootstepClip);

    }


    public void PlayerShootSound()
    {
        sfxSource.PlayOneShot(playerShootClip);
    }

    public void ReloadSound()
    {
        sfxSource.PlayOneShot(reloadSoundClip);
    }

    public void EnemyShootSound()
    {
        sfxSource.PlayOneShot(enemyShootClip);
    }

    public void KeyCollectSound()
    {
        sfxSource.PlayOneShot(keyCollectClip);
    }

    public void KeyAllCollectedSound()
    {
        sfxSource.PlayOneShot(keyAllCollectedClip);
    }

    public void StartChaseMusic()
    {
        if (!chaseMusicSource.isPlaying)
        {
            chaseMusicSource.clip = enemyChaseMusicClip;
            chaseMusicSource.loop = true;
            chaseMusicSource.Play();
        }
    }

    public void StopChaseMusic()
    {
        if (chaseMusicSource.isPlaying)
        {
            chaseMusicSource.Stop();
        }
    }

    public void PlayerDeathSound()
    {
        sfxSource.PlayOneShot(playerDeathClip);
    }

    public void EnemyDeathSound()
    {
        sfxSource.PlayOneShot(enemyDeathClip);
    }

    public void BoxDestroySound()
    {
        sfxSource.PlayOneShot(boxDestroyClip);
    }

    public void ButtonClickSound()
    {
        sfxSource.PlayOneShot(bigButtonsClickClip);
    }

    public void ArrowShootSound()
    {
        sfxSource.PlayOneShot(arrowShootClip);
    }

    public void HealingSound()
    {
        sfxSource.PlayOneShot(healingSoundClip);
    }

    public void TeleportSound()
    {
        sfxSource.PlayOneShot(teleportSoundClip);
    }

    public void ButtonHoverSound()
    {
        sfxSource.PlayOneShot(buttonHoverClip);
    }

    public void PlayLittleButtonClickSound()
    {
        sfxSource.PlayOneShot(littleButtonsClickClip);
    }

    public void MuteSFX()
    {
        SFXvolume = sfxSource.volume;
        sfxSource.volume = 0f;
    }

    public void UnmuteSFX()
    {
        sfxSource.volume = SFXvolume;
    }
}
