using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Reflection;
using System.Collections;
using UnityEngine.UI;


public class HouseManager : MonoBehaviour
{
    public List<int> inventory;

    public List<SpecialResponse> specialResponses;
    public Dictionary<string, List<string>> altNames;

    public int room = 0;
    public Dictionary<string, GameObject> roomsByName;
    public List<GameObject> rooms;

    GameObject currentRoom
    {
        get
        {
            return rooms[room];
        }
    }

    string roomName
    {
        get
        {
            return currentRoom.Name;
        }
    }

    public List<GameObject> itemsList;

    public int health = 100;
    XElement parsedHouseXml;
    public TextAsset xmlDocument;

    public GradualTextRevealer text;

    Dictionary<string, MethodInfo> commands;
    List<string> specialCommands;

    public Image image;

    void SetupHouse()
    {
        rooms = XMLParser.ReadRooms(parsedHouseXml);
        roomsByName = rooms.ToDictionary(x => x.Name, x => x);
        specialResponses = XMLParser.ReadSpecialResponses(parsedHouseXml);
        specialCommands = XMLParser.ReadCommands(parsedHouseXml);
		itemsList = XMLParser.ReadItems (parsedHouseXml);

        // Inventory
        inventory = new List<int>();
        health = 100;
    }

    void Start()
    {
        parsedHouseXml = XElement.Parse(xmlDocument.text);
        SetupHouse();
        SetupCommands();

        // Alt Names
        AltNamesParser altNamesParser = gameObject.GetComponent(typeof(AltNamesParser)) as AltNamesParser;
        TextAsset altNamesText = altNamesParser.xmlDocument;
        XmlDocument altNamesDoc = new XmlDocument();
        altNamesDoc.LoadXml(altNamesText.text);
        altNames = altNamesParser.ReadXML(altNamesDoc);
    }

    /// <summary>
    /// This looks for all [Command] methods and adds them to the dictionary.
    /// </summary>
    void SetupCommands()
    {
        commands = new Dictionary<string, MethodInfo>();
        var methods = typeof(HouseManager).GetMethods();
        foreach (var method in methods)
        {
            foreach (var attr in method.GetCustomAttributes(typeof(CommandAttribute), false).Cast<CommandAttribute>())
            {
                var cmdname = attr.Name;

                if (cmdname == null) cmdname = method.Name.ToLower();

                Debug.LogFormat("Registering command name {0} to method {1}", cmdname, method);
                commands.Add(cmdname, method);
            }
        }
    }

    Texture2D GetImageByName(string name)
    {
        return Resources.Load(name) as Texture2D;
    }

    
    public void ReadInput(string text)
    {
        if (health > 0)
        {
            Debug.LogFormat("Running command: {0}", text);

            var tokens = text.Shlex();

            var cmdName = tokens[0];

           // var cmd = commands[cmdName];

			if (commands.ContainsKey (cmdName)) {
				var cmd = commands[cmdName];
				cmd.Invoke(this, new object[] { tokens });
				return;
			}
				
            OtherCommands(text);

        }
        else
        {
            ResetHouse();
            room = 0;
            UpdateRoomState();
        }
    }

    void Update() { }

    public void ResetHouse()
    {
        SetupHouse();
        AddText("Type some shit here. Good luck, and don't fuck up this time!");
    }

    public void AddText(string txt)
    {
        text.StartReveal("\n" + txt + "\n");
    }

    GameObject GetObjectByName(string name)
    {
        return GetObjectByName(name, string.Equals);
    }

    GameObject GetObjectByName(string name, Func<string, string, bool> finder)
    {
        name = name.ToLower();
        return currentRoom.Objects.Find(x => finder(name, x.Name) || AltNameCheck(name, "look") == x.Index);
    }

	public void RemoveItemState(int item, int state){
		var obj = rooms [room].GetObjectById (item);
		obj.States.Remove (obj.States [state]);
	}

    void SetImage(Texture2D tex)
    {
        image.sprite = Sprite.Create(tex, image.sprite.rect, image.sprite.pivot);
    }

