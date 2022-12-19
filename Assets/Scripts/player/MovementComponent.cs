using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.EventSystems;

public class MovementComponent : MonoBehaviour
{
    private Rigidbody m_rigidbody;

    private Vector3 m_inputDirection;
    private Vector3 m_velocity;
    private Vector3 m_additionalForce;
    private Vector3 m_additionalRotationDirection;
    [SerializeField] bool LookTowardsMovementDirection = false;

    [SerializeField] private float m_speed, m_maxSpeed, m_jumpSpeed;
    private float m_requestedSpeed;

    [Tooltip("Percentage of speed boost to apply to reduce the friction when landing on angled surface")]
    [Range(0, 1)][SerializeField] private float m_landSpeedAssist = -1;
    [SerializeField] private float m_landAssistAngle;
    bool m_applyLandAssist = false;

    [SerializeField]
    [Range(0, 1)] float m_inAirControl;
    [SerializeField] private float m_gravityAccelaration;
    
    [SerializeField] private float m_turnSpeed;
    [SerializeField] private float m_friction;

    [SerializeField] private LayerMask m_jumpGroundCheckMask;

    [SerializeField] private Transform m_characterFeet;

    [SerializeField] float m_jumpBuffer=.5f;
    private bool doJump = false;
    private float m_lastJumpRqst;
    private float m_lastGroundTime;

    private void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();

