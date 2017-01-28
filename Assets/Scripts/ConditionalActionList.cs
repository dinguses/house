using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ConditionalActionList {

	//public int Index {get;set;}
	public int Type { get; set; }
	public Dictionary<int, int> ConditionalActions { get; set; }

	public ConditionalActionList(/*int index,*/ int type, Dictionary<int, int> conditionalActions)
	{
		//Index = index;
		Type = type;
		ConditionalActions = conditionalActions;
	}
}
