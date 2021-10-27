using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bistol : Skill
{
    int level = 0;
    float[] portionBistol = new float[3] { 0.2f, 0.6f, 1f };
    int[] damage = new int[3] { -10, -10, -15 };
    float[] hitForceFactor = new float[3] { 0.4f, 0.4f, 0.4f };

    public override void _OnRestart(RestartEvent e)
    {
        base._OnRestart(e);
        Destroy(gameObject.GetComponent<Bistol>());
    }
    public static void Register(SideName side)
    {
        if (GameManager.Instance.sideSpawner[side].GetComponent<Bistol>() == null)
        {
            GameManager.Instance.sideSpawner[side].AddComponent<Bistol>();
        }
        else
        {
            GameManager.Instance.sideSpawner[side].GetComponent<Bistol>().Upgrade();
        }
    }

    public void Upgrade()
    {
        level += 1;
    }
    public void BistolSetBubble(GameObject bubble)
    {
        if (bubble.GetComponent<SaturnOnBubble>() != null)
        {
            return;
        }
        // if (gameObject.GetComponent<SpawnerController>().spawningBubbleIndex)
        BubbleController bc = bubble.GetComponent<BubbleController>();
        BistolOnBubble bob = bubble.AddComponent<BistolOnBubble>();
        bob.damage = damage[level];
        bob.hitForce = bc.hitForce * hitForceFactor[level];
        // This will change collider size as well
    }

    public int NumBistolEffectedBubble(int numBubble)
    {
        return Mathf.FloorToInt(numBubble * portionBistol[level]);
    }
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        level = 0;
        SpawnerController Spawner = gameObject.GetComponent<SpawnerController>();
        // Spawner.setBubbleMethod += BistolSetBubble;
        // Spawner.numSpawningBubble += 2;
        Spawner.singleNodeEffectRange[BistolSetBubble] = NumBistolEffectedBubble;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
