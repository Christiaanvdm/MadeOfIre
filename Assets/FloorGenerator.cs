using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Complete
{
	class FloorGenerator : MonoBehaviour
	{
		private void Start()
		{
			// Get starter room
			var startRoom = Resources.Load("StartRoom", typeof(GameObject)) as GameObject;
			startRoom.transform.position = new Vector3(0, 0, 0);

		}
	}
}
