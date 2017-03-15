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
	public List<GameObject> inventory;

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
	public GradualTextRevealer inventoryText;

    Dictionary<string, MethodInfo> commands;
    List<string> specialCommands;
	List<ItemGroup> itemGroups;
	ItemGroup currentItemGroup;
	List<CheckItem> checkItems;
	Dictionary<int, string> audioIndex;
	bool playerKnowsCombo, playerKnowsBeartrap, playerKnowsFiretrap;
	bool bearTrapMade, fireTrapMade, bucketTrapMade, shitOnStairs;
	bool playerOutOfTime;
	bool inputLockdown;
	Dictionary<int, List<string>> lockdownOptions;
	bool multiSequence;
	bool killerInBedroom, killerInKitchen, killerInLair, killerInShack;
	bool playerBedroomShot, playerLairThreaten;
	bool tapeRecorderUsed;
	int killerTimer;
	int pizzaTimer;
	int policeTimer;
	int killerCap;
	int pizzaCap;
	int pizzaCap2;
	int policeCap;
	int dummyStepsCompleted;
	bool dummyAssembled;
	bool audioLooping, stopAudio;
	bool gasMaskOn;
	bool inventoryUp;
	bool twoLayerLook;
	bool healthScreenUp;
	Dictionary<int, List<string>> deathImages;
	Dictionary<int, List<string>> deathOverlays;
	Dictionary<int, List<string>> useWithWhats;
	string currOverlay;
	int currMultiSequence;
	int currLockdownOption;
	int multiSequenceStep;
	List<MultiSequence> multiSequences;
	List<Image> inventoryImages;
	List<GradualTextRevealer> inventoryTextboxes;

    public Image image;
	public Image overlayImage;
	public Image gasMaskOverlay;
	public Image basementOverlay;
	public Image basementOverlay2;
	public Image basementOverlay3;
	public Image basementOverlay4;
	public Image inv0, inv1, inv2, inv3, inv4, inv5, inv6, inv7, inv8, inv9, inv10, inv11, inv12, inv13, inv14, inv15, inv16, inv17, inv18, inv19; 
	public GradualTextRevealer invText0, invText1, invText2, invText3, invText4, invText5, invText6, invText7, invText8, invText9, invText10, invText11, invText12, invText13, invText14, invText15, invText16, invText17, invText18, invText19, inventoryTopText;
	public GradualTextRevealer helpText;
	public AudioSource audioSource;
	public AudioSource loopingAudioSource;

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
		inventoryImages = new List<Image> ();
		inventoryTextboxes = new List<GradualTextRevealer> ();
		inventoryImages.Add (inv0);
		inventoryImages.Add (inv1);
		inventoryImages.Add (inv2);
		inventoryImages.Add (inv3);
		inventoryImages.Add (inv4);
		inventoryImages.Add (inv5);
		inventoryImages.Add (inv6);
		inventoryImages.Add (inv7);
		inventoryImages.Add (inv8);
		inventoryImages.Add (inv9);
		inventoryImages.Add (inv10);
		inventoryImages.Add (inv11);
		inventoryImages.Add (inv12);
		inventoryImages.Add (inv13);
		inventoryImages.Add (inv14);
		inventoryImages.Add (inv15);
		inventoryImages.Add (inv16);
		inventoryImages.Add (inv17);
		inventoryImages.Add (inv18);
		inventoryImages.Add (inv19);

		inventoryTextboxes.Add (invText0);
		inventoryTextboxes.Add (invText1);
		inventoryTextboxes.Add (invText2);
		inventoryTextboxes.Add (invText3);
		inventoryTextboxes.Add (invText4);
		inventoryTextboxes.Add (invText5);
		inventoryTextboxes.Add (invText6);
		inventoryTextboxes.Add (invText7);
		inventoryTextboxes.Add (invText8);
		inventoryTextboxes.Add (invText9);
		inventoryTextboxes.Add (invText10);
		inventoryTextboxes.Add (invText11);
		inventoryTextboxes.Add (invText12);
		inventoryTextboxes.Add (invText13);
		inventoryTextboxes.Add (invText14);
		inventoryTextboxes.Add (invText15);
		inventoryTextboxes.Add (invText16);
		inventoryTextboxes.Add (invText17);
		inventoryTextboxes.Add (invText18);
		inventoryTextboxes.Add (invText19);

		// Set up various variables
		inventory = new List<GameObject>();
		inventory.Add (GetObjectByName ("map"));
		room = 0;
        health = 100;
		killerTimer = 0;
		pizzaTimer = -1;
		policeTimer = -1;
		killerCap = 20;
		pizzaCap = 5;
		pizzaCap2 = 10;
		policeCap = 5;
		dummyStepsCompleted = 0;
		dummyAssembled = false;
		audioLooping = stopAudio = false;
		gasMaskOn = false;
		inventoryUp = false;
		currentItemGroup = null;
		multiSequenceStep = 0;
		playerKnowsCombo = playerKnowsBeartrap = playerKnowsFiretrap = false;
		bearTrapMade = fireTrapMade = bucketTrapMade = shitOnStairs = false;
		playerOutOfTime = false;
		playerBedroomShot = playerLairThreaten = false;
		inputLockdown = false;
		multiSequence = false;
		tapeRecorderUsed = false;
		twoLayerLook = false;
		healthScreenUp = false;
		killerInBedroom = killerInKitchen = killerInLair = killerInShack = false;
		currOverlay = "";
    }

    void Start()
    {
		parsedHouseXml = XElement.Parse(xmlDocument.text);


        // Alt Names
        AltNamesParser altNamesParser = gameObject.GetComponent(typeof(AltNamesParser)) as AltNamesParser;
        TextAsset altNamesText = altNamesParser.xmlDocument;
        XmlDocument altNamesDoc = new XmlDocument();
        altNamesDoc.LoadXml(altNamesText.text);
        altNames = altNamesParser.ReadXML(altNamesDoc);

		SetupHouse();
		SetupCommands();
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
		dict.Add (9, new List<string> { "outsidedummy-knife", "outsidedummy-mace" });
		return dict;
	}

	Dictionary<int, List<string>> GetDeathImages()
	{
		Dictionary<int, List<string>> dict = new Dictionary<int, List<string>> ();
		dict.Add (0, new List<string> { "lrdeath", "lrdeath2" });
		dict.Add (1, new List<string> { "death-knife", "death-knife2" });
		dict.Add (4, new List<string> { "death-katana", "death-katana2", "death-knife", "death-knife2" });
		dict.Add (5, new List<string> { "death-gun", "death-gun2", "death-katana", "death-katana2", "death-knife", "death-knife2", "death-mace", "death-mace2" });
		dict.Add (6, new List<string> { "death-gun", "death-gun2", "death-katana", "death-katana2", "death-knife", "death-knife2", "death-mace", "death-mace2" });
		dict.Add (7, new List<string> { "death-knife", "death-knife2", "death-mace", "death-mace2" });
		dict.Add (10, new List<string> { "lrdeath3", "lrdeath4" });
		return dict;
	}

	Dictionary<int, List<string>> GetLockdownOptions()
	{
		Dictionary<int, List<string>> dict = new Dictionary<int, List<string>> ();

		List<string> killerNames = new List<string> {"killer", "intruder", "bad guy", "shape", "villain", "guy", "man", "lady", "woman", "person", "thing"};

		dict.Add (0, new List<string> { "use knife", "use the knife", "get knife", "stab [killer]", "stab the [killer]", "use knife on [killer]", "use knife on the [killer]", "stab [killer] with knife", "stab [killer] with the knife" });
		dict.Add (1, new List<string> { "use [item]" , "shoot [item]", "fire [item]", "use [item] on [killer]", "fire [item] at [killer]", "shoot [item] at [killer]", "shoot [killer]", "shoot at [killer]" });
		dict.Add (2, new List<string> { "threaten [0]", "threaten the [0]",
		"use [1] with [0]", "use [1] on [0]", "threaten [0] with [1]",
			"use [2] with [0]", "use [2] on [0]", "threaten [0] with [2]",
			"use [3] with [0]", "use [3] on [0]", "threaten [0] with [3]",
			"use [4] with [0]", "use [4] on [0]", "threaten [0] with [4]",
			"use [5] with [0]", "use [5] on [0]", "threaten [0] with [5]"});
		dict.Add (3, new List<string> { "rake", "the rake", "with rake", "with the rake", "use with rake", "use with the rake" });
		dict.Add (4, new List<string> { "dummy" , "with dummy", "use with dummy", "rake", "with rake", "use with rake", "tarp", "with tarp", "use with tarp", "rake and tarp",
		"with rake and tarp", "use with rake and tarp"});
		dict.Add (5, new List<string> { "dummy" , "with dummy", "use with dummy", "rake", "with rake", "use with rake", "tarp", "with tarp", "use with tarp", "rake and tarp",
			"with rake and tarp", "use with rake and tarp" });
		dict.Add (6, new List<string> { "lock door" });
		dict.Add (7, new List<string> { "[0]", "[1]", "[2]", "[3]", "call [0]", "call [1]", "call [2]", "call [3]", "dial [0]", "dial [1]", "dial [2]", "dial [3]"});
		dict.Add (8, new List<string> { "1", "2", "3", "4", "[0]", "[1]", "[2]", "[3]", "read [0]", "read [1]", "read [2]", "read [3]", "look [0]", "look [1]", "look [2]", "look [3]"
			,"read 1", "read 2", "read 3", "read 4", "look 1", "look 2", "look 3", "look 4", "use 1", "use 2", "use 3", "use 4", "use [0]", "use [1]", "use [2]", "use [3]"});
		dict.Add (9, new List<string> { "[0]", "[1]", "basement", "back", "look [0]", "look [1]" });
		dict.Add (10, new List<string> { "[0]", "[1]", "basement", "back", "open [0]", "open [1]" });
		dict.Add (11, new List<string> { "[0]", "[1]", "basement", "back", "close [0]", "close [1]" });
		dict.Add (12, new List<string> { "[0]", "[1]", "basement", "back", "use [0]", "use [1]" });
		dict.Add (13, new List<string> { "[0]", "[1]", "bedroom", "bathroom", "look [0]", "look [1]" });
		dict.Add (14, new List<string> { "[0]", "[1]", "bedroom", "bathroom", "open [0]", "open [1]" });
		dict.Add (15, new List<string> { "[0]", "[1]", "bedroom", "bathroom", "close [0]", "close [1]" });
		dict.Add (16, new List<string> { "[0]", "[1]", "bedroom", "bathroom", "use [0]", "use [1]" });

		for (int i = 0; i < dict.Count; ++i) {
			string itemName = "";

			List<string> items = new List<string>();

			switch (i) {
			case 0:
				itemName = "knife";
				break;
			case 1:
				itemName = "revolver";
				break;
			case 2:
				items.Add ("teddy bear");
				items.Add ("scalpel");
				items.Add ("spoon");
				items.Add ("scissors");
				items.Add ("knife");
				items.Add ("blender");
				break;
			case 3:
				itemName = "knife";
				break;
			case 4:
				itemName = "knife";
				break;
			case 5:
				itemName = "knife";
				break;
			case 6:
				itemName = "knife";
				break;
			case 7:
				items.Add ("speed dial 1");
				items.Add ("speed dial 2");
				items.Add ("speed dial 3");
				items.Add ("speed dial 4");
				break;
			case 8:
				items.Add ("book 1");
				items.Add ("book 2");
				items.Add ("book 3");
				items.Add ("book 4");
				break;
			case 9:
				items.Add ("back door");
				items.Add ("basement door");
				break;
			case 10:
				items.Add ("back door");
				items.Add ("basement door");
				break;
			case 11:
				items.Add ("back door");
				items.Add ("basement door");
				break;
			case 12:
				items.Add ("back door");
				items.Add ("basement door");
				break;
			case 13:
				items.Add ("bedroom door");
				items.Add ("bathroom door");
				break;
			case 14:
				items.Add ("bedroom door");
				items.Add ("bathroom door");
				break;
			case 15:
				items.Add ("bedroom door");
				items.Add ("bathroom door");
				break;
			case 16:
				items.Add ("bedroom door");
				items.Add ("bathroom door");
				break;
			default:
				break;
			}

			List<List<string>> dictAlts = new List<List<string>>();
			List<string> toAdd = new List<string>();
			List<string> toAddToo = new List<string> ();
			List<string> toRemove = new List<string> ();
			List<string> toRemoveToo = new List<string> ();

			foreach (KeyValuePair<string, List<string>> entry in altNames)
			{
				if (itemName != "") {
					if (entry.Key == itemName) {
						List<string> altNamesToGet = new List<string> ();
						altNamesToGet.Add (entry.Key);
						foreach (var x in entry.Value) {
							altNamesToGet.Add (x);
						}
						dictAlts.Add (altNamesToGet);
					}
				} 
				else {
					for (int j = 0; j < items.Count; ++j) {
						if (entry.Key == items [j]) {
							List<string> altNamesToGet = new List<string> ();
							altNamesToGet.Add (entry.Key);
							foreach (var x in entry.Value) {
								altNamesToGet.Add (x);
							}
							dictAlts.Add (altNamesToGet);
						}
					}
				}
			}

			foreach (var dictItem in dict[i]) {
				if (dictItem.Contains ("[item]")) {
					foreach (var dictAltItem in dictAlts[0]) {
						string tempDictAltItem = dictItem.Replace ("[item]", dictAltItem);
						if (dictItem.Contains ("[killer]")) {
							foreach (var killerName in killerNames) {
								toAdd.Add (tempDictAltItem.Replace ("[killer]", killerName));
							}
						} else {
							toAdd.Add (tempDictAltItem);
						}
					}
				} else if (dictItem.Contains ("[0]") || dictItem.Contains("[1]") || dictItem.Contains("[2]") || dictItem.Contains("[3]") || dictItem.Contains("[4]")
					|| dictItem.Contains("[5]")) {
					for (int j = 0; j < items.Count; ++j) {		
						foreach (var dictAltItem in dictAlts[j]) {
							if (dictItem.Contains ("[" + j + "]")) {
								string tempDictAltItem = dictItem.Replace ("[" + j + "]", dictAltItem);
								if (dictItem.Contains ("[killer]")) {
									foreach (var killerName in killerNames) {
										toAdd.Add (tempDictAltItem.Replace ("[killer]", killerName));
									}
								} else {
									toAdd.Add (tempDictAltItem);
								}
							}
						}
					}
				}
				else {
					if (dictItem.Contains ("[killer]")) {
						foreach (var killerName in killerNames) {
							toAdd.Add (dictItem.Replace ("[killer]", killerName));
						}
					}
				}
			}

			// second pass
			if (items.Count > 0) {
				foreach (var toAddItem in toAdd) {
					if (toAddItem.Contains ("[0]") || toAddItem.Contains ("[1]") || toAddItem.Contains ("[2]") || toAddItem.Contains ("[3]") || toAddItem.Contains ("[4]")
					   || toAddItem.Contains ("[5]")) {
						for (int j = 0; j < items.Count; ++j) {		
							foreach (var dictAltItem in dictAlts[j]) {
								if (toAddItem.Contains ("[" + j + "]")) {
									toAddToo.Add (toAddItem.Replace ("[" + j + "]", dictAltItem));
								}
							}
						}
					}
				}
			}

			// removal pass
			foreach (var toAddItem in toAdd) {
				if (toAddItem.Contains ("[0]") || toAddItem.Contains ("[1]") || toAddItem.Contains ("[2]") || toAddItem.Contains ("[3]") || toAddItem.Contains ("[4]")
				    || toAddItem.Contains ("[5]")) {
					toRemove.Add (toAddItem);
				}
			}

			foreach (var toAddItem in toAddToo) {
				if (toAddItem.Contains ("[0]") || toAddItem.Contains ("[1]") || toAddItem.Contains ("[2]") || toAddItem.Contains ("[3]") || toAddItem.Contains ("[4]")
					|| toAddItem.Contains ("[5]")) {
					toRemoveToo.Add (toAddItem);
				}
			}

			foreach (var toRemoveItem in toRemove) {
				toAdd.Remove (toRemoveItem);
			}

			foreach (var toRemoveItem in toRemoveToo) {
				toAddToo.Remove (toRemoveItem);
			}

			foreach (var toAddItem in toAdd) {
				dict [i].Add (toAddItem);
			}

			foreach (var toAddItem in toAddToo) {
				dict [i].Add (toAddItem);
			}

		}

		return dict;
	}

    Texture2D GetImageByName(string name)
    {
        return Resources.Load(name) as Texture2D;
    }

	Texture2D GetRandomDeathImage()
	{
		int currRoom = currentRoom.Index;
		if (currRoom == 0 && policeTimer >= policeCap && killerTimer < killerCap)
			currRoom = 10;
		List<string> imageList = deathImages [currRoom];
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

	void PlayClip (AudioClip clip) {
		audioSource.loop = false;
		audioSource.clip = clip;
		audioSource.Play ();
	}

	void PlayLoopingAudio (int clipId) {
		AudioClip thisClip = GetClip (clipId);
		audioLooping = true;
		loopingAudioSource.loop = true;
		loopingAudioSource.clip = thisClip;
		loopingAudioSource.Play ();
	}

	void StopLoopingAudio (){
		loopingAudioSource.Stop ();
	}

	void ResetOverlay(){
		Texture2D blankOverlay = Resources.Load ( "blankoverlay" ) as Texture2D;
		SetOverlay (blankOverlay);
	}

	void SetGasMaskOverlay(bool equip) {
		string overlay = "";
		if (equip) {
			overlay = "gasmaskoverlay";
		} else {
			overlay = "blankoverlay";
		}
		Texture2D maskOverlay = Resources.Load (overlay) as Texture2D;
		gasMaskOverlay.sprite = Sprite.Create(maskOverlay, image.sprite.rect, image.sprite.pivot);
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
		if (inventoryUp) {
			ResetInventory ();

			if (gasMaskOn) {
				SetGasMaskOverlay (true);
			}

			SetOverlay (GetImageByName (currOverlay));

			inventoryUp = false;
		}

		if (healthScreenUp) {
			AddHelpText ("");

			if (gasMaskOn) {
				SetGasMaskOverlay (true);
			}

			SetOverlay (GetImageByName (currOverlay));

			healthScreenUp = false;
		}

		if (!multiSequence) {
			if (killerInBedroom) {
				if (!playerBedroomShot) {
					SetImage (GetImageByName (currentRoom.currentState.Image));
					SetOverlay (GetRandomDeathOverlay ());
					AddText ("o shit, the killer's in your bedroom!");
					playerBedroomShot = true;
					currLockdownOption = 1;
					inputLockdown = true;
				} else {

					if (killerInKitchen) {
						AddText ("The killer runs away!");
						UpdateRoomState ();
						killerInBedroom = false;
						inputLockdown = false;
						currLockdownOption = 0;
						SetOverlay (GetImageByName ("bedroomblood"));
						return;
					}

					var options = lockdownOptions [currLockdownOption];

					foreach (var option in options) {
						if (text == option) {
							BedroomGunshot ();
							return;
						}
					}

					string weapon = currOverlay.Split ('-').Last ();
					var overlays = deathImages [currentRoom.Index];
					if (weapon == "katana") {
						SetImage (GetImageByName (overlays.ElementAt (UnityEngine.Random.Range (0, 2))));
					} else {
						SetImage (GetImageByName (overlays.ElementAt (UnityEngine.Random.Range (2, 4))));
					}
					ResetOverlay ();
					SetGasMaskOverlay (false);
					AddText ("You died. Maybe you should have fought back? Press [ENTER] to continue!");
					health = 0;
					killerInBedroom = false;
				}
			} else if (killerInKitchen && currentRoom.Index == 1) {
				var options = lockdownOptions [currLockdownOption];

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
				SetGasMaskOverlay (false);
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
					currLockdownOption = 2;
					inputLockdown = true;
				} else {
					var options = lockdownOptions [currLockdownOption];

					foreach (var option in options) {
						if (text == option) {

							if (text.Contains ("scalpel")) {
								LairThreaten ("scalpel");
							} else if (text.Contains ("spoon")) {
								LairThreaten ("spoon");
							} else if (text.Contains ("scissors")) {
								LairThreaten ("scissors");
							} else if (text.Contains ("blender")) {
								LairThreaten ("blender");
							} else if (text.Contains ("knife")) {
								LairThreaten ("knife");
							} else {
								LairThreaten ("");
							}
							return;
						}
					}

					string weapon = currOverlay.Split ('-').Last ();
					var overlays = deathImages [currentRoom.Index];
					if (weapon == "gun") {
						SetImage (GetImageByName (overlays.ElementAt (UnityEngine.Random.Range (0, 2))));
					} 
					else if (weapon == "katana") {
						SetImage (GetImageByName (overlays.ElementAt (UnityEngine.Random.Range (2, 4))));
					}
					else if (weapon == "knife") {
						SetImage (GetImageByName (overlays.ElementAt (UnityEngine.Random.Range (4, 6))));
					}
					else {
						SetImage (GetImageByName (overlays.ElementAt (UnityEngine.Random.Range (6, 8))));
					}
					ResetOverlay ();
					SetGasMaskOverlay (false);
					AddText ("You died. Maybe you could have saved yourself. Press [ENTER] to continue!");
					health = 0;
					killerInLair = false;
				}
			} 
			else if (killerInShack) {
				var options = lockdownOptions [currLockdownOption];

				foreach (var option in options) {
					if (text == option) {
						ShackLock ();
						return;
					}
				}

				string weapon = currOverlay.Split ('-').Last ();
				var overlays = deathImages [currentRoom.Index];
				if (weapon == "knife") {
					SetImage (GetImageByName (overlays.ElementAt (UnityEngine.Random.Range (0, 2))));
				} 
				else {
					SetImage (GetImageByName (overlays.ElementAt (UnityEngine.Random.Range (2, 4))));
				}
				ResetOverlay ();
				SetGasMaskOverlay (false);
				AddText ("You died. Maybe you could have LOCKED THE DOOR YA DINGU. Press [ENTER] to continue!");
				health = 0;
				killerInShack = false;
			}
			else {
				if (health > 0) {
					if (inputLockdown) {
						var options = lockdownOptions [currLockdownOption];
						foreach (var option in options) {
							if (text == option) {

								if (currLockdownOption == 7 || currLockdownOption == 8 || currLockdownOption == 9 || currLockdownOption == 10
									|| currLockdownOption == 11 || currLockdownOption == 12 || currLockdownOption == 13 || currLockdownOption == 14
									|| currLockdownOption == 15 || currLockdownOption == 16) {
									LockdownResponse (text, true);
									return;
								}

								CombineItems ();
								return;
							}
						}
						if (currLockdownOption == 7 || currLockdownOption == 8 || currLockdownOption == 9 || currLockdownOption == 10
							|| currLockdownOption == 11 || currLockdownOption == 12 || currLockdownOption == 13 || currLockdownOption == 14
							|| currLockdownOption == 15 || currLockdownOption == 16) {
							LockdownResponse (text, false);
							return;
						}
					} 
					else {
						if (text != "") {	
						
							if (gasMaskOn) {
								SetGasMaskOverlay (true);
							}
							//Debug.LogFormat ("Running command: {0}", text);
							var tokens = text.Shlex ();

							tokens.Remove ("the");

							var cmdName = tokens [0];

							if (commands.ContainsKey (cmdName)) {
								var cmd = commands [cmdName];
								cmd.Invoke (this, new object[] { tokens });
								return;
							}	

							OtherCommands (text);
						}
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
		SetGasMaskOverlay (false);
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
				// TODO WIN logic
				multiSequence = false;
				health = 0;
				AddAdditionalText ("\n\nPress [ENTER] to restart.");
				return;
			} else {
				// LOSE
				multiSequence = false;
				health = 0;
				AddAdditionalText ("\n\nPress [ENTER] to restart.");
				return;

			}
		}
		else {
			AddAdditionalText ("\n\nPress [ENTER] to continue.");
		}
	}

    void Update() 
	{
		if (audioLooping && stopAudio && audioSource.isPlaying) {
			StopLoopingAudio ();
			stopAudio = false;
		}
	}

    public void ResetHouse()
    {
        SetupHouse();
        AddText("Type some shit here. Good luck, and don't fuck up this time!");
    }

    public void AddText(string txt)
    {
        //text.StartReveal("\n" + txt + "\n");

		txt = txt.Replace ("[NEWLINE]", "\r");
		txt = txt.Replace ("[DOUBLENEWLINE]", "\n");
		text.AppendText(txt);
    }

	public void AddAdditionalText(string txt)
	{
		txt = txt.Replace ("[NEWLINE]", "\n");
		txt = txt.Replace ("[DOUBLENEWLINE]", "\n");
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

	GameObject GetObjectFromInv(string name) {
		GameObject item = null;
		name = name.ToLower();
		foreach (var obj in inventory) {
			if (AltNameCheck (name, "look") == obj.Index || name == obj.Name) {
				item = obj;
				break;
			}
		}

		return item;
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
		if (currentRoom.Index == 5 && currentRoom.currentState.Image == tex.name) {
			if (bearTrapMade || bucketTrapMade || fireTrapMade || shitOnStairs) {
				if (bearTrapMade) {
					SetBasementOverlay (0, true);
				}

				if (bucketTrapMade) {
					SetBasementOverlay (1, true);
				}

				if (fireTrapMade) {
					SetBasementOverlay (2, true);
				}

				if (shitOnStairs) {
					SetBasementOverlay (3, true);
				}
			}
		} else {
			SetBasementOverlay (4, false);
		}

		if (tex.name == "orangejuice" || tex.name == "icecubetrays" || tex.name == "flashlight" || tex.name == "bleach" || tex.name == "bucket") {
			twoLayerLook = true;
		}

        image.sprite = Sprite.Create(tex, image.sprite.rect, image.sprite.pivot);
		image.sprite.name = tex.name;
    }

	void SetInventoryImage(Texture2D tex, int invItem){
		var thisInvItem = inventoryImages [invItem];
		thisInvItem.sprite = Sprite.Create (tex, thisInvItem.sprite.rect, thisInvItem.sprite.pivot);
	}

	void AddGenericInventoryText(string txt){
		inventoryTopText.AppendText (txt);
	}

	void AddInventoryText(string txt, int invText){
		var thisInvText = inventoryTextboxes [invText];
		thisInvText.AppendText (txt);
	}

	void AddHelpText(string txt){
		helpText.AppendText (txt);
	}

	void ResetInventory(){
		for (int i = 0; i < inventoryImages.Count; ++i) {
			SetInventoryImage (GetImageByName ("blankinv"), i);
		}

		for (int i = 0; i < inventoryTextboxes.Count; ++i) {
			AddInventoryText ("", i);
		}

		AddGenericInventoryText ("");
	}

	void SetOverlay(Texture2D tex)
	{
		overlayImage.sprite = Sprite.Create(tex, overlayImage.sprite.rect, overlayImage.sprite.pivot);
		currOverlay = tex.name;
	}

	void SetBasementOverlay(int trapNo, bool setImage)
	{
		switch (trapNo) {
		case 0:
			if (setImage) {
				basementOverlay.sprite = Sprite.Create (GetImageByName ("basementoverlay"), basementOverlay.sprite.rect, basementOverlay.sprite.pivot);
			} else {
				basementOverlay.sprite = Sprite.Create (GetImageByName ("blankoverlay"), basementOverlay.sprite.rect, basementOverlay.sprite.pivot);
			}
			break;
		case 1:
			if (setImage) {
				basementOverlay2.sprite = Sprite.Create (GetImageByName ("basementoverlay2"), basementOverlay.sprite.rect, basementOverlay.sprite.pivot);
			} else {
				basementOverlay2.sprite = Sprite.Create (GetImageByName ("blankoverlay"), basementOverlay.sprite.rect, basementOverlay.sprite.pivot);
			}
			break;
		case 2:
			if (setImage) {
				basementOverlay3.sprite = Sprite.Create (GetImageByName ("basementoverlay3"), basementOverlay.sprite.rect, basementOverlay.sprite.pivot);
			} else {
				basementOverlay3.sprite = Sprite.Create (GetImageByName ("blankoverlay"), basementOverlay.sprite.rect, basementOverlay.sprite.pivot);
			}
			break;
		case 3:
			if (setImage) {
				basementOverlay4.sprite = Sprite.Create (GetImageByName ("basementoverlay4"), basementOverlay.sprite.rect, basementOverlay.sprite.pivot);
			} else {
				basementOverlay4.sprite = Sprite.Create (GetImageByName ("blankoverlay"), basementOverlay.sprite.rect, basementOverlay.sprite.pivot);
			}
			break;
		case 4:
			if (setImage) {
				basementOverlay.sprite = Sprite.Create (GetImageByName ("basementoverlay"), basementOverlay.sprite.rect, basementOverlay.sprite.pivot);
				basementOverlay2.sprite = Sprite.Create (GetImageByName ("basementoverlay2"), basementOverlay.sprite.rect, basementOverlay.sprite.pivot);
				basementOverlay3.sprite = Sprite.Create (GetImageByName ("basementoverlay3"), basementOverlay.sprite.rect, basementOverlay.sprite.pivot);
				basementOverlay4.sprite = Sprite.Create (GetImageByName ("basementoverlay4"), basementOverlay.sprite.rect, basementOverlay.sprite.pivot);
			} else {
				basementOverlay.sprite = Sprite.Create (GetImageByName ("blankoverlay"), basementOverlay.sprite.rect, basementOverlay.sprite.pivot);
				basementOverlay2.sprite = Sprite.Create (GetImageByName ("blankoverlay"), basementOverlay.sprite.rect, basementOverlay.sprite.pivot);
				basementOverlay3.sprite = Sprite.Create (GetImageByName ("blankoverlay"), basementOverlay.sprite.rect, basementOverlay.sprite.pivot);
				basementOverlay4.sprite = Sprite.Create (GetImageByName ("blankoverlay"), basementOverlay.sprite.rect, basementOverlay.sprite.pivot);
			}
			break;
		default:
			break;
		}
	}

	void ImageCheckAndShow(int itemIndex, int itemState, string image){
		var item = itemsList [itemIndex];
		switch (image) {
		case "showitem":
			if (item.currentState.Image == "checkitem") {
				SetImage (GetCheckItemImage (itemIndex));
				break;
			}
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

	void RemoveFromInv(int index){
		inventory.Remove (inventory.Where(w => w.Index == index).FirstOrDefault());
	}

	bool IsInInv (int index){
		bool playerHasItem = false;
		foreach (var obj in inventory) {
			if (obj.Index == index) {
				playerHasItem = true;
				break;
			}
		}
		return playerHasItem;
	}

	void BedroomGunshot () {
		SetImage (GetImageByName ("gunshotaction"));
		AddText ("You swing the revolver upwards, pointing it at the killer. He tenses, and a look of determined rage is visible on his face. You hesitate for a moment, and the killer starts to move. Now or never. You pull the trigger and involuntarily shut your eyes. Next second, you re-open them to find that the killer has vanished. You inch forward, searching for any sign of him, and see a trail of blood on the floor.\n\nPress [ENTER] to continue.");
		SetGasMaskOverlay (false);
		ResetOverlay ();
		//killerInBedroom = false;
		killerInKitchen = true;
	}

	void KitchenStab () {
		SetImage (GetImageByName ("knifeaction"));
		AddText ("You stab the killer and kill him. WOO");
		SetGasMaskOverlay (false);
		killerInKitchen = false;
		ResetOverlay ();
		health = 0;

		// TODO: WIN
	}

	void LairThreaten (string weapon) {
		switch (weapon) {
		case "scalpel":
			SetImage (GetImageByName ("bearscalpel"));
			multiSequence = true;
			currMultiSequence = 24;
			break;
		case "spoon":
			SetImage (GetImageByName ("bearspoon"));
			health = 0;
			break;
		case "scissors":
			SetImage (GetImageByName ("bearscissors"));
			multiSequence = true;
			currMultiSequence = 24;
			break;
		case "knife":
			SetImage (GetImageByName ("bearknife"));
			multiSequence = true;
			currMultiSequence = 24;
			break;
		case "blender":
			SetImage (GetImageByName ("bearblender"));
			multiSequence = true;
			currMultiSequence = 24;
			break;
		case "":
			List<string> bearItems = new List<string> ();
			if (IsInInv (23))
				bearItems.Add ("bearknife");
			if (IsInInv (35))
				bearItems.Add ("bearblender");
			if (IsInInv (74))
				bearItems.Add ("bearscalpel");
			if (IsInInv (75))
				bearItems.Add ("bearscissors");
			if (IsInInv (76))
				bearItems.Add ("bearspoon");

			string imageName = bearItems [UnityEngine.Random.Range (0, bearItems.Count)];

			if (imageName == "bearspoon") {
				health = 0;
			} else {
				multiSequence = true;
				currMultiSequence = 24;
			}

			if (multiSequence) {
				AddText ("You threaten the bear and win. WOO");
				if (pizzaTimer >= 0 && pizzaTimer <= pizzaCap2) {
					currMultiSequence = 25;
				}

				if (policeTimer >= 0) {
					currMultiSequence = 26;
				}
			} else {
				AddText ("You threaten the bear with a spoon??? YOU DIE");
			}

			SetGasMaskOverlay (false);
			SetImage (GetImageByName (imageName));
			break;
		}


		killerInLair = false;
		ResetOverlay ();

		// TODO: Win
	}

	void ShackLock() {
		SetImage (GetImageByName ("shackwin"));
		AddText ("You lock the door and win. WOO");
		killerInShack = false;
		ResetOverlay ();
		SetGasMaskOverlay (false);
		health = 0;
	}

	void LockdownResponse(string text, bool valid) {
		inputLockdown = false;
		switch (currLockdownOption) {
		case 7:
			if (valid) {
				if ((text.Contains ("1") && !text.Contains ("911")) || text.Contains ("paulie") || text.Contains ("pizza")) {
					OtherCommands ("call 1");
					return;
				} else if (text.Contains ("2") || text.Contains ("jones")) {
					OtherCommands ("call 2");
					return;
				} else if (text.Contains ("3") || text.Contains ("phillip") || text.Contains ("glean")) {
					OtherCommands ("call 3");
					return;
				} else if (text.Contains ("4") || text.Contains ("911") || text.Contains ("police")) {
					OtherCommands ("call 4");
					return;
				} else {
					break;
				}
			} else {
				AddText ("I'm not sure I know that number");
				return;
			}
		case 8:
			if (valid) {
				if (text.Contains ("1")) {
					OtherCommands ("read book 1");
					return;
				} else if (text.Contains ("2")) {
					OtherCommands ("read book 2");
					return;
				} else if (text.Contains ("3")) {
					OtherCommands ("read book 3");
					return;
				} else if (text.Contains ("4")) {
					OtherCommands ("read book 4");
					return;
				} else {
					break;
				}
			} else {
				AddText ("That's not one of the books on this bookshelf.");
				return;
			}
		case 9:
			if (valid) {
				if (text.Contains ("back") || text.Contains ("yard")) {
					Look ("look back door".Shlex ());
					return;
				} else if (text.Contains ("basement") || text.Contains ("cellar")) {
					Look ("look basement door".Shlex ());
					return;
				} else {
					break;
				}
			} else {
				AddText ("I'm still not sure which door you're talking about.");
				return;
			}
		case 10:
			if (valid) {
				if (text.Contains ("back") || text.Contains ("yard")) {
					OtherCommands ("open back door");
					return;
				} else if (text.Contains ("basement") || text.Contains ("cellar")) {
					OtherCommands ("open basement door");
					return;
				} else {
					break;
				}
			} else {
				AddText ("I'm still not sure which door you're talking about.");
				return;
			}
		case 11:
			if (valid) {
				if (text.Contains ("back") || text.Contains ("yard")) {
					OtherCommands ("close back door");
					return;
				} else if (text.Contains ("basement") || text.Contains ("cellar")) {
					OtherCommands ("close basement door");
					return;
				} else {
					break;
				}
			} else {
				AddText ("I'm still not sure which door you're talking about.");
				return;
			}
		case 12:
			if (valid) {
				if (text.Contains ("back") || text.Contains ("yard")) {
					Use ("use back door".Shlex ());
					return;
				} else if (text.Contains ("basement") || text.Contains ("cellar")) {
					Use ("use basement door".Shlex ());
					return;
				} else {
					break;
				}
			} else {
				AddText ("I'm still not sure which door you're talking about.");
				return;
			}
		case 13:
			if (valid) {
				if (text.Contains ("bedroom")) {
					Look ("look bedroom door".Shlex ());
					return;
				} else if (text.Contains ("bathroom") || text.Contains ("rest") || text.Contains ("wash") || text.Contains ("powder") || text.Contains ("lavatory")) {
					Look ("look bathroom door".Shlex ());
					return;
				} else {
					break;
				}
			} else {
				AddText ("I'm still not sure which door you're talking about.");
				return;
			}
		case 14:
			if (valid) {
				if (text.Contains ("bedroom")) {
					OtherCommands ("open bedroom door");
					return;
				} else if (text.Contains ("bathroom") || text.Contains ("rest") || text.Contains ("wash") || text.Contains ("powder") || text.Contains ("lavatory")) {
					OtherCommands ("open bathroom door");
					return;
				} else {
					break;
				}
			} else {
				AddText ("I'm still not sure which door you're talking about.");
				return;
			}
		case 15:
			if (valid) {
				if (text.Contains ("bedroom")) {
					OtherCommands ("close bedroom door");
					return;
				} else if (text.Contains ("bathroom") || text.Contains ("rest") || text.Contains ("wash") || text.Contains ("powder") || text.Contains ("lavatory")) {
					OtherCommands ("close bathroom door");
					return;
				} else {
					break;
				}
			} else {
				AddText ("I'm still not sure which door you're talking about.");
				return;
			}
		case 16:
			if (valid) {
				if (text.Contains ("bedroom")) {
					Use ("use bedroom door".Shlex());
					return;
				} else if (text.Contains ("bathroom") || text.Contains ("rest") || text.Contains ("wash") || text.Contains ("powder") || text.Contains ("lavatory")) {
					Use ("use bathroom door".Shlex());
					return;
				} else {
					break;
				}
			} else {
				AddText ("I'm still not sure which door you're talking about.");
				return;
			}
		default:
			break;
		}

		UpdateRoomState ();
	}

	void CombineItems() {
		inputLockdown = false;
		switch (currLockdownOption) {
		case 3:
			AddText ("You wrap the tarp around the rake. Hmm, this almost looks like a human-shaped dummy. If only it had a head");
			ChangeState (80, 1);
			ChangeState (82, 1);
			dummyStepsCompleted++;
			break;
		case 4:
			AddText ("You put the pinata on top of the rake and tarp. Now you just need to lure the killer into here");
			RemoveFromInv (67);
			ChangeState (102, 1);
			dummyStepsCompleted++;
			break;
		case 5:
			AddText ("You put the tape recorder into the dummy");
			RemoveFromInv (57);
			dummyStepsCompleted++;
			break;
		default:
			break;
		}

		if (dummyStepsCompleted == 3) {
			dummyAssembled = true;
		}

		UpdateRoomState ();
	}

	void ListInventory() {

		SetImage (GetImageByName ("blankoverlay"));
		AddText ("");

		string invOutput = "";
		string invImage = "";

		AddGenericInventoryText ("You look through your pockets and see the following items:");

		for (int i = 0; i < inventory.Count; ++i) {
			var obj = inventory [i];
			invOutput = obj.Name;
			invImage = "inv-" + obj.Name;

			SetInventoryImage (GetImageByName (invImage), i);
			AddInventoryText (invOutput, i);
		}

		SetGasMaskOverlay (false);
		SetOverlay (GetImageByName ("blankoverlay"));

		inventoryUp = true;
	}

    [Command]
	public void Look(List<string> argv = null)
    {
		if (argv == null) {

			if (!playerOutOfTime) {

				ResetOverlay ();

				if (killerInKitchen && currentRoom.Index == 1) {
					SetOverlay (GetImageByName ("kinjuredoverlay"));
					AddText ("It's the killer! Better get him!");
				} 
				else if (killerInKitchen && currentRoom.Index == 0) {
					//blood
					SetOverlay(GetImageByName("lrblood"));
					AddText ("you hear the killer in the kitchen");
				} 
				else if (killerInKitchen && currentRoom.Index == 2) {
					//blood
					SetOverlay(GetImageByName("hallwayblood"));
					AddText ("you hear the killer run downstairs, leaving a blood trail behind him.");
				}
				else if (killerInKitchen && currentRoom.Index == 4) {
					//blood
					SetOverlay(GetImageByName("bedroomblood"));
					AddText ("you shot him! but it's not done yet.");
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

				if (currentRoom.Index == 5 && (bearTrapMade || bucketTrapMade || fireTrapMade || shitOnStairs)) {
					HideNoItem ("basement");
					return;
				} 
				else {
					ResetItemGroup ();
					SetImage (GetImageByName (currentRoom.currentState.Image));
					UpdateRoomState ();
					SetOverlay (GetRandomDeathOverlay ());
					PlayDeathSequence ();
					AddText ("you died!");
					return;
				}
			}
		}

        else if (argv.Count == 1)
        {
			if (twoLayerLook) {
				if (currentItemGroup != null) {
					var igObj = itemsList [currentItemGroup.BaseItemIndex];
					Look (("look " + igObj.Name).Shlex ());
					twoLayerLook = false;
					AddText ("");
				}
			}
			else {
				ResetItemGroup ();
				AddText (currentRoom.currentState.Description);
				ResetOverlay ();

				if (killerInKitchen && currentRoom.Index == 1) {
					SetOverlay (GetImageByName ("kinjuredoverlay"));
				} else if (killerInKitchen && currentRoom.Index == 0) {
					SetOverlay (GetImageByName ("lrblood"));
				} else if (killerInKitchen && currentRoom.Index == 2) {
					SetOverlay (GetImageByName ("hallwayblood"));
				} else if (killerInKitchen && currentRoom.Index == 4) {
					SetOverlay (GetImageByName ("bedroomblood"));
				}

				SetImage (GetImageByName (currentRoom.currentState.Image));
				UpdateRoomState ();
				return;
			}
        }

        int itemNameStart = (argv[1] != "at") ? 1 : 2;

        string itemName = string.Join(" ", argv.Skip(itemNameStart).ToArray());

		bool invItem = false;

        Debug.LogFormat("Looking at ({0})", itemName);

		if (itemName == "inventory" || itemName == "pockets") {
			ListInventory ();
			return;
		}

		if (itemName == "room" || itemName == "around") {
			Look ("look".Shlex());
			return;
		}

		var obj = GetObjectByName (itemName);
		if (obj == null) {
			obj = GetObjectFromInv (itemName);
		}

        if (obj == null || obj.currentState.Description == "")
        {
			AddText(GenericLook());
        }
        else
		{
			if (IsInInv (obj.Index)) {
				invItem = true;
			}

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

			if (obj.Index == 17) {
				OtherCommands ("read book 1");
				return;
			}
			if (obj.Index == 18) {
				OtherCommands ("read book 2");
				return;
			}
			if (obj.Index == 19) {
				OtherCommands ("read book 3");
				return;
			}
			if (obj.Index == 20) {
				OtherCommands ("read book 4");
				return;
			}

			if (obj.Index == 111) {
				inputLockdown = true;
				currLockdownOption = 9;
			}

			if (obj.Index == 112) {
				inputLockdown = true;
				currLockdownOption = 13;
			}

			if (invItem) {
				SetImage (GetImageByName ("invbig-" + obj.Name));
				ResetOverlay ();
				SetGasMaskOverlay (false);
				return;
			}
				
            if (obj.currentState.Image != "")
            {
				ResetOverlay ();
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
		if (item == -1 && CheckDeath (itemState))
            return 1; //Return 1 for death

		if (item == -2) {
			
			AudioClip clipToPlay = GetClip (itemState);
			PlayClip (clipToPlay);

			if (itemState == 4) {
				PlayLoopingAudio (5);
			}

			if (itemState == 6) {
				stopAudio = true;
			}

			if (itemState == 7) {
				PlayLoopingAudio (8);
			}

			if (itemState == 9) {
				stopAudio = true;
			}

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

				// Stop the freezer audio loop
				if (currentItemGroup.BaseItemIndex == 26 && loopingAudioSource.clip.name == "freezeramb" && loopingAudioSource.isPlaying) {
					AudioClip clipToPlay = GetClip (6);
					stopAudio = true;
					PlayClip (clipToPlay);
				}

				// Stop the fridge audio loop
				if (currentItemGroup.BaseItemIndex == 25 && loopingAudioSource.clip.name == "fridgeamb" && loopingAudioSource.isPlaying) {
					AudioClip clipToPlay = GetClip (9);
					stopAudio = true;
					PlayClip (clipToPlay);
				}

				if (!currentItemGroup.NonResetItems.Contains (item)) {
					var obj = currentRoom.GetObjectById (item);
					obj.State = 0;
					itemsList [item].State = 0;
				}
			}

			twoLayerLook = false;
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
					if (s == 1) { //Death
						ResetOverlay ();
						SetGasMaskOverlay (false);
						break;
					}
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

		if (killerInKitchen && currentRoom.Index == 1) {
			SetOverlay (GetImageByName ("kinjuredoverlay"));
		} else if (killerInKitchen && currentRoom.Index == 0) {
			SetOverlay (GetImageByName ("lrblood"));
		} else if (killerInKitchen && currentRoom.Index == 2) {
			SetOverlay (GetImageByName ("hallwayblood"));
		} else if (killerInKitchen && currentRoom.Index == 4) {
			SetOverlay (GetImageByName ("bedroomblood"));
		} else {
			ResetOverlay ();
		}
    }

    public void ItemActions(int itemIndex)
    {
        var item = currentRoom.GetObjectById(itemIndex);

		if (item == null) {
			item = itemsList [itemIndex];
		}

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
				policeTimer++;
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

		if (pizzaTimer == pizzaCap2) {
			ChangeState (94, 4);
			ChangeState (95, 4);
		}

		if (pizzaTimer > pizzaCap2) {
			ChangeState (94, 1);
			ChangeState (95, 1);
		}

		if (policeTimer == policeCap) {
			AddText ("You hear a police siren faintly at first, but quickly growing in volumne.\n\n");
			killerCap += 3;
			ChangeState (94, 3);
			ChangeState (95, 3);
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

					if (obj == null) {
						foreach (var invItem in inventory) {
							foreach (var item in itemsList) {
								if (invItem.Name == item.Name) {
									obj = GetObjectFromInv (invItem.Name);
								}
							}
						}
					}


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
				ResetOverlay ();

				if (health <= 0)
					return;
					

				ResetItemGroup ();
				room = newRoom;

				Look (null);
				return;
			}
			else{
				switch (currentRoom.Index) {
				case 1:
					var doorObj = GetObjectByName ("back door");
					if (doorObj.State == 1) {
						AddText ("It's too dark out there. \n\n");
					} else {
						AddText ("You would need to open the door first.");
					}
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

				if (obj.Index == 3) {
					killerCap += 5;
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

		if (item.currentState.Gettable == 1) {
			inventory.Add (item);
			item.State++;
			itemsList [item.Index].State = item.State;
			foreach (KeyValuePair<int, int> actions in item.currentState.ConditionalActions.ConditionalActions) {
				if (ChangeState (actions.Key, actions.Value) == 1)
					break;
			}

			if (item.currentState.Image != "") {
				ImageCheckAndShow (item.Index, item.State, item.currentState.Image);
				ResetOverlay ();
				roomImage = false;
			}

			// Gun
			if (item.Index == 58) {
				killerInBedroom = true;
			}

			// Teddy Bear
			if (item.Index == 87) {
				if (IsInInv (74) || IsInInv (75) || IsInInv (76)) {
					killerInLair = true;
				}

			}

			if (item.Index == 74 || item.Index == 75 || item.Index == 76) {

				ImageCheckAndShow (73, 0, "checkitem");
				roomImage = false;

				if (IsInInv (87)) {
					killerInLair = true;
				}
			}

			if (twoLayerLook) {
				twoLayerLook = false;
			}

		} else {
			if (item.currentState.Get == "") {
				AddText(GenericGet());
			} else {
				AddText (item.currentState.Get);
			}

			if (item.currentState.Image != "") {
				ImageCheckAndShow (item.Index, item.State, item.currentState.Image);
				ResetOverlay ();
				roomImage = false;
			} else {
				if (image.sprite.name == currentRoom.currentState.Image) {
					roomImage = false;
				}
			}
		}

		UpdateItemGroup (item.Index);
		UpdateRoomState (roomImage);
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
			foreach (var obj in inventory) {
				if (AltNameCheck (itemName, "look") == obj.Index || itemName == obj.Name) {
					item = obj;
				}
			}

			if (item == null) {
				AddText (GenericUse ());
				return;
			}
        }

        var useResponses = specialResponses
               .Where(x => x.ItemIndex == item.Index)
               .Where(x => x.Command == "Use");

		if (useResponses.Count() == 0) {
			AddText(GenericUse ());
			UpdateItemGroup (item.Index);

			if (item.currentState.Image != "") {
				ImageCheckAndShow (item.Index, item.State, item.currentState.Image);
				ResetOverlay ();
			}

			return;
		}

		foreach (var response in useResponses) {

			if (response.ItemIndex == 66) {
				if (!bearTrapMade) {
					if (IsInInv(23) && IsInInv(34)) {
						if (playerKnowsBeartrap) {
							AddText ("you make the bear trap and put it on the stairs");
							RemoveFromInv (23);
							RemoveFromInv (34);
							ChangeState (98, 1);
							bearTrapMade = true;
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
					if ((IsInInv(15) && IsInInv(28) && IsInInv(43)) || (IsInInv(64) && IsInInv(28) && IsInInv(43))) {
						if (playerKnowsFiretrap) {
							AddText ("you make the fire trap and put it on the stairs");
							RemoveFromInv (28);
							RemoveFromInv (43);
							if (IsInInv (15))
								RemoveFromInv (15);
							if (IsInInv (64))
								RemoveFromInv (64);
							ChangeState (100, 1);
							fireTrapMade = true;
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
					if (IsInInv (31) && IsInInv (47) && IsInInv (65)) {
						if (gasMaskOn) {
							AddText ("you make the bucket trap and put it on the stairs.");
							RemoveFromInv (31);
							RemoveFromInv (47);
							RemoveFromInv (65);
							ChangeState (99, 1);
							bucketTrapMade = true;
							SetImage (GetImageByName ("buckettrap"));
							return;
						}
						else {
							ChangeState (-1, 100);
							AddText ("You choke on the smoke ya dummy! Press [ENTER] to retry. \n\n");
							SetImage (GetImageByName ("gasdeath"));
							return;
						}
					}
				}
			}
				

			if (response.ItemIndex == 57) {
				if (IsInInv(57)){
					if (currentRoom.Index == 5) {
						HideNoItem ("tape");
						return;
					} 
					else {
						if (tapeRecorderUsed) {
							if (currentRoom.Index == 8) {
								AddText ("what do you want to use it with?");
								inputLockdown = true;
								currLockdownOption = 5;
								return;
							}
							else {
								AddText ("you already recorded on it, but this isn't a good place to use it");
								UpdateRoomState (roomImage);
								UpdateItemGroup (item.Index);
								return;
							}
						}
						else {
							tapeRecorderUsed = true;
							AddText ("you record your weak little voice");
							UpdateRoomState (roomImage);
							UpdateItemGroup (item.Index);
							return;
						}
					}
				}
				else{
					
				}
			}
			if (response.ItemIndex == 80 && currentRoom.Index == 8) {
				inputLockdown = true;
				currLockdownOption = 3;
			}

			if (response.ItemIndex == 67 && currentRoom.Index == 8) {
				inputLockdown = true;
				currLockdownOption = 4;
			}

			if (response.ItemIndex == 7) {
				ResetOverlay ();
				if (IsInInv (15) || IsInInv(64)) {
					if (!IsInInv (43)) {
						AddText ("Eh, you don't have anything to light it with");
						ImageCheckAndShow (response.ItemIndex, response.ItemState, "showitem");
						return;
					}
				} 
				else {
					if (IsInInv (43)) {
						AddText ("You need something to light first!");
						ImageCheckAndShow (response.ItemIndex, response.ItemState, "showitem");
						return;
					} 
					else {
						AddText ("You'd need something flamable and something to light it.");
						ImageCheckAndShow (response.ItemIndex, response.ItemState, "showitem");
						return;
					}
				}
			}

			if (response.ItemIndex == 41) {
				if (!gasMaskOn) {
					if (!IsInInv (41)) {
						List<string> tokens = new List<string> ();
						tokens.Add ("get");
						tokens.Add ("gas");
						tokens.Add ("mask");
						Get (tokens);
						AddText ("You pick up the gas mask and put it on");
						UpdateRoomState ();
						//inventory.Add (item);
					} else {
						AddText ("You take the mask out of your pocket and put it on");
					}
					SetGasMaskOverlay (true);
					gasMaskOn = true;
					return;
				} 
				else {
					AddText ("You take the gas mask off");
					SetGasMaskOverlay (false);
					gasMaskOn = false;
					return;
				}
			}

			if (response.ItemIndex == 9) {
				Move ("move hallway".Shlex());
				return;
			}
			if (response.ItemIndex == 59) {
				Move ("move basement".Shlex());
				return;
			}
			if (response.ItemIndex == 38) {
				Move ("move living room".Shlex());
				return;
			}
			if (response.ItemIndex == 61) {
				Move ("move kitchen".Shlex());
				return;
			}

			if (response.ItemIndex == 2) {
				OtherCommands ("open window");
				return;
			}

			if (response.ItemIndex == 16) {
				if (item.State == 0){
					OtherCommands ("open drawer");
				}
				else{
					OtherCommands ("close drawer");
				}
				return;
			}

			if (response.ItemIndex == 24) {
				Move ("move backyard".Shlex());
				return;
			}

			if (response.ItemIndex == 37) {
				Move ("move basement".Shlex());
				return;
			}

			if (response.ItemIndex == 39) {
				Move ("move bedroom".Shlex());
				return;
			}

			if (response.ItemIndex == 40) {
				Move ("move bathroom".Shlex());
				return;
			}

			if (response.ItemIndex == 42) {
				Move ("move hallway".Shlex());
				return;
			}

			if (response.ItemIndex == 53) {
				Move ("move hallway".Shlex());
				return;
			}

			if (response.ItemIndex == 17) {
				OtherCommands ("read book 1");
				return;
			}
			if (response.ItemIndex == 18) {
				OtherCommands ("read book 2");
				return;
			}
			if (response.ItemIndex == 19) {
				OtherCommands ("read book 3");
				return;
			}
			if (response.ItemIndex == 20) {
				OtherCommands ("read book 4");
				return;
			}

			if (response.ItemIndex == 1) {
				inputLockdown = true;
				currLockdownOption = 7;
			}

			if (response.ItemIndex == 8) {
				OtherCommands ("open front door");
				return;
			}

			if (response.ItemIndex == 6) {
				inputLockdown = true;
				currLockdownOption = 8;
			}

			if (response.ItemIndex == 111) {
				inputLockdown = true;
				currLockdownOption = 12;
			}

			if (response.ItemIndex == 112) {
				inputLockdown = true;
				currLockdownOption = 16;
			}

			AddText (response.Response);

			foreach (KeyValuePair<int, int> actions in response.Actions) {
				if (ChangeState (actions.Key, actions.Value) == 1)
					break;
			}

			if (response.Image != "") {
				ImageCheckAndShow (response.ItemIndex, response.ItemState, response.Image);
				ResetOverlay ();
				roomImage = false;
			}

			UpdateItemGroup (item.Index);
			UpdateRoomState (roomImage);
			return;
		}
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
		case "back":
			if (!twoLayerLook) {
				AddText ("You step back");
				Look (null);
			} else {
				if (currentItemGroup != null) {
					var obj = itemsList [currentItemGroup.BaseItemIndex];
					Look (("look " + obj.Name).Shlex ());
					twoLayerLook = false;
					AddText ("");
				}
			}
			break;
		case "shit":
			command = "Shit";
			break;
		case "drink":
			command = "Drink";
			break;
		case "reset":
			ResetHouse ();
			UpdateRoomState ();
			return;
		case "inventory":
			ListInventory ();
			return;
		case "put":
		case "take":
		case "equip":
		case "wear":
			var tempTokens = itemName.Shlex ();

			for (int i = 0; i < tempTokens.Count; ++i) {
				if (tempTokens [i] == "on" || tempTokens [i] == "off") {
					tempTokens.Remove (tempTokens [i]);
				}
			}

			itemName = "use";

			for (int i = 0; i < tempTokens.Count; ++i) {
				itemName += " " + tempTokens [i];
			}

			var useItem = GetObjectByName (itemName);
			if (useItem == null) {
				useItem = GetObjectFromInv (itemName);
			}
			var tokens = itemName.Shlex ();
			Use (tokens);
			return;
		case "scream":
		case "yell":
			Scream ();
			break;
		case "turn":
			Turn (itemName);
			return;
		case "unplug":
			Turn (("off ") + itemName);
			return;
		default:
			AddText ("I don't know how to do that");
            return;
        }
        var item = GetObjectByName(itemName);
		if (item == null) {
			item = GetObjectFromInv (itemName);
			if (item == null) return;
		}
			
        for (int j = 0; j < specialResponses.Count; ++j)
        {
            object[] parameters = new object[2];
            parameters[0] = item.Index;
            parameters[1] = j;
			MethodInfo mInfo = typeof(HouseManager).GetMethod(command);
            mInfo.Invoke(this, parameters);
        }
    }

	public void Turn(string text){
		var turnText = text.Shlex ();
		var objName = text.Split (new char[] { ' ' }, 2) [1].ToLower ();

		var obj = GetObjectByName (text.Split (new char[] { ' ' }, 2) [1].ToLower ());
		bool roomImage = false;
		if (obj != null) {
			for (int j = 0; j < specialResponses.Count; ++j) {
				if (specialResponses [j].Command == "Turn On" && specialResponses [j].ItemIndex == obj.Index && specialResponses [j].ItemState == obj.State && turnText [0] == "on") {
					AddText (specialResponses [j].Response);

					foreach (KeyValuePair<int, int> actions in specialResponses[j].Actions) {
						if (ChangeState (actions.Key, actions.Value) == 1)
							break;
					}

					if (specialResponses [j].Image != "") {
						ImageCheckAndShow (obj.Index, obj.State, specialResponses [j].Image);
						roomImage = false;
					}

					UpdateItemGroup (obj.Index);
					UpdateRoomState (roomImage);

					return;
				} else if (specialResponses [j].Command == "Turn Off" && specialResponses [j].ItemIndex == obj.Index && specialResponses [j].ItemState == obj.State && turnText [0] == "off") {
					AddText (specialResponses [j].Response);

					foreach (KeyValuePair<int, int> actions in specialResponses[j].Actions) {
						if (ChangeState (actions.Key, actions.Value) == 1)
							break;
					}

					if (specialResponses [j].Image != "") {
						ImageCheckAndShow (obj.Index, obj.State, specialResponses [j].Image);
						roomImage = false;
					}

					UpdateItemGroup (obj.Index);
					UpdateRoomState (roomImage);

					return;
				} 
				else {
					if (turnText [0] == "on") {
						AddText ("I don't think I can turn that on.");
					}
					else if (turnText [0] == "off") {
						AddText ("I don't think that's something I can turn off.");	
					}
				}
			}
		} 
		else {
			if (turnText [0] == "on") {
				AddText ("I can't turn on what I can't see.");
			}
			else if (turnText [0] == "off") {
				AddText ("I can't turn off what I can't see.");	
			}
			else if (turnText [0] == "around" || turnText [0] == "back") {
				if (!twoLayerLook) {
					AddText ("You step back");
					Look (null);
				} else {
					if (currentItemGroup != null) {
						var subObj = itemsList [currentItemGroup.BaseItemIndex];
						Look (("look " + subObj.Name).Shlex ());
						twoLayerLook = false;
						AddText ("");
					}
				}
			}
			else {
				
			}
		}
	}

	public void Scream(){

		if (currentRoom.Index == 5) {
			HideNoItem ("scream");
		} 
		else {
			List<string> imageList = deathImages [5];
			string imageName = imageList [UnityEngine.Random.Range (0, imageList.Count)];
			AddText ("You yell at the top of your lungs and the killer gets ya.\n\nPress [ENTER] to restart.");
			SetImage (GetImageByName (imageName));
			health = 0;
			return;
		}
	}

	public void Help()
	{
		/*SetImage (GetImageByName ("blankoverlay"));
		AddText ("");

		string invOutput = "";
		string invImage = "";

		AddGenericInventoryText ("You look through your pockets and see the following items:");

		for (int i = 0; i < inventory.Count; ++i) {
			var obj = inventory [i];
			invOutput = obj.Name;
			invImage = "inv-" + obj.Name;

			SetInventoryImage (GetImageByName (invImage), i);
			AddInventoryText (invOutput, i);
		}

		SetGasMaskOverlay (false);
		SetOverlay (GetImageByName ("blankoverlay"));

		inventoryUp = true;*/

		SetImage (GetImageByName ("blankoverlay"));
		AddText ("");

		SetGasMaskOverlay (false);
		SetOverlay (GetImageByName ("blankoverlay"));
		healthScreenUp = true;

		AddHelpText ("HELLO AND WELCOME TO THE HELP SCREEN\n\nIn this game, you interact with the world around you by typing commands. The Four Basic Commands are LOOK, GET, USE, and MOVE. However you’ll want to try other commands to see what works–or you won’t survive. For example, if you wanted to move up the stairs, you might say MOVE HALLWAY or USE STAIRCASE.\n\nThe goal is to survive. There are multiple ways to fight, escape, or outwit the killer, but it may take some trial and error.\n\nItems that you pick up will be stored in your inventory, which you can see by typing INVENTORY. Which is nice.");
	}

	public void HideNoItem(string source = "")
	{
		if (currentRoom.Index == 5) {
			ImageCheckAndShow (61, 0, "checkitem");

			if (source == "scream") {
				AddText ("YouSCREEEAAAAM !! I'M WILD DOWOODYWD !! !@JKL You hear him start to come down the stairs. Press [ENTER]");
			} else if (source == "tape") {
				if (tapeRecorderUsed) {
					AddText ("You lay down the tape recorder and do a hidelydoo. You hear da bad boy start to come down the stairs. Press [ENTER]");
				} else {
					AddText ("YouSCREEEAAAAM !! I'M WILD DOWOODYWD into the tape recorder. You hear him start to come down the stairs. Press [ENTER]");
				}
			}
			else if (source == "basement") {
				AddText ("As you descend the stairs, you hear the killer closely behind you. You hear da bad boy start to come down the stairs. Press [ENTER]");
			}
			else {
				AddText ("You hide and wait for the killer to show up. You hear him start to come down the stairs. Press [ENTER]");
			}
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

		if ((currentRoom.Index == 7 || currentRoom.Index == 8) && dummyAssembled) {
			killerInShack = true;
			SetImage (GetImageByName ("outside2"));

			List<string> imageList = deathOverlays [9];
			string imageName = imageList[UnityEngine.Random.Range(0, imageList.Count)];
			currOverlay = imageName;
			SetOverlay(GetImageByName(imageName));
			AddText ("you hide, looks like the killer's INSPECTING");

			currLockdownOption = 6;
			inputLockdown = true;
		}
	}

	public void Drink(int i, int j){
		var item = itemsList [i];
		bool roomImage = true;
		if (specialResponses [j].Command == "Drink" && specialResponses [j].ItemIndex == item.Index && (specialResponses [j].ItemState == item.State || IsInInv(i))) {
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

	public void Shit(int i, int j)
	{
		var item = itemsList [i];
		bool roomImage = true;
		if (specialResponses [j].Command == "Shit" && specialResponses [j].ItemIndex == item.Index && specialResponses [j].ItemState == item.State) {
			AddText(specialResponses[j].Response);

			if (item.Index == 61) {
				shitOnStairs = true;
			}

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

			if (item.Index == 20) {
				multiSequence = true;
				currMultiSequence = 27;
				AddText("“First, don’t take your mind off the situation, especially by reading a How To manual.” Wow, that’s really good advice. Maybe the silhouette behind you would appreciate that advice. Wait a minute... You don’t know any silhouettes!\n\nPress [ENTER] to continue.");
				SetImage (GetImageByName ("bookdeath"));
				ResetOverlay ();
				SetGasMaskOverlay (false);
				PlayClip (GetClip (2));
				return;
			}

            AddText(specialResponses[j].Response);

            foreach (KeyValuePair<int, int> actions in specialResponses[j].Actions)
            {
				if (ChangeState (actions.Key, actions.Value) == 1) {
					ResetOverlay ();
					SetGasMaskOverlay (false);
					break;
				}
                    
            }

			if (specialResponses [j].Image != "") {
				ImageCheckAndShow (item.Index, item.State, specialResponses [j].Image);
				roomImage = false;
			}

			if (!killerInKitchen) {
				ResetOverlay ();
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
			ResetOverlay ();

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
			int startState = item.State;
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

			if (item.Index == 8 && policeTimer >= policeCap) {
				AddText ("Oh no, that's not a cop!");
				SetImage (GetRandomDeathImage());
				health = 0;
				return;
			}

			if (item.Index == 8 && killerInKitchen) {
				AddText ("you make it as far as the front porch before the killer gets ya\n\nPress [ENTER] to restart.");
				SetImage(GetImageByName("porchdeath"));
				health = 0;
				return;
			}

			if (item.Index == 2) {
				ResetOverlay ();
				SetGasMaskOverlay (false);
				if (killerInKitchen) {
					AddText ("you make it as far as the front porch before the killer gets ya\n\nPress [ENTER] to restart.");
					SetImage(GetImageByName("porchdeath"));
				} else {
					if (policeTimer >= policeCap) {
						SetImage (GetImageByName ("windowdeath2"));
						AddText ("You open the window and attempt to climb out, BUT THE KILLER GETS YA!\n\nPress [ENTER] to restart.");
					} else if (pizzaTimer >= pizzaCap && pizzaTimer <= pizzaCap2) {
						SetImage (GetImageByName ("windowdeath3"));
						AddText ("the pizza man tries to help you climb through the window, BUT THE KILLER GETS YA!\n\nPress [ENTER] to restart.");
					}
					else {
						SetImage (GetImageByName ("windowdeath"));
						AddText ("You open the window and attempt to climb out, BUT THE KILLER GETS YA!\n\nPress [ENTER] to restart.");
					}

				}


				health = 0;
				return;
			}

			if (item.Index == 111) {
				inputLockdown = true;
				currLockdownOption = 10;
			}

			if (item.Index == 112) {
				inputLockdown = true;
				currLockdownOption = 14;
			}

            AddText(specialResponses[j].Response);

            int state = item.State;
            foreach (KeyValuePair<int, int> actions in specialResponses[j].Actions)
            {
				if (ChangeState (actions.Key, actions.Value, 1) == 1) {
					ResetOverlay ();
					SetGasMaskOverlay (false);
					break;
				}
                   
            }

			if (health > 0 && !inputLockdown && startState != item.State) AddAdditionalText ("\n\n" + item.currentState.Description);

            if (specialResponses[j].Image != "")
            {
				ResetOverlay ();
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

			if (item.Index == 111) {
				inputLockdown = true;
				currLockdownOption = 11;
			}

			if (item.Index == 112) {
				inputLockdown = true;
				currLockdownOption = 15;
			}

            int state = item.State;
            foreach (KeyValuePair<int, int> actions in specialResponses[j].Actions)
            {
				if (ChangeState (actions.Key, actions.Value, 1) == 1) {
					ResetOverlay ();
					SetGasMaskOverlay (false);
					break;
				}
            }

            if (specialResponses[j].Image != "")
            {
				ResetOverlay ();
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
        responses.Add("Lookin' good.");
		responses.Add("Look, cookbook, gobbledegook.");

        return responses[UnityEngine.Random.Range(0, responses.Count)];
    }

    public string GenericGet()
    {
        List<string> responses = new List<string>();
        responses.Add("Nah.");
		responses.Add("No. Your mother never hugged you.");
		responses.Add("Can't.");
        return responses[UnityEngine.Random.Range(0, responses.Count)];
    }

    public string GenericUse()
    {
        List<string> responses = new List<string>();
		responses.Add("Reduce, reuse, no.");
		responses.Add("You can’t use that.");

        return responses[UnityEngine.Random.Range(0, responses.Count)];
    }
}