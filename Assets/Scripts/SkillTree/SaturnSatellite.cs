using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaturnSatellite : MonoBehaviour
{
    public float degree;
    public float length;
    public float lengthFactor;
    public int damage;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        degree += Time.deltaTime;
        if (degree > 2 * Mathf.PI)
        {
            degree -= 2 * Mathf.PI;
        }
        gameObject.transform.localPosition = new Vector3(
            Mathf.Cos(degree) * length * lengthFactor,
            Mathf.Sin(degree) * length,
            0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<BubbleController>() != null)
        {
            if (collision.gameObject.GetComponent<HasSide>().side !=
                gameObject.GetComponent<HasSide>().side)
            {
                if (collision.gameObject.GetComponent<LinkenOnBubble>() != null)
                {
                    if (collision.gameObject.GetComponent<LinkenOnBubble>().breakLinken())
                    {
                        return;
                    }
                }
                collision.gameObject.GetComponent<BubbleController>().HealthChange(damage);
            }
        }
    }
}
