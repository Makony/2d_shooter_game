using UnityEngine;
using System.Collections;

public class SecurityCamera : MonoBehaviour
{
    public SpriteRenderer eyeSpriteRenderer;
    public AudioClip detectionBeepSound;

    private AudioSource audioSource;
    private bool isDetecting = false;

    void Awake()
    {
        detectionBeepSound = Resources.Load<AudioClip>("Audios/detectionBeepSound");

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) { audioSource = gameObject.AddComponent<AudioSource>(); }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isDetecting)
        {
            StartCoroutine(DetectionSequence(other));
        }
    }

    private IEnumerator DetectionSequence(Collider2D playerCollider)
    {
        isDetecting = true;

        if (detectionBeepSound != null)
        {
            audioSource.pitch = detectionBeepSound.length / 3.0f;
            audioSource.clip = detectionBeepSound;
            audioSource.loop = false;
            audioSource.Play();
        }

        float detectionStartTime = Time.time;
        while (Time.time < detectionStartTime + 3f)
        {
            if (playerCollider == null || !playerCollider.IsTouching(GetComponent<Collider2D>()))
            {
                audioSource.Stop();
                if (eyeSpriteRenderer != null) eyeSpriteRenderer.color = Color.green;
                isDetecting = false;
                yield break;
            }

            if (eyeSpriteRenderer != null) eyeSpriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            if (eyeSpriteRenderer != null) eyeSpriteRenderer.color = Color.green;
            yield return new WaitForSeconds(0.1f);
        }

        audioSource.Stop();

        if (playerCollider != null && playerCollider.IsTouching(GetComponent<Collider2D>()))
        {
            SoundTheAlarm();
        }
        else
        {
            isDetecting = false;
        }


    }
    
    public void SoundTheAlarm()
    {
        if (eyeSpriteRenderer != null) eyeSpriteRenderer.color = Color.red;
        LevelManager.Instance.TriggerGlobalAlarm();
    }
}
