using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tmpCamera : MonoBehaviour
{
    [SerializeField]GameObject m_subject;
    [SerializeField]Vector3 m_offset;
    [SerializeField] float speed;
    void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position,m_subject.transform.position+m_offset, speed * Time.deltaTime);
    }
}