    [Command]
    public void Look(List<string> argv = null)
    {
        if (argv == null || argv.Count == 1)
        {
            int roomState = currentRoom.State;
            AddText(currentRoom.currentState.Description);
            SetImage(GetImageByName(currentRoom.currentState.Image));
            return;
        }

        int itemNameStart = (argv[1] != "at") ? 1 : 2;

        string itemName = string.Join(" ", argv.Skip(itemNameStart).ToArray());

        Debug.LogFormat("Looking at ({0})", itemName);

        var obj = GetObjectByName(itemName);

        if (obj == null || obj.currentState.Description == "")
        {
            AddText(GenericLook());
        }
        else
        {
            AddText(obj.currentState.Description);
            if (obj.currentState.Image != "")
            {
                SetImage(GetImageByName(obj.currentState.Image));
            }
            else
            {
                int roomState = currentRoom.State;
				SetImage(GetImageByName(currentRoom.States[roomState].Image));
            }
        }
    }

    public int ChangeState(int item, int itemState, int flag = 0)
    {
        if (item == -1 && CheckDeath(itemState))
            return 1; //Return 1 for death



		if (itemState < 0 || item == -2) {
			if (itemState < 0) {				
				itemState = -1 * itemState;
			} else {
				item = itemState;
				itemState = 0;
			}

			var obj = itemsList[item];
			var capacity = obj.States.Capacity - 2;
			if (obj.States.Count != capacity) {
					
				RemoveItemState (item, itemState);
				itemsList [obj.Index].States.Remove (obj.States [itemState]);

				if (obj.State == obj.States.Count) {
					obj.State--;
					rooms [room].GetObjectById (item).State--;
				}
			}

			return 0;
		}

		var gameObject = itemsList[item];

        //Calling ItemActions if the flag is set to 1
        //If it is set to 2 then we're checking to see if the item is in the wrong state
        if (flag == 2)
        {
            if (gameObject.State != itemState)
            {
                Debug.Log("WrongState");
                return 2;
            }
        }
        else
        {
			foreach (var thisRoom in rooms) {
				var obj = rooms [thisRoom.Index].GetObjectById (item);
				if (obj != null) {
					obj.State = itemState;
					gameObject.State = itemState;
				}
			}

            if (flag == 1)
                ItemActions(gameObject.Index);
        }

        return 0; //if everything goes smoothly then do this
    }

	public void UpdateRoomState(bool updateImage = true)
    {
        for (int j = 0; j < currentRoom.States.Count; ++j)
        {
            bool wrongState = false;
            int s = 0;
            foreach (KeyValuePair<int, int> actions in currentRoom.States[j].ConditionalActions.ConditionalActions)
            {
                if (actions.Key != 0)
                {
                    s = ChangeState(actions.Key, actions.Value, 2);
                    if (s == 1) //Death
                        break;
                    else if (s == 2) //wrongState
                        wrongState = true;
                }
            }

            if (!wrongState)
            {
                currentRoom.State = j;

				if (updateImage) {
					if (currentRoom.States[j].Image != "")
					{
						SetImage(GetImageByName(currentRoom.States[j].Image));
					}
				}
            }
        }
    }

    public void ItemActions(int itemIndex)
    {
        var item = currentRoom.GetObjectById(itemIndex);
        int state = item.State;
        foreach (KeyValuePair<int, int> actions in item.States[state].ConditionalActions.ConditionalActions)
        {
            if (ChangeState(actions.Key, actions.Value) == 1)
                break;
        }
    }

    public int AltNameCheck(string nameToCheck, string type)
    {
        nameToCheck = nameToCheck.ToLower();
        switch (type)
        {
            case "move":
                foreach (KeyValuePair<string, List<string>> entry in altNames)
                {
                    if (!entry.Value.Any(x => x == nameToCheck)) continue;
					var key = 0;

					if (int.TryParse (entry.Key, out key)) {
						if (currentRoom.AdjacentRooms.Any(x => x == key)) return key;
					}          
                }
                break;
            case "look":
                foreach (KeyValuePair<string, List<string>> entry in altNames)
                {
                    if (!entry.Value.Any(x => x == nameToCheck)) continue;

                    var obj = currentRoom.GetObjectByName(entry.Key);

                    if (obj != null) return obj.Index;
                }
                break;
            default:
                break;
        }

        return -1;
    }

