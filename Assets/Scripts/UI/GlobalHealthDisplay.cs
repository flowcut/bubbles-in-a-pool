using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalHealthDisplay : MonoBehaviour
{
    public GameObject outsideHealthDisplay;
    public GameObject insideHealthDisplay;
    public GameObject frontWave;
    public GameObject backWave;
    public GameObject mask;

    public GameObject ring;
    public GameObject outerRing;

    public float fullHealthYPos = -20f;
    public float zeroHealthYPos = -90f;

    public float leftMostXPos = 50f;
    public float rightMostXPos = -50f;

    public float frontWaveSpeed = -20f;
    public float backWaveSpeed = -15f;

    public SideName side;
    // Start is called before the first frame update
    void Start()
    {
        side = GetComponent<HasSide>().side;
        Vector4 color = GameManager.sideColorOffset[side] / 255f;
        outsideHealthDisplay.GetComponent<Text>().color = color;
        frontWave.GetComponent<Image>().color = color;
        ring.GetComponent<Image>().color = color;
        outerRing.GetComponent<Image>().color = color;
        color[3] /= 2;
        backWave.GetComponent<Image>().color = color;
    }

    // Update is called once per frame
    void Update()
    {
        int current_health = GameManager.sideCurrentHealth[side];
        int max_health = GameManager.sideMaximumHealth[side];

        Vector3 newPos = frontWave.GetComponent<RectTransform>().localPosition;
        newPos[0] += frontWaveSpeed * Time.deltaTime;
        if (newPos[0] < rightMostXPos)
        {
            newPos[0] += 100f;
        }

        newPos[1] = zeroHealthYPos +
            (fullHealthYPos - zeroHealthYPos) * current_health / max_health;
        frontWave.GetComponent<RectTransform>().localPosition = newPos;

        insideHealthDisplay.GetComponent<RectTransform>().position =
            outsideHealthDisplay.GetComponent<RectTransform>().position;

        newPos = backWave.GetComponent<RectTransform>().localPosition;
        newPos[0] += backWaveSpeed * Time.deltaTime;
        if (newPos[0] < rightMostXPos)
        {
            newPos[0] += 100f;
        }
        newPos[1] = zeroHealthYPos +
            (fullHealthYPos - zeroHealthYPos) * current_health / max_health;
        backWave.GetComponent<RectTransform>().localPosition = newPos;

        insideHealthDisplay.GetComponent<Text>().text = current_health.ToString();
        outsideHealthDisplay.GetComponent<Text>().text = current_health.ToString();
    }
}
