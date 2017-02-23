using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MultiSequence{

	public Dictionary<string, string> Steps { get; set; }
	public bool Win { get; set; }

	public MultiSequence(Dictionary<string, string> steps, bool win)
	{
		Steps = steps;
		Win = win;
	}
}
