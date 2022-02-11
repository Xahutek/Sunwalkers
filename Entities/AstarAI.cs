using UnityEngine;
using System.Collections;
using Pathfinding;

[RequireComponent(typeof(Seeker),typeof(Rigidbody))]
public class AstarAI : MonoBehaviour
{
    public Seeker seeker;

    public Transform targetObject = null;
    public Vector3 targetPosition;

    public Path path=null;
    public bool reachedEndOfPath;
    public float speed = 3;
    public float reconciderPathTime=0.5f;
    private float nextWaypointDistance = 3, reconciderTimer;
    private int currentWaypoint = 0;

    public bool stop = false, moving;

    private void Awake()
    {
        seeker = GetComponent<Seeker>();
        targetPosition = transform.position;
    }
    public void Start()
    {        
        seeker.StartPath(transform.position, targetPosition, OnPathComplete); //I also do this every 0.5 seconds.
    }
    public void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
/*#if !UNITY_EDITOR*/
        else
            Debug.LogError("PATH FEEDBACK: Path to " + targetPosition + " failed! Reason for failure is: " + p.errorLog);
/*#endif*/
    }

    public void Recalculate(Vector3 newTargetPos)
    {
        targetPosition = newTargetPos;
        targetObject = null;

        reconciderTimer = Globe.time + reconciderPathTime;
        seeker.StartPath(transform.position, targetPosition, OnPathComplete);
    }
    public void FixedUpdate()
    {
        if (targetObject)
            targetPosition = targetObject.transform.position;

        if (Globe.time > reconciderTimer&&!stop)
        {
            reconciderTimer = Globe.time + reconciderPathTime;
            seeker.StartPath(transform.position, targetPosition, OnPathComplete);
        }
        if (path == null) return;

        if (stop)
            return;

        // Check in a loop if we are close enough to the current waypoint to switch to the next one.
        // We do this in a loop because many waypoints might be close to each other and we may reach
        // several of them in the same frame.
        reachedEndOfPath = false;
        // The distance to the next waypoint in the path
        float distanceToWaypoint;
        while (true)
        {
            // If you want maximum performance you can check the squared distance instead to get rid of a
            // square root calculation. But that is outside the scope of this tutorial.
            distanceToWaypoint = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);
            if (distanceToWaypoint < nextWaypointDistance)
            {
                // Check if there is another waypoint or if we have reached the end of the path
                if (currentWaypoint + 1 < path.vectorPath.Count)
                {
                    currentWaypoint++;
                }
                else
                {
                    // Set a status variable to indicate that the agent has reached the end of the path.
                    // You can use this to trigger some special code if your game requires that.
                    reachedEndOfPath = true;
                    break;
                }
            }
            else
            {
                break;
            }
        }
        // Slow down smoothly upon approaching the end of the path
        // This value will smoothly go from 1 to 0 as the agent approaches the last waypoint in the path.
        var speedFactor = reachedEndOfPath ? Mathf.Sqrt(distanceToWaypoint / nextWaypointDistance) : 1f;
        // Direction to the next waypoint
        // Normalize it so that it has a length of 1 world unit
        Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
        // Multiply the direction by our desired speed to get a velocity
        Vector3 velocity = dir * speed * speedFactor;
        // Move the agent using the Rigidbody component
        if(dir.magnitude>0.25f)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.FromToRotation(Vector3.forward,dir), 4*Globe.fixedDeltaTime);

        transform.position += velocity * Globe.fixedDeltaTime;
    }
}