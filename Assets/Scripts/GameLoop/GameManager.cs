using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private PlayerMovement playerMovement;
    [SerializeField] private GameObject startGameUI;
    [SerializeField] private GameObject gameUI;
    public bool playerFalling;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        playerMovement = PlayerMovement.Instance;
        playerMovement.enabled = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void Update()
    {
        if (Input.GetMouseButton(0) && Input.GetMouseButton(1) && !playerFalling)
        {
            playerMovement.enabled = true;
            startGameUI.SetActive(false);
            gameUI.SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            string thisScene = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(thisScene);
        }
    }
    public void ResetPlayer()
    {
        playerMovement.rb.isKinematic = true;
        playerMovement.playerState = EPlayerState.Climbing;
        playerMovement.ResetLimbs();
        playerFalling = false;
    }
}
