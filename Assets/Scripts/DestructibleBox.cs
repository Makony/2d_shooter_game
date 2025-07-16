using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DestructibleBox : MonoBehaviour
{
    private Collider2D boxCollider;

    public Boolean destroyed = false;
    public Boolean didPlayerDestroy = false;

    void Awake()
    {
        boxCollider = GetComponent<Collider2D>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet") && collision.gameObject.layer == LayerMask.NameToLayer("Bullet (friendly)"))
        {
            didPlayerDestroy = true;
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Bullet"))
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (destroyed) return;

        destroyed = true;

        if (didPlayerDestroy)
        {
            LevelManager.Instance.Boxbuff(gameObject.transform.position);
        }
        else
        {
            LevelManager.Instance.BoxEnemyDrop(gameObject.transform.position);
        }
        
    }
}