using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Complete
{
    public class PodiumCard : MonoBehaviour
    {
        private Vector3 originalScale;
        private PodiumManager podium;
        public SkillDetail skillDetail;

        private float scaleModifier = 2f;
        // Start is called before the first frame update
        void Start()
        {
            originalScale = transform.localScale;
            podium = transform.GetComponentInParent<PodiumManager>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnMouseOver()
        {
            transform.localScale = originalScale * scaleModifier;
        }

        private void OnMouseExit()
        {
            transform.localScale = originalScale;
        }

        private void OnMouseDown()
        {
            podium.ShowAddCards(skillDetail);
        }


    }
}