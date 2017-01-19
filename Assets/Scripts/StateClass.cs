﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StateClass{

	public int Image {get; set;}
	public string Description { get; set; }
	public string Get { get; set; }
	public int Gettable { get; set; }
	public ConditionalActionListClass ConditionalActions { get; set; }

	public StateClass(int image, string description, string get, int gettable, ConditionalActionListClass conditionalActions)
	{
		Image = image;
		Description = description;
		Get = get;
		Gettable = gettable;
		ConditionalActions = conditionalActions;
	}

}
