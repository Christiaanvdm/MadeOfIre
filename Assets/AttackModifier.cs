using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Complete
{


    public class AttackModifier : SimpleTimer
    {
        public void setup(string modifier_type, float modifier_magnitude, string icon_name = "")
        {
            type = modifier_type;
            magnitude = modifier_magnitude;
        }

       
    }
}