using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
    MovementComponent m_movementComponent;
    [SerializeField]
    GameManager gameManager;

    [SerializeField]
    Tube tube;
    

    [HideInInspector]
    public Vector3 offsetFromCenter;

    Vector3 touchInitPos = Vector3.zero;

    Touch touch;

    void Start()
    {
        tube = GameObject.FindGameObjectWithTag("Tube").GetComponent<Tube>();
        m_movementComponent = GetComponent<MovementComponent>();    
    }

    void Update()
    {
        if (gameManager.gameState != GameManager.State.Over)
        {
            if (tube)
            {
                if (GetComponent<Rigidbody>().velocity.magnitude > 14)
                    m_movementComponent.SetMovementDirection(Vector3.right);
                if (Input.GetAxisRaw("Horizontal") == -1)
                {
                    if (!m_movementComponent.IsInAir())
                    {
                        m_movementComponent.ApplyForce(Vector3.right, 5);
                    }
                    m_movementComponent.SetMovementDirection(Vector3.forward);
                }
                else if (Input.GetAxisRaw("Horizontal") == 1)
                {
                    if (!m_movementComponent.IsInAir())
                    {
                        m_movementComponent.ApplyForce(Vector3.right, 5);
                    }
                    m_movementComponent.SetMovementDirection(Vector3.back);
                }

                TouchInput();
            }
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "obstacle" && gameManager.gameState == GameManager.State.Running)
        {
            gameManager.PlayerDied();
        }
    }

    void TouchInput()
    {
        if (Input.touchCount == 1)
        {
            touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                touchInitPos = touch.position;
            }

            if (touch.phase == TouchPhase.Moved)
            {
                if (touch.position.x > touchInitPos.x + 1)
                {
                }
                else if (touch.position.x < touchInitPos.x - 1)
                {
                }
                touchInitPos = touch.position;
            }
        }
    }
}
