using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Complete
{
	public class StartRoomManager : MonoBehaviour
	{
		public GameObject exit;
		void Start() {
			var hallways = Resources.LoadAll<GameObject>("Hallways/North");
			var hallway = hallways.First();
			GameObject newHallways = Instantiate(hallway) as GameObject;
			newHallways.transform.SetParent(this.transform.parent);
			newHallways.transform.localPosition = exit.transform.localPosition;
			newHallways.transform.position = exit.transform.position ;
			newHallways.GetComponent<ConnectorInit>().SpawnRoom();
		}
	}
}
