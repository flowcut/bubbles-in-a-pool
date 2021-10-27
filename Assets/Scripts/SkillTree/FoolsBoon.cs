using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoolsBoon : Skill
{
    int level = 0;

    public static void Register(SideName side)
    {
        if (GameManager.Instance.sideSpawner[side].GetComponent<FoolsBoon>() == null)
        {
            GameManager.Instance.sideSpawner[side].AddComponent<FoolsBoon>();
        }
        else
        {
            GameManager.Instance.sideSpawner[side].GetComponent<FoolsBoon>().Upgrade();
        }
    }

    public void Upgrade()
    {
        level += 1;
    }

    public override void _OnRestart(RestartEvent e)
    {
        base._OnRestart(e);
        Destroy(gameObject.GetComponent<FoolsBoon>());
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        level = 0;
        SpawnerController Spawner = gameObject.GetComponent<SpawnerController>();
        Spawner.numSpawningBubble += 1;
        // Spawner.setBubbleMethod += BouldSetBubble;
        // Spawner.numSpawningBubble += 2;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
