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
		private Transform exit;

		void Start() {
			exit = transform.parent.Find("Exit");
			SpawnRoom(0);
		}

		public void SpawnRoom(int mazeDepth)
		{
			var rooms = Resources.LoadAll<GameObject>("Rooms/South");
			var room = rooms.First(x => x.name == "ARoom2");
			GameObject newRoom = Instantiate(room) as GameObject;


			newRoom.transform.position = exit.position + (newRoom.transform.position - newRoom.transform.Find("Entrance").position);
			newRoom.transform.Find("RoomArea").GetComponent<RoomManager>().InitializeMap(mazeDepth);
		}
	}
}
