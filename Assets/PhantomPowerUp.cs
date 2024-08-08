using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhantomPowerUp : MonoBehaviour
{
    PlayerType playerType;
    Player player;

    private void Start()
    {
        player = GetComponentInParent<Player>();
        if(player != null )
            playerType = player.playerType;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Block"))
        {
            if (collision.GetComponent<Block>().PlayerType == playerType)
            {
                if (player != null)
                    player.SpawnBlockExplodeParticel(collision);
                Destroy(collision.gameObject);
            }

        }
    }
}
