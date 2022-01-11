using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    [SerializeField] GameObject[] Boids;

    [SerializeField] float speed, speed_Max, speed_rotation;

    [Header("Weight")]    //weight of each property means how much of that factor influence final direction of the void
    [SerializeField] float weight_Allignment, weight_Cohession, weight_Avoidance;

    [Header("Collision")]
    [SerializeField] float radius_NeighbourDetection, radius_CollisionRadius;

    void Start()
    {
        Boids = GameObject.FindGameObjectsWithTag("boid");
        for (int i = 0; i < Boids.Length; i++)
        {
            Vector3 dir = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
            Boids[i].GetComponent<Rigidbody>().velocity = dir;
        }
    }

    void Update()
    {
        UpdateEachBoid();
    }

    void UpdateEachBoid()
    {
        for (int i = 0; i < Boids.Length; i++)
        {
            Vector3 direction;

            direction = Allignement(Boids[i]) * weight_Allignment;

            direction += Cohession(Boids[i]) * weight_Cohession;

            direction += Avoidance(Boids[i]) * weight_Avoidance;

            float forwardAngle = Mathf.Atan2(direction.y, direction.y);
            forwardAngle *= 57.2f;//ConvertToAngle

            //Collision avoidance
            for (int k = -5; k < 6; k++)
            {
                float testAngle = forwardAngle + k;//angle offset lefft to right from boids forward 

                Vector3 RayDir = new Vector3(Mathf.Cos(testAngle), 0, Mathf.Sin(testAngle));

                RaycastHit hit;
                Ray ray = new Ray(Boids[i].transform.position, direction);

                if (Physics.Raycast(ray, out hit, 1))
                {
                    if (hit.transform.CompareTag("obstacle"))
                    {
                        Boids[i].transform.position = new Vector3(-Boids[i].transform.position.x, -Boids[i].transform.position.y, transform.position.z);

                        Vector3 dir = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
                        Boids[i].GetComponent<Rigidbody>().velocity = dir;
                    }
                }

                if (i == 0)
                    Debug.DrawRay(ray.origin, ray.direction * 1, Color.red);
            }
          
            Vector3 velocity = Boids[i].GetComponent<Rigidbody>().velocity;

            //Add the calculated velocity to the existing to make it accelarate in the direction
            velocity += direction * Mathf.Clamp((velocity * speed).magnitude, 0, speed_Max) * Time.deltaTime;

            Boids[i].transform.rotation = Quaternion.LookRotation(velocity * Time.deltaTime);
         
            Boids[i].GetComponent<Rigidbody>().velocity = velocity;


        }
    }

    Vector3 Allignement(GameObject aBoid)
    {
        Vector3 avrgDirection = Vector3.zero;

        Collider[] neighbours = Physics.OverlapSphere(aBoid.transform.position, radius_NeighbourDetection);  //Get list of boids in range


        for (int i = 0; i < neighbours.Length; i++)
        {
            avrgDirection += neighbours[i].transform.forward;           //Add the forward direction of all the neighbouring boids
        }

        avrgDirection /= neighbours.Length; //Find the average forward direction of all the neighbouring boids to add it to the new velocity
        avrgDirection.Normalize();

        return avrgDirection;
    }

    Vector3 Cohession(GameObject aBoid)
    {
        Vector3 avrgPosition = Vector3.zero;

        Collider[] neighbours = Physics.OverlapSphere(aBoid.transform.position, radius_NeighbourDetection); //Get list of boids in range

        for (int i = 0; i < neighbours.Length; i++)
        {
            avrgPosition += neighbours[i].transform.position;   //Add the positions of all the neighbouring boids
        }

        avrgPosition /= neighbours.Length; //Get the center position of all the boids by averaging the total 

        return (avrgPosition - aBoid.transform.position).normalized;        //return the direction from current boid to the flock's center to be added to the velocity's direction
    }

    Vector3 Avoidance(GameObject aBoid)
    {
        Vector3 avrgPosition = Vector3.zero;

        Collider[] neighbours = Physics.OverlapSphere(aBoid.transform.position, radius_CollisionRadius);

        for (int i = 0; i < neighbours.Length; i++)
        {
            avrgPosition += neighbours[i].transform.position;
        }

        avrgPosition /= neighbours.Length;

        return (aBoid.transform.position - avrgPosition).normalized;
    }
}
