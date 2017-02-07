using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpecialResponse{

	public int ItemIndex { get; set; }
	public string Image { get; set; }
	public string Command { get; set; }
	public string Response { get; set; }
	public int ItemState { get; set; }
	public Dictionary<int, int> Actions { get; set; }

	public SpecialResponse(int itemIndex, string image, string command, string response, int itemState, Dictionary<int, int> actions)
	{
		ItemIndex = itemIndex;
		Image = image;
		Command = command;
		Response = response;
		ItemState = itemState;
		Actions = actions;
	}
}
