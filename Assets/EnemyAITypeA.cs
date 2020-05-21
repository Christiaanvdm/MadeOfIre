//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//namespace Complete
//{
//    public class EnemyAITypeA : EnemyAI2D
//    {
//        public override void FireAtEnemy()
//        {
//            //base.FireAtEnemy();
//        }

//        protected override void LoadSprite()
//        {
//            enemyManager = gameObject.GetComponent<EnemyManager2D>();
//            GameObject spriteExample = Resources.Load("EnemySpriteA") as GameObject;
//            enemySprite = Instantiate(spriteExample, gameObject.transform.position, spriteExample.transform.rotation);
//            enemySprite.GetComponent<EnemySpriteManager>().enemy = enemyManager;
//        }

//        protected override void SetupBase()
//        {
//            base.SetupBase();
//        }
//    }
//}