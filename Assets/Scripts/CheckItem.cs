using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CheckItem{

	public int BaseItemIndex { get; set; }
	public int BaseItemState { get; set; }
	public int CompareItem { get; set; }
	public Dictionary <int, string> States { get; set; }

	public CheckItem(int baseItemIndex, int baseItemState, int compareItem, Dictionary <int, string> states)
	{
		BaseItemIndex = baseItemIndex;
		BaseItemState = baseItemState;
		CompareItem = compareItem;
		States = states;
	}
}