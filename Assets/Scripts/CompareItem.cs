using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CompareItem{

	public string ImageName { get; set; }
	public string Overlay { get; set; }
	public string Description {get;set;}
	public Dictionary <int, int> States { get; set; }

	public CompareItem(string imageName, string overlay, string description, Dictionary <int, int> states)
	{
		ImageName = imageName;
		Overlay = overlay;
		Description = description;
		States = states;
	}
}