using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BistolOnBubble : MonoBehaviour
{
    Subscription<SnapEvent> snapEventSubscription;
    public List<GameObject> targets = new List<GameObject>();
    public GameObject rangeAttackRing;
    public GameObject rangeAttackEffect;

    public GameObject rangeAttackRingObj;
    // public GameObject rangeAttackEffectObj;

    public int damage;
    public float hitForce;
    // Start is called before the first frame update
    void Start()
    {
        snapEventSubscription = EventBus.Subscribe<SnapEvent>(_OnSnap);
        rangeAttackEffect = Resources.Load<GameObject>("Prefabs/Bistol Bullet");
        rangeAttackRing = Resources.Load<GameObject>("Prefabs/Bistol Ring");

        rangeAttackRingObj = Instantiate(rangeAttackRing,
            gameObject.transform.position,
            Quaternion.identity,
            gameObject.transform);
        Vector3 scale = rangeAttackRingObj.transform.localScale;
        scale *= gameObject.GetComponent<BubbleController>().radius
            / GameManager.bubbleStandardRadius;
        rangeAttackRingObj.transform.localScale = scale;
        rangeAttackRingObj.GetComponent<BistolRing>().p =
            gameObject;
        rangeAttackRingObj.AddComponent<HasSide>().side =
            gameObject.GetComponent<HasSide>().side;

    }

    void _OnSnap(SnapEvent e)
    {
        BistolAttack(targets);
    }

    private void BistolAttack(List<GameObject> targetList)
    {
        if (targetList.Count > 0)
        {
            // Debug.Log("Attack!");
            GameObject selectedTarget = null;
            while (targetList.Count > 0)
            {
                selectedTarget = targetList[Random.Range(0, targetList.Count)];
                if (selectedTarget != null &&
                    selectedTarget.GetComponent<BubbleController>() != null)
                {
                    if (selectedTarget.GetComponent<LinkenOnBubble>() != null)
                    {
                        if (selectedTarget.GetComponent<LinkenOnBubble>().breakLinken())
                        {
                            return;
                        }
                    }
                    selectedTarget.GetComponent<BubbleController>().HealthChange(damage);
                    break;
                }
                else
                {
                    targetList.Remove(selectedTarget);
                }
            }
            
            if (selectedTarget == null)
            {
                return;
            }            

            Vector3 direction = selectedTarget.transform.position - gameObject.transform.position;
            direction = direction.normalized;
            selectedTarget.GetComponent<Rigidbody2D>().AddForce(
                direction * hitForce, ForceMode2D.Impulse);

            DefaultHurtingEffect(selectedTarget);
        }
    }

    private void DefaultHurtingEffect(GameObject target)
    {
        Vector3 direction = target.transform.position - gameObject.transform.position;
        direction = direction.normalized;
        GameObject hurtingEffectObj = Instantiate(
                rangeAttackEffect,
                gameObject.transform.position +
                    direction * target.GetComponent<BubbleController>().radius,
                Quaternion.identity);
        hurtingEffectObj.GetComponent<HurtingEffectController>().direction = direction;
        hurtingEffectObj.GetComponent<HurtingEffectController>().sizeFactor =
            (float)damage / GameManager.bubbleStandardHitDamage;
        hurtingEffectObj.GetComponent<HurtingEffectController>().distanceFactor =
            hitForce / GameManager.bubbleStandardHitForce * 3;
        hurtingEffectObj.AddComponent<HasSide>().side =
            gameObject.GetComponent<HasSide>().side;
        hurtingEffectObj.GetComponent<Painter>().TakeEffect();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        Destroy(rangeAttackRingObj);
        EventBus.Unsubscribe(snapEventSubscription);
    }
}
