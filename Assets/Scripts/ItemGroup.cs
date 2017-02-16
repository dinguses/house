using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemGroup{

	public int BaseItemIndex { get; set; }
	public List<int> Items { get; set; }
	public List<int> NonResetItems { get; set; }

	public ItemGroup(int baseItemIndex, List<int> items, List<int> nonResetItems)
	{
		BaseItemIndex = baseItemIndex;
		Items = items;
		NonResetItems = nonResetItems;
	}
}