    public bool CheckDeath(int damage)
    {
        health = health - damage;
        return health <= 0;
    }


    GameObject GetRoomByName(string name)
    {
        name = name.ToLower();
        return rooms.Find(x => name.Contains(x.Name) || AltNameCheck(name, "move") == x.Index);
    }

    [Command]
	public void Move(List<string> argv)
    {
        int newRoom = room;
        bool isRoom = false;
        string roomName = string.Join(" ", argv.Skip((argv[1] != "to") ? 1 : 2).ToArray());

        var newRoomObj = GetRoomByName(roomName);
        if (newRoomObj != null)
        {
            newRoom = newRoomObj.Index;
            isRoom = true;
        }

        if (currentRoom.AdjacentRooms.Contains(newRoom))
        {
            room = newRoom;
            Look(null);
			UpdateRoomState ();
            return;
        }


		if (!isRoom && argv[0] == "move")
        {
            var obj = GetObjectByName(roomName, (x, y) => x.Contains(y));
            if (obj == null || obj.State != 0) return;

            var moveResponses = specialResponses
                .Where(x => x.ItemIndex == obj.Index)
                .Where(x => x.Command == "Move");
            foreach (var response in moveResponses)
            {
                foreach (KeyValuePair<int, int> actions in response.Actions)
                {
                    if (ChangeState(actions.Key, actions.Value) == 1)
                        break;
                }
                AddText(response.Response);

                UpdateRoomState();

                return; // TODO: will there only ever be one?
            }
        }
    }

	[Command]
	public void Go(List<string> argv){
		Move (argv);
	}

	[Command]
	public void Enter(List<string> argv){
		Move (argv);
	}

    [Command]
	public void Get(List<string> argv)
    {
        string itemName = string.Join(" ", argv.Skip(1).ToArray());
		bool roomImage = true;
        var item = GetObjectByName(itemName);
        if (item == null)
        {
            AddText(GenericGet());
            return;
        }
        int state = item.State;
		AddText(item.States[state].Get);

        if (item.States[state].Gettable == 1)
        {
            inventory.Add(item.Index);
            item.State++;
			itemsList [item.Index].State = item.State;

            state = item.State;
            foreach (KeyValuePair<int, int> actions in item.States[state].ConditionalActions.ConditionalActions)
            {
                if (ChangeState(actions.Key, actions.Value) == 1)
                    break;
            }

			if (item.States[item.State].Image != "")
			{
				SetImage(GetImageByName(item.States[item.State].Image));
				roomImage = false;
			}
        }

		UpdateRoomState(roomImage);
    }

    [Command]
    public void Use(List<string> argv)
    {
        string itemName = string.Join(" ", argv.Skip(1).ToArray());

        //In Room
        var item = GetObjectByName(itemName);
        if (item == null)
        {
            AddText(GenericUse());
            return;
        }
        var useResponses = specialResponses
               .Where(x => x.ItemIndex == item.Index)
               .Where(x => x.Command == "Use");

        foreach (var response in useResponses)
        {
            foreach (KeyValuePair<int, int> actions in response.Actions)
            {
                if (ChangeState(actions.Key, actions.Value) == 1)
                    break;
            }

            AddText(response.Response);
            return;
        }
                  
        // Inventory
        /*for (int i = 0; i < itemsList.Count; ++i) {
            if (text == itemsList [i].Name) {
                for (int j = 0; j < inventory.Count; ++j) {
                    if (itemsList [i].Index == inventory [j]) {

                    }
                }
            }
        }*/
    }

    public void OtherCommands(string text)
    {
        string command = text.Split(new char[] { ' ' }, 2)[0].ToLower();
        string itemName = text.Split(new char[] { ' ' }, 2)[1].ToLower();
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
        var item = GetObjectByName(itemName);
        if (item == null) return;

        for (int j = 0; j < specialResponses.Count; ++j)
        {
            object[] parameters = new object[2];
            parameters[0] = item.Index;
            parameters[1] = j;
			MethodInfo mInfo = typeof(HouseManager).GetMethod(command);
            mInfo.Invoke(this, parameters);
        }
    }


