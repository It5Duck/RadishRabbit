using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    private static bool isCollected = false;

    private void Awake()
    {
        gameObject.SetActive(!isCollected);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isCollected = true;
            gameObject.SetActive(false);
        }
    }
}
