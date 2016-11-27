using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Reflection;
using System.Collections;
using UnityEngine.UI;

class XMLParser : MonoBehaviour
{
    public TextAsset xmlDocument;
    public TextAppender appender;
	public Sprite[] images;
	public Image image;
	public List<ObjectClass> rooms;

    public static XmlDocument house;
    int room = 0;


    void Start()
    {
        house = new XmlDocument();
		house.LoadXml(xmlDocument.text);
    }

	public List<ObjectClass> ReadXML(XmlDocument xml)
	{
		XmlDocument houseXML = xml;
		List<ObjectClass> roomsList = new List<ObjectClass>();
		for (int i = 0; i < houseXML ["house"].FirstChild.ChildNodes.Count; ++i) {
			int index = i;
			string name = houseXML ["house"].FirstChild.ChildNodes [i].Attributes.GetNamedItem ("name").Value.ToLower ();
			List<StateClass> roomStates = new List<StateClass> ();
			for (int j = 0; j < houseXML ["house"].FirstChild.ChildNodes [i] ["states"].ChildNodes.Count; ++j) {
				int image = int.Parse(houseXML["house"].FirstChild.ChildNodes [i]["states"].ChildNodes[j]["image"].InnerText);
				string description = houseXML ["house"].FirstChild.ChildNodes [i] ["states"].ChildNodes [j] ["description"].InnerText;
				string get = "";
				int gettable = 0;

				Dictionary<int, int> prerequisites = new Dictionary<int, int> ();
				for (int x = 0; x < houseXML ["house"].FirstChild.ChildNodes [i] ["states"].ChildNodes [j] ["prerequisites"].ChildNodes.Count; ++x) {
					int item = int.Parse(houseXML ["house"].FirstChild.ChildNodes [i] ["states"].ChildNodes [j] ["prerequisites"].ChildNodes [x] ["item"].InnerText);
					int itemState = int.Parse(houseXML ["house"].FirstChild.ChildNodes [i] ["states"].ChildNodes [j] ["prerequisites"].ChildNodes [x] ["itemstate"].InnerText);
					prerequisites.Add (item, itemState);
				}

				ConditionalActionListClass conditionalActions = new ConditionalActionListClass (1, prerequisites);
				StateClass state = new StateClass (image, description, get, gettable, conditionalActions);
				roomStates.Add (state);
			}

			List<ObjectClass> items = new List<ObjectClass> ();
			for (int k = 0; k < houseXML["house"].FirstChild.ChildNodes [i] ["items"].ChildNodes.Count; ++k){
				int itemIndex = int.Parse(houseXML["house"].FirstChild.ChildNodes [i] ["items"].ChildNodes[k]["index"].InnerText);
				string itemName = houseXML["house"].FirstChild.ChildNodes [i] ["items"].ChildNodes[k].Attributes.GetNamedItem ("name").Value.ToLower ();
				List<StateClass> itemStates = new List<StateClass> ();
				for (int y = 0; y < houseXML ["house"].FirstChild.ChildNodes [i] ["items"].ChildNodes [k] ["states"].ChildNodes.Count; ++y) {
					int image = int.Parse(houseXML["house"].FirstChild.ChildNodes [i]["items"].ChildNodes[k]["states"].ChildNodes[y]["image"].InnerText);
					string description = houseXML ["house"].FirstChild.ChildNodes [i] ["items"].ChildNodes [k] ["states"].ChildNodes[y]["description"].InnerText;
					string get = houseXML["house"].FirstChild.ChildNodes [i] ["items"].ChildNodes [k] ["states"].ChildNodes[y]["get"].InnerText;
					int gettable = int.Parse(houseXML ["house"].FirstChild.ChildNodes [i] ["items"].ChildNodes [k] ["states"].ChildNodes [y] ["gettable"].InnerText);

					Dictionary<int, int> actions = new Dictionary<int, int> ();
					for (int x = 0; x < houseXML["house"].FirstChild.ChildNodes [i]["items"].ChildNodes[k]["states"].ChildNodes[y]["actions"].ChildNodes.Count; ++x) {
						int item = int.Parse(houseXML["house"].FirstChild.ChildNodes [i]["items"].ChildNodes[k]["states"].ChildNodes[y]["actions"].ChildNodes [x] ["item"].InnerText);
						int itemState = int.Parse(houseXML["house"].FirstChild.ChildNodes [i]["items"].ChildNodes[k]["states"].ChildNodes[y]["actions"].ChildNodes [x] ["itemstate"].InnerText);
						actions.Add (item, itemState);
					}

					ConditionalActionListClass conditionalActions = new ConditionalActionListClass (2, actions);
					StateClass state = new StateClass (image, description, get, gettable, conditionalActions);
					itemStates.Add (state);
				}

				List<ObjectClass> emptyList = new List<ObjectClass> ();
				List<int> emptyIntList = new List<int> ();
				ObjectClass newItem = new ObjectClass (itemIndex, itemName, 0, emptyList, itemStates, emptyIntList);
				items.Add (newItem);
			}

			List<int> adjacentRooms = new List<int> ();
			int test = (houseXML ["house"].FirstChild.ChildNodes [i] ["adjacentrooms"].ChildNodes.Count);
			for (int z = 0; z < houseXML ["house"].FirstChild.ChildNodes [i] ["adjacentrooms"].ChildNodes.Count; ++z) {
				adjacentRooms.Add(int.Parse (houseXML ["house"].FirstChild.ChildNodes [i] ["adjacentrooms"].ChildNodes[z].InnerText));
			}

			ObjectClass thisRoom = new ObjectClass (index, name, 0, items, roomStates, adjacentRooms );
			roomsList.Add (thisRoom);
		}

		return roomsList;
	
	}
		
