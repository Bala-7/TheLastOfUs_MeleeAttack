using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPS_Player : MonoBehaviour
{
    private Rigidbody _rb;

    #region Camera
    public Camera _cam;
    private CameraMovement _cm;
    private Vector3 camFwd;
    #endregion


    #region Movement
    [Range(1.0f, 10.0f)]
    public float walk_speed;
    [Range(1.0f, 10.0f)]
    public float backwards_walk_speed;
    [Range(1.0f, 10.0f)]
    public float strafe_speed;

    [Range(2.0f, 10.0f)]
    public float jump_force;
    #endregion


    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }


    void FixedUpdate()
    {
        // Gets the input
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        bool jump = Input.GetButtonDown("Jump");

        // Calculate camera relative directions to move:
        camFwd = Vector3.Scale(_cam.transform.forward, new Vector3(1, 1, 1)).normalized;
        Vector3 camFlatFwd = Vector3.Scale(_cam.transform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 flatRight = new Vector3(_cam.transform.right.x, 0, _cam.transform.right.z);

        Vector3 m_CharForward = Vector3.Scale(camFlatFwd, new Vector3(1, 0, 1)).normalized;
        Vector3 m_CharRight = Vector3.Scale(flatRight, new Vector3(1, 0, 1)).normalized;


        // Draws a ray to show the direction the player is aiming at
        Debug.DrawLine(transform.position, transform.position + camFwd * 5f, Color.red);

        float w_speed = (v > 0) ? walk_speed : backwards_walk_speed;
        Vector3 move = v * m_CharForward * w_speed + h * m_CharRight * strafe_speed;
        transform.position += move * Time.deltaTime;    // Move the actual player

        // Jump 
        if (jump)
        {
            _rb.AddForce(Vector3.up * jump_force, ForceMode.Impulse);
        }
    }
}
