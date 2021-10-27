using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSetSide : MonoBehaviour
{
    ParticleSystem particle;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetSide()
    {
        particle = GetComponent<ParticleSystem>();
        SideName side = GetComponent<HasSide>().side;
        var col = particle.colorOverLifetime;
        Gradient grad = new Gradient();

        var colorKey = new GradientColorKey[2];
        colorKey[0].color = GameManager.sideColorOffset[side] / 255f;
        colorKey[0].time = 0.0f;
        colorKey[1].color = GameManager.sideColorOffset[side] / 255f;
        colorKey[1].time = 1.0f;

        var alphaKey = new GradientAlphaKey[2];
        alphaKey[0].alpha = 1.0f;
        alphaKey[0].time = 0.0f;
        alphaKey[1].alpha = 0.0f;
        alphaKey[0].time = 1.0f;

        grad.SetKeys(colorKey, alphaKey);
        col.color = grad;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
