using UnityEngine;
using System.Collections.Generic;

public class JPSTester : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GridManager gridManager;
    [SerializeField] private Transform player;
    [SerializeField] private Transform enemy;
    
    [Header("Settings")]
    [SerializeField] private float enemySpeed = 3f;
    [SerializeField] private float repathInterval = 1f;
    
    private JPS pathfinder;
    private List<Node> currentPath;
    private int pathIndex;
    private float repathTimer = 0f;

    void Awake()
    {
        if (gridManager == null) gridManager = FindObjectOfType<GridManager>();
        if (gridManager == null) Debug.LogError("GridManager reference missing!", this);
        if (player == null) Debug.LogError("Player reference missing!", this);
        if (enemy == null) Debug.LogError("Enemy reference missing!", this);
    }

    void Start()
    {
        if (gridManager != null && gridManager.GetGrid() != null)
            pathfinder = new JPS(gridManager.GetGrid());
        else
            Debug.LogError("GridManager not ready!", this);
        
        FindPath();
    }

    void Update()
    {
        repathTimer += Time.deltaTime;
        if (repathTimer >= repathInterval)
        {
            repathTimer = 0f;
            FindPath();
        }
        
        MoveEnemy();
    }
    
    void FindPath()
    {
        if (pathfinder == null || player == null || enemy == null) return;
            
        Node startNode = gridManager.WorldToNode(enemy.position);
        Node targetNode = gridManager.WorldToNode(player.position);
        
        if (startNode != null && targetNode != null && startNode.IsWalkable() && targetNode.IsWalkable())
        {
            currentPath = pathfinder.FindPath(startNode, targetNode);
            pathIndex = 0;
            
            // This part is still useful to prevent a small stutter on repathing.
            if (currentPath != null && currentPath.Count > 0 && currentPath[0] == startNode)
            {
                currentPath.RemoveAt(0);
            }
        }
    }
    
    void MoveEnemy()
    {
        if (currentPath == null || currentPath.Count == 0 || pathIndex >= currentPath.Count)
            return;
        
        Vector2 targetPos = gridManager.NodeToWorld(currentPath[pathIndex]);
        enemy.position = Vector2.MoveTowards(enemy.position, targetPos, enemySpeed * Time.deltaTime);
        
        if (Vector2.Distance(enemy.position, targetPos) < 0.1f)
        {
            pathIndex++;
        }
    }
    
    void OnDrawGizmosSelected()
    {
        if (currentPath == null || gridManager == null) return;
        
        Gizmos.color = Color.green;
        for (int i = 0; i < currentPath.Count - 1; i++)
        {
            Vector2 start = gridManager.NodeToWorld(currentPath[i]);
            Vector2 end = gridManager.NodeToWorld(currentPath[i + 1]);
            Gizmos.DrawLine(start, end);
            Gizmos.DrawSphere(start, 0.1f);
        }
        
        if (currentPath.Count > 0)
        {
            Gizmos.DrawSphere(gridManager.NodeToWorld(currentPath[currentPath.Count - 1]), 0.1f);
        }
    }
}