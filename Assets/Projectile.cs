using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Complete
{


    public class Projectile : MonoBehaviour
    {
        public Rigidbody rigidbody;
        public Color color;
        private Renderer renderer;
      
 
        

        public void setColor()
        {

        }

  

      



       

        public void MakeRed()
        {
            //float redValue = 1;
            ////float fRedValue = (currentHealth / (maximumHealth - minimumHealth));
            //Color newColor = new Color(redValue, 0, 0);
            ////Find the Specular shader and change its Color to red
            //renderer.material.shader = Shader.Find("Standard");
            //renderer.material.SetColor("_Color", newColor);
        }

        public void MakeWhite()
        {
            //float redValue = 0;
            ////float fRedValue = (currentHealth / (maximumHealth - minimumHealth));
            //Color newColor = new Color(redValue, 0, 0);
            ////Find the Specular shader and change its Color to red
            //renderer.material.shader = Shader.Find("Standard");
            //renderer.material.SetColor("_Color", newColor);
        }

        public void UpdateColor(Color color)
        {
            //renderer.material.shader = Shader.Find("Standard");
            //renderer.material.SetColor("_Color", color);
        }

    }
}