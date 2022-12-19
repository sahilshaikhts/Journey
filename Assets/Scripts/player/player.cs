using UnityEngine;

public class player : MonoBehaviour
{
    [SerializeField]
    GameManager m_gameManager;
    [SerializeField]
    ParticleSystem m_snowDustEffect;
    MovementComponent m_movementComponent;
    PlayerController m_playerController;

    void Start()
    {
        m_playerController = new PlayerController();
        m_movementComponent = GetComponent<MovementComponent>();
    }

    void Update()
    {
        if (m_gameManager.gameState != GameManager.State.Over)
        {
            Movement();
            if (m_movementComponent.IsOnGround() == false)
            {
                m_snowDustEffect.Stop();
            }
            else
                m_snowDustEffect.Play();

        }
    }

    private void FixedUpdate()
    {
        Vector3 direction = m_movementComponent.GetMovementDirection();
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(Mathf.Rad2Deg * -Mathf.Atan2(direction.y, direction.x), 90, 0), 5 * Time.fixedDeltaTime);

        float angle = Vector3.Angle(Vector3.up, m_movementComponent.GetGroundNormal());

    }

    void Movement()
    {
        m_movementComponent.SetMovementDirection(Vector3.right);

        if (m_playerController.GetMoveDirection() != Vector3.zero)
        {
            m_movementComponent.SetMovementDirection(m_playerController.GetMoveDirection());
        }
        if (m_playerController.CheckJump())
        {
            m_movementComponent.Jump();
        }
        if (m_playerController.CheckForceDrop())
        {
            if (!m_movementComponent.IsOnGround())
                m_movementComponent.SetMovementDirection(Vector3.down);
        }
    }
}
