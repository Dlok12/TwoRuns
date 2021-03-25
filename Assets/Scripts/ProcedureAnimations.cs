using System;
using System.Collections;
using UnityEngine;

namespace TwoRuns
{
    class ProcedureAnimations
    {
        public static IEnumerator Fade(Material material, Action afterAnimation = null, float afterFadeDelay = 2f, float fadeDelay = 2f, float fadeTick = .1f)
        {
            float nTick = fadeDelay / fadeTick;
            Color color = material.color;

            for (float ft = 1f; ft >= 0; ft -= 1 / nTick)
            {
                color = material.color;
                color.a = ft;
                material.color = color;
                yield return new WaitForSeconds(fadeTick);
            }
            color.a = 0;
            material.color = color;

            yield return new WaitForSeconds(afterFadeDelay);

            color.a = 1;
            material.color = color;

            afterAnimation();
        }
        public static IEnumerator Shake(Transform transform, float amplitude = .3f, float timeSec = .7f, float tickSec = .01f)
        {
            int nTick = (int)(timeSec / tickSec);
            Vector3 position = transform.localPosition;
            for (int i = 0; i < nTick; i++)
            {
                transform.localPosition = position + new Vector3(
                    UnityEngine.Random.Range(-amplitude, amplitude),
                    UnityEngine.Random.Range(-amplitude, amplitude),
                    UnityEngine.Random.Range(-amplitude, amplitude));
                yield return new WaitForSeconds(tickSec);
            }
            transform.localPosition = position;
        }
        public static IEnumerator Stay(Transform transform, Action afterAnimation = null, float time = 4f)
        {
            float t = 0;
            while (t < time)
            {
                transform.position += new Vector3(0, 0, -Level.CurrentLevel.levelSpeed);
                yield return new WaitForSeconds(Time.fixedDeltaTime);
                t += Time.fixedDeltaTime;
            }

            transform.localPosition = Vector3.zero;
            afterAnimation();
        }
    }
}
