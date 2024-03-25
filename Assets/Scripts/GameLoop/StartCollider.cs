using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartCollider : MonoBehaviour
{
    private GameManager gm;

    private void Start()
    {
        gm = GameManager.instance;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerMovement player))
        {
            if (player.playerState == EPlayerState.Falling)
            {
                gm.ResetPlayer();
            }
        }
    }
}
