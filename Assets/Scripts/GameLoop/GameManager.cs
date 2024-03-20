using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private PlayerMovement playerMovement;

    private void Start()
    {
        playerMovement = PlayerMovement.Instance;
        playerMovement.enabled = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void Update()
    {
        if (Input.GetMouseButton(0) && Input.GetMouseButton(1))
        {
            playerMovement.enabled = true;
        }
    }
}