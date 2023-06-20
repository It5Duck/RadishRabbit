using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private static int radishes;
    private static int clovers;
    private static int currentStage;
    private static int currentCheckpoint;
    private static Checkpoint[] checkpoints;
    private static PlayerMovement player;

    [SerializeField] private Checkpoint[] refCheckpoints;

    private void Awake()
    {
        player = GetComponent<PlayerMovement>();
        checkpoints = refCheckpoints;
        currentCheckpoint = 0;
    }
    public static void GameOver()
    {
        player.gameObject.SetActive(false);
        player.transform.position = checkpoints[currentCheckpoint].spawnPosition;
        player.gameObject.SetActive(true);
    }

    public static void NextCheckpoint(int index)
    {
        if (currentCheckpoint < index)
        {
            currentCheckpoint = index;
        }
    }

    public static int FindCheckPoint(Checkpoint checkpoint)
    {
        for (int i = 0; i < checkpoints.Length; i++)
        {
            if (checkpoints[i] == checkpoint)
            {
                return i;
            }
        }
        return 0;
    }
}
