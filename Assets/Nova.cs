using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Complete {
    public class Nova : MonoBehaviour
    {
        private float NovaEffectDuration = 3f;
        private float magnitude = 0.5f;
        private int iterations = 8;
        private float maxSize = 13f;
        private float minSize;
        private float stepSize;
        GameObject player;
        // Start is called before the first frame update
        void Start()
        {

            StartCoroutine("expandWave");
            stepSize = minSize = maxSize / iterations;
            transform.localScale = new Vector3(stepSize, 1, stepSize);

            transform.parent.SetParent(GameObject.Find("Player").transform);
            transform.parent.localPosition = new Vector3(0, 0, 0);


        }
        int expansionCount = 0;
        IEnumerator expandWave()
        {
            yield return new WaitForFixedUpdate();

            for (; ; )
            {
                yield return new WaitForSeconds(0.1f);
                transform.localScale += new Vector3(stepSize, 1, stepSize);
                expansionCount += 1;
                if (expansionCount == iterations)
                {
                    Destroy(transform.parent.gameObject);
                }
            }
        }
        // Update is called once per frame
        void Update()
        {
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "enemy")
            {
                IEnemyController enemy = other.gameObject.GetComponentInParent<IEnemyController>();
                ModifierInfo newAM = new ModifierInfo();
                newAM.type = "half_speed";
                newAM.duration = NovaEffectDuration;
                newAM.context = "debuff";
                newAM.magnitude = magnitude;
                enemy.addDebuff(newAM);
            }

        }

    }
}