using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtingEffectController : MonoBehaviour
{
    public float life = 0.5f;
    public Vector3 direction;

    public float distanceFactor = 1;
    public float sizeFactor = 1;

    private float distance = GameManager.bubbleStandardRadius * 6;
    private SpriteRenderer sr;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        gameObject.transform.up = direction.normalized;
        distance *= distanceFactor;
        gameObject.transform.localScale *= sizeFactor;
        StartCoroutine(CoroutineUtilities.MoveObjectOverTime(
                gameObject.transform,
                gameObject.transform.position,
                gameObject.transform.position + direction * distance,
                life
            ));
        StartCoroutine(Fade());
        Destroy(gameObject, life + 0.1f);
    }

    private IEnumerator Fade()
    {
        float timer = life;
        Vector4 initial_color = sr.color;
        while (timer > 0)
        {
            Vector4 new_color = initial_color;
            new_color[3] *= timer / life;
            sr.color = new_color;
            timer -= Time.deltaTime;
            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
