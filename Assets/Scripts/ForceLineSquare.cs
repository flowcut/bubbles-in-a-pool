using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceLineSquare : MonoBehaviour
{
    public GameObject p;
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<BubbleController>() != null)
        {
            p.GetComponent<ForceLineController>().targets.Add(collision.gameObject);
        }
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        p.GetComponent<ForceLineController>().targets.Remove(collision.gameObject);
    }
}
