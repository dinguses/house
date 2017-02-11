using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemGroup{

	public int BaseItemIndex { get; set; }
	public List<int> Items { get; set; }

	public ItemGroup(int baseItemIndex, List<int> items)
	{
		BaseItemIndex = baseItemIndex;
		Items = items;
	}
}
