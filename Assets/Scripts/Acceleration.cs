using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Acceleration : MonoBehaviour
{
    public Vector3 acceleration;


    private void FixedUpdate()
    {
        float mass = GetComponent<Rigidbody2D>().mass;
        GetComponent<Rigidbody2D>().AddForce(mass * acceleration, ForceMode2D.Force);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
