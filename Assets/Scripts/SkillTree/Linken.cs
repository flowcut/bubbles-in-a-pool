using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Linken : Skill
{
    int level = 0;
    int[] linkenCoolDown = new int[3] { 6, 4, 2 };


    public override void _OnRestart(RestartEvent e)
    {
        base._OnRestart(e);
        Destroy(gameObject.GetComponent<Linken>());
    }

    public static void Register(SideName side)
    {
        if (GameManager.Instance.sideSpawner[side].GetComponent<Linken>() == null)
        {
            GameManager.Instance.sideSpawner[side].AddComponent<Linken>();
        }
        else
        {
            GameManager.Instance.sideSpawner[side].GetComponent<Linken>().Upgrade();
        }
    }

    public void Upgrade()
    {
        level += 1;
    }

    public void LinkenSetBubble(GameObject bubble)
    {
        BubbleController bc = bubble.GetComponent<BubbleController>();
        LinkenOnBubble lob = bubble.AddComponent<LinkenOnBubble>();
        lob.coolDown = linkenCoolDown[level];
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        level = 0;
        SpawnerController Spawner = gameObject.GetComponent<SpawnerController>();
        Spawner.setBubbleMethod += LinkenSetBubble;
        // Spawner.setBubbleMethod += BistolSetBubble;
        // Spawner.numSpawningBubble += 2;
        // Spawner.singleNodeEffectRange[BistolSetBubble] = NumBistolEffectedBubble;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
