using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float m_CameraMovementSpeed;
    public float m_CameraZoomSpeed;

    private Transform m_Camerachild;

    private float m_Vertical;
    private float m_Horizontal;
    private float m_ZoomIn;

    // Start is called before the first frame update
    void Start()
    {
        m_Camerachild = transform.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime;

        m_Vertical = Input.GetAxis("Vertical");
        m_Horizontal = Input.GetAxis("Horizontal");
        m_ZoomIn = 0;
        if (Input.GetKey(KeyCode.Q))
        {
            m_ZoomIn = -1;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            m_ZoomIn = 1;
        }

        CameraMove(dt);
    }

    private void CameraMove(float dt)
    {
        transform.position += Vector3.right * m_Horizontal * dt * m_CameraMovementSpeed;
        transform.position += Vector3.forward * m_Vertical * dt * m_CameraMovementSpeed;
        m_Camerachild.transform.position += m_Camerachild.forward * m_ZoomIn * dt * m_CameraZoomSpeed;
    }
}
