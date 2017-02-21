using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameObject {
	
	public int Index {get; set;}
	public string Name { get; set; }
	public int DeleteCap { get; set; }
	public bool Visited { get; set; }
	public int State { get; set; }
	public List<GameObject> Objects {get; set;}
	public List<State> States { get; set; }
	public List<int> AdjacentRooms { get; set; }

    public State currentState
    {
        get
        {
            return States[State];
        }
    }

    public GameObject GetObjectById(int id)
    {
        return Objects.Find(x => x.Index == id);
    }

    public GameObject GetObjectByName(string name)
    {
        return Objects.Find(x => x.Name == name);
    }

	public GameObject(int index, string name, int deleteCap, bool visited, int state, List<GameObject> objects, List<State> states, List<int> adjacentRooms)
	{
		Index = index;
		Name = name;
		DeleteCap = deleteCap;
		Visited = visited;
		State = state;
		Objects = objects;
		States = states;
		AdjacentRooms = adjacentRooms;
	}
}
