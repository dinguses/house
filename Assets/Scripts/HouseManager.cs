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

	public int room;
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

    public int health;
    XElement parsedHouseXml;
    public TextAsset xmlDocument;

    public GradualTextRevealer text;

    Dictionary<string, MethodInfo> commands;
    List<string> specialCommands;
	List<ItemGroup> itemGroups;
	ItemGroup currentItemGroup;
	List<CheckItem> checkItems;
	Dictionary<int, string> audioIndex;
	bool playerKnowsCombo, playerKnowsBeartrap, playerKnowsFiretrap;
	bool bearTrapMade, fireTrapMade, bucketTrapMade;
	bool playerOutOfTime;
	bool inputLockdown;
	Dictionary<int, List<string>> lockdownOptions;
	bool multiSequence;
	bool killerInBedroom, killerInKitchen, killerInLair;
	bool playerBedroomShot, playerLairThreaten;
	int killerTimer;
	int pizzaTimer;
	int policeTimer;
	int killerCap;
	int pizzaCap;
	int pizzaCap2;
	int policeCap;
	Dictionary<int, List<string>> deathImages;
	Dictionary<int, List<string>> deathOverlays;
	string currOverlay;
	int currMultiSequence;
	int multiSequenceStep;
	List<MultiSequence> multiSequences;

    public Image image;
	public Image overlayImage;
	public AudioSource audioSource;


    void SetupHouse()
    {
		// Set up rooms, items, object lists, etc
        rooms = XMLParser.ReadRooms(parsedHouseXml);
        roomsByName = rooms.ToDictionary(x => x.Name, x => x);
        specialResponses = XMLParser.ReadSpecialResponses(parsedHouseXml);
        specialCommands = XMLParser.ReadCommands(parsedHouseXml);
		itemsList = XMLParser.ReadItems (parsedHouseXml);
		itemGroups = XMLParser.ReadItemGroups (parsedHouseXml);
		checkItems = XMLParser.ReadCheckItems (parsedHouseXml);
		audioIndex = XMLParser.ReadAudioIndex (parsedHouseXml);
		multiSequences = XMLParser.ReadMultiSequences (parsedHouseXml);
		deathImages = GetDeathImages ();
		deathOverlays = GetDeathOverlays ();
		lockdownOptions = GetLockdownOptions ();

		// Set up various variables
        inventory = new List<int>();
		room = 0;
        health = 100;
		killerTimer = 0;
		pizzaTimer = -1;
		policeTimer = -1;
		killerCap = 20;
		pizzaCap = 5;
		pizzaCap2 = 10;
		policeCap = 5;
		currentItemGroup = null;
		multiSequenceStep = 0;
		playerKnowsCombo = playerKnowsBeartrap = playerKnowsFiretrap = false;
		bearTrapMade = fireTrapMade = bucketTrapMade = false;
		playerOutOfTime = false;
		playerBedroomShot = playerLairThreaten = false;
		inputLockdown = false;
		multiSequence = false;
		killerInBedroom = killerInKitchen = killerInLair = false;
		currOverlay = "";
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

	Dictionary<int, List<string>> GetDeathOverlays()
	{
		Dictionary<int, List<string>> dict = new Dictionary<int, List<string>> ();
		dict.Add (0, new List<string> { "lr-katana", "lr-knife", "lr-mace", "lr-gun" });
		dict.Add (1, new List<string> { "k-katana", "k-knife" });
		dict.Add (2, new List<string> { "hall-knife", "hall-gun" });
		dict.Add (3, new List<string> { "br-knife", "br-gun" });
		dict.Add (4, new List<string> { "bedr-katana", "bedr-knife" });
		dict.Add (5, new List<string> { "bment-katana", "bment-knife", "bment-mace", "bment-gun" });
		dict.Add (6, new List<string> { "lair-katana", "lair-knife", "lair-mace", "lair-gun" });
		dict.Add (7, new List<string> { "outside-knife", "outside-mace" });
		dict.Add (8, new List<string> { "shack-knife", "shack-mace" });
		return dict;
	}

	Dictionary<int, List<string>> GetDeathImages()
	{
		Dictionary<int, List<string>> dict = new Dictionary<int, List<string>> ();
		dict.Add (0, new List<string> { "lrdeath", "lrdeath2" });
		dict.Add (1, new List<string> { "death-knife", "death-knife2" });
		dict.Add (4, new List<string> { "death-katana", "death-katana2", "death-knife", "death-knife2" });
		dict.Add (5, new List<string> { "death-gun", "death-gun2", "death-katana", "death-katana2", "death-knife", "death-knife2", "death-mace", "death-mace2" });
		return dict;
	}

	Dictionary<int, List<string>> GetLockdownOptions()
	{
		Dictionary<int, List<string>> dict = new Dictionary<int, List<string>> ();
		dict.Add (1, new List<string> { "use knife", "get knife", "stab killer" });
		dict.Add (4, new List<string> { "use gun" , "shoot gun", "shoot killer", "fire gun", "use pistol", "shoot pistol", "fire pistol" });
		dict.Add (6, new List<string> { "threaten bear" });
		return dict;
	}

    Texture2D GetImageByName(string name)
    {
        return Resources.Load(name) as Texture2D;
    }

	Texture2D GetRandomDeathImage()
	{
		List<string> imageList = deathImages [currentRoom.Index];
		string imageName = imageList[UnityEngine.Random.Range(0, imageList.Count)];
		return Resources.Load(imageName) as Texture2D;
	}

	Texture2D GetRandomDeathOverlay()
	{
		List<string> imageList = deathOverlays [currentRoom.Index];
		string imageName = imageList[UnityEngine.Random.Range(0, imageList.Count)];
		currOverlay = imageName;
		return Resources.Load(imageName) as Texture2D;
	}

	Texture2D GetCheckItemImage (int baseIndex){
		string image = "";
		var baseObj = itemsList [baseIndex];
		foreach (CheckItem ci in checkItems) {
			if (ci.BaseItemIndex == baseIndex && ci.BaseItemState == baseObj.State) {
				foreach (var compareItem in ci.CompareItems)
				{
					bool getImage = true;
					image = compareItem.ImageName;
					foreach (var item in compareItem.States) {
						var obj = itemsList [item.Key];

						if (obj.State != item.Value) {
							getImage = false;
						}

					}

					if (getImage) {

						if (compareItem.Overlay != "") {
							SetOverlay (GetImageByName(compareItem.Overlay));
						}

						return Resources.Load (image) as Texture2D;
					}
				}
			}
		}

		return Resources.Load (currentRoom.currentState.Image) as Texture2D;
	}

	AudioClip GetClip (int clipId)	{
		AudioClip thisClip = Resources.Load(audioIndex [clipId]) as AudioClip;
		return thisClip;
	}

	void ResetOverlay(){
		Texture2D blankOverlay = Resources.Load ( "blankoverlay" ) as Texture2D;
		SetOverlay (blankOverlay);
	}

	string GetCheckItemDescription (int baseIndex){
		string description = "";
		var baseObj = itemsList [baseIndex];
		foreach (CheckItem ci in checkItems) {
			if (ci.BaseItemIndex == baseIndex && ci.BaseItemState == baseObj.State) {
				foreach (var compareItem in ci.CompareItems)
				{
					bool getDesc = true;
					description = compareItem.Description;
					foreach (var item in compareItem.States) {
						var obj = itemsList [item.Key];

						if (obj.State != item.Value) {
							getDesc = false;
						}

					}

					if (getDesc) {
						return description;
					}
				}
			}
		}

		return description;
	}
		    
    public void ReadInput(string text)
    {
		if (!multiSequence) {
			if (killerInBedroom) {
				if (!playerBedroomShot) {
					SetImage (GetImageByName (currentRoom.currentState.Image));
					SetOverlay (GetRandomDeathOverlay ());
					AddText ("o shit, the killer's in your bedroom!");
					playerBedroomShot = true;
					inputLockdown = true;
				} else {

					var options = lockdownOptions [currentRoom.Index];

					foreach (var option in options) {
						if (text == option) {
							BedroomGunshot ();
							return;
						}
					}

					string weapon = currOverlay.Split ('-').Last ();
					var overlays = deathImages [currentRoom.Index];
					if (weapon == "katana") {
						SetImage (GetImageByName(overlays.ElementAt(UnityEngine.Random.Range (0, 2))));
					} 
					else {
						SetImage (GetImageByName(overlays.ElementAt(UnityEngine.Random.Range (2, 4))));
					}
					ResetOverlay ();
					AddText ("You died. Maybe you should have fought back? Press [ENTER] to continue!");
					health = 0;
					killerInBedroom = false;
				}
			} 
			else if (killerInKitchen && currentRoom.Index == 1) {
				var options = lockdownOptions [currentRoom.Index];

				foreach (var option in options) {
					if (text == option) {
						KitchenStab ();
						return;
					}
				}

				string weapon = currOverlay.Split ('-').Last ();
				var overlays = deathImages [currentRoom.Index];
				SetImage (GetImageByName (overlays.ElementAt (UnityEngine.Random.Range (0, 2))));
				ResetOverlay ();
				AddText ("You died. Maybe you should have finished him off when you had the chance. Press [ENTER] to continue!");
				health = 0;
				killerInKitchen = false;
			}
			else if (killerInLair) {
				if (!playerLairThreaten) {
					SetImage (GetImageByName (currentRoom.currentState.Image));
					SetOverlay (GetRandomDeathOverlay ());
					AddText ("o shit, the killer's here!");
					playerLairThreaten = true;
					inputLockdown = true;
				}
				else {
					var options = lockdownOptions [currentRoom.Index];

					foreach (var option in options) {
						if (text == option) {
							LairThreaten ();
							return;
						}
					}

					string weapon = currOverlay.Split ('-').Last ();
					var overlays = deathImages [currentRoom.Index];
					SetImage (GetImageByName (overlays.ElementAt (UnityEngine.Random.Range (0, 2))));
					ResetOverlay ();
					AddText ("You died. Maybe you could have saved yourself. Press [ENTER] to continue!");
					health = 0;
					killerInLair = false;
				}
			}
			else {
				if (health > 0) {
					if (text != "") {
					
						Debug.LogFormat ("Running command: {0}", text);

						var tokens = text.Shlex ();

						var cmdName = tokens [0];

						ResetOverlay ();

						if (commands.ContainsKey (cmdName)) {
							var cmd = commands [cmdName];
							cmd.Invoke (this, new object[] { tokens });
							return;
						}	

						OtherCommands (text);
					}

				} else {
					ResetHouse ();
					UpdateRoomState ();
				}
			}
		} 
		else {
			AdvanceMultiSequence ();
		}
    }

	void AdvanceMultiSequence(){
		ResetOverlay ();
		var ms = multiSequences [currMultiSequence];
		var step = ms.Steps.ElementAt (multiSequenceStep);
		if (step.Key == "randomdeath") {
			SetImage (GetRandomDeathImage ());
		} else {
			SetImage(GetImageByName(step.Key));
		}
		AddText(step.Value);
		multiSequenceStep++;

		if (multiSequenceStep == ms.Steps.Count) {
			if (ms.Win) {
				// TODO win logic
				multiSequence = false;
				health = 0;
				return;

			} else {
				multiSequence = false;
				health = 0;
				return;
			}
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
        //text.StartReveal("\n" + txt + "\n");
		text.AppendText(txt);
    }

	public void AddAdditionalText(string txt)
	{
		text.AddAdditionalText (txt);
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

		for (int i = 0; i < rooms.Count; ++i) {
			if (rooms [i].GetObjectById (item) != null) {
				var obj = rooms [i].GetObjectById (item);
				obj.States.Remove (obj.States [state]);
			}
		}
	}

    void SetImage(Texture2D tex)
    {
        image.sprite = Sprite.Create(tex, image.sprite.rect, image.sprite.pivot);
    }

	void SetOverlay(Texture2D tex)
	{
		overlayImage.sprite = Sprite.Create(tex, image.sprite.rect, image.sprite.pivot);
		currOverlay = tex.name;
	}

	void ImageCheckAndShow(int itemIndex, int itemState, string image){
		var item = itemsList [itemIndex];
		switch (image) {
		case "showitem":
			SetImage (GetImageByName (item.States [item.State].Image)); 
			break;
		case "checkitem":
			SetImage (GetCheckItemImage (itemIndex));
			break;
		case "deathimage":
			SetImage (GetRandomDeathImage ());
			break;
		default:
			SetImage (GetImageByName (image));
			break;
		}
	}

	void PlayDeathSequence(){
		string weapon = currOverlay.Split ('-').Last ();
		multiSequence = true;

		switch (weapon) {
		case "katana":
			currMultiSequence = UnityEngine.Random.Range(0, 2);
			break;
		case "knife":
			currMultiSequence = UnityEngine.Random.Range(2, 4);
			break;
		case "gun":
			currMultiSequence = UnityEngine.Random.Range(4, 6);
			break;
		case "mace":
			currMultiSequence = UnityEngine.Random.Range(6, 8);
			break;
		default:
			break;
		}
	}

	void BedroomGunshot () {
		SetImage (GetImageByName ("gunshotaction"));
		AddText ("You shot the killer...in the leg. I guess you suck at shooting, huh DUNGO");
		ResetOverlay ();
		killerInBedroom = false;
		killerInKitchen = true;
	}

	void KitchenStab () {
		SetImage (GetImageByName ("knifeaction"));
		AddText ("You stab the killer and kill him. WOO");
		killerInKitchen = false;
		ResetOverlay ();
		health = 0;

		// TODO: WIN
	}

	void LairThreaten () {
		SetImage (GetImageByName ("bearaction"));
		AddText ("You threaten the bear and win. WOO");
		killerInLair = false;
		ResetOverlay ();
		health = 0;
	}

    [Command]
	public void Look(List<string> argv = null)
    {
		if (argv == null) {

			if (!playerOutOfTime) {

				if (killerInKitchen && currentRoom.Index == 1) {
					SetOverlay (GetImageByName ("kinjuredoverlay"));
					AddText ("It's the killer! Better get him!");
				} 
				else if (killerInKitchen && currentRoom.Index == 0) {
					//blood
					AddText ("you hear the killer in the kitchen");
				} 
				else if (killerInKitchen && currentRoom.Index == 2) {
					//blood
					AddText ("you hear the killer run downstairs, leaving a blood trail behind him.");
				}
				else {
					
					if (!currentRoom.Visited && currentRoom.Index != 0) {
						AddAdditionalText (currentRoom.currentState.Description);
						currentRoom.Visited = true;
					}
				}

				ResetItemGroup ();
				SetImage (GetImageByName (currentRoom.currentState.Image));
				UpdateRoomState ();
				return;


			} 
			else {
				ResetItemGroup ();
				SetImage (GetImageByName (currentRoom.currentState.Image));
				UpdateRoomState ();
				SetOverlay(GetRandomDeathOverlay ());
				PlayDeathSequence ();
				AddText ("you died!");
				return;
			}
		}

        else if (argv.Count == 1)
        {
			ResetItemGroup ();
			AddText(currentRoom.currentState.Description);
            SetImage(GetImageByName(currentRoom.currentState.Image));
			UpdateRoomState ();
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
			UpdateItemGroup (obj.Index);

			if (obj.currentState.Description == "checkitem") {
				AddText (GetCheckItemDescription (obj.Index));
			} else {
				AddText(obj.currentState.Description);
			}

			// If player is reading combination code, they 'know' the code
			if (obj.Index == 93 && obj.State == 1) {
				playerKnowsCombo = true;
			}
				
			// If player is reading Bear book, they 'know' the trap blueprint
			if (obj.Index == 13) {
				playerKnowsBeartrap = true;
			}

			// If player looks at poster, they 'know' the fire trap blueprint
			if (obj.Index == 97) {
				playerKnowsFiretrap = true;
			}
				
            if (obj.currentState.Image != "")
            {
				ImageCheckAndShow (obj.Index, obj.State, obj.currentState.Image);
            }
            else
            {
				SetImage(GetImageByName(currentRoom.currentState.Image));
            }
        }
    }

    public int ChangeState(int item, int itemState, int flag = 0)
    {
        if (item == -1 && CheckDeath(itemState))
            return 1; //Return 1 for death

		if (item == -2) {
			AudioClip clipToPlay = GetClip (itemState);
			audioSource.PlayOneShot (clipToPlay);
			return 0;
		}


		var gameObject = itemsList[item];

		if (itemState < 0 ) {
			itemState = -1 * itemState;
			if (gameObject.States.Count != gameObject.DeleteCap) {
					
				RemoveItemState (item, itemState);
				itemsList [gameObject.Index].States.Remove (gameObject.States [itemState]);

				if (gameObject.State == gameObject.States.Count) {
					gameObject.State--;
					rooms [room].GetObjectById (item).State--;
				}
			}

			return 0;
		}


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

	public void ResetItemGroup(){
		if (currentItemGroup != null) {
			foreach (int item in currentItemGroup.Items) {
				if (!currentItemGroup.NonResetItems.Contains (item)) {
					var obj = currentRoom.GetObjectById (item);
					obj.State = 0;
					itemsList [item].State = 0;
				}
			}

			currentItemGroup = null;
		}
	}

	public void UpdateItemGroup(int item){
		var obj = currentRoom.GetObjectById (item);

		if (obj != null) {

			if (currentItemGroup != null) {
				if (!currentItemGroup.Items.Contains (item)) {

					ResetItemGroup ();

					foreach (ItemGroup ig in itemGroups) {
						if (ig.BaseItemIndex == item) {
							currentItemGroup = ig;
						} else {
							currentItemGroup = null;
						}
					}
				}
			} 
			else {
				foreach (ItemGroup ig in itemGroups) {
					if (ig.BaseItemIndex == item) {
						currentItemGroup = ig;
					}
				}
			}
		}
	}

	public void UpdateRoomState(bool updateImage = true, int specificRoom = 0)
    {
		var room = (specificRoom == 0) ? currentRoom : rooms [specificRoom];
		for (int j = 0; j < room.States.Count; ++j)
        {
            bool wrongState = false;
            int s = 0;
			foreach (KeyValuePair<int, int> actions in room.States[j].ConditionalActions.ConditionalActions)
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
				room.State = j;

				if (updateImage) {
					if (room.States[j].Image != "")
					{
						SetImage(GetImageByName(room.States[j].Image));
					}
				}
            }
        }
    }

    public void ItemActions(int itemIndex)
    {
        var item = currentRoom.GetObjectById(itemIndex);
		foreach (KeyValuePair<int, int> actions in item.currentState.ConditionalActions.ConditionalActions)
        {
            if (ChangeState(actions.Key, actions.Value) == 1)
                break;
        }
    }

	public void UpdateTimers()
	{
		if (!killerInKitchen) {
			killerTimer++;

			if (pizzaTimer >= 0) {
				pizzaTimer++;
			}

			if (policeTimer >= 0) {
				policeCap++;
			}
		}

		// If the player has left the living room, the killer no longer should show in the window and peephole
		if (killerTimer == 1) {
			ChangeState (94, 1);
			ChangeState (95, 1);
		} else if (killerTimer == 5) {
			AddText ("You think you hear a distant creaking of the floorboards. \n\n");
		} else if (killerTimer == 10) {
			AddText ("That sounded awfully close... \n\n");
		}

		else if (killerTimer == 15) {
			//ChangeState (-1, 100);
			//SetImage (GetRandomDeathImage ());
			//AddText ("You died! \n\n");
			playerOutOfTime = true;
		}

		if (pizzaTimer == pizzaCap) {
			AddText ("You hear the sweet sweet pizza man at the door \n\n" );
			ChangeState (94, 2);
			ChangeState (95, 2);
		}

		if (policeTimer == policeCap) {
			AddText ("cops are here, dingo \n\n");
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

	[Command("go")]
	[Command("enter")]
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
			UpdateRoomState (false, newRoomObj.Index);
        }

		if (currentRoom.AdjacentRooms.Contains (newRoom)) {
			if (newRoomObj.currentState.Gettable == 1) {
				AddText ("");

				// If going to the secret lair, lock the living room
				if (rooms [newRoom].Index == 6) {
					currentRoom.States [currentRoom.State].Gettable = 0;
					AddText ("As you enter the lair, you hear the door close behind you. WHOOPS! HAHAHA \n\n");
				}

				UpdateTimers ();

				if (health <= 0)
					return;
					

				ResetItemGroup ();
				room = newRoom;

				Look (null);
				return;
			}
			else{
				switch (currentRoom.Index) {
					// Secret Lair to living room
				case 1:
					AddText ("It's too dark out there. \n\n");
					break;
				case 6:
					AddText ("The door closed behind you, idiot. \n\n");
					break;
				default:
					break;
				}
			}

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
				UpdateItemGroup (obj.Index);
				UpdateRoomState();

				return; // TODO: will there only ever be one?
            }
        }
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
		AddText(item.currentState.Get);

		if (item.currentState.Gettable == 1)
        {
            inventory.Add(item.Index);
            item.State++;
			itemsList [item.Index].State = item.State;
			foreach (KeyValuePair<int, int> actions in item.currentState.ConditionalActions.ConditionalActions)
            {
                if (ChangeState(actions.Key, actions.Value) == 1)
                    break;
            }

			if (item.currentState.Image != "")
			{
				ImageCheckAndShow (item.Index, item.State, item.currentState.Image);
				roomImage = false;
			}

			// Gun
			if (item.Index == 58) {
				killerInBedroom = true;
			}

			// Teddy Bear
			if (item.Index == 87) {
				if (inventory.Contains (74) || inventory.Contains (75) || inventory.Contains (76)) {
					killerInLair = true;
				}

			}

			if (item.Index == 74 || item.Index == 75 || item.Index == 76) {

				ImageCheckAndShow (73, 0, "checkitem");
				roomImage = false;

				if (inventory.Contains (87)) {
					killerInLair = true;
				}
			}

        }

		UpdateItemGroup (item.Index);
		UpdateRoomState(roomImage);
    }

    [Command]
    public void Use(List<string> argv)
    {
        string itemName = string.Join(" ", argv.Skip(1).ToArray());
		bool roomImage = true;

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

		foreach (var response in useResponses) {

			if (response.ItemIndex == 66) {
				if (!bearTrapMade) {
					if (inventory.Contains (23) && inventory.Contains (34)) {
						if (playerKnowsBeartrap) {
							AddText ("you make the bear trap and put it on the stairs");
							inventory.Remove (23);
							inventory.Remove (34);
							ChangeState (98, 1);
							SetImage (GetImageByName ("beartrap"));
							return;
						}
						else {
							AddText ("you have the components for...something sharp!?");
							SetImage (GetImageByName ("workbench"));
							return;
						}

					}
				} 
				if (!fireTrapMade) {
					if ((inventory.Contains (15) && inventory.Contains (28) && inventory.Contains(43)) || (inventory.Contains (64) && inventory.Contains (28) && inventory.Contains(43))) {
						if (playerKnowsFiretrap) {
							AddText ("you make the fire trap and put it on the stairs");
							inventory.Remove (28);
							inventory.Remove (43);
							if (inventory.Contains (15))
								inventory.Remove (15);
							if (inventory.Contains (64))
								inventory.Remove (64);
							ChangeState (100, 1);
							SetImage (GetImageByName ("firetrap"));
							return;
						}
						else {
							AddText ("you have the components for...something fire-related?");
							SetImage (GetImageByName ("workbench"));
							return;
						}

					}
				} 
				if (!bucketTrapMade) {
					if (inventory.Contains (31) && inventory.Contains (47) && inventory.Contains (65)) {
						if (inventory.Contains (41)) {
							AddText ("you make the bucket trap and put it on the stairs.");
							inventory.Remove (31);
							inventory.Remove (47);
							inventory.Remove (65);
							ChangeState (99, 1);
							SetImage (GetImageByName ("buckettrap"));
							return;
						}
						else {
							ChangeState (-1, 100);
							AddText ("You choke on the smoke ya dummy! Press [ENTER] to retry. \n\n");
							SetImage (GetImageByName ("buckettrap"));
							return;
						}
					}
				}
			}

			AddText (response.Response);

			foreach (KeyValuePair<int, int> actions in response.Actions) {
				if (ChangeState (actions.Key, actions.Value) == 1)
					break;
			}

			if (item.currentState.Image != "") {
				ImageCheckAndShow (item.Index, item.State, item.currentState.Image);
				roomImage = false;
			}

			UpdateItemGroup (item.Index);
			UpdateRoomState (roomImage);
			return;
		}                  
        // Inventory
		/*for (int i = 0; i < itemsList.Count; ++i){
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
		string itemName = "";
		if (text.Any (x => Char.IsWhiteSpace (x))) {
			itemName = text.Split (new char[] { ' ' }, 2) [1].ToLower ();
		}
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
		case "hide":
			if (itemName == "") {
				HideNoItem ();
			} else {
				command = "Hide";
			}
			break;
		case "help":
			Help ();
			return;
		case "wait":
			command = "Wait";
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

	public void Help()
	{
		AddText ("In this game, you interact with the world around you by typing commands. The Four Basic Commands are Look, Get, Use, and Move, however appropriate synonyms are recognized. For example, if you wanted to move up the stairs, you might say MOVE HALLWAY or USE STAIRCASE. The goal of this game is to survive. This can be achieved in a number of ways, but it may take some trial and error.");
	}

	public void HideNoItem()
	{
		if (currentRoom.Index == 5) {
			ImageCheckAndShow (61, 0, "checkitem");
			AddText ("You hide and wait for the killer to show up. You hear him start to come down the stairs. Press [ENTER]");
			multiSequence = true;

			if (itemsList [98].State == 0 && itemsList [99].State == 0 && itemsList [100].State == 0 && itemsList [101].State == 0) {
				currMultiSequence = UnityEngine.Random.Range(0, 8);
			}
			else if (itemsList [98].State == 1 && itemsList [99].State == 0 && itemsList [100].State == 0 && itemsList [101].State == 0) {
				currMultiSequence = 9;
			}
			else if (itemsList [98].State == 0 && itemsList [99].State == 1 && itemsList [100].State == 0 && itemsList [101].State == 0) {
				currMultiSequence = 10;
			}
			else if (itemsList [98].State == 0 && itemsList [99].State == 0 && itemsList [100].State == 1 && itemsList [101].State == 0) {
				currMultiSequence = 11;
			}
			else if (itemsList [98].State == 0 && itemsList [99].State == 0 && itemsList [100].State == 0 && itemsList [101].State == 1) {
				currMultiSequence = 12;
			}
			else if (itemsList [98].State == 1 && itemsList [99].State == 1 && itemsList [100].State == 0 && itemsList [101].State == 0) {
				currMultiSequence = 13;
			}
			else if (itemsList [98].State == 1 && itemsList [99].State == 0 && itemsList [100].State == 1 && itemsList [101].State == 0) {
				currMultiSequence = 14;
			}
			else if (itemsList [98].State == 1 && itemsList [99].State == 0 && itemsList [100].State == 0 && itemsList [101].State == 1) {
				currMultiSequence = 15;
			}
			else if (itemsList [98].State == 0 && itemsList [99].State == 1 && itemsList [100].State == 1 && itemsList [101].State == 0) {
				currMultiSequence = 16;
			}
			else if (itemsList [98].State == 0 && itemsList [99].State == 0 && itemsList [100].State == 1 && itemsList [101].State == 1) {
				currMultiSequence = 17;
			}
			else if (itemsList [98].State == 0 && itemsList [99].State == 1 && itemsList [100].State == 0 && itemsList [101].State == 1) {
				currMultiSequence = 18;
			}
			else if (itemsList [98].State == 1 && itemsList [99].State == 1 && itemsList [100].State == 1 && itemsList [101].State == 0) {
				currMultiSequence = 19;
			}
			else if (itemsList [98].State == 1 && itemsList [99].State == 1 && itemsList [100].State == 0 && itemsList [101].State == 1) {
				currMultiSequence = 20;
			}
			else if (itemsList [98].State == 1 && itemsList [99].State == 0 && itemsList [100].State == 1 && itemsList [101].State == 1) {
				currMultiSequence = 21;
			}
			else if (itemsList [98].State == 0 && itemsList [99].State == 1 && itemsList [100].State == 1 && itemsList [101].State == 1) {
				currMultiSequence = 22;
			}
			else if (itemsList [98].State == 1 && itemsList [99].State == 1 && itemsList [100].State == 1 && itemsList [101].State == 1) {
				currMultiSequence = 23;
			}
		}
	}

	public void Hide(int i, int j)
	{
		var item = itemsList [i];
		bool roomImage = true;
		if (specialResponses [j].Command == "Hide" && specialResponses [j].ItemIndex == item.Index && specialResponses [j].ItemState == item.State) {
			AddText(specialResponses[j].Response);

			foreach (KeyValuePair<int, int> actions in specialResponses[j].Actions)
			{
				if (ChangeState(actions.Key, actions.Value) == 1)
					break;
			}

			if (specialResponses[j].Image != "")
			{
				ImageCheckAndShow (item.Index, item.State, specialResponses [j].Image);
				roomImage = false;
			}

			UpdateItemGroup (item.Index);
			UpdateRoomState(roomImage);

			return;
		}
	}

    public void Read(int i, int j)
    {
        var item = itemsList[i];
		bool roomImage = true;
        if (specialResponses[j].Command == "Read" && specialResponses[j].ItemIndex == item.Index && specialResponses[j].ItemState == item.State)
        {
			// If player is reading combination code, they 'know' the code
			if (item.Index == 93 && item.State == 1) {
				playerKnowsCombo = true;
			}

			// If player is reading Bear book, they 'know' the trap blueprint
			if (item.Index == 19) {
				playerKnowsBeartrap = true;
			}

            AddText(specialResponses[j].Response);

            foreach (KeyValuePair<int, int> actions in specialResponses[j].Actions)
            {
                if (ChangeState(actions.Key, actions.Value) == 1)
                    break;
            }

            if (specialResponses[j].Image != "")
            {
				ImageCheckAndShow (item.Index, item.State, specialResponses [j].Image);
				roomImage = false;
            }

			UpdateItemGroup (item.Index);
			UpdateRoomState(roomImage);

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
				ImageCheckAndShow (item.Index, item.State, specialResponses[j].Image);

                foreach (KeyValuePair<int, int> actions in specialResponses[j].Actions)
                {
                    if (ChangeState(actions.Key, actions.Value) == 1)
                        break;
                }

				UpdateItemGroup (item.Index);
				UpdateRoomState(false);

				if (item.Index == 11) {
					pizzaTimer = 0;
				} else if (item.Index == 14) {
					policeTimer = 0;
				}

                return;
            }
            else
            {
                AddText("Hmm, there’s no dial tone anymore. That’s...not normal, right? The killer must have cut the phone line.");
				ImageCheckAndShow (item.Index, item.State, specialResponses[j].Image);

				UpdateItemGroup (item.Index);
				UpdateRoomState(false);

                return;
            }
        }
    }


    public void Open(int i, int j)
    {
        var item = itemsList[i];
		bool roomImage = true;
		if (specialResponses[j].Command == "Open" && specialResponses[j].ItemIndex == item.Index && specialResponses[j].ItemState == item.State)
        {
			if (item.Index == 55 && item.State == 1 && !playerKnowsCombo) {
				AddText ("ah man, if only I knew the dumb combo");
				SetImage(GetImageByName("safe"));
				return;
			}

			if (item.Index == 8 && pizzaTimer >= pizzaCap && pizzaTimer <= pizzaCap2) {
				multiSequence = true;
				currMultiSequence = 8;
				AddText ("The pizza mans gets the bad guy, whoa!");
				SetImage (GetImageByName ("pizzawin"));
				return;
			}

			if (item.Index == 8 && killerInKitchen) {
				AddText ("No time for that, the killer's almost dead!");
				return;
			}

            AddText(specialResponses[j].Response);


            int state = item.State;
            foreach (KeyValuePair<int, int> actions in specialResponses[j].Actions)
            {
                if (ChangeState(actions.Key, actions.Value, 1) == 1)
                    break;
            }

            if (specialResponses[j].Image != "")
            {
				ImageCheckAndShow (item.Index, item.State, specialResponses [j].Image);
				roomImage = false;
            }

			UpdateItemGroup (i);
			UpdateRoomState (roomImage);

            return;
        }
    }

    public void Close(int i, int j)
    {
        var item = itemsList[i];
		bool roomImage = true;
		if (specialResponses[j].Command == "Close" && specialResponses[j].ItemIndex == item.Index && specialResponses[j].ItemState == item.State)
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
				ImageCheckAndShow (item.Index, item.State, specialResponses [j].Image);
				roomImage = false;
            }

			UpdateItemGroup (i);
			UpdateRoomState (roomImage);

            return;
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
