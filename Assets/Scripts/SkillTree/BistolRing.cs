using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BistolRing : MonoBehaviour
{
    public GameObject p;
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<BubbleController>() != null &&
            collision.gameObject.GetComponent<HasSide>() != null &&
            collision.gameObject.GetComponent<HasSide>().side != 
            gameObject.GetComponent<HasSide>().side)
        {
            p.GetComponent<BistolOnBubble>().targets.Add(collision.gameObject);
        }        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        p.GetComponent<BistolOnBubble>().targets.Remove(collision.gameObject);
    }
}
