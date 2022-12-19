using UnityEngine;
namespace CameraSystem
{
    public class BaseCamera:MonoBehaviour
    {
        [SerializeField] Camera m_camera;

        public void Activate()
        {
            m_camera.enabled = true;
        }

        public void Deactivate()
        {
            m_camera.enabled = false;
        }

        public Camera GetCamera() { return m_camera; }
    }
}
