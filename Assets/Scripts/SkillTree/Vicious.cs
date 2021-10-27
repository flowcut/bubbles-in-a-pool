using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vicious : Skill
{
    int level = 0;
    int[] numTargets = new int[3] { 1, 2, 3 };

    public override void _OnRestart(RestartEvent e)
    {
        base._OnRestart(e);
        Destroy(gameObject.GetComponent<Vicious>());
    }

    public static void Register(SideName side)
    {
        if (GameManager.Instance.sideSpawner[side].GetComponent<Vicious>() == null)
        {
            GameManager.Instance.sideSpawner[side].AddComponent<Vicious>();
        }
        else
        {
            GameManager.Instance.sideSpawner[side].GetComponent<Vicious>().Upgrade();
        }
    }

    public void Upgrade()
    {
        level += 1;
    }

    public void ViciousSetBubble(GameObject bubble)
    {
        BubbleController bc = bubble.GetComponent<BubbleController>();
        bc.attackMethod = ViciousAttack;
        bc.ring.GetComponent<SpriteRenderer>().sprite =
            Resources.Load<Sprite>("icon/1x/Demon");
    }

    public void ViciousAttack(List<GameObject> targetList)
    {
        BubbleController bc = gameObject.GetComponent<BubbleController>();
        if (targetList.Count > 0)
        {
            // Debug.Log("Attack!");
            // GameObject selectedTarget = targetList[Random.Range(0, targetList.Count)];
            targetList.Sort((p1, p2) =>
            p1.GetComponent<BubbleController>().currentHealth.CompareTo(p1.GetComponent<BubbleController>().currentHealth));

            for (int i=0; i < Mathf.Min(numTargets[level], targetList.Count); ++i)
            {
                GameObject selectedTarget = targetList[i];
                if (selectedTarget != null)
                {
                    if (selectedTarget.GetComponent<LinkenOnBubble>() != null)
                    {
                        if (selectedTarget.GetComponent<LinkenOnBubble>().breakLinken())
                        {
                            continue;
                        }
                    }
                    selectedTarget.GetComponent<BubbleController>().HealthChange(bc.damage);

                    Vector3 direction = selectedTarget.transform.position - gameObject.transform.position;
                    direction = direction.normalized;
                    selectedTarget.GetComponent<Rigidbody2D>().AddForce(
                        direction * bc.hitForce, ForceMode2D.Impulse);

                    bc.DefaultHurtingEffect(selectedTarget);
                }
            }                        
        }
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        level = 0;
        SpawnerController Spawner = gameObject.GetComponent<SpawnerController>();
        Spawner.setBubbleMethod += ViciousSetBubble;
        // Spawner.setBubbleMethod += BistolSetBubble;
        // Spawner.numSpawningBubble += 2;
        // Spawner.singleNodeEffectRange[BistolSetBubble] = NumBistolEffectedBubble;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
