using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmperorCircle : MonoBehaviour
{
    public GameObject SolidCircle;
    public GameObject p;
    public float forceMagnitude;
    public float radius = 5.5f;
    // Start is called before the first frame update

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<BubbleController>() != null)
        {
            p.GetComponent<EmperorController>().targets.Add(collision.gameObject);
        }        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        p.GetComponent<EmperorController>().targets.Remove(collision.gameObject);
    }
    void Start()
    {
        radius = GetComponent<CircleCollider2D>().radius;
    }

    // Update is called once per frame
    void Update()
    {
        if (p.GetComponent<EmperorController>().targets.Count > 0)
        {
            foreach (var bubble in p.GetComponent<EmperorController>().targets)
            {
                Vector3 distance = bubble.transform.position - gameObject.transform.position;
                Vector3 direction = distance.normalized;
                if (distance.magnitude > radius + bubble.GetComponent<BubbleController>().radius)
                {
                    continue;
                }
                float force = forceMagnitude;
                force *= distance.magnitude / (radius + bubble.GetComponent<BubbleController>().radius);
                bubble.GetComponent<Rigidbody2D>().AddForce(
                    force * direction, ForceMode2D.Force);
            }
        } 
        else
        {
            SolidCircle.SetActive(true);
        }
    }
}
