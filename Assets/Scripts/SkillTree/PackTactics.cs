using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackTactics : Skill
{
    public float[] shrinkingScale = new float[3] { 0.8f, 0.7f, 0.6f };
    public int[] lifeDeduction = new int[3] { -10, -15, -20 };
    public int[] bubbleAddition = new int[3] { 3, 3, 3 };
    // 3, 5, 7
    public int level = 0;

    public override void _OnRestart(RestartEvent e)
    {
        base._OnRestart(e);
        Destroy(gameObject.GetComponent<PackTactics>());
    }
    public static void Register(SideName side)
    {
        if (GameManager.Instance.sideSpawner[side].GetComponent<PackTactics>() == null)
        {
            GameManager.Instance.sideSpawner[side].AddComponent<PackTactics>();
        }
        else
        {
            GameManager.Instance.sideSpawner[side].GetComponent<PackTactics>().Upgrade();
        }
    }

    public void PackTacticsSetBubble(GameObject bubble)
    {
        BubbleController bc = bubble.GetComponent<BubbleController>();

        bc.maxHealth += lifeDeduction[level];
        bc.currentHealth += lifeDeduction[level];

        bc.radius *= shrinkingScale[level];
        bc.hitForce *= shrinkingScale[level];

        bubble.transform.localScale *= shrinkingScale[level];
        bubble.GetComponent<Rigidbody2D>().mass *= shrinkingScale[level];
        // This will change collider size as well
    }

    public void Upgrade()
    {
        level += 1;
        SpawnerController Spawner = gameObject.GetComponent<SpawnerController>();
        // Spawner.setBubbleMethod += PackTacticsSetBubble;
        Spawner.numSpawningBubble += bubbleAddition[level];
    }
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        level = 0;
        SpawnerController Spawner = gameObject.GetComponent<SpawnerController>();
        Spawner.setBubbleMethod += PackTacticsSetBubble;
        Spawner.numSpawningBubble += bubbleAddition[level];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
