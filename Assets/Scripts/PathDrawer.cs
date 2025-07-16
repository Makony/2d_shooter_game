using UnityEngine;
using System.Collections.Generic;


public class PathDrawer : MonoBehaviour
{
    private Transform playerTransform;
    private Transform exitTransform;
    private LineRenderer lineRenderer;
    private JPS jpsPathfinder;
    public Material litMaterial;



    public static PathDrawer Instance { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }
    


    public void CalculatePath()
    {
        if (lineRenderer == null) { lineRenderer = gameObject.AddComponent<LineRenderer>(); }
        lineRenderer.material = litMaterial;
        SetupLineRenderer();
        Debug.Log("We calculate sht");

        GameObject player = LevelManager.Instance.player;
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("no transform. like Titan Fall 3 .O (Pathfindinghelper)");
            return;
        }

        GameObject exit = MapManager.Instance.transform.Find("FinishingRoom(Clone)").gameObject;
        if (exit != null)
        {
            exitTransform = exit.transform;
        }
        else
        {
            Debug.LogError("no exit found which is impossible but :U");
            return;
        }

        Grid pathfindingGrid = GridManager.Instance.GetGrid();
        if (pathfindingGrid != null)
        {
            jpsPathfinder = new JPS(pathfindingGrid);
        }
        else
        {
            Debug.LogError("PathfinderController: Pathfinding grid not found in GridManager!");
            return;
        }

        DrawPath();
    }

    public void DrawPath()
    {
        if (playerTransform == null || exitTransform == null || jpsPathfinder == null)
        {
            return;
        }

        Node startNode = GridManager.Instance.WorldToNode(playerTransform.position);
        Node goalNode = GridManager.Instance.WorldToNode(exitTransform.position + new Vector3(10f,10f));

        Node walkableStartNode = GridManager.Instance.FindNearestWalkableNode(startNode);
        Node walkableGoalNode = GridManager.Instance.FindNearestWalkableNode(goalNode);

        if (walkableStartNode == null || walkableGoalNode == null)
        {
            Debug.LogWarning("Could not find a walkable start or goal node for the path.");
            lineRenderer.positionCount = 0;
            return;
        }

        List<Node> path = jpsPathfinder.FindPath(walkableStartNode, walkableGoalNode);

        //Draw the path using the LineRenderer
        if (path != null && path.Count > 0)
        {
            lineRenderer.positionCount = path.Count;
            for (int i = 0; i < path.Count; i++)
            {
                // Convert each node in the path back to a world position
                Vector3 worldPosition = GridManager.Instance.NodeToWorld(path[i]);
                lineRenderer.SetPosition(i, worldPosition);
            }
        }
        else
        {
            // If no path is found, clear the line
            lineRenderer.positionCount = 0;
            Debug.LogWarning("No path found to the exit.");
        }
    }

    private void SetupLineRenderer()
    {
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.sortingOrder = 1;
    }
}