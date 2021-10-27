using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotSpRing : Skill
{
    public int level = 0;
    public int[] healthRegenPerSnap = { 15, 25, 35 };
    public static void Register(SideName side)
    {
        if (GameManager.Instance.sideSpawner[side].GetComponent<HotSpRing>() == null)
        {
            GameManager.Instance.sideSpawner[side].AddComponent<HotSpRing>();
        }
        else
        {
            GameManager.Instance.sideSpawner[side].GetComponent<HotSpRing>().Upgrade();
        }
    }

    public void HotSpRingSetBubble(GameObject bubble)
    {
        HotSpRingOnBubble h = bubble.AddComponent<HotSpRingOnBubble>();
        h.healthRegen = healthRegenPerSnap[level];
    }

    public override void _OnRestart(RestartEvent e)
    {
        base._OnRestart(e);
        Destroy(gameObject.GetComponent<HotSpRing>());
    }

    public void Upgrade()
    {
        level += 1;
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        level = 0;
        SpawnerController Spawner = gameObject.GetComponent<SpawnerController>();
        Spawner.setBubbleMethod += HotSpRingSetBubble;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
