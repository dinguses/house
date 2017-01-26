using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
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

    public static XElement house;
    int room = 0;


    void Start()
    {
		house = XElement.Parse(xmlDocument.text);
    }

    public List<ObjectClass> ReadXML(XElement house)
    {
        List<ObjectClass> roomsList = new List<ObjectClass>();
        List<SpecialResponseClass> specialResponses = new List<SpecialResponseClass>();

        int room_index = -1;
        foreach (var room in house.Element("rooms").Elements())
        {
            room_index++;

            string name = room.Attr("name").ToLower();

            List<StateClass> roomStates = new List<StateClass>();

            foreach (var state in room.Element("states").Elements())
            {
                int image_id = int.Parse(state.Element("image").Value);
                string description = state.Elt("description");
                Dictionary<int, int> prerequisites = state.Element("prerequisites").Elements().ToDictionary(
                    x => int.Parse(x.Elt("item")), x => int.Parse(x.Elt("itemstate")));

                ConditionalActionListClass conditionalActions = new ConditionalActionListClass(1, prerequisites);

                string get = "";
                int gettable = 0;

                roomStates.Add(new StateClass(image_id, description, get, gettable, conditionalActions));
            }


            List<ObjectClass> items = new List<ObjectClass>();
            foreach (var item in room.Element("items").Elements())
            {
                int itemIndex = int.Parse(item.Elt("index"));
                string itemName = item.Attr("name").ToLower();

                List<StateClass> itemStates = new List<StateClass>();

                foreach (var state in item.Element("states").Elements())
                {
                    int image = int.Parse(state.Elt("image"));
                    string description = state.Elt("description");
                    string get = state.Elt("get");
                    int gettable = int.Parse(state.Elt("gettable"));

                    Dictionary<int, int> actions = state.Element("actions").Elements().ToDictionary(
                        x => int.Parse(x.Elt("item")), x => int.Parse(x.Elt("itemstate")));

                    ConditionalActionListClass conditionalActions = new ConditionalActionListClass(2, actions);
                    itemStates.Add(new StateClass(image, description, get, gettable, conditionalActions));
                }

                List<ObjectClass> emptyList = new List<ObjectClass>();
                List<int> emptyIntList = new List<int>();
                ObjectClass newItem = new ObjectClass(itemIndex, itemName, 0, emptyList, itemStates, emptyIntList);
                items.Add(newItem);
                //HouseManager.itemsList.Add (newItem);
            }

            List<int> adjacentRooms = room.Element("adjacentrooms").Elements().Select(x => int.Parse(x.Value)).ToList();

            ObjectClass thisRoom = new ObjectClass(room_index, name, 0, items, roomStates, adjacentRooms);
            roomsList.Add(thisRoom);
        }

        foreach (var specialresponse in house.Element("specialresponses").Elements())
        {
            int itemIndex = int.Parse(specialresponse.Elt("itemindex"));
            int image = int.Parse(specialresponse.Elt("image"));
            string command = specialresponse.Elt("command");
            string response = specialresponse.Elt("response");
            int requiredItemState = int.Parse(specialresponse.Elt("itemstate"));

            Dictionary<int, int> actions = specialresponse.Element("actions").Elements().ToDictionary(
                x => int.Parse(x.Elt("item")), x => int.Parse(x.Elt("itemstate")));

            specialResponses.Add(new SpecialResponseClass(itemIndex, image, command, response, requiredItemState, actions));
        }


        HouseManager.specialResponses = specialResponses;
        HouseManager.commands = house.Element("commands").Elements().Select(x => x.Value).ToList();

        return roomsList;
    }

    public void ReadInput(string text)
    {
        if (HouseManager.health > 0)
        {
            bool defaultCommand = false;
            for (int i = 0; i < HouseManager.commands.Count; ++i)
            {
                if (text.ToLower().Contains(HouseManager.commands[i].ToLower()))
                {
                    object[] parameters = new object[1];
                    parameters[0] = text;
                    MethodInfo mInfo = typeof(XMLParser).GetMethod(HouseManager.commands[i]);
                    mInfo.Invoke(this, parameters);
                    defaultCommand = true;
                    return;
                }
            }
            if (!defaultCommand)
            {
                OtherCommands(text);
            }
        }else
        {
            HouseManager.ResetHouse();
            room = 0;
            UpdateRoomState();
        }
    }

    public void Look(string text)
    {
		if (text.Length > 5) {
			string itemName = text.Remove (0, 5);
			for (int i = 0; i < HouseManager.rooms[room].Objects.Count; ++i) {
				if (itemName.ToLower () == HouseManager.rooms[room].Objects[i].Name || AltNameCheck(itemName, "look") == HouseManager.rooms[room].Objects[i].Index) {
					int state = HouseManager.rooms [room].Objects [i].State;

					if (HouseManager.rooms[room].Objects[i].States[state].Description == "") {
                        AddText(GenericLook());
                    } else {

                        AddText(HouseManager.rooms[room].Objects[i].States[state].Description);
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
            AddText(GenericLook());
        }
		else
		{
			int state = HouseManager.rooms [room].State;
            AddText(HouseManager.rooms[room].States[state].Description);
            image.sprite = images [HouseManager.rooms [room].States [state].Image];
		}
    }

    public void Move(string text)
    {
        int newRoom = room;
		bool isRoom = false;
		string roomName = text.Remove (0, 5);
		for (int j = 0; j < HouseManager.rooms.Count; ++j)
        {
			if (text.ToLower().Contains(HouseManager.rooms[j].Name) || AltNameCheck(roomName, "move") == j)
            {
				newRoom = j;
				isRoom = true;
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

		if (!isRoom) {
			for (int z = 0; z < HouseManager.rooms [room].Objects.Count; ++z) {
				if (text.ToLower().Contains(HouseManager.rooms [room].Objects [z].Name)) {
					for (int y = 0; y < HouseManager.specialResponses.Count; ++y) {
						if (HouseManager.rooms [room].Objects [z].Index == HouseManager.specialResponses [y].ItemIndex) {
							if (HouseManager.specialResponses [y].Command == "Move") {
								if (HouseManager.rooms [room].Objects [z].State == 0) {

									foreach (KeyValuePair<int, int> actions in HouseManager.specialResponses[y].Actions) {
                                        if (ChangeState(actions.Key, actions.Value) == 1)
                                            break;
									}
                                    AddText(HouseManager.specialResponses[y].Response);

									UpdateRoomState ();

									return;
								}
							}
						}
					}
				}
			}
		}
    }

	public void Get(string text)
	{
		string itemName = text.Remove (0, 4);
		for (int i = 0; i < HouseManager.rooms[room].Objects.Count; ++i) {
			if (itemName.ToLower () == HouseManager.rooms[room].Objects[i].Name) {
				int state = HouseManager.rooms [room].Objects [i].State;

				if (HouseManager.rooms [room].Objects [i].States [state].Gettable == 1) {
					HouseManager.inventory.Add(HouseManager.rooms [room].Objects [i].Index);
					HouseManager.rooms [room].Objects [i].State++;

					state = HouseManager.rooms [room].Objects [i].State;
					int test = HouseManager.rooms [room].Objects [i].States [state].ConditionalActions.Type;
					foreach (KeyValuePair<int, int> actions in HouseManager.rooms [room].Objects [i].States[state].ConditionalActions.ConditionalActions) {
                        if (ChangeState(actions.Key, actions.Value) == 1)
                            break;
                    }
				}

                AddText(HouseManager.rooms[room].Objects[i].States[state].Get);
                UpdateRoomState ();

				return;
			}
		}

		// if there's no item of that name
        AddText(GenericGet());
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
                                if (ChangeState(actions.Key, actions.Value) == 1)
                                    break;
                            }

                            AddText(HouseManager.specialResponses[y].Response);
                            return;
						}
					}
				}
					
				// if there's no item of that name
                AddText(GenericUse());
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
        switch (command)
        {
            case "read":
                command = "Read";
                break;
            case "dial":
            case "call":
                command = "Call";
                break;
            case "open":
                command = "Open";
                break;
            case "shut":
            case "close":
                command = "Close";
                break;
            default:
                AddText("I don't know how to do that");
                return;
        }
        for (int i = 0; i < HouseManager.rooms[room].Objects.Count; ++i)
        {
            if (item == HouseManager.rooms[room].Objects[i].Name)
            {
                for (int j = 0; j < HouseManager.specialResponses.Count; ++j)
                {
                    object[] parameters = new object[2];
                    parameters[0] = i;
                    parameters[1] = j;
                    MethodInfo mInfo = typeof(XMLParser).GetMethod(command);
                    mInfo.Invoke(this, parameters);
                }
            }
        }
    }

    public void Read(int i, int j)
    {
        if (HouseManager.specialResponses[j].Command == "Read" && HouseManager.specialResponses[j].ItemIndex == HouseManager.rooms[room].Objects[i].Index && HouseManager.specialResponses[j].ItemState == HouseManager.rooms[room].Objects[i].State)
        {
            AddText(HouseManager.specialResponses[j].Response);

            foreach (KeyValuePair<int, int> actions in HouseManager.specialResponses[j].Actions)
            {
                if (ChangeState(actions.Key, actions.Value) == 1)
                    break;
            }

            if (HouseManager.specialResponses[j].Image != -1)
            {
                image.sprite = images[HouseManager.specialResponses[j].Image];
            }

            UpdateRoomState();

            return;
        }
    }

    public void Call(int i, int j)
    {
        if (HouseManager.specialResponses[j].Command == "Call" && HouseManager.specialResponses[j].ItemIndex == HouseManager.rooms[room].Objects[i].Index)
        {
            if (HouseManager.rooms[room].Objects[i].State == 0)
            {
                AddText(HouseManager.specialResponses[j].Response);

                int state = HouseManager.rooms[room].Objects[i].State;
                foreach (KeyValuePair<int, int> actions in HouseManager.specialResponses[j].Actions)
                {
                    if (ChangeState(actions.Key, actions.Value) == 1)
                        break;
                }

                for (int z = 0; z < HouseManager.rooms[room].Objects.Count; ++z)
                {
                    if (HouseManager.rooms[room].Objects[z].Name == "drawer")
                    {
                        int drawerState = HouseManager.rooms[room].Objects[z].State;

                        if (drawerState == 0)
                        {
                            image.sprite = images[4];
                        }
                        else
                        {
                            image.sprite = images[5];
                        }
                    }
                }
                return;
            }
            else
            {
                AddText("Hmm, there’s no dial tone anymore. That’s...not normal, right? The killer must have cut the phone line.");

                for (int z = 0; z < HouseManager.rooms[room].Objects.Count; ++z)
                {
                    if (HouseManager.rooms[room].Objects[z].Name == "drawer")
                    {
                        int drawerState = HouseManager.rooms[room].Objects[z].State;

                        if (drawerState == 0)
                        {
                            image.sprite = images[4];
                        }
                        else
                        {
                            image.sprite = images[5];
                        }
                    }
                }

                return;
            }
        }   
    }


    public void Open(int i, int j)
    {
        if (HouseManager.specialResponses[j].Command == "Open" && HouseManager.specialResponses[j].ItemIndex == HouseManager.rooms[room].Objects[i].Index)
        {
            if (HouseManager.rooms[room].Objects[i].State == 0)
            {
                AddText(HouseManager.specialResponses[j].Response);


                int state = HouseManager.rooms[room].Objects[i].State;
                foreach (KeyValuePair<int, int> actions in HouseManager.specialResponses[j].Actions)
                {
                    if (ChangeState(actions.Key, actions.Value, 1) == 1)
                        break;
                }

                if (HouseManager.specialResponses[j].Image != -1)
                {
                    image.sprite = images[HouseManager.specialResponses[j].Image];
                }

                return;
            }
        }
    }

    public void Close(int i, int j)
    {
        if (HouseManager.specialResponses[j].Command == "Close" && HouseManager.specialResponses[j].ItemIndex == HouseManager.rooms[room].Objects[i].Index)
        {
            if (HouseManager.rooms[room].Objects[i].State == 1)
            {
                AddText(HouseManager.specialResponses[j].Response);

                int state = HouseManager.rooms[room].Objects[i].State;
                foreach (KeyValuePair<int, int> actions in HouseManager.specialResponses[j].Actions)
                {
                    if (ChangeState(actions.Key, actions.Value, 1) == 1)
                        break;
                }

                if (HouseManager.specialResponses[j].Image != -1)
                {
                    image.sprite = images[HouseManager.specialResponses[j].Image];
                }

                return;
            }
        }
    }

    public void ItemActions(int itemIndex) {
		for (int i = 0; i < HouseManager.rooms [room].Objects.Count; ++i) {
			if (itemIndex == HouseManager.rooms [room].Objects [i].Index) {
				int state = HouseManager.rooms [room].Objects [i].State;
				foreach (KeyValuePair<int, int> actions in HouseManager.rooms [room].Objects[i].States[state].ConditionalActions.ConditionalActions) {
                    if (ChangeState(actions.Key, actions.Value) == 1)
                        break;
                }
			}
		}
	}

	public void UpdateRoomState(){
		for (int j = 0; j < HouseManager.rooms [room].States.Count; ++j) {
			bool wrongState = false;
            int s = 0;
			foreach (KeyValuePair<int, int> actions in HouseManager.rooms [room].States[j].ConditionalActions.ConditionalActions) {
				if (actions.Key != 0) {
                    s = ChangeState(actions.Key, actions.Value, 2);
                    if (s == 1) //Death
                        break;
                    else if (s == 2) //wrongState
                        wrongState = true;
                }
			}

			if (!wrongState) {
				HouseManager.rooms [room].State = j;

				if (HouseManager.rooms [room].States [j].Image != -1) {
					image.sprite = images [HouseManager.rooms [room].States [j].Image];
				}
			}
		}
	}

	public int AltNameCheck(string nameToCheck, string type){

		switch (type) {
		case "move":
			foreach (KeyValuePair<string, List<string>> entry in HouseManager.altNames) {
				foreach (var altNameString in entry.Value) {
					if (nameToCheck.ToLower () == altNameString) {

						for (int i = 0; i < HouseManager.rooms [room].AdjacentRooms.Count; ++i) {
							if (int.Parse(entry.Key) == HouseManager.rooms [room].AdjacentRooms [i]) {
								return int.Parse(entry.Key);
							}
						}
					}
				}
			}
			break;
		case "look":
			foreach (KeyValuePair<string, List<string>> entry in HouseManager.altNames) {
				foreach (var altNameString in entry.Value) {
					if (nameToCheck.ToLower () == altNameString) {
						for (int i = 0; i < HouseManager.rooms [room].Objects.Count; ++i) {
							if (entry.Key == HouseManager.rooms [room].Objects [i].Name) {
								return HouseManager.rooms [room].Objects [i].Index;
							}
						}
					}
				}
			}
			break;
		default:
			break;
		}

		return -1;
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

    public bool CheckDeath(int damage)
    {
        HouseManager.health = HouseManager.health - damage;
        if(HouseManager.health <= 0)
            return true;
        return false;
    }

    //this let's the house manager put whatever text we want it to put after death
    public void AddText(string input)
    {
        appender.text.text = "";
        appender.AppendText(input);
    }

    public int ChangeState(int item, int itemState, int flag = 0)
    {
        if (item == -1)
            if (CheckDeath(itemState))
                return 1; //Return 1 for death

        foreach (ObjectClass oc in HouseManager.rooms[room].Objects)
        {
            if (oc.Index == item)
            {
                //Calling ItemActions if the flag is set to 1
                //If it is set to 2 then we're checking to see if the item is in the wrong state
                if (flag == 2)
                {
                    if (oc.Index == item && oc.State != itemState)
                    {
                        Debug.Log("WrongState");
                        return 2;
                    }
                }else
                {
                    oc.State = itemState;       
                    if (flag == 1)
                        ItemActions(oc.Index);
                }         
            }
        }
        return 0; //if everything goes smoothly then do this
    }
}

