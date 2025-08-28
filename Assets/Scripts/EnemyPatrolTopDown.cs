using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyPatrolTopDown : MonoBehaviour

{

  public Transform[] waypoints; // Array to hold the waypoints

  public float speed = 2.0f; // Speed of the patrol

  public float waitTime = 2.0f; // Time to wait at each waypoint



  private int currentWaypointIndex = 0; // Index of the current waypoint

  private bool isWaiting = false; // To track if the enemy is currently waiting



  void Start()

  {

    if (waypoints.Length > 0)

    {

      StartCoroutine(Patrol());

    }

  }



  IEnumerator Patrol()

  {

    while (true)

    {

      if (!isWaiting)

      {

        MoveTowardsWaypoint();

        // Check if the enemy has reached the waypoint

        if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position) < 0.1f)

        {

          StartCoroutine(WaitAtWaypoint());

        }

      }

      yield return null; // Wait for the next frame

    }

  }



  void MoveTowardsWaypoint()

  {

    if (waypoints.Length == 0)

      return;



    // Calculate the direction to the current waypoint

    Vector3 direction = (waypoints[currentWaypointIndex].position - transform.position).normalized;

    float step = speed * Time.deltaTime; // Move speed per frame



    // Move the enemy towards the waypoint

    transform.position = Vector3.MoveTowards(transform.position, waypoints[currentWaypointIndex].position, step);

     

    // Flip the enemy to face the direction of movement

    FlipEnemy(direction);

  }



  void FlipEnemy(Vector3 direction)

  {

    if (direction.magnitude > 0.01f) // If moving

    {

      float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

      transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90)); // Adjust the angle if needed

    }

  }



  IEnumerator WaitAtWaypoint()

  {

    isWaiting = true;

    yield return new WaitForSeconds(waitTime); // Wait for the specified time

    isWaiting = false;

    currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length; // Move to the next waypoint

  }

}


