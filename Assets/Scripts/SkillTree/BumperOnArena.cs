using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BumperOnArena : MonoBehaviour
{

    SideName side;
    public float forceMagnitude;
    private GameObject stand;

    // Start is called before the first frame update
    void Start()
    {        
        forceMagnitude = GameManager.Instance.Arena.GetComponent<BBumper>().forceMagnitude;
        stand = Resources.Load<GameObject>("Prefabs/Stand");
    }

    private void OnEnable()
    {
        side = GetComponent<HasSide>().side;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<BubbleController>() != null)
        {
            GameObject bubble = collision.gameObject;
            if (bubble.GetComponent<HasSide>().side != side)
            {
                if (bubble.GetComponent<LinkenOnBubble>() != null)
                {
                    if (bubble.GetComponent<LinkenOnBubble>().breakLinken())
                    {
                        return;
                    }
                }
                bubble.GetComponent<BubbleController>().HealthChange(
                    GameManager.Instance.Arena.GetComponent<BBumper>().HealthDeduction);
                Vector3 direction = (bubble.GetComponent<Transform>().position -
                    gameObject.transform.position).normalized;
                bubble.GetComponent<Rigidbody2D>().AddForce(
                    direction * forceMagnitude, ForceMode2D.Impulse);
                GameObject standObj = Instantiate(stand);
                standObj.GetComponent<StandAnimation>().cloneSprite(gameObject);
                standObj.GetComponent<StandAnimation>().shift =
                    -direction * 10;
            }
            else
            {
                
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
