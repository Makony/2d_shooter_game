using UnityEngine;
using System.Collections;

public class Arrow : MonoBehaviour
{
    private float damage = 2f;

    void Start()
    {
        Destroy(gameObject, 10f);
        StartCoroutine(EnableColliderAfterDelay());
    }

    private IEnumerator EnableColliderAfterDelay()
    {
        GetComponent<Collider2D>().enabled = false;

        yield return new WaitForSeconds(0.25f);

        GetComponent<Collider2D>().enabled = true;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if (player != null)
            {
                player.Health -= damage;
                LevelManager.Instance.UpdateAllStats();
                Destroy(gameObject);
                if (player.Health <= 0)
                {
                    player.Die();
                }
            }
        }
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.Health -= damage;
                LevelManager.Instance.UpdateAllStats();
                Destroy(gameObject);
            }
        }
        
        if (collision.gameObject.CompareTag("Box"))
        {
            Destroy(collision.gameObject);
        }
        Destroy(gameObject);
    }
}