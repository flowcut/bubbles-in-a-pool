using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthDisplay : MonoBehaviour
{
    [SerializeField] private GameObject health_mask;
    [SerializeField] private BubbleController bc;
    public float full_health_y = 0.84f;
    public float low_health_y = -7.18f;
    public float left_most_x = 4.51f;
    public float right_most_x = -4.38f;
    public float moving_speed = 3.0f;

    private Transform parent_transform;
    private void Awake()
    {
        // health = GetComponent<Health>();
        
    }

    // Start is called before the first frame update
    void Start()
    {
        moving_speed *= transform.localScale[0];
        left_most_x *= transform.localScale[0];
        right_most_x *= transform.localScale[0];
        full_health_y *= transform.localScale[1];
        low_health_y *= transform.localScale[1];
        if (moving_speed < 0)
        {
            right_most_x *= -1;
            left_most_x *= -1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        int current_health = bc.currentHealth;
        int maximum_health = bc.maxHealth;
        Vector3 previous_position = health_mask.transform.localPosition;
        previous_position[0] -= moving_speed * Time.deltaTime;
        if (moving_speed < 0)
        {
            if (previous_position[0] > right_most_x)
            {
                previous_position[0] = left_most_x;
            }
        } else
        {
            if (previous_position[0] < right_most_x)
            {
                previous_position[0] = left_most_x;
            }
        }
        
        previous_position[1] = low_health_y +
            (full_health_y - low_health_y) * current_health / maximum_health;
        health_mask.transform.localPosition = previous_position;

    }
}
