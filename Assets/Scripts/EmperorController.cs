using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmperorController : MonoBehaviour
{
    public GameObject circle;
    public float forceMagnitude = 5.0f;

    public List<GameObject> targets = new List<GameObject>();

    public bool canUse = true;

    private GameObject circleObj;
    private Vector3 initialPos;

    // Start is called before the first frame update
    void Start()
    {
        
        

    }

    // Update is called once per frame
    void Update()
    {
        if (circleObj != null)
        {
            circleObj.transform.position =
                Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 0, 10);
        }

        if (Input.GetButtonDown("Fire1") && canUse)
        {
            initialPos = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 0, 10);
            // Debug.Log(initialPos.ToString());
            circleObj = Instantiate(circle, initialPos, Quaternion.identity);
            circleObj.GetComponent<EmperorCircle>().p = gameObject;
            circleObj.GetComponent<EmperorCircle>().forceMagnitude = forceMagnitude;
        }

        if (Input.GetButtonUp("Fire1") && canUse)
        {
            canUse = false;
            targets.Clear();
            Destroy(circleObj);
            canUse = true;
        }
    }
}
