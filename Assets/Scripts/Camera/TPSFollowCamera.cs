using UnityEngine;

namespace CameraSystem
{
    public class TPSFollowCamera : MonoBehaviour
    {
        [SerializeField] GameObject m_targetObject;
        [SerializeField] LayerMask m_obstacleCheckMask;

        [SerializeField] Vector3 m_offset;

        [SerializeField] float m_followSpeed;

        Vector3 m_currentRotation;
        Vector3 dampVelocity = new Vector3();

        private void Start()
        {
            m_currentRotation = transform.rotation.eulerAngles;
        }
        private void Update()
        {
            MoveCamera();
        }

        private void MoveCamera()
        {
            Vector3 finalPosition;

            finalPosition = m_targetObject.transform.position + m_offset;
            finalPosition = CheckForObstacle(m_targetObject.transform.position, finalPosition, m_offset.sqrMagnitude);
            finalPosition = Vector3.Slerp(transform.position, finalPosition, m_followSpeed * Time.deltaTime);

            transform.position = finalPosition;
        }

        Vector3 CheckForObstacle(Vector3 targetPosition, Vector3 cameraPosition, float distacne)
        {
            RaycastHit hit;
            Vector3 direction = (cameraPosition - targetPosition).normalized;

            Ray ray = new Ray(targetPosition, direction);

            if (Physics.Raycast(ray, out hit, distacne * distacne, m_obstacleCheckMask))
            {
                Debug.Log("obst");
                return hit.point - ray.direction * 0.25f;
            }

            return cameraPosition;
        }

    }
}
