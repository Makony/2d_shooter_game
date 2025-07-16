using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SecurityCamera : MonoBehaviour
{
    public SpriteRenderer eyeSprite;
    public AudioClip detectionBeepSound;

    private AudioSource audioSource;
    private bool isDetecting = false;
    private GameObject player;

    private float rotationLimit = 30f;
    private Quaternion initialRotation;

    private List<Enemy> nearbyEnemies = new List<Enemy>();


    void Awake()
    {
        detectionBeepSound = Resources.Load<AudioClip>("Audios/detectionBeepSound");

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) { audioSource = gameObject.AddComponent<AudioSource>(); }
        Transform eye = transform.Find("eye");
        if (eye != null)
        {
            eye.TryGetComponent<SpriteRenderer>(out eyeSprite);
        }
        else
        {
            Debug.Log("no eye :O we facked up :o");
        }
        initialRotation = transform.rotation;
    }



    void Update()
    {
        if (LevelManager.Instance.player != null && isDetecting)
        {
            player = LevelManager.Instance.player;
            Vector2 direction = (player.transform.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle);

            float angleDifference = Mathf.DeltaAngle(initialRotation.eulerAngles.z, targetRotation.eulerAngles.z);

            float clampedAngleDifference = Mathf.Clamp(angleDifference, -rotationLimit, rotationLimit);

            Quaternion cappedRotation = initialRotation * Quaternion.Euler(0, 0, clampedAngleDifference);
            transform.rotation = Quaternion.Slerp(transform.rotation, cappedRotation, Time.deltaTime * 1f);
            float step = 1f * Time.deltaTime;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, cappedRotation, step);
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isDetecting)
        {
            StartCoroutine(DetectionSequence(other));
        }
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null && !nearbyEnemies.Contains(enemy))
            {
                nearbyEnemies.Add(enemy);
                enemy.OnDeath += HandleEnemyDeathInView;
            }
        }

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null && nearbyEnemies.Contains(enemy))
            {
                enemy.OnDeath -= HandleEnemyDeathInView;
                nearbyEnemies.Remove(enemy);
            }
        }
    }

    private void HandleEnemyDeathInView()
    {
        SoundTheAlarm();
        DialogManager.Instance.ShowDialogWithTimer("The Camera saw an enemy die. \n\nRIP... \n\n to you :_:", 5f);
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
        float interval = 0.5f;
        while (Time.time < detectionStartTime + 3f)
        {
            if (playerCollider == null || !playerCollider.IsTouching(GetComponent<Collider2D>()))
            {
                audioSource.Stop();
                if (eyeSprite != null) eyeSprite.color = Color.green;
                isDetecting = false;
                yield break;
            }

            if (eyeSprite != null) eyeSprite.color = Color.red;
            yield return new WaitForSeconds(interval);
            if (eyeSprite != null) eyeSprite.color = Color.green;
            yield return new WaitForSeconds(interval);
            interval /= 1.5f;
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
        if (eyeSprite != null) eyeSprite.color = Color.red;
        CCTVManager.Instance.Detected();
        PolygonCollider2D CCTVcollider = GetComponent<PolygonCollider2D>();

        if (CCTVcollider != null)
        {
            CCTVcollider.enabled = false;
        }
        else
        {
            Debug.LogWarning("The CCTV " + name + " is missing a SecurityCamera script.", gameObject);
        }
        LevelManager.Instance.TriggerGlobalAlarm();
        LevelManager.Instance.IsDetected = true;
    }
}
