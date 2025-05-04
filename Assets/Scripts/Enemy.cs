using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{

    //Note: not complete. Just added this from the NavMesh2D video that I sent you so I could test out how all of this works.
    //Gonna add usable behaviours and etc. from the now obsolet "EnemyMovement" script. (Rotation and etc.)

    /*03.05, A: added auto assign "target" under Start()
     * 
     * 
     * 
     */








    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] 
    Transform target;


    NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;


        //03.05,A: auto assign Player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            target = playerObj.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(target.position);
    }


  /*  private void OnDestroy()
    {
        
    }
  */
}
