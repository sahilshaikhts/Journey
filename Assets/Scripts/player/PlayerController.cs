using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController
{
    Touch touch;
    Vector3 m_touchInitPos = Vector3.zero;

    public Vector3 GetMoveDirection()
    {
        int swipeDirection = TouchSwipeDirection();

        if (Input.GetAxisRaw("Horizontal") == -1 || swipeDirection == -1)
        {
            return Vector3.forward;
        }
        else if (Input.GetAxisRaw("Horizontal") == 1 || swipeDirection == 1)
        {
            return Vector3.back;
        }
        return Vector3.zero;
    }

    public bool CheckJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            return true;

        return false;
    }

    public bool CheckForceDrop()
    {
        if (Input.GetKey(KeyCode.Space))
            return true;

        return false;
    }


    int TouchSwipeDirection()
    {
        if (Input.touchCount == 1)
        {
            touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                m_touchInitPos = touch.position;
            }

            if (touch.phase == TouchPhase.Moved)
            {
                if (touch.position.x > m_touchInitPos.x + 1)
                {
                    return 1;
                }
                else if (touch.position.x < m_touchInitPos.x - 1)
                {
                    return -1;
                }
            }
        }
        return 0;
    }
}
