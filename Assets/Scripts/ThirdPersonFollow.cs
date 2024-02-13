using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonFollow : MonoBehaviour
{
    public Transform target;
    public float followSpeed = 30f;
    public float xCamRotation = 18f;
    public Vector3 camOffset = new(0f, -2f, 4f);

    private void LateUpdate()
    {
        if (UserSettings.IsGameOver || UserSettings.IsPaused) return; // no camera follow unless game is active

        if (target == null)
        {
            Debug.LogError("Follow target not set");
            return;
        }

        float angle = target.eulerAngles.y;
        Quaternion camRotation = Quaternion.Euler(xCamRotation, angle, 0f);

        // chat GPT helped me figure out how to calculate my camera position and add interpolation
        Vector3 camPosition = target.position - (camRotation * camOffset);
        transform.position = Vector3.Lerp(transform.position, camPosition, followSpeed * Time.deltaTime);
        transform.rotation = camRotation;

    }
}
