using System;
using System.Collections;
using UnityEngine;

namespace Util
{
    public class Tween
    {
        public static IEnumerator Linear(float durationInSeconds, Action<float> update, Action complete = null, bool useRealTime = true)
        {
            float changePerMs = 1 / (durationInSeconds * 1000f);
            float value = 0;
            float lastUpdateTime = (useRealTime ? Time.realtimeSinceStartup : Time.time);

            while (value < 1.0f)
            {
                yield return null;
                float now = (useRealTime ? Time.realtimeSinceStartup : Time.time);
                float deltaTime = (now - lastUpdateTime) * 1000;
                lastUpdateTime = now;

                float delta = deltaTime * changePerMs;
                value = Mathf.Min(delta + value, 1.0f);
                update.Invoke(value);
            }
            
            complete.Invoke();
        }
    }
}