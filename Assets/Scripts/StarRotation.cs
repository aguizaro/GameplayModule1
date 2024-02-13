using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarRotation : MonoBehaviour
{
    [SerializeField] private GameManager _manager;

    public float rotationSpeed = 0.5f;

    private void FixedUpdate()
    {
        if (UserSettings.IsPaused || UserSettings.IsGameOver) return;
        transform.Rotate(Vector3.up * rotationSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameObject.SetActive(false);
            _manager.StarCollected();

            
        }
    }
}
