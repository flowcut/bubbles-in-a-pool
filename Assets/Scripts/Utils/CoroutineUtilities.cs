using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoroutineUtilities
{
    public static IEnumerator MoveObjectOverTime(Transform target, Vector3 initial_pos, Vector3 dest_pos, float duration_sec)
    {
        float initial_time = Time.time;
        // The "progress" variable will go from 0.0f -> 1.0f over the course of "duration_sec" seconds.
        float progress = (Time.time - initial_time) / duration_sec;
        
        while (progress < 1.0f)
        {
            if (target == null)
            {
                break;
            }
            // Recalculate the progress variable every frame. Use it to determine
            // new position on line from "initial_pos" to "dest_pos"
            
            progress = (Time.time - initial_time) / duration_sec;
            Vector3 new_position = Vector3.Lerp(initial_pos, dest_pos, progress);
            target.position = new_position;

            // yield until the end of the frame, allowing other code / coroutines to run
            // and allowing time to pass.
            yield return null;
        }

        if (target != null)
        {
            target.position = dest_pos;
        }
    }

    public static IEnumerator RotateObjectOverTime(Transform target, float degree, float duration_sec)
    {
        float initial_time = Time.time;
        // The "progress" variable will go from 0.0f -> 1.0f over the course of "duration_sec" seconds.
        float progress = (Time.time - initial_time) / duration_sec;
        Vector3 initial_rot = target.eulerAngles;
        Vector3 dest_rot = target.eulerAngles;
        dest_rot.z += degree;

        while (progress < 1.0f)
        {
            if (target == null)
            {
                break;
            }
            // Recalculate the progress variable every frame. Use it to determine
            // new position on line from "initial_pos" to "dest_pos"
            progress = (Time.time - initial_time) / duration_sec;
            Vector3 new_rot = Vector3.Lerp(initial_rot, dest_rot, progress);
            target.eulerAngles = new_rot;

            // yield until the end of the frame, allowing other code / coroutines to run
            // and allowing time to pass.
            yield return null;
        }

        if (target != null)
        {
            target.eulerAngles = dest_rot;
        }
    }
}