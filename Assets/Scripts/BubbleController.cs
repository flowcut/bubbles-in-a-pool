using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleController : MonoBehaviour
{
    Subscription<SnapEvent> snapEventSubscription;
    Subscription<RestartEvent> restartEventSubscription;

    public int maxHealth;
    public int currentHealth;
    public float radius;
    public int damage;
    public float hitForce;
    public int globalDamage = 1;
    // public SideName side;

    public List<GameObject> attackTargets = new List<GameObject>();
    public List<string> modifiers = new List<string>();
    public GameObject ring;


    public delegate void AttackDelegate(List<GameObject> targetList);
    public AttackDelegate attackMethod;

    public GameObject defaultHurtingEffect;

    private SideName side;

    // Start is called before the first frame update
    void Start()
    {
        restartEventSubscription = EventBus.Subscribe<RestartEvent>(_OnRestart);
        snapEventSubscription = EventBus.Subscribe<SnapEvent>(_OnSnap);
        attackMethod = DefaultAttack;
        side = GetComponent<HasSide>().side;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.transform.position.magnitude > 300.0f)
        {
            Destroy(gameObject);
        }
        if (currentHealth == 0)
        {
            Destroy(gameObject);
        }
    }

    void _OnSnap(SnapEvent e)
    {        
        if (e.current_snap != GameManager.clockNumSnap -1)
        {
            attackMethod(targetList: attackTargets);
        }
        
    } 

    void _OnRestart(RestartEvent e)
    {
        if (GetComponent<BubbleEnvironmentalForce>() != null)
        {
            GetComponent<BubbleEnvironmentalForce>().enabled = false;
            GetComponent<CircleCollider2D>().enabled = false;
            Vector3 direction = -gameObject.transform.position.normalized;
            direction[0] = -direction[0];
            if (direction[1] < 0)
            {
                direction[1] = -direction[1];
            }
            GetComponent<Rigidbody2D>().AddForce(direction * 400f, ForceMode2D.Impulse);
            GetComponent<Rigidbody2D>().gravityScale = 0;
        }
    }

    public void HealthChange(int delta)
    {
        currentHealth += delta;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }


        if (currentHealth <= 0)
        {
            // death hurting
            currentHealth = 0;            
        }
    }

    private void DefaultAttack(List<GameObject> targetList)
    {
        if (targetList.Count > 0)
        {
            // Debug.Log("Attack!");
            GameObject selectedTarget = targetList[Random.Range(0, targetList.Count)];

            if (selectedTarget.GetComponent<LinkenOnBubble>() != null)
            {
                if (selectedTarget.GetComponent<LinkenOnBubble>().breakLinken())
                {
                    return;
                }
            }

            selectedTarget.GetComponent<BubbleController>().HealthChange(damage);

            Vector3 direction = selectedTarget.transform.position - gameObject.transform.position;
            direction = direction.normalized;
            selectedTarget.GetComponent<Rigidbody2D>().AddForce(
                direction * hitForce, ForceMode2D.Impulse);

            DefaultHurtingEffect(selectedTarget);
        }
    }

    public void DefaultHurtingEffect(GameObject target)
    {
        Vector3 direction = target.transform.position - gameObject.transform.position;
        direction = direction.normalized;
        GameObject hurtingEffectObj = Instantiate(
                defaultHurtingEffect,
                target.transform.position +
                    direction * target.GetComponent<BubbleController>().radius,
                Quaternion.identity);
        hurtingEffectObj.GetComponent<HurtingEffectController>().direction = direction;
        hurtingEffectObj.GetComponent<HurtingEffectController>().sizeFactor =
            damage / GameManager.bubbleStandardHitDamage;
        hurtingEffectObj.GetComponent<HurtingEffectController>().distanceFactor =
            hitForce / GameManager.bubbleStandardHitForce;
        hurtingEffectObj.AddComponent<HasSide>().side =
            target.GetComponent<HasSide>().side;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Debug.Log("Touch!");
        if (collision.gameObject.GetComponent<BubbleController>() != null
            && collision.gameObject.GetComponent<HasSide>().side != side)
        {
            attackTargets.Add(collision.gameObject);
        }        
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        attackTargets.Remove(collision.gameObject);
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(snapEventSubscription);
    }
}
