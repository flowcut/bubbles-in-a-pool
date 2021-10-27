using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillLevelDisplay : MonoBehaviour
{
    public GameObject lv1;
    public GameObject lv2;
    public GameObject lv3;
    public int level = 0;
    // Start is called before the first frame update

    public void SetColor(Vector4 color)
    {
        lv1.GetComponent<Image>().color = color;
        lv2.GetComponent<Image>().color = color;
        lv3.GetComponent<Image>().color = color;
    }
    public void LevelUp()
    {
        level++;
        switch (level)
        {
            case 1:
                lv1.SetActive(true);
                break;
            case 2:
                lv2.SetActive(true);
                break;
            case 3:
                lv3.SetActive(true);
                break;
        }

    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
