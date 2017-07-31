using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

static class XMLParser
{
    public static List<GameObjectNew> ReadRooms(XElement house)
    {
        List<GameObjectNew> roomsList = new List<GameObjectNew>();

        int room_index = -1;
        foreach (var room in house.Element("rooms").Elements())
        {
            room_index++;

            string name = room.Attr("name").ToLower();

            List<State> roomStates = new List<State>();

            foreach (var state in room.Element("states").Elements())
            {
                string image_id = state.Element("image").Value;
                string description = state.Elt("description");
                Dictionary<int, int> prerequisites = state.Element("prerequisites").Elements().ToDictionary(
                    x => int.Parse(x.Elt("item")), x => int.Parse(x.Elt("itemstate")));

                ConditionalActionList conditionalActions = new ConditionalActionList(1, prerequisites);

                string get = "";
				int gettable = int.Parse(state.Elt("gettable"));

                roomStates.Add(new State(image_id, description, get, gettable, conditionalActions));
            }


            List<GameObjectNew> items = new List<GameObjectNew>();
            foreach (var item in room.Element("items").Elements())
            {
                int itemIndex = int.Parse(item.Elt("index"));
                string itemName = item.Attr("name").ToLower();
				int deleteCap = (item.Elt ("deletecap") != null) ? int.Parse (item.Elt ("deletecap")) : 0;

                List<State> itemStates = new List<State>();

                foreach (var state in item.Element("states").Elements())
                {
                    string image = state.Elt("image");
                    string description = state.Elt("description");
                    string get = state.Elt("get");
                    int gettable = int.Parse(state.Elt("gettable"));

                    Dictionary<int, int> actions = state.Element("actions").Elements().ToDictionary(
                        x => int.Parse(x.Elt("item")), x => int.Parse(x.Elt("itemstate")));

                    ConditionalActionList conditionalActions = new ConditionalActionList(2, actions);
                    itemStates.Add(new State(image, description, get, gettable, conditionalActions));
                }

                List<GameObjectNew> emptyList = new List<GameObjectNew>();
                List<int> emptyIntList = new List<int>();
				GameObjectNew newItem = new GameObjectNew(itemIndex, itemName, deleteCap, false, 0, emptyList, itemStates, emptyIntList);
                items.Add(newItem);
            }

            List<int> adjacentRooms = room.Element("adjacentrooms").Elements().Select(x => int.Parse(x.Value)).ToList();

			GameObjectNew thisRoom = new GameObjectNew(room_index, name, 0, false, 0, items, roomStates, adjacentRooms);
            roomsList.Add(thisRoom);
        }

        return roomsList;
    }

	public static List<GameObjectNew> ReadItems(XElement house)
	{
		List<GameObjectNew> itemsList = new List<GameObjectNew>();

		int room_index = -1;
		foreach (var room in house.Element("rooms").Elements())
		{
			room_index++;
			foreach (var item in room.Element("items").Elements())
			{
				int itemIndex = int.Parse(item.Elt("index"));
				string itemName = item.Attr("name").ToLower();
				int deleteCap = (item.Elt ("deletecap") != null) ? int.Parse (item.Elt ("deletecap")) : 0;

				List<State> itemStates = new List<State>();

				foreach (var state in item.Element("states").Elements())
				{
					string image = state.Elt("image");
					string description = state.Elt("description");
					string get = state.Elt("get");
					int gettable = int.Parse(state.Elt("gettable"));

					Dictionary<int, int> actions = state.Element("actions").Elements().ToDictionary(
						x => int.Parse(x.Elt("item")), x => int.Parse(x.Elt("itemstate")));

					ConditionalActionList conditionalActions = new ConditionalActionList(2, actions);
					itemStates.Add(new State(image, description, get, gettable, conditionalActions));
				}

				List<GameObjectNew> emptyList = new List<GameObjectNew>();
				List<int> emptyIntList = new List<int>();
				GameObjectNew newItem = new GameObjectNew(itemIndex, itemName, deleteCap, false, 0, emptyList, itemStates, emptyIntList);
				itemsList.Add(newItem);
			}
		}



		return itemsList.OrderBy(w => w.Index).ToList();
	}

    public static List<SpecialResponse> ReadSpecialResponses(XElement house)
    {
        return house.Element("specialresponses").Elements().Select(specialresponse =>
        {
            int itemIndex = int.Parse(specialresponse.Elt("itemindex"));
            string image = specialresponse.Elt("image");
            string command = specialresponse.Elt("command");
            string response = specialresponse.Elt("response");
            int requiredItemState = int.Parse(specialresponse.Elt("itemstate"));

            Dictionary<int, int> actions = specialresponse.Element("actions").Elements().ToDictionary(
                x => int.Parse(x.Elt("item")), x => int.Parse(x.Elt("itemstate")));

            return new SpecialResponse(itemIndex, image, command, response, requiredItemState, actions);
        }).ToList();
    }

    public static List<String> ReadCommands(XElement house)
    {
        return house.Element("commands").Elements().Select(x => x.Value).ToList();
    }

	public static List<ItemGroup> ReadItemGroups(XElement house)
	{
		return house.Element("itemgroups").Elements().Select(itemgroup =>
		{
			int baseItemIndex = int.Parse(itemgroup.Elt("baseitem"));
			List<int> items = itemgroup.Element("items").Elements().Select(
				x => int.Parse(x.Value)).ToList();
			List<int> nonResetItems = itemgroup.Element("nonreset").Elements().Select(
				x => int.Parse(x.Value)).ToList();
			return new ItemGroup(baseItemIndex, items, nonResetItems);
		}).ToList();
	}

	public static List<CheckItem> ReadCheckItems(XElement house)
	{
		return house.Element("checkitems").Elements().Select(checkitem =>
		{
			int baseItemIndex = int.Parse(checkitem.Elt("baseitem"));
			int baseItemState = int.Parse(checkitem.Elt("baseitemstate"));

			List<CompareItem> compareItems = new List<CompareItem>();

			foreach (var ci in checkitem.Element("compareitems").Elements()){
				Dictionary <int, int> states = ci.Element("items").Elements().ToDictionary(
						x => int.Parse(x.Elt("id")), x => int.Parse(x.Elt("state")));
				string image = ci.Elt("image");
				string description = (ci.Elt ("look") != null) ? ci.Elt ("look") : "";
				string overlay = (ci.Elt("overlay") != null) ? ci.Elt("overlay") : "";
				CompareItem thisCI = new CompareItem(image, overlay, description, states);
				compareItems.Add(thisCI);
			}
				
			return new CheckItem(baseItemIndex, baseItemState, compareItems);
		}).ToList();
	}

	public static Dictionary<int, string> ReadAudioIndex(XElement house)
	{
		Dictionary<int, string> audioClips = house.Element("audioindex").Elements().ToDictionary(
			x => int.Parse(x.Elt("id")), x => x.Elt("name"));

		return audioClips;
	}

	public static List<MultiSequence> ReadMultiSequences(XElement house)
	{
		return house.Element("multisequences").Elements().Select(multisequence =>
		{

			Dictionary<string, string> steps = multisequence.Element("steps").Elements().ToDictionary(
				x => x.Elt("image"), x => x.Elt("text"));
			bool win = bool.Parse(multisequence.Elt("win"));

			return new MultiSequence(steps, win);
		}).ToList();
	}
}