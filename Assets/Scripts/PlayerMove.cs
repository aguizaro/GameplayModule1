using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// used unity's example code: https://docs.unity3d.com/ScriptReference/CharacterController.Move.html

public class PlayerMove : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    [SerializeField] private float playerSpeed = 2.0f;
    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private float gravityValue = -9.81f;
    [SerializeField] private float rotationSensitivity = 2f;
    [SerializeField] private SnailTrail _pool;

    private void Start()
    {
        Application.targetFrameRate = 60;

        controller = gameObject.AddComponent<CharacterController>();
    }

    void Update()
    {
        if (UserSettings.IsGameOver || UserSettings.IsPaused) return; // nothing happens if paused or game over

        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        //rotate player left right (Y axis)
        float mouseX = Input.GetAxis("Mouse X") * rotationSensitivity;
        transform.Rotate(Vector3.up * mouseX);

        Vector3 move = new Vector3(0, 0, Input.GetAxis("Vertical"));
        // local space to world space
        move = transform.TransformDirection(move);
        controller.Move(move * playerSpeed * Time.deltaTime);
        if (move.magnitude > 0 && groundedPlayer)
        {
            EmitSnailTrail();
        }

        // Changes the height position of the player..
        if (Input.GetButtonDown("Jump") && groundedPlayer)
        {
            Debug.Log("Jump! - gameOver: " + UserSettings.IsGameOver + " paused: " + UserSettings.IsPaused);
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    void EmitSnailTrail()
    {
        GameObject particle = _pool.GetPooledObj();
        particle.transform.position = new Vector3(transform.position.x, transform.position.y - 2f, transform.position.z);
        particle.SetActive(true);
    }
}