using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpecialResponseClass : MonoBehaviour {

	public int ItemIndex { get; set; }
	public int Image { get; set; }
	public string Command { get; set; }
	public string Response { get; set; }
	public Dictionary<int, int> Actions { get; set; }

	public SpecialResponseClass(int itemIndex, int image, string command, string response, Dictionary<int, int> actions)
	{
		ItemIndex = itemIndex;
		Image = image;
		Command = command;
		Response = response;
		Actions = actions;
	}
}
