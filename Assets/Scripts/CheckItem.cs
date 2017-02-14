using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CheckItem{

	public int BaseItemIndex { get; set; }
	public int BaseItemState { get; set; }
	public List<CompareItem> CompareItems {get;set;}
	/*public int CompareItem { get; set; }
	public Dictionary <int, string> States { get; set; }*/

	public CheckItem(int baseItemIndex, int baseItemState, List<CompareItem> compareItems)
	{
		BaseItemIndex = baseItemIndex;
		BaseItemState = baseItemState;
		CompareItems = compareItems;
		/*CompareItem = compareItem;
		States = states;*/
	}
}