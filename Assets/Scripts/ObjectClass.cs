using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectClass{
	
	public int Index {get; set;}
	public string Name { get; set; }
	public int State { get; set; }
	public List<ObjectClass> Objects {get; set;}
	public List<StateClass> States { get; set; }
	public List<int> AdjacentRooms { get; set; }

	public ObjectClass(int index, string name, int state, List<ObjectClass> objects, List<StateClass> states, List<int> adjacentRooms)
	{
		Index = index;
		Name = name;
		State = state;
		Objects = objects;
		States = states;
		AdjacentRooms = adjacentRooms;
	}
}
