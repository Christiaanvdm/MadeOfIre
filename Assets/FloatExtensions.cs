using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Complete
{
    public static class FloatExtensions
    {
        public static int ToMilliSeconds(this float input) {
            return Mathf.RoundToInt(input * 1000);
        }
    }
}
