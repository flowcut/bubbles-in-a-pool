using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaturnOnBubble : MonoBehaviour
{
    public int numSatellites;
    public float[] initialDegree = new float[7]
    {
        2 * Mathf.PI,
        2 * Mathf.PI / 3,
        2 * Mathf.PI / 3 * 2,
        2 * Mathf.PI / 4,
        2 * Mathf.PI / 4 * 3,
        2 * Mathf.PI,
        2 * Mathf.PI / 2,
    };

    public float[] length = new float[7]
    {
        7,
        7,
        7,
        11,
        11,
        13,
        13,
    };

    public int damage;

    public GameObject[] satellite = new GameObject[2];
    public List<GameObject> satelliteObjs = new List<GameObject>();

    public float lengthFactor = 4;
    public float speedFactor = 4;

    // private float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        // timer = 0;
        gameObject.transform.position = new Vector3(0, -200, 0);

        satellite[0] = Resources.Load<GameObject>("Prefabs/satellite_1");
        satellite[1] = Resources.Load<GameObject>("Prefabs/satellite_2");
        for ( int i=0; i < numSatellites; ++i)
        {
            Vector3 v = new Vector3(
                Mathf.Cos(initialDegree[i]) * length[i] * lengthFactor,
                Mathf.Sin(initialDegree[i]) * length[i],
                0);
            GameObject satelliteObj = Instantiate(satellite[Random.Range(0, 2)],
                gameObject.transform.position + v,
                Quaternion.identity,
                gameObject.transform);
            satelliteObj.GetComponent<Transform>().Rotate(0, 0, Random.Range(0, 360));
            satelliteObj.GetComponent<HasSide>().side =
                gameObject.GetComponent<HasSide>().side;
            satelliteObj.GetComponent<Painter>().TakeEffect();
            satelliteObj.GetComponent<SaturnSatellite>().degree = initialDegree[i];
            satelliteObj.GetComponent<SaturnSatellite>().length = length[i];
            satelliteObj.GetComponent<SaturnSatellite>().lengthFactor = lengthFactor;
            satelliteObj.GetComponent<SaturnSatellite>().damage = damage;
            satelliteObjs.Add(satelliteObj);
        }
        gameObject.transform.Rotate(0, 0, 30);
        StartCoroutine(Entering());
    }

    public IEnumerator Entering()
    {
        float smoothTime = 1.0f;
        float timer = 0.0f;
        Vector3 targetPosition = Vector3.zero;
        Vector3 velocity = Vector3.zero;
        while (timer < smoothTime + 1f)
        {
            gameObject.GetComponent<Transform>().position =
            Vector3.SmoothDamp(
                gameObject.GetComponent<Transform>().position,
                targetPosition, ref velocity,
                smoothTime, Mathf.Infinity, Time.deltaTime);
            yield return null;
            timer += Time.unscaledDeltaTime;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        foreach(var satellite in satelliteObjs)
        {
            Destroy(satellite);
        }
    }
}