    public void Read(int i, int j)
    {
        var item = itemsList[i];
        if (specialResponses[j].Command == "Read" && specialResponses[j].ItemIndex == item.Index && specialResponses[j].ItemState == item.State)
        {
            AddText(specialResponses[j].Response);

            foreach (KeyValuePair<int, int> actions in specialResponses[j].Actions)
            {
                if (ChangeState(actions.Key, actions.Value) == 1)
                    break;
            }

            if (specialResponses[j].Image != "")
            {
                SetImage(GetImageByName(specialResponses[j].Image));
            }

            UpdateRoomState();

            return;
        }
    }

    public void Call(int i, int j)
    {
        var item = itemsList[i];
        if (specialResponses[j].Command == "Call" && specialResponses[j].ItemIndex == item.Index)
        {
            if (item.State == 0)
            {
                AddText(specialResponses[j].Response);

                int state = item.State;
                foreach (KeyValuePair<int, int> actions in specialResponses[j].Actions)
                {
                    if (ChangeState(actions.Key, actions.Value) == 1)
                        break;
                }

                for (int z = 0; z < currentRoom.Objects.Count; ++z)
                {
                    if (currentRoom.Objects[z].Name == "drawer")
                    {
                        int drawerState = currentRoom.Objects[z].State;

                        if (drawerState == 0)
                        {
                            SetImage(GetImageByName("phone4"));
                        }
                        else
                        {
                            SetImage(GetImageByName("phone5"));
                        }
                    }
                }
                return;
            }
            else
            {
                AddText("Hmm, there’s no dial tone anymore. That’s...not normal, right? The killer must have cut the phone line.");

                for (int z = 0; z < currentRoom.Objects.Count; ++z)
                {
                    if (currentRoom.Objects[z].Name == "drawer")
                    {
                        int drawerState = currentRoom.Objects[z].State;

                        if (drawerState == 0)
                        {
                            SetImage(GetImageByName("phone4"));
                        }
                        else
                        {
                            SetImage(GetImageByName("phone5"));
                        }
                    }
                }

                return;
            }
        }
    }


    public void Open(int i, int j)
    {
        var item = itemsList[i];
        if (specialResponses[j].Command == "Open" && specialResponses[j].ItemIndex == item.Index)
        {
            if (item.State == 0)
            {
                AddText(specialResponses[j].Response);


                int state = item.State;
                foreach (KeyValuePair<int, int> actions in specialResponses[j].Actions)
                {
                    if (ChangeState(actions.Key, actions.Value, 1) == 1)
                        break;
                }

                if (specialResponses[j].Image != "")
                {
					if (specialResponses [j].Image == "showitem") {
						SetImage(GetImageByName(item.States [item.State].Image));
					} else {
						SetImage(GetImageByName(specialResponses[j].Image));
					}
                }

                return;
            }
        }
    }

    public void Close(int i, int j)
    {
        var item = itemsList[i];
        if (specialResponses[j].Command == "Close" && specialResponses[j].ItemIndex == item.Index)
        {
            if (item.State == 1)
            {
                AddText(specialResponses[j].Response);

                int state = item.State;
                foreach (KeyValuePair<int, int> actions in specialResponses[j].Actions)
                {
                    if (ChangeState(actions.Key, actions.Value, 1) == 1)
                        break;
                }

                if (specialResponses[j].Image != "")
                {
                    SetImage(GetImageByName(specialResponses[j].Image));
                }

                return;
            }
        }
    }



    public string GenericLook()
    {
        List<string> responses = new List<string>();
        responses.Add("I can't see that");
        responses.Add("CAN'T SEE IT");

        return responses[UnityEngine.Random.Range(0, responses.Count)];
    }

    public string GenericGet()
    {
        List<string> responses = new List<string>();
        responses.Add("I can't get that");
        responses.Add("CAN'T GET IT");

        return responses[UnityEngine.Random.Range(0, responses.Count)];
    }

    public string GenericUse()
    {
        List<string> responses = new List<string>();
        responses.Add("I can't use that");
        responses.Add("CAN'T USE IT");

        return responses[UnityEngine.Random.Range(0, responses.Count)];
    }
}
