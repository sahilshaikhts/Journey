using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
    [SerializeField]
    GameManager gameManager;

    [SerializeField]
    Tube tube;

    [HideInInspector]
    public int index = -1;

    public float speed, rotSpeed;

    float radiansFromPoint;

    [HideInInspector]
    public Vector3 offsetFromCenter;
    Vector3 touchInitPos = Vector3.zero;

    Touch touch;

    void Start()
    {
        GetComponent<Rigidbody>().isKinematic = true;

        tube = GameObject.FindGameObjectWithTag("Tube").GetComponent<Tube>();
        radiansFromPoint = (3 * Mathf.PI) / 2;
        offsetFromCenter = new Vector3(0, Mathf.Sin(radiansFromPoint), Mathf.Cos(radiansFromPoint));
        RotateInsidePipe(0);
    }

    void Update()
    {
        if (gameManager.gameState != GameManager.State.Over)
        {
            if (tube)
            {

                if (Input.GetAxisRaw("Horizontal") == -1)
                {
                    RotateInsidePipe(1);
                }
                else if (Input.GetAxisRaw("Horizontal") == 1)
                {
                    RotateInsidePipe(-1);
                }

                TouchInput();

                if (index >= tube.path.Length || index == -1)
                {
                    index = 1;
                    transform.position = tube.path[0];
                }

                if ((tube.path[index] + offsetFromCenter).x - transform.position.x <= 0)
                {
                    index++;
                }
                else
                {
                    transform.position += speed * (((tube.path[index] + offsetFromCenter) - transform.position).normalized)*Time.deltaTime;
                }
            }
        }
    }

    void RotateInsidePipe(int dir)
    {
        Vector3 centerPos = transform.position - offsetFromCenter;

        radiansFromPoint += (dir * rotSpeed*Time.deltaTime);
        offsetFromCenter = new Vector3(0, Mathf.Sin(radiansFromPoint), Mathf.Cos(radiansFromPoint));
        offsetFromCenter *= 0.8f;

        transform.position = centerPos + offsetFromCenter;
    }

    public Vector3 GetTargetPosition()
    {
        return tube.path[index];
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
                    RotateInsidePipe(-2);
                }
                else if (touch.position.x < touchInitPos.x - 1)
                {
                    RotateInsidePipe(2);
                }
                touchInitPos = touch.position;
            }
        }
    }
}
