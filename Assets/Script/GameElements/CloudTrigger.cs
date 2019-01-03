using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameElements
{
    public class CloudTrigger : MonoBehaviour
    {
        public Animation[] clouds;
        public float triggerDuration;

        public void Hide()
        {
            for (int i = 0; i < clouds.Length; i++)
            {
                clouds[i].Rewind();
                clouds[i].gameObject.SetActive(false);
            }
        }
        public void Show()
        {
            for (int i = 0; i < clouds.Length; i++)
            {
                clouds[i].gameObject.SetActive(true);
            }
            gameObject.SetActive(true);
            StartCoroutine(Trigger());
        }
        private IEnumerator Trigger()
        {
            for (int i = 0; i < clouds.Length; i++)
            {
                clouds[i].Stop();
                clouds[i].Rewind();
            }
            for (int i = 0; i < clouds.Length; i++)
            {
                clouds[i].Play();
                yield return new WaitForSeconds(triggerDuration);
            }
        }
    }
}