using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StateClass : MonoBehaviour {

	public int Image {get; set;}
	public string Description { get; set; }
	public string Get { get; set; }
	public ConditionalActionListClass ConditionalActions { get; set; }

	public StateClass(int image, string description, string get, ConditionalActionListClass conditionalActions)
	{
		Image = image;
		Description = description;
		Get = get;
		ConditionalActions = conditionalActions;
	}

}
