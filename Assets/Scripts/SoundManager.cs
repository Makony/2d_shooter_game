using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Sources")]
    public AudioSource sfxSource;
    public AudioSource chaseMusicSource;
    public AudioSource backgroundMusicSource;

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
    public AudioClip buttonsClickClip;
    public AudioClip backgroundMusicClip;
    public AudioClip arrowShootClip;

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
        sfxSource.PlayOneShot(buttonsClickClip);
    }

    public void ArrowShootSound()
    {
        sfxSource.PlayOneShot(arrowShootClip);
    }
}
