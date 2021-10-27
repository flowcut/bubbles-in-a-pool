using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ForceLineController : MonoBehaviour
{
    public GameObject square;
    public float forceMagnitude = 1.0f;

    private GameObject squareObj;
    private Vector3 initialPos;
    private float standardLength = 13f;
    private float defaultScale = 15;
    private Vector3 direction;

    public bool canUse = true;

    public List<GameObject> targets = new List<GameObject>();
    public GameObject ForceLineButton;
    // Start is called before the first frame update
    void Start()
    {
        square = Resources.Load<GameObject>("Prefabs/SquareEffect");
        defaultScale = square.transform.localScale[0];
    }


    // Update is called once per frame
    void Update()
    {
        if (squareObj != null)
        {
            Vector3 distance = Camera.main.ScreenToWorldPoint(Input.mousePosition) + 
                new Vector3(0, 0, 10) - initialPos;
            direction = distance.normalized;
            squareObj.transform.up = direction;
            float length = Mathf.Clamp(distance.magnitude, 0, 5 * standardLength);
            Vector3 scale = squareObj.transform.localScale;
            scale[1] = length / standardLength * defaultScale;
            squareObj.transform.localScale = scale;
        }


        if (Input.GetButtonDown("Fire1") && canUse)
        {

            initialPos = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 0, 10);
            // Debug.Log(initialPos.ToString());
            squareObj = Instantiate(square, initialPos, Quaternion.identity);
            squareObj.GetComponent<ForceLineSquare>().p = gameObject;
        }

        if (Input.GetButtonDown("Fire2"))
        {
            canUse = false;
            if (squareObj != null)
            {
                // GameObject newSquareObj = squareObj;
                Destroy(squareObj);                
            }
            canUse = true;
            // this.enabled = false;
        }

        if (Input.GetButtonUp("Fire1") && canUse && squareObj != null)
        {
            canUse = false;
            StartCoroutine(ForceLine());
            // ForceLineButton.GetComponent<Button>().interactable = false;
            // this.enabled = false;
        }
        
    }

    private IEnumerator ForceLine()
    {
        GameObject newSquareObj = squareObj;
        BoxCollider2D collider = newSquareObj.GetComponent<BoxCollider2D>();
        collider.enabled = true;
        yield return null;
        foreach (GameObject target in targets)
        {
            if (target != null)
                target.GetComponent<Rigidbody2D>().AddForce(
                    direction * forceMagnitude, ForceMode2D.Impulse);
        }
        targets.Clear();
        yield return null;
        Destroy(newSquareObj);
        canUse = true;
    }
}
