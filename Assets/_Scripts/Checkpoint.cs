using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] public Vector2 spawnPosition;

    private void Awake()
    {
        spawnPosition = transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player.NextCheckpoint(Player.FindCheckPoint(this));
        }
    }
}
