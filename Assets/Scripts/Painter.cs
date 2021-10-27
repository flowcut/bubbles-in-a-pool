using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Painter : MonoBehaviour
{
    // Start is called before the first frame update
    public float Opacity = 1.0f;
    private SpriteRenderer sr;
    void Start()
    {
        TakeEffect();
    }

    public void TakeEffect()
    {
        sr = GetComponent<SpriteRenderer>();
        SideName side = FindSide(gameObject);

        Vector4 tempColor = GameManager.sideColorOffset[side];
        tempColor[0] /= 255.0f;
        tempColor[1] /= 255.0f;
        tempColor[2] /= 255.0f;
        tempColor[3] = Opacity;
        sr.color = tempColor;
    }

    private SideName FindSide(GameObject current)
    {
        if (current.GetComponent<HasSide>() != null)
        {
            return current.GetComponent<HasSide>().side;
        }

        if (current.transform.parent != null)
        {
            return FindSide(current.transform.parent.gameObject);
        }
        else
        {
            return GameManager.defaultSide;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
