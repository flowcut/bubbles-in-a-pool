using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KickerOnArena : MonoBehaviour
{
    SideName side;
    public float forceMagnitude;

    public Vector3 forceToOther;
    private GameObject stand;

    void Start()
    {
        
        forceMagnitude = GameManager.Instance.Arena.GetComponent<BKicker>().forceMagnitude;
        forceToOther = forceToOther.normalized * forceMagnitude;
        stand = Resources.Load<GameObject>("Prefabs/Stand");
    }

    private void OnEnable()
    {
        side = GetComponent<HasSide>().side;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<BubbleController>() != null)
        {
            GameObject bubble = collision.gameObject;
            if (bubble.GetComponent<HasSide>().side == side)
            {
                bubble.GetComponent<BubbleController>().HealthChange(
                    GameManager.Instance.Arena.GetComponent<BKicker>().HealthRegen);
                Vector3 direction = - (bubble.GetComponent<Transform>().position).normalized;
                bubble.GetComponent<Rigidbody2D>().AddForce(
                    direction * forceMagnitude, ForceMode2D.Impulse);
                GameObject standObj = Instantiate(stand);
                standObj.GetComponent<StandAnimation>().cloneSprite(gameObject);
                standObj.GetComponent<StandAnimation>().shift =
                    gameObject.transform.position.normalized * 20;
            }
            else
            {
                bubble.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                bubble.GetComponent<Rigidbody2D>().AddForce(
                    forceToOther, ForceMode2D.Impulse);
            }
        }
    }
}