    public void ReadInput(string text)
    {
        for(int j = 0; j < house["house"].LastChild.ChildNodes.Count; ++j)
        {
            if(text.ToLower().Contains(house["house"].LastChild.ChildNodes[j].InnerText.ToLower()))
            {
                object[] parameters = new object[1];
                parameters[0] = text;
                MethodInfo mInfo = typeof(XMLParser).GetMethod(house["house"].LastChild.ChildNodes[j].InnerText);
                mInfo.Invoke(this, parameters);
                return;
            }
        }
    }

    public void Look(string text)
    {
		if (text.Length > 5) {
			string itemName = text.Remove (0, 5);
			for (int i = 0; i < HouseManager.rooms[room].Objects.Count; ++i) {
				if (itemName.ToLower ().Contains (HouseManager.rooms[room].Objects[i].Name)) {
					int state = HouseManager.rooms [room].Objects [i].State;
					string description = HouseManager.rooms [room].Objects [i].States [state].Description;
					appender.text.text = "";
					appender.AppendText (description);

					if (HouseManager.rooms [room].Objects [i].States [state].Image != null) {
						image.sprite = images [HouseManager.rooms [room].Objects [i].States [state].Image];
					}
				}
			}
		}
		else
		{
			int state = HouseManager.rooms [room].State;
			string description = HouseManager.rooms [room].States [state].Description;
			appender.text.text = "";
			appender.AppendText(description);

			if (HouseManager.rooms [room].States [state].Image != null) {
				image.sprite = images [HouseManager.rooms [room].States [state].Image];
			}
		}
    }

    public void Move(string text)
    {
        int newRoom = room;
		for (int j = 0; j < HouseManager.rooms.Count; ++j)
        {
			if (text.ToLower().Contains(HouseManager.rooms[j].Name))
            {
                newRoom = j;
                break;
            }
        }

		for (int i = 0; i < HouseManager.rooms[room].AdjacentRooms.Count; ++i)
        {
			if(newRoom == HouseManager.rooms[room].AdjacentRooms[i])
            {
                room = newRoom;
                Look("");
                return;
            }
        }
    }

	public void Get(string text)
	{
		string itemName = text.Remove (0, 4);
		for (int i = 0; i < HouseManager.rooms[room].Objects.Count; ++i) {
			if (itemName.ToLower ().Contains (HouseManager.rooms[room].Objects[i].Name)) {
				int state = HouseManager.rooms [room].Objects [i].State;
				string get = HouseManager.rooms [room].Objects [i].States [state].Get;

				if (HouseManager.rooms [room].Objects [i].States [state].Gettable == 1) {
					// Inventory
				}

				appender.text.text = "";
				appender.AppendText (get);

				/*if (HouseManager.rooms [room].Objects [i].States [state].Image != null) {
					image.sprite = images [HouseManager.rooms [room].Objects [i].States [state].Image];
				}*/
			}
		}
	}
}

