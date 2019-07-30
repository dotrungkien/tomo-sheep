﻿using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public class GameController : MonoBehaviour
{
    public Sheep[] whiteSheeps;
    public Sheep[] blackSheeps;
    public GameObject[] sheepIcons;
    public Transform[] wSpawnPositions;
    public Transform[] bSpawnPositions;

    public float coolDown = 0.0f;
    public bool isPlaying = false;
    public bool isReady = false;

    [HideInInspector]
    public List<int> sheeps;
    public SheepIcon[] icons;

    private Sheep currentSheep = null;
    private System.Random rand;

    public void UpdateIcons()
    {
        for (int i = 0; i < icons.Length; i++)
        {
            icons[i].SwitchSheep(sheeps[i]);
        }
    }

    public void NextTurn()
    {
        sheeps.RemoveAt(0);
        UpdateIcons();
        ResetCooldown();
    }

    public void Play(string tx)
    {
        isPlaying = true;
        isReady = true;
        coolDown = 0f;
        var subTx = tx.Substring(0, 8);
        int seed = Convert.ToInt32(subTx, 16);
        rand = new System.Random(seed);
        sheeps = new List<int>();
        for (int i = 0; i < 30; i++)
        {
            sheeps.Add(rand.Next() % 5);
        }
        UpdateIcons();
        // Debug.Log(string.Format("First 5 sheeps: {0}", string.Join(", ", sheeps.Take(5))));
    }

    public void AddNewSheep()
    {
        sheeps.Add(rand.Next() % 5);
    }

    private void Update()
    {
        if (!isPlaying) return;
        if (!isReady)
        {
            coolDown -= Time.deltaTime;
            if (coolDown <= 0f)
            {
                coolDown = 0.0f;
                isReady = true;
                // AddNewSheep();
            }
        }
    }

    public void SpawnLane(int laneIndex)
    {
        if (isReady)
        {
            SpawnSheeps(true, sheeps[0], laneIndex);
            NextTurn();
        }
        else
        {
            StartCoroutine(PrepareSheep(sheeps[0], laneIndex));
        }
    }

    public IEnumerator PrepareSheep(int sheepIndex, int laneIndex)
    {
        if (currentSheep == null)
        {
            currentSheep = Instantiate<Sheep>(whiteSheeps[sheepIndex], wSpawnPositions[laneIndex].position, Quaternion.identity, wSpawnPositions[laneIndex]);
            yield return new WaitForSeconds(coolDown);
            currentSheep.BeSpawned();
            NextTurn();
        }
        else
        {
            currentSheep.transform.position = wSpawnPositions[laneIndex].position;
        }
    }


    void SpawnSheeps(bool isWhite, int sheepIndex, int laneIndex)
    {
        if (isWhite)
        {
            Sheep sheep = Instantiate<Sheep>(whiteSheeps[sheepIndex], wSpawnPositions[laneIndex].position, Quaternion.identity, wSpawnPositions[laneIndex]);
            sheep.BeSpawned();
        }
        else
        {
            Sheep sheep = Instantiate<Sheep>(blackSheeps[sheepIndex], bSpawnPositions[laneIndex].position, Quaternion.identity, bSpawnPositions[laneIndex]);
            sheep.BeSpawned();
            sheep.direction = -1;
        }

    }

    public void ResetCooldown()
    {
        coolDown = 3.0f;
        isReady = false;
        currentSheep = null;
    }
}