        if (m_characterFeet == null) Debug.LogError("Assign a transform of character's Feet");
    }

    private void FixedUpdate()
    {
        m_velocity = m_rigidbody.velocity;
        CalculateVelocity();
    }

    private void CalculateVelocity()
    {
        Vector3 newVelocity = m_velocity;
        float speed = m_speed, maxSpeed = m_maxSpeed;
        bool onGround = CheckIfOnGround();
        if (m_requestedSpeed > 0)
        {
            speed = maxSpeed = m_requestedSpeed;
        }
        if (!onGround)
        {
            Vector3 movDirection = m_inputDirection + m_additionalForce.normalized;
            movDirection *= speed * m_inAirControl + m_additionalForce.magnitude;

            newVelocity += movDirection * Time.fixedDeltaTime;
            newVelocity = Vector3.ClampMagnitude(newVelocity, maxSpeed + m_additionalForce.magnitude);

            m_inputDirection = Vector3.zero;

            m_velocity = newVelocity;
            m_applyLandAssist = true;
            ApplyDownwardSpeed();
        }
        else
        if (m_inputDirection.sqrMagnitude > 0.1f)
        {
            Vector3 movDirection = m_inputDirection + m_additionalForce.normalized;

            movDirection.Normalize();

            Vector3 groundTangent = movDirection - Vector3.Project(movDirection, GetGroundNormal());
            groundTangent.Normalize();
            movDirection = groundTangent;

            Vector3 velAlongMoveDir = Vector3.Project(newVelocity, movDirection);
            velAlongMoveDir *= 2;
            if (Vector3.Dot(velAlongMoveDir, movDirection) > 0.0f)
            {
                newVelocity = Vector3.Lerp(newVelocity, velAlongMoveDir, m_turnSpeed * Time.fixedDeltaTime);
            }
            else
            {
                newVelocity = Vector3.Lerp(newVelocity, Vector3.zero, m_turnSpeed * Time.fixedDeltaTime);
            }
            movDirection *= speed + m_additionalForce.magnitude;

            newVelocity += movDirection * Time.fixedDeltaTime;
            newVelocity = Vector3.ClampMagnitude(newVelocity, maxSpeed + m_additionalForce.magnitude);

            m_inputDirection = Vector3.zero;

            m_velocity = newVelocity;
            if (LookTowardsMovementDirection)
            {
                Vector3 facingDirection = m_velocity.normalized;
                facingDirection.Normalize();
                facingDirection.y = 0;

                if (m_rigidbody.velocity.magnitude > 0.1f)
                    RotateTowards(facingDirection);
            }
            m_lastGroundTime = Time.time;
        }
        else Friction();

        if (m_applyLandAssist == true)
        {
            Vector3 groundNormal = GetGroundNormal();

            float groundNormalAngles = Vector3.Angle(Vector3.up, GetGroundNormal());

            if (groundNormalAngles > 0 && groundNormalAngles < m_landAssistAngle)
            {
                Quaternion slopeRotation = Quaternion.FromToRotation(Vector3.up, groundNormal);

                Vector3 adjustedVelocity = slopeRotation * m_velocity;
                adjustedVelocity *= m_speed * m_landSpeedAssist;
                adjustedVelocity = Vector3.ClampMagnitude(newVelocity, maxSpeed + m_additionalForce.magnitude);

                m_velocity = adjustedVelocity;
            }

        }

        if (doJump) m_lastJumpRqst = Time.time;

        //Allow jump if last jump request within jumpBuffer time or if player just toke off from the ground by some other force.
        if ((onGround && Time.time <= m_lastJumpRqst + m_jumpBuffer) || (doJump == true && Time.time <= m_lastGroundTime + m_jumpBuffer))
            PerformJump();

        m_additionalForce = Vector3.zero;
        m_applyLandAssist = false;
        doJump = false;
        
        ApplyVelocity();

        if (m_additionalRotationDirection.sqrMagnitude > 0)
        {
            RotateTowards(m_additionalRotationDirection);
            m_additionalRotationDirection = Vector3.zero;
        }
    }


    private void Friction() { m_velocity = Vector3.Lerp(m_velocity, new Vector3(0, m_rigidbody.velocity.y, 0), m_friction * Time.fixedDeltaTime); }

    private void ApplyDownwardSpeed() { m_velocity.y += m_gravityAccelaration * Time.fixedDeltaTime; }

    private void ApplyVelocity()
    {
        Vector3 velocityDiff = m_velocity - m_rigidbody.velocity;
        m_rigidbody.AddForce(velocityDiff, ForceMode.VelocityChange);
    }

    public void StopMovement()
    {
        m_rigidbody.velocity = Vector3.zero;
    }

    void PerformJump()
    {
        m_velocity.y = m_jumpSpeed;
        transform.position += new Vector3(0.0f, 0.2f, 0.0f);
        doJump = false;
    }

    private bool CheckIfOnGround()
    {
        Ray ray = new Ray(m_characterFeet.position, Vector3.down);

        if (Physics.Raycast(ray, 0.5f, m_jumpGroundCheckMask))
        {
            return true;
        }
        return false;
    }

    public Vector3 GetGroundNormal()
    {
        Ray ray = new Ray(m_characterFeet.position, Vector3.down);

        RaycastHit hit = new RaycastHit();
        Vector3 normal = Vector3.zero;

        if (Physics.Raycast(ray, out hit, 2f, m_jumpGroundCheckMask))
        {
            normal = hit.normal;
        }
        return normal;
    }

    public void SetMovementDirection(Vector3 direction, float aSpeed)
    {
        m_inputDirection += direction;
        m_inputDirection.Normalize();
        m_requestedSpeed = aSpeed;
    }

    public void SetMovementDirection(Vector3 direction)
    {
        m_inputDirection += direction;
        m_inputDirection.Normalize();
    }
    public Vector3 GetMovementDirection() { return m_velocity.normalized; }
    public void AddRotation(Vector3 direction)
    {
        m_additionalRotationDirection = direction;
    }

    void RotateTowards(Vector3 direction)
    {
        direction.Normalize();

        m_rigidbody.rotation = Quaternion.LookRotation(direction);
    }

    public void ApplyForce(Vector3 direction, float force)
    {
        m_additionalForce += direction * force;
    }

    public void Jump() { doJump = true; }

    public bool IsOnGround() => CheckIfOnGround();

    void MoveHandler(Vector3 direction) { SetMovementDirection(direction); }
}