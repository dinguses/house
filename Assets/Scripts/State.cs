using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class State {

	public string Image {get; set;}
	public string Description { get; set; }
	public string Get { get; set; }
	public int Gettable { get; set; }
	public ConditionalActionList ConditionalActions { get; set; }

	public State(string image, string description, string get, int gettable, ConditionalActionList conditionalActions)
	{
		Image = image;
		Description = description;
		Get = get;
		Gettable = gettable;
		ConditionalActions = conditionalActions;
	}

}
