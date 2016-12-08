using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpecialResponseClass : MonoBehaviour {

	public int ItemIndex { get; set; }
	public string Command { get; set; }
	public string Response { get; set; }
	public Dictionary<int, int> Actions { get; set; }

	public SpecialResponseClass(int itemIndex, string command, string response, Dictionary<int, int> actions)
	{
		ItemIndex = itemIndex;
		Command = command;
		Response = response;
		Actions = actions;
	}
}
