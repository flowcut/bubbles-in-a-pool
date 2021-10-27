using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandAnimation : MonoBehaviour
{
    public float life = 0.7f;
    public Vector3 shift = new Vector3(0, 0, 0);
    public float scaleFactor = 1.2f;

    private float timer = 0;
    private Vector3 originalScale;
    private SpriteRenderer sr;
    private Color color;
    // Start is called before the first frame update
    void Start()
    {
        sr = gameObject.GetComponent<SpriteRenderer>();
        color = sr.color;
        color[3] /= 2;
        sr.color = color;
        originalScale = gameObject.transform.localScale;
        Destroy(gameObject, life);
    }

    public void cloneSprite(GameObject target)
    {
        Debug.Log("Cloning Sprite.");
        gameObject.GetComponent<SpriteRenderer>().sprite =
            target.GetComponent<SpriteRenderer>().sprite;
        gameObject.GetComponent<SpriteRenderer>().color =
            target.GetComponent<SpriteRenderer>().color;
        gameObject.transform.position = target.transform.position;
        gameObject.transform.rotation = target.transform.rotation;
        gameObject.transform.localScale = target.transform.localScale;
    }
    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        Color newColor = color;
        newColor[3] *= (life - timer) / life;
        sr.color = newColor;

        gameObject.transform.position += shift * (Time.deltaTime / life);

        gameObject.transform.localScale = originalScale *
            (1 + (scaleFactor - 1) * (timer / life));
    }
}
