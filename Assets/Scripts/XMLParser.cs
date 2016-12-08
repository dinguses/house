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
		List<SpecialResponseClass> specialResponses = new List<SpecialResponseClass> ();
		List<string> commands = new List<string> ();
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
				//HouseManager.itemsList.Add (newItem);
			}

			List<int> adjacentRooms = new List<int> ();
			int test = (houseXML ["house"].FirstChild.ChildNodes [i] ["adjacentrooms"].ChildNodes.Count);
			for (int z = 0; z < houseXML ["house"].FirstChild.ChildNodes [i] ["adjacentrooms"].ChildNodes.Count; ++z) {
				adjacentRooms.Add(int.Parse (houseXML ["house"].FirstChild.ChildNodes [i] ["adjacentrooms"].ChildNodes[z].InnerText));
			}

			ObjectClass thisRoom = new ObjectClass (index, name, 0, items, roomStates, adjacentRooms );
			roomsList.Add (thisRoom);
		}

		for (int j=0; j < houseXML ["house"]["specialresponses"].ChildNodes.Count; ++j) {
			int itemIndex = int.Parse( houseXML ["house"]["specialresponses"].ChildNodes[j] ["itemindex"].InnerText);
			string command =  houseXML ["house"]["specialresponses"].ChildNodes [j] ["command"].InnerText;
			string response =  houseXML ["house"]["specialresponses"].ChildNodes [j] ["response"].InnerText;

			Dictionary<int, int> actions = new Dictionary<int, int> ();
			for (int a = 0; a <houseXML ["house"]["specialresponses"].ChildNodes [j] ["actions"].ChildNodes.Count; ++a) {
				int item = int.Parse(houseXML ["house"]["specialresponses"].ChildNodes [j] ["actions"].ChildNodes [a] ["item"].InnerText);
				int itemState = int.Parse(houseXML ["house"]["specialresponses"].ChildNodes [j] ["actions"].ChildNodes [a] ["itemstate"].InnerText);
				actions.Add (item, itemState);
			}

			SpecialResponseClass src = new SpecialResponseClass (itemIndex, command, response, actions);
			specialResponses.Add (src);
		}

		for (int j = 0; j < house ["house"].LastChild.ChildNodes.Count; ++j) {
			string command = house ["house"].LastChild.ChildNodes [j].InnerText;
			commands.Add (command);
		}

		HouseManager.specialResponses = specialResponses;
		HouseManager.commands = commands;

		return roomsList;
	
	}
		
    public void ReadInput(string text)
    {
		bool defaultCommand = false;
		for(int i = 0; i < HouseManager.commands.Count; ++i)
        {
			if (text.ToLower ().Contains (HouseManager.commands [i].ToLower ())) {
				object[] parameters = new object[1];
				parameters [0] = text;
				MethodInfo mInfo = typeof(XMLParser).GetMethod (HouseManager.commands [i]);
				mInfo.Invoke (this, parameters);
				defaultCommand = true;
				return;
			}
        }
		if (!defaultCommand) {
			OtherCommands (text);	
		}
    }

    public void Look(string text)
    {
		if (text.Length > 5) {
			string itemName = text.Remove (0, 5);
			for (int i = 0; i < HouseManager.rooms[room].Objects.Count; ++i) {
				if (itemName.ToLower () == HouseManager.rooms[room].Objects[i].Name) {
					int state = HouseManager.rooms [room].Objects [i].State;
					string description = HouseManager.rooms [room].Objects [i].States [state].Description;


					if (description == "") {
						appender.text.text = "";
						appender.AppendText (GenericLook());
					} else {

						appender.text.text = "";
						appender.AppendText (description);

						if (HouseManager.rooms [room].Objects [i].States [state].Image != -1) {				
							image.sprite = images [HouseManager.rooms [room].Objects [i].States [state].Image];
						} else {
							int roomState = HouseManager.rooms [room].State;
							image.sprite = images [HouseManager.rooms [room].States [roomState].Image];
						}

					}

					return;
				}
			}

			// if there's no item of that name
			appender.text.text = "";
			appender.AppendText (GenericLook ());
		}
		else
		{
			int state = HouseManager.rooms [room].State;
			string description = HouseManager.rooms [room].States [state].Description;
			appender.text.text = "";
			appender.AppendText(description);
			image.sprite = images [HouseManager.rooms [room].States [state].Image];
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
			if (itemName.ToLower () == HouseManager.rooms[room].Objects[i].Name) {
				int state = HouseManager.rooms [room].Objects [i].State;
				string get = HouseManager.rooms [room].Objects [i].States [state].Get;

				if (HouseManager.rooms [room].Objects [i].States [state].Gettable == 1) {
					HouseManager.inventory.Add(HouseManager.rooms [room].Objects [i].Index);
					HouseManager.rooms [room].Objects [i].State++;

					state = HouseManager.rooms [room].Objects [i].State;
					int test = HouseManager.rooms [room].Objects [i].States [state].ConditionalActions.Type;
					foreach (KeyValuePair<int, int> actions in HouseManager.rooms [room].Objects [i].States[state].ConditionalActions.ConditionalActions) {
						int item = actions.Key;
						int itemState = actions.Value;

						foreach (ObjectClass oc in HouseManager.rooms [room].Objects){
							if (oc.Index == item) {
								oc.State = itemState;
							}
						}
					}
				}

				appender.text.text = "";
				appender.AppendText (get);
				return;
			}

			// if there's no item of that name
			appender.text.text = "";
			appender.AppendText (GenericGet());
		}
	}

	public void Use(string text){
		string itemName = text.Remove (0, 4);

		//In Room
		for (int z = 0; z < HouseManager.rooms [room].Objects.Count; ++z) {
			if (itemName == HouseManager.rooms [room].Objects [z].Name) {
				for (int y = 0; y < HouseManager.specialResponses.Count; ++y) {
					if (HouseManager.rooms [room].Objects [z].Index == HouseManager.specialResponses [y].ItemIndex) {
						if (HouseManager.specialResponses [y].Command == "Use") {

							foreach (KeyValuePair<int, int> actions in HouseManager.specialResponses[y].Actions) {
								int item = actions.Key;
								int itemState = actions.Value;


								foreach (ObjectClass oc in HouseManager.rooms [room].Objects){
									if (oc.Index == item) {
										oc.State = itemState;
									}
								}
							}

							string response = HouseManager.specialResponses [y].Response;
							appender.text.text = "";
							appender.AppendText(response);
							return;
						}
					}
				}
					
				// if there's no item of that name
				appender.text.text = "";
				appender.AppendText(GenericUse());
			}
		}

		// Inventory
		/*for (int i = 0; i < HouseManager.itemsList.Count; ++i) {
			if (text == HouseManager.itemsList [i].Name) {
				for (int j = 0; j < HouseManager.inventory.Count; ++j) {
					if (HouseManager.itemsList [i].Index == HouseManager.inventory [j]) {
						
					}
				}
			}
		}*/
	}

	public void OtherCommands(string text)
	{
		string command = text.Split(new char[] { ' ' }, 2)[0].ToLower();
		string item = text.Split(new char[] { ' ' }, 2)[1].ToLower();
		string response = "";

		switch (command) {
		case "read":
			for (int i = 0; i < HouseManager.rooms[room].Objects.Count; ++i) {
				if (item == HouseManager.rooms[room].Objects[i].Name) {
					for (int j = 0; j < HouseManager.specialResponses.Count; ++j) {
						if (HouseManager.specialResponses [j].Command == "Read" && HouseManager.specialResponses[j].ItemIndex == HouseManager.rooms[room].Objects[i].Index) {
							response = HouseManager.specialResponses [j].Response;
							appender.text.text = "";
							appender.AppendText(response);
							return;
						}
					}
				}
			}
			break;
		case "dial":
		case "call":
			for (int i = 0; i < HouseManager.rooms [room].Objects.Count; ++i) {
				if (item == HouseManager.rooms [room].Objects [i].Name) {
					for (int j = 0; j < HouseManager.specialResponses.Count; ++j) {
						if (HouseManager.specialResponses [j].Command == "Call" && HouseManager.specialResponses [j].ItemIndex == HouseManager.rooms [room].Objects [i].Index) {
							if (HouseManager.rooms [room].Objects [i].State == 0) {
								response = HouseManager.specialResponses [j].Response;
								appender.text.text = "";
								appender.AppendText (response);

								int state = HouseManager.rooms [room].Objects [i].State;
								foreach (KeyValuePair<int, int> actions in HouseManager.specialResponses[j].Actions) {
									int actionItem = actions.Key;
									int actionItemState = actions.Value;

									foreach (ObjectClass oc in HouseManager.rooms [room].Objects) {
										if (oc.Index == actionItem) {
											oc.State = actionItemState;
										}
									}
								}
								return;
							} else {
								response = "Hmm, there’s no dial tone anymore. That’s...not normal, right? The killer must have cut the phone line.";
								appender.text.text = "";
								appender.AppendText (response);
								return;
							}

						}
					}
				}
			}
			break;
		default:
			response = "I don't know how to do that";
			appender.text.text = "";
			appender.AppendText(response);
			break;
		}
			
	}

	public void Read(string text)
	{
		string itemName = text.Remove (0, 5);
		for (int i = 0; i < HouseManager.rooms[room].Objects.Count; ++i) {
			if (itemName.ToLower () == HouseManager.rooms[room].Objects[i].Name) {
				for (int j = 0; j < HouseManager.specialResponses.Count; ++j) {
					if (HouseManager.specialResponses [j].Command == "Read") {
						string response = HouseManager.specialResponses [j].Response;
						appender.text.text = "";
						appender.AppendText(response);
						return;
					}
				}
			}
		}
	}

	public string GenericLook(){
		List<string> responses = new List<string> ();
		responses.Add ("I can't see that");
		responses.Add ("CAN'T SEE IT");

		return responses[UnityEngine.Random.Range( 0, responses.Count )];
	}

	public string GenericGet(){
		List<string> responses = new List<string>();
		responses.Add("I can't get that");
		responses.Add ("CAN'T GET IT");

		return responses[UnityEngine.Random.Range( 0, responses.Count )];
	}

	public string GenericUse(){
		List<string> responses = new List<string> ();
		responses.Add ("I can't use that");
		responses.Add ("CAN'T USE IT");

		return responses[UnityEngine.Random.Range( 0, responses.Count )];
	}
}

