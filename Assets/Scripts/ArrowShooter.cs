using UnityEngine;
using System.Collections;

public class ArrowShooter : MonoBehaviour
{
    public GameObject arrowPrefab;
    private Animator animator;

    public float arrowSpeed = 3f;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void ShootArrow(Vector2 targetPosition)
    {
        if (animator != null)
        {
            animator.SetBool("IsShooting", true);
        }

        SoundManager.Instance.ArrowShootSound();

        GameObject arrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity);

        Vector2 shootDirection = (targetPosition - (Vector2)transform.position).normalized;
        arrow.GetComponent<Rigidbody2D>().linearVelocity = shootDirection * arrowSpeed;

        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
        arrow.transform.rotation = Quaternion.Euler(0, 0, angle + 180f);

        StartCoroutine(ResetShootingAnimation());
    }

    private IEnumerator ResetShootingAnimation()
    {
        yield return new WaitForSeconds(1f);

        if (animator != null)
        {
            animator.SetBool("IsShooting", false);
        }
    }
}