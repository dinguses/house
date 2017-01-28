using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameObject {
	
	public int Index {get; set;}
	public string Name { get; set; }
	public int State { get; set; }
	public List<GameObject> Objects {get; set;}
	public List<State> States { get; set; }
	public List<int> AdjacentRooms { get; set; }

	public GameObject(int index, string name, int state, List<GameObject> objects, List<State> states, List<int> adjacentRooms)
	{
		Index = index;
		Name = name;
		State = state;
		Objects = objects;
		States = states;
		AdjacentRooms = adjacentRooms;
	}
}
