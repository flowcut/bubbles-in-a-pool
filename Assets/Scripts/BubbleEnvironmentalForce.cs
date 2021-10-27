using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleEnvironmentalForce : MonoBehaviour
{
    public float maxForce = 5.0f;
    public float innerRadius = 66.0f;
    public float outerRadius = 89.0f;
    public float standardBubbleRadius = 4.0f;
    // Start is called before the first frame update
    void Start()
    {
        maxForce = GameManager.arenaMaxForce;
        innerRadius = GameManager.arenaInnerRadius;
        outerRadius = GameManager.arenaOuterRadius;
        standardBubbleRadius = GameManager.bubbleStandardRadius;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        Vector3 distance = transform.position;
        float radius = transform.localScale[0] * standardBubbleRadius;
        if (distance.magnitude - radius > outerRadius)
        {
            return;
        }
        if (distance.magnitude + radius > innerRadius)
        {
            float force_factor = 1;
            if (distance.magnitude - radius < innerRadius)
            {
                force_factor *= (distance.magnitude + radius - innerRadius) / (2 * radius);
            }
            Vector3 direction = -distance.normalized;
            GetComponent<Rigidbody2D>().AddForce(direction * force_factor * maxForce);
        }
    }
}
