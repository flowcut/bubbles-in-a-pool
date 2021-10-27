using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saturn : Skill
{
    int level = 0;
    int[] numSatellites = new int[3] { 3, 5, 7 };
    int[] damage = new int[3] { -5, -5, -5 };
    int[] health = new int[3] { 300, 400, 500 };

    public override void _OnRestart(RestartEvent e)
    {
        base._OnRestart(e);
        Destroy(gameObject.GetComponent<Saturn>());
    }

    public static void Register(SideName side)
    {
        if (GameManager.Instance.sideSpawner[side].GetComponent<Saturn>() == null)
        {
            GameManager.Instance.sideSpawner[side].AddComponent<Saturn>();
        }
        else
        {
            GameManager.Instance.sideSpawner[side].GetComponent<Saturn>().Upgrade();
        }
    }

    public void Upgrade()
    {
        level += 1;
    }

    public void SaturnSetBubble(GameObject bubble)
    {
        BubbleController bc = bubble.GetComponent<BubbleController>();
        bc.maxHealth = health[level];
        bc.currentHealth = bc.maxHealth;

        bc.radius = GameManager.bubbleStandardRadius * 3;
        bc.damage = 0;
        bc.hitForce = 0;

        bubble.GetComponent<Rigidbody2D>().mass = GameManager.bubbleStandardMass * 50;
        bubble.GetComponent<Rigidbody2D>().gravityScale = 0.01f;

        bubble.GetComponent<Transform>().localScale *= 3;
        if (bubble.GetComponent<BistolOnBubble>() != null)
        {
            Destroy(bubble.GetComponent<BistolOnBubble>());
        }
        SaturnOnBubble sob = bubble.AddComponent<SaturnOnBubble>();
        sob.numSatellites = numSatellites[level];
        sob.damage = damage[level];
    }

    public void SaturnAttack(List<GameObject> targetList)
    {
        return;
    }

    public int NumBistolEffectedBubble(int numBubble)
    {
        return 1;
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        level = 0;
        SpawnerController Spawner = gameObject.GetComponent<SpawnerController>();
        // Spawner.setBubbleMethod += BistolSetBubble;
        Spawner.numSpawningBubble += 1;
        Spawner.singleNodeEffectRange[SaturnSetBubble] = NumBistolEffectedBubble;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
