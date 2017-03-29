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
    int killerCap, killerCap2, killerCap3, killerCap4;
	int pizzaCap, pizzaCap2;
	int policeCap;
	int dummyStepsCompleted;
	bool dummyAssembled;
	bool audioLooping, stopAudio;
	bool gasMaskOn;
	bool inventoryUp;
	bool twoLayerLook;
	bool helpScreenUp, doubleHelp;
	bool playerHiding;
	bool bloodSeen2, bloodSeen3;
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
    string textWaiting;
	List<AudioClip> toPlaySoundFX;
	List<AudioClip> soundFXQueue;
	int toPlayAmb;
	int soundFXTimer;
	bool lockMask;

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
	public AudioSource musicTrack;
	public AudioSource ambientSource;

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
		inventoryTextboxes = new List<GradualTextRevealer> { invText0, invText1, invText2, invText3, invText4, invText5, invText6, invText7, invText8, invText9, invText10, invText11, invText12, invText13, invText14, invText15, invText16, invText17, invText18, invText19 };
		inventoryImages = new List<Image> { inv0, inv1, inv2, inv3, inv4, inv5, inv6, inv7, inv8, inv9, inv10, inv11, inv12, inv13, inv14, inv15, inv16, inv17, inv18, inv19  };
		// Set up various variables
		room = 0;
		inventory = new List<GameObject>();
		inventory.Add (GetObjectByName ("map"));
        health = 100;
		killerTimer = 0;
		pizzaTimer = -1;
		policeTimer = -1;
        killerCap = 1;
        killerCap2 = 5;
        killerCap3 = 10;
        killerCap4 = 15;
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
		helpScreenUp = doubleHelp = false;
		playerHiding = false;
		killerInBedroom = killerInKitchen = killerInLair = killerInShack = false;
		currOverlay = "";
        textWaiting = "";
		bloodSeen2 = bloodSeen3 = false;
		toPlaySoundFX = new List<AudioClip> ();
		soundFXQueue = new List<AudioClip> ();
		toPlayAmb = 0;
		soundFXTimer = UnityEngine.Random.Range(600, 900);
		lockMask = false;
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
		PlayMusicTrack (GetClip (13));
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

		dict.Add (0, new List<string> { "use [0]", "get [0]", "stab [killer]", "use [0] on [killer]", "stab [killer] with [0]",
			"pistol whip [killer]", "hit [killer] with [1]", "attack [killer] with [1]", "smack [killer] with [1]" });
		dict.Add (1, new List<string> { "use [item]" , "shoot [item]", "fire [item]", "use [item] on [killer]", "fire [item] at [killer]", "shoot [item] at [killer]", "shoot [killer]", "shoot at [killer]" });
		dict.Add (2, new List<string> { "threaten [0]", 
		"use [1] with [0]", "use [1] on [0]", "use [1]",
			"use [2] with [0]", "use [2] on [0]", "use [2]",
			"use [3] with [0]", "use [3] on [0]", "use [3]",
			"use [4] with [0]", "use [4] on [0]", "use [4]",
			"use [5] with [0]", "use [5] on [0]", "use [5]"});
		dict.Add (3, new List<string> { "[0]", "with [0]", "use with [0]", "use [1] with [0]", "combine [0] and [1]", "combine [1] and [0]" });
		dict.Add (4, new List<string> { "[0]" , "with [0]", "use with [0]", "[1]", "with [1]", "use with [1]", "[2]", "with [2]", "use with [2]", "[1] and [2]", "use with [1] and [2]", "with [1] and [2]" });
		dict.Add (5, new List<string> { "[0]" , "with [0]", "use with [0]", "[1]", "with [1]", "use with [1]", "[2]", "with [2]", "use with [2]", "[1] and [2]", "use with [1] and [2]", "with [1] and [2]" });
		dict.Add (6, new List<string> { "lock [0]", "bolt [0]", "latch [0]", "close [0]", "shut [0]", "use [1]",
		"lock door", "bolt door", "latch door", "close door", "shut door"});
		dict.Add (7, new List<string> { "[0]", "[1]", "[2]", "[3]", "[4]", "call [0]", "call [1]", "call [2]", "call [3]", "call [4]", "dial [0]", "dial [1]", "dial [2]", "dial [3]", "dial [4]" });
		dict.Add (8, new List<string> { "1", "2", "3", "4", "666", "[0]", "[1]", "[2]", "[3]", "[4]", "read [0]", "read [1]", "read [2]", "read [3]", "read [5]", "look [0]", "look [1]", "look [2]", "look [3]", "look [4]"
			,"read 1", "read 2", "read 3", "read 4", "read 666", "look 1", "look 2", "look 3", "look 4", "look 666", "use 1", "use 2", "use 3", "use 4", "use 666", "use [0]", "use [1]", "use [2]", "use [3]", "use [4]"});
		dict.Add (9, new List<string> { "[0]", "[1]", "basement", "back", "look [0]", "look [1]" });
		dict.Add (10, new List<string> { "[0]", "[1]", "basement", "back", "open [0]", "open [1]" });
		dict.Add (11, new List<string> { "[0]", "[1]", "basement", "back", "close [0]", "close [1]" });
		dict.Add (12, new List<string> { "[0]", "[1]", "basement", "back", "use [0]", "use [1]" });
		dict.Add (13, new List<string> { "[0]", "[1]", "bedroom", "bathroom", "look [0]", "look [1]" });
		dict.Add (14, new List<string> { "[0]", "[1]", "bedroom", "bathroom", "open [0]", "open [1]" });
		dict.Add (15, new List<string> { "[0]", "[1]", "bedroom", "bathroom", "close [0]", "close [1]" });
		dict.Add (16, new List<string> { "[0]", "[1]", "bedroom", "bathroom", "use [0]", "use [1]" });
		dict.Add (17, new List<string> { "[0]", "with [0]", "use with [0]", "use [1] with [0]", "combine [0] and [1]", "combine [1] and [0]" });
		dict.Add (18, new List<string> { "[0]", "[1]", "shed", "back", "look [0]", "look [1]" });
		dict.Add (19, new List<string> { "[0]", "[1]", "shed", "back", "open [0]", "open [1]" });
		dict.Add (20, new List<string> { "[0]", "[1]", "shed", "back", "close [0]", "close [1]" });
		dict.Add (21, new List<string> { "[0]", "[1]", "shed", "back", "use [0]", "use [1]" });
		dict.Add (22, new List<string> { "lock [0]", "bolt [0]", "latch [0]", "use [1]",
			"lock door", "bolt door", "latch door"});
		dict.Add (23, new List<string> { "[0]", "on [0]", "use [1] on [0]", "use [1] with [0]", "with [0]", "use with [0]", "use it with [0]" });
		dict.Add (24, new List<string> { "[0]", "on [0]", "use [1] on [0]", "use [2] with [0]", "with [0]", "use with [0]", "use it with [0]" });
		dict.Add (25, new List<string> { "[0]", "on [0]", "use [1] on [0]", "use [3] with [0]", "with [0]", "use with [0]", "use it with [0]" });
		dict.Add (26, new List<string> { "[0]", "on [0]", "use [1] on [0]", "use [4] with [0]", "with [0]", "use with [0]", "use it with [0]" });
		dict.Add (27, new List<string> { "[0]", "on [0]", "use [1] on [0]", "use [5] with [0]", "with [0]", "use with [0]", "use it with [0]" });


		for (int i = 0; i < dict.Count; ++i) {
			string itemName = "";

			List<string> items = new List<string>();

			switch (i) {
			case 0:
				items.Add ("knife");
				items.Add ("revolver");
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
				items.Add ("rake");
				items.Add ("tarp");
				break;
			case 4:
				items.Add ("dummy");
				items.Add ("tarp");
				items.Add ("rake");
				break;
			case 5:
				items.Add ("dummy");
				items.Add ("tarp");
				items.Add ("rake");
				break;
			case 6:
				items.Add ("shed door");
				items.Add ("lock");
				break;
			case 7:
				items.Add ("speed dial 1");
				items.Add ("speed dial 2");
				items.Add ("speed dial 3");
				items.Add ("speed dial 4");
				items.Add ("speed dial 666");
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
			case 17:
				items.Add ("tarp");
				items.Add ("rake");
				break;
			case 18:
				items.Add ("back door");
				items.Add ("shed door");
				break;
			case 19:
				items.Add ("back door");
				items.Add ("shed door");
				break;
			case 20:
				items.Add ("back door");
				items.Add ("shed door");
				break;
			case 21:
				items.Add ("back door");
				items.Add ("shed door");
				break;
			case 22:
				items.Add ("shed door");
				items.Add ("lock");
				break;
			case 23:
				items.Add ("teddy bear");
				items.Add ("scalpel");
				break;
			case 24:
				items.Add ("teddy bear");
				items.Add ("spoon");
				break;
			case 25:
				items.Add ("teddy bear");
				items.Add ("scissors");
				break;
			case 26:
				items.Add ("teddy bear");
				items.Add ("knife");
				break;
			case 27:
				items.Add ("teddy bear");
				items.Add ("blender");
				break;
			default:
				break;
			}

			List<List<string>> dictAlts = new List<List<string>>();
			SortedDictionary <int, List<string>> dictAltsDict = new SortedDictionary<int, List<string>> ();
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
							dictAltsDict.Add (j, altNamesToGet);
						}
					}
				}
			}
				
			foreach (var dictItem in dictAltsDict) {
				dictAlts.Add (dictItem.Value);
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

	public string ScrubInput(string toScrub, string whatToScrub){
		if (whatToScrub.Contains (" " + toScrub))
			return (whatToScrub.Replace (" " + toScrub, ""));
		else if (whatToScrub.Contains (toScrub + " "))
			return (whatToScrub.Replace (toScrub + " ", ""));
		else
			return whatToScrub;
	}
		    
    public void ReadInput(string text)
    {
		if (inventoryUp) {
			ResetInventory ();

			if (gasMaskOn) {
				SetGasMaskOverlay (true);
			}

			SetOverlay (GetImageByName (currOverlay));
			SetImage (GetImageByName(currentRoom.currentState.Image));
			inventoryUp = false;
		}

		if (helpScreenUp) {

			if (text == "help") {
				//AddHelpText ("");
				if (!doubleHelp) {
					AddAdditionalHelpText ("\n\nThis is all the help you're gonna get, BUB");
					doubleHelp = true;
				}

				return;
			} 
			else {
				AddHelpText ("");

				if (gasMaskOn) {
					SetGasMaskOverlay (true);
				}

				SetImage (GetImageByName(currentRoom.currentState.Image));

				SetOverlay (GetImageByName (currOverlay));
				doubleHelp = false;
				helpScreenUp = false;
			}
		}

		if (!multiSequence) {
			if (killerInBedroom) {
				if (!playerBedroomShot) {
					SetImage (GetImageByName (currentRoom.currentState.Image));
					SetOverlay (GetRandomDeathOverlay ());
					AddText ("oh no, the killer's in your bedroom!");
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
					text = ScrubInput ("the", text);
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
				text = ScrubInput ("the", text);
				foreach (var option in options) {
					if (text == option) {
						KitchenStab (text);
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
					AddText ("Suddenly, you hear a loud grinding and sliding, which startles you into hyperarousal. The false panel of the fireplace has moved. You back away towards the other exit, as, to your horror, you see the the feet of the clean suit begin to descend the stairs into the room, followed by the rest of the murderer. What do you do?");
					playerLairThreaten = true;
					currLockdownOption = 2;
					inputLockdown = true;
				} else {
					var options = lockdownOptions [currLockdownOption];
					text = ScrubInput ("the", text);
					foreach (var option in options) {
						if (text == option) {

							if (text.Contains ("scalpel")) {
								if (text == "use scalpel") {
									currLockdownOption = 23;
									AddText ("What do you want to use it with?");
								}
								else {
									LairThreaten ("scalpel");
								}
							} else if (text.Contains ("spoon")) {
								if (text == "use spoon") {
									currLockdownOption = 24;
									AddText ("What do you want to use it with?");
								}
								else {
									LairThreaten ("spoon");
								}
							} else if (text.Contains ("scissors")) {
								if (text == "use scissors") {
									currLockdownOption = 25;
									AddText ("What do you want to use it with?");
								}
								else {
									LairThreaten ("scissors");
								}
							} else if (text.Contains ("blender")) {
								if (text == "use blender") {
									currLockdownOption = 26;
									AddText ("What do you want to use it with?");
								}
								else {
									LairThreaten ("blender");
								}
							} else if (text.Contains ("knife")) {
								if (text == "use knife") {
									currLockdownOption = 27;
									AddText ("What do you want to use it with?");
								}
								else {
									LairThreaten ("knife");
								}
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
				text = ScrubInput ("the", text);
				foreach (var option in options) {
					if (text == option) {
						ShackLock (text);
						return;
					}
				}
					
				// Player closed door, didn't lock
				if (currLockdownOption == 22) {
					SetImage (GetImageByName ("shedloss"));
					AddText ("You died. Maybe you could have done more than JUST closing the door. Press [ENTER] to continue!");

				} 
				else {
					string weapon = currOverlay.Split ('-').Last ();
					var overlays = deathImages [currentRoom.Index];
					if (weapon == "knife") {
						SetImage (GetImageByName (overlays.ElementAt (UnityEngine.Random.Range (0, 2))));
					} 
					else {
						SetImage (GetImageByName (overlays.ElementAt (UnityEngine.Random.Range (2, 4))));
					}
					AddText ("You died. Maybe you could have LOCKED THE DOOR YA DINGU. Press [ENTER] to continue!");
				}

				ResetOverlay ();
				SetGasMaskOverlay (false);
				health = 0;
				killerInShack = false;
			}
			else if (playerHiding) {
                Look(null);
                UpdateTimers();
                AddText("You emerge from the closet like a beautiful butterfly emerging from a coccon.\n\n");
                AddAdditionalText(textWaiting);
                textWaiting = "";
                playerHiding = false;
                return;
			}
			else {
				if (health > 0) {
					if (inputLockdown) {
						var options = lockdownOptions [currLockdownOption];
						text = ScrubInput ("the", text);
						foreach (var option in options) {
							if (text == option) {

								if (currLockdownOption == 7 || currLockdownOption == 8 || currLockdownOption == 9 || currLockdownOption == 10
									|| currLockdownOption == 11 || currLockdownOption == 12 || currLockdownOption == 13 || currLockdownOption == 14
									|| currLockdownOption == 15 || currLockdownOption == 16 || currLockdownOption == 18  || currLockdownOption == 19
									|| currLockdownOption == 20 || currLockdownOption == 21) {
									LockdownResponse (text, true);
									return;
								}

								CombineItems ();
								return;
							}
						}
						if (currLockdownOption == 7 || currLockdownOption == 8 || currLockdownOption == 9 || currLockdownOption == 10
							|| currLockdownOption == 11 || currLockdownOption == 12 || currLockdownOption == 13 || currLockdownOption == 14
							|| currLockdownOption == 15 || currLockdownOption == 16 || currLockdownOption == 18  || currLockdownOption == 19
							|| currLockdownOption == 20 || currLockdownOption == 21) {
							LockdownResponse (text, false);
							return;
						}
					} 
					else {
						if (text != "") {	
						
							if (gasMaskOn && !lockMask) {
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
		if (!audioSource.isPlaying && soundFXQueue.Count != 0) {
			PlayClip(soundFXQueue[0]);
			if (soundFXQueue[0].name == "freezerclose" || soundFXQueue[0].name == "fridgeclose" && stopAudio){
				StopLoopingAudio();
				stopAudio = false;
			}

			if (soundFXQueue[0].name == "freezeropen" || soundFXQueue[0].name == "fridgeopen" && toPlayAmb != 0) {
				PlayLoopingAudio (toPlayAmb);
				stopAudio = false;
				toPlayAmb = 0;
			}

			soundFXQueue.Remove(soundFXQueue[0]);
		}

		soundFXTimer -= 1;

		if (soundFXTimer <= 0) {

			int playSound = UnityEngine.Random.Range(0, 3);

			if (playSound >= 1) {
				PlayAmbientClip (GetClip (GetAmbientNoise ()));
			}

			soundFXTimer = UnityEngine.Random.Range(600, 900);
		}
	}

    public void ResetHouse()
    {
        SetupHouse();
		AddText (GetResetText ());
		AddAdditionalText ("You recline in your easy chair. It is late and your living room is lit only by harsh, fluorescent light from the lamp behind you. There is a slight draft in the room that chills you, and the thought of your warm bed begins to form in your mind. Suddenly, there is a series of loud knocks at the front door, feet from you, causing you to jolt. As your heart races, you think, 'Who could that be?' You suppose you had better take a look.");
    }

    public void AddText(string txt)
    {
        //text.StartReveal("\n" + txt + "\n");

		txt = txt.Replace ("[NEWLINE]", "\r");
		txt = txt.Replace ("[DOUBLENEWLINE]", "\n\n");
		text.AppendText(txt);
    }

	public void AddAdditionalText(string txt)
	{
		txt = txt.Replace ("[NEWLINE]", "\r");
		txt = txt.Replace ("[DOUBLENEWLINE]", "\n\n");
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
		}
		else {
			SetBasementOverlay (4, false);
		}

		if (currentRoom.Index == 0 && killerInKitchen && currentRoom.currentState.Image == tex.name) {
			SetOverlay(GetImageByName("lrblood"));
		}
		if (currentRoom.Index == 2 && killerInKitchen && currentRoom.currentState.Image == tex.name) {
			SetOverlay(GetImageByName("hallwayblood"));
		}
		if (currentRoom.Index == 4 && killerInKitchen && currentRoom.currentState.Image == tex.name) {
			SetOverlay(GetImageByName("bedroomblood"));
		}

		if (tex.name == "orangejuice" || tex.name == "icecubetrays" || tex.name == "flashlight" || tex.name == "bleach" || tex.name == "bucket") {
			twoLayerLook = true;
		} else {
			twoLayerLook = false;
		}

		if (image.sprite.name.Contains ("invbig")) {
			if (!tex.name.Contains ("invbig")) {
				lockMask = false;

				if (gasMaskOn && !inventoryUp && !helpScreenUp) {
					SetGasMaskOverlay (true);
				}
			}
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

	void AddAdditionalHelpText(string txt){
		helpText.AddAdditionalText (txt);
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

		if (currentRoom.Index == 3) {
			multiSequence = true;
			currMultiSequence = 28;
			return;
		}

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

	void KitchenStab (string text) {

		if (text.Contains ("knife")) {
			SetImage (GetImageByName ("knifeaction"));
			AddText ("You stab the killer and kill him. WOO");
		} else {
			SetImage (GetImageByName ("whipaction"));
			AddText ("You whip your pistol and kill him. WOO");
		}


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
            AddText("You threaten the bear with a spoon??? YOU DIE\n\nPress [ENTER] to restart!");
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

	void ShackLock(string text) {

		if (text.Contains ("close") || text.Contains ("shut")) {
			SetImage (GetImageByName ("lock2"));
			AddText ("You shut the door...but that won't be enough to hold back this maniac!");
			ResetOverlay ();
			currLockdownOption = 22;
		}

		else {

			SetImage (GetImageByName ("shackwin"));
			AddText ("You lock the door and win. WOO");
			killerInShack = false;
			ResetOverlay ();
			SetGasMaskOverlay (false);

			// WIN
			health = 0;
		}
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
				}
				else if (text.Contains ("666")) {
					OtherCommands ("call 666");
					return;
				}
				else {
					break;
				}
			} else {
				AddText ("Hm, you don’t seem to have the number written down for that.");
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
				}
				else if (text.Contains ("666")) {
					OtherCommands ("read book 666");
					return;
				}
				else {
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
		case 18:
			if (valid) {
				if (text.Contains ("back")) {
					Look ("look back door".Shlex());
					return;
				} else if (text.Contains ("shed") || text.Contains ("shack")) {
					Look ("look shed door".Shlex());
					return;
				} else {
					break;
				}
			} else {
				AddText ("I'm still not sure which door you're talking about.");
				return;
			}
		case 19:
			if (valid) {
				if (text.Contains ("back")) {
					OtherCommands ("open back door");
					return;
				} else if (text.Contains ("shed") || text.Contains ("shack")) {
					OtherCommands ("open shed door");
					return;
				} else {
					break;
				}
			} else {
				AddText ("I'm still not sure which door you're talking about.");
				return;
			}
		case 20:
			if (valid) {
				if (text.Contains ("back")) {
					OtherCommands ("close back door");
					return;
				} else if (text.Contains ("shed") || text.Contains ("shack")) {
					OtherCommands ("close shed door");
					return;
				} else {
					break;
				}
			} else {
				AddText ("I'm still not sure which door you're talking about.");
				return;
			}
		case 21:
			if (valid) {
				if (text.Contains ("back")) {
					Use ("use back door".Shlex());
					return;
				} else if (text.Contains ("shed") || text.Contains ("shack")) {
					Use ("use shed door".Shlex());
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
		bool updateRoom = true;
		switch (currLockdownOption) {
		case 3:
			AddText ("You wrap the tarp around the rake. Hmm, this almost looks like a human-shaped dummy. If only it had a head");
			ChangeState (80, 1);
			ChangeState (82, 1);
			ChangeState (115, 1);
			SetImage (GetImageByName ("dummystep1"));
			updateRoom = false;
			dummyStepsCompleted++;
			break;
		case 4:
			if (dummyStepsCompleted == 1) {
				AddText ("You put the pinata on top of your effigy. Now you just need to lure the killer into here");
			}
			else {
				AddText ("You put the pinata on top of your effigy. Finally, it is complete! Better get out of sight before the killer gets here.");
			}
			RemoveFromInv (67);
			ChangeState (102, 1);
			SetImage (GetImageByName ("dummystep2"));
			updateRoom = false;
			dummyStepsCompleted++;
			break;
		case 5:
			if (dummyStepsCompleted == 1) {
				AddText ("You put the tape recorder into the dummy. It still needs a head though.");
				SetImage (GetImageByName ("dummystep1"));
			}
			else {
				AddText ("You put the tape recorder into the dummy. Finally, it is complete! Better get out of sight before the killer gets here.");
				SetImage (GetImageByName ("dummystep2"));
			}
			RemoveFromInv (57);
			updateRoom = false;
			dummyStepsCompleted++;
			break;
		case 17:
			AddText ("You wrap the tarp around the rake. Hmm, this almost looks like a human-shaped dummy. If only it had a head");
			ChangeState (80, 1);
			ChangeState (82, 1);
			ChangeState (115, 1);
			SetImage (GetImageByName ("dummystep1"));
			updateRoom = false;
			dummyStepsCompleted++;
			break;
		default:
			break;
		}

		if (dummyStepsCompleted == 3) {
			dummyAssembled = true;
		}

		UpdateRoomState (updateRoom);
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

	[Command("inspect")]
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
				else if (killerInKitchen && currentRoom.Index == 0 && !bloodSeen3) {
					AddText ("you hear the killer in the kitchen");
					bloodSeen3 = true;
				} 
				else if (killerInKitchen && currentRoom.Index == 2 && !bloodSeen2) {
					AddText ("you hear the killer run downstairs, leaving a blood trail behind him.");
					bloodSeen2 = true;
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
            if (twoLayerLook)
            {
                if (currentItemGroup != null)
                {
                    var igObj = itemsList[currentItemGroup.BaseItemIndex];
                    Look(("look " + igObj.Name).Shlex());
                    twoLayerLook = false;
                    AddText("");
                }
            }
            else {
                ResetItemGroup();

                AddText(currentRoom.currentState.Description);

				ResetOverlay ();
				SetImage (GetImageByName (currentRoom.currentState.Image));
				UpdateRoomState ();
				return;
			}
        }

		int itemNameStart = (argv[1] != "at" && argv[1] != "out" && argv[1] != "through") ? 1 : 2;

        string itemName = string.Join(" ", argv.Skip(itemNameStart).ToArray());

		if (argv [1] == "under") {

			itemName = itemName.Replace ("under ", "");

			var underObj = GetObjectByName (itemName);
			if (underObj == null) {
				underObj = GetObjectFromInv (itemName);
			}

			if (underObj != null) {
				if (underObj.Index == 85) {
					SetImage (GetImageByName ("rug2"));
					AddText ("Just crumbs under here");
				}
				else {
					AddText ("I don't need to look under that");
				}
			}
			else {
				AddText ("Look under what?");
			}

			return;
		}

		bool invItem = false;

        //Debug.LogFormat("Looking at ({0})", itemName);

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

		if (obj == null || (obj.currentState.Description == "" && !IsInInv(obj.Index)))
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
				if (invItem && obj.currentState.Description == "") {
					int invState = obj.State + 1;
					string invDesc = obj.States [invState].Description;
					AddText (invDesc);
				}
				else {
					AddText (obj.currentState.Description);
				}
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
			if (obj.Index == 122) {
				OtherCommands ("read book 666");
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

			if (obj.Index == 60) {
				inputLockdown = true;
				currLockdownOption = 18;
			}

			if (invItem) {
				SetImage (GetImageByName ("invbig-" + obj.Name));
				lockMask = true;
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
			toPlaySoundFX.Add (clipToPlay);
			//PlayClip (clipToPlay);

			if (itemState == 4) {
				toPlayAmb = 5;
			}

			if (itemState == 6) {
				stopAudio = true;
			}

			if (itemState == 7) {
				toPlayAmb = 8;
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
				if (currentItemGroup.BaseItemIndex == 26 && item == 26 && loopingAudioSource.clip.name == "freezeramb" && loopingAudioSource.isPlaying) {
					AudioClip clipToPlay = GetClip (6);
					stopAudio = true;
					toPlaySoundFX.Add (clipToPlay);
				}

				// Stop the fridge audio loop
				if (currentItemGroup.BaseItemIndex == 25 && loopingAudioSource.clip.name == "fridgeamb" && loopingAudioSource.isPlaying) {
					AudioClip clipToPlay = GetClip (9);
					stopAudio = true;
					toPlaySoundFX.Add (clipToPlay);
				}

				if (currentItemGroup.BaseItemIndex == 33) {
					var obj = GetObjectByName ("oven");

					if (obj.State != 0) {
						AudioClip clipToPlay = GetClip (1);
						stopAudio = true;
						toPlaySoundFX.Add (clipToPlay);
					}
				}

				if (!currentItemGroup.NonResetItems.Contains (item)) {
					var obj = currentRoom.GetObjectById (item);
					obj.State = 0;
					itemsList [item].State = 0;
				}
			}

			AudioClip firstClip = new AudioClip ();
			AudioClip secondClip = new AudioClip ();
			if (toPlaySoundFX.Count == 2) {
				secondClip = toPlaySoundFX [0];
				firstClip = toPlaySoundFX [1];
				toPlaySoundFX.Remove (secondClip);
				toPlaySoundFX.Remove (firstClip);
				toPlaySoundFX.Add (firstClip);
				toPlaySoundFX.Add (secondClip);
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
							break;
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

	void PlayClips()
	{
		soundFXQueue = toPlaySoundFX;
		toPlaySoundFX = new List<AudioClip> ();
	}

	void PlayClip (AudioClip audioClip) {
		audioSource.loop = false;
		audioSource.clip = audioClip;
		audioSource.Play ();
	}

	void PlayMusicTrack (AudioClip audioClip) {
		musicTrack.loop = false;
		musicTrack.clip = audioClip;
		musicTrack.Play ();
	}

	void PlayAmbientClip (AudioClip audioClip) {
		ambientSource.loop = false;
		ambientSource.clip = audioClip;
		ambientSource.Play ();
	}

	public void UpdateRoomState(bool updateImage = true, int specificRoom = 0)
    {
		var room = (specificRoom == 0) ? currentRoom : rooms [specificRoom];
		PlayClips ();
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

		if (!killerInKitchen) {
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
        bool updateAllTimers = true;

        if (playerHiding)
        {
            updateAllTimers = false;
            if (pizzaTimer >= 0 && pizzaTimer < pizzaCap)
            {
                killerTimer += (pizzaCap - pizzaTimer);
                pizzaTimer = pizzaCap;
            }
            else if (pizzaTimer >= pizzaCap && pizzaTimer < pizzaCap2)
            {
                killerTimer += (pizzaCap2 - pizzaTimer);
                pizzaTimer = pizzaCap2;
            }
            else if (policeTimer >= 0 && policeTimer < policeCap)
            {
                killerTimer += (policeCap - policeTimer);
                policeTimer = policeCap;
            }
            else
            {
                if (killerTimer < killerCap2)
                {
                    killerTimer = killerCap2;
                }
                else if (killerTimer >= killerCap2 && killerTimer < killerCap3)
                {
                    killerTimer = killerCap3;
                }
                else if (killerTimer >= killerCap3 && killerTimer < killerCap4)
                {
                    killerTimer = killerCap4;
                }
            }
        }

        if (!killerInKitchen && updateAllTimers)
        {
            killerTimer++;

            if (pizzaTimer >= 0)
            {
                pizzaTimer++;
            }

            if (policeTimer >= 0)
            {
                policeTimer++;
            }
        }

        // If the player has left the living room, the killer no longer should show in the window and peephole
        if (killerTimer == killerCap)
        {
            ChangeState(94, 1);
            ChangeState(95, 1);
        }
        else if (killerTimer == killerCap2)
        {
            textWaiting += "You think you hear a distant creaking of the floorboards. \n\n";
        }
        else if (killerTimer == killerCap3)
        {
            textWaiting += "That sounded awfully close... \n\n";
        }

        else if (killerTimer == killerCap4)
        {
            //ChangeState (-1, 100);
            //SetImage (GetRandomDeathImage ());
            //AddText ("You died! \n\n");
            playerOutOfTime = true;
        }

        if (pizzaTimer == pizzaCap)
        {
            textWaiting += "You hear the sweet sweet pizza man at the door \n\n";
            ChangeState(94, 2);
            ChangeState(95, 2);
        }

        if (pizzaTimer == pizzaCap2)
        {
            ChangeState(94, 4);
            ChangeState(95, 4);
        }

        if (pizzaTimer > pizzaCap2)
        {
            ChangeState(94, 1);
            ChangeState(95, 1);
        }

        if (policeTimer == policeCap)
        {
            textWaiting += "You hear a police siren faintly at first, but quickly growing in volumne.\n\n";
            killerCap += 3;
            ChangeState(94, 3);
            ChangeState(95, 3);
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
							if (entry.Key == invItem.Name) {
								obj = GetObjectFromInv (invItem.Name);
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
		// ENTER CODE
		if (argv [0] == "enter" && currentRoom.Index == 4) {
			if (argv [1] == "code" || argv [1] == "combination") {

				var painting = GetObjectByName ("painting");

				if (painting.State == 0) {
					AddText ("I don't know what you're talking about");
				} 
				else {
					ImageCheckAndShow (painting.Index, painting.State, painting.currentState.Image);
					if (playerKnowsCombo) {
						OtherCommands ("open safe");
					}
					else 
					{
						AddText ("I would enter the code if I could remember it!");
					}
				}

				return;
			}
		}

        int newRoom = room;
        bool isRoom = false;
		bool roomImage = true;
        string roomName = string.Join(" ", argv.Skip((argv[1] != "to") ? 1 : 2).ToArray());

        var newRoomObj = GetRoomByName(roomName);
		if (newRoomObj != null) {

			if (newRoomObj.Name == currentRoom.Name) {
				AddText ("That's where you already are, dummy!");
				return;
			}

			newRoom = newRoomObj.Index;
			isRoom = true;
			UpdateRoomState (false, newRoomObj.Index);
		} else {
			List<string> altName = new List<string> ();
			int altNameId = -1;
			foreach (KeyValuePair<string, List<string>> entry in altNames)
			{
				if (entry.Value.Any (x => x == roomName)) {
					altName = entry.Value;
					isRoom = int.TryParse (entry.Key, out altNameId);
				}
			}

			if (isRoom) {

				if (altNameId == currentRoom.Index && altName.Contains (roomName)) {
					AddText ("That's where you already are, dummy!");
					return;
				}
			}
		}

		if (currentRoom.AdjacentRooms.Contains (newRoom)) {
			if (newRoomObj.currentState.Gettable == 1) {

                if (textWaiting != "")
                {
                    AddText(textWaiting);
                    textWaiting = "";
                }
                else
                {
                    AddText("");
                }

                //AddText ("");

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

		if (!isRoom && argv [0] == "move") {
			var obj = GetObjectByName (roomName, (x, y) => x.Contains (y));
			if (obj == null) 
			{
				AddText (GenericMove ());
				return;
			}

			var moveResponses = specialResponses
                .Where (x => x.ItemIndex == obj.Index)
				.Where (x => x.ItemState == obj.State)
                .Where (x => x.Command == "Move");

			if (moveResponses.Count() == 0) {
				AddText(GenericItemMove ());
				UpdateItemGroup (obj.Index);

				if (obj.currentState.Image != "") {
					ImageCheckAndShow (obj.Index, obj.State, obj.currentState.Image);
					ResetOverlay ();
				}

				return;
			}

			foreach (var response in moveResponses) {
				foreach (KeyValuePair<int, int> actions in response.Actions) {
					if (ChangeState (actions.Key, actions.Value) == 1)
						break;
				}

				if (obj.Index == 3) {
					killerCap += 5;
				}

				if (response.Image != "") {
					ImageCheckAndShow (response.ItemIndex, response.ItemState, response.Image);
					ResetOverlay ();
					roomImage = false;
				} else {
					if (image.sprite.name == currentRoom.currentState.Image) {
						roomImage = false;
					}
				}

				UpdateItemGroup (obj.Index);
				UpdateRoomState (roomImage);
				AddText (response.Response);

				return;
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

			toPlaySoundFX.Add (GetClip(12));

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

			if (item.Index == 47) {
				item.State = 0;
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

            if (item.Index == 54)
            {
                AddText("You pick the painting up and attempt to jam it into your pockets. Having no luck, you set it down and notice the safe embeded in the wall");
                ChangeState(54, 1);
                ChangeState(55, 1);
            }
            else if (item.Index == 93)
            {
                AddText("Eh, this piece of paper's just gonna weigh you down. You do read it though: Safe Combination: 1-2-4-3. Really original, pal.");
                playerKnowsCombo = true;
            }
			else if (item.Index == 123)
			{
				AddText(GenericGet ());
				return;
			}
            else {

                if (item.currentState.Get == "")
                {
                    AddText(GenericGet());
                }
                else {
                    AddText(item.currentState.Get);
                }
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
			item = GetObjectFromInv (itemName);

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

			if (response.ItemIndex == 80)
			{
				if (currentRoom.Index == 8) {
					if (dummyStepsCompleted == 0) {
						inputLockdown = true;
						currLockdownOption = 3;
					}
					else {
						AddText ("It's already a part of the dummy now!");
						return;
					}
				}
				else {
					AddText ("I don't think this is the right place for that");
					return;
				}

			}

			if (response.ItemIndex == 82)
			{
				if (currentRoom.Index == 8) {
					if (dummyStepsCompleted == 0) {
						inputLockdown = true;
						currLockdownOption = 17;
					}
					else {
						AddText ("It's already a part of the dummy now!");
						return;
					}
				}
				else {
					AddText ("I don't think this is the right place for that");
					return;
				}

			}

			if (response.ItemIndex == 67)
			{
				if (currentRoom.Index == 8) {
					inputLockdown = true;
					currLockdownOption = 4;
				}
				else {
					AddText ("I don't think this is the right place for that");
					return;
				}

			}
				
			if (response.ItemIndex == 7) {
				ResetOverlay ();
				if (IsInInv (15) || IsInInv(64)) {
					if (!IsInInv (43)) {
						AddText ("You don’t have anything to light it with.");
						ImageCheckAndShow (response.ItemIndex, response.ItemState, "showitem");
						return;
					}
				} 
				else {
					if (IsInInv (43)) {
						AddText ("You don’t have anything flammable on you.");
						ImageCheckAndShow (response.ItemIndex, response.ItemState, "showitem");
						return;
					} 
					else {
						AddText ("You don’t have the means of starting a fire on you.");
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

					if (image.sprite.name.Contains("invbig") || image.sprite.name == "blankoverlay") {
						UpdateRoomState ();
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

			if (response.ItemIndex == 0) {
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
			if (response.ItemIndex == 78) {
				Move ("move kitchen".Shlex());
				return;
			}
			if (response.ItemIndex == 83) {
				Move ("move shed".Shlex());
				return;
			}
			if (response.ItemIndex == 120) {
				Move ("move kitchen".Shlex());
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

			if (response.ItemIndex == 60) {
				inputLockdown = true;
				currLockdownOption = 21;
			}

			if (response.ItemIndex == 5) {
				Look ("look peephole".Shlex());
				return;
			}

            if (response.ItemIndex == 96)
            {
                Use("use drawer".Shlex());
                return;
            }

			if (response.ItemIndex == 89)
			{
				Use("use drawer".Shlex());
				return;
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

	void GetDummyItems(){
		room = 4;
		inventory.Add (GetObjectByName("tape recorder"));
		inventory.Add (GetObjectByName("flashlight"));
		room = 5;
		inventory.Add (GetObjectByName ("pinata"));
		room = 7;
		/*inventory.Add (52);
		inventory.Add (67);*/
	}

    public void OtherCommands(string text)
    {
		if (text.Contains ("the")) {
			if (text.Contains (" the"))
				text = text.Replace (" the", "");
			else if (text.Contains("the ")) {
				text = text.Replace ("the ", "");
			}
		}

        string command = text.Split(new char[] { ' ' }, 2)[0].ToLower();
		string itemName = "";
		if (text.Any (x => Char.IsWhiteSpace (x))) {
			itemName = text.Split (new char[] { ' ' }, 2) [1].ToLower ();
		}

		var tempTokens = new List<string> ();

        switch (command)
        {
		case "dummyitems":
			GetDummyItems ();
			return;
		case "read":

			if (itemName == "1") {
				OtherCommands ("read book 1");
				return;
			}
			else if (itemName == "2") {
				OtherCommands ("read book 2");
				return;
			}
			else if (itemName == "3") {
				OtherCommands ("read book 3");
				return;
			}
			else if (itemName == "4") {
				OtherCommands ("read book 4");
				return;
			}

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
				return;
			} else {

				if (itemName.Contains("under ")){
					itemName.Replace ("under ", "");
				}

				command = "Hide";
			}
			break;
		case "help":
			Help ();
			return;
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
			return;
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
		case "equip":
		case "wear":
			tempTokens = itemName.Shlex ();

			for (int i = 0; i < tempTokens.Count; ++i) {
				if (tempTokens [i] == "on") {
					tempTokens.Remove (tempTokens [i]);
					if (itemName.Contains("on ")) itemName = itemName.Replace ("on ", "");
					else if (itemName.Contains(" on")) itemName = itemName.Replace (" on", "");
				}
			}

			var useItem = GetObjectByName (itemName);
			if (useItem == null) {
				useItem = GetObjectFromInv (itemName);
			}

			itemName = "use";

			for (int i = 0; i < tempTokens.Count; ++i) {
				itemName += " " + tempTokens [i];
			}
				
			if (useItem != null) {
				if (useItem.Index == 41) {
					var tokens = itemName.Shlex ();
					Use (tokens);
				} else {
					if (useItem.currentState.Image != "") {
						SetImage (GetImageByName (useItem.currentState.Image));
					}

					AddText ("I don't think I can do that");
				}
			} else {
				AddText ("I don't think I can do that.");
			}

			return;
		case "take":
			tempTokens = itemName.Shlex ();

			bool takeOff = false;
			for (int i = 0; i < tempTokens.Count; ++i) {
				if (tempTokens [i] == "off") {
					tempTokens.Remove (tempTokens [i]);
					if (itemName.Contains("off ")) itemName = itemName.Replace ("off ", "");
					else if (itemName.Contains(" off")) itemName = itemName.Replace (" off", "");
					takeOff = true;
				}
			}

			var takeItem = GetObjectByName (itemName);
			if (takeItem == null) {
				takeItem = GetObjectFromInv (itemName);
			}

			if (takeOff) {
				if (takeItem.Index == 41) {
					tempTokens.Insert (0, "use");
					Use (tempTokens);
					return;
				}
				else {
					if (takeItem.currentState.Image != null) {
						SetImage (GetImageByName (takeItem.currentState.Image));
						AddText ("I don't think I can do that");
						return;
					}
				}
			} else {
				tempTokens.Insert (0, "get");
				Get (tempTokens);
				return;
			}
			break;
		case "acquire":
		case "obtain":
			Get (("get " + itemName).Shlex ());
			return;
		case "pick":
			if (itemName.Contains (" up") || itemName.Contains ("up ")) {
				if (itemName.Contains (" up"))
					Get (("get " + itemName.Replace (" up", "")).Shlex ());
				else
					Get (("get " + itemName.Replace ("up ", "")).Shlex ());
			} else {
				AddText ("I don't know how to do that");
			}
			return;
		case "perish":
		case "expire":
		case "die":
			KillSelf ();
			return;
		case "give":
			if (itemName.Contains ("up")) {
				KillSelf ("You decide to give up living. Great!");
				return;
			} else {
				AddText ("I don't know how to do that");
				return;
			}
		case "sit":

			if (itemName == "" || itemName == "down") {
				if (currentRoom.Index == 0) {
					Use ("use arm chair".Shlex ());
				} else {
					AddText ("Eh, nowhere comfy to sit down here.");
				}
			}
			else {
				if (itemName.Contains ("on") || itemName.Contains ("in") || itemName.Contains ("down on")) {
					if (itemName.Contains("down on "))
						itemName = itemName.Replace ("down on ", "");
					else if (itemName.Contains ("in "))
						itemName = itemName.Replace ("in ", "");
					else if (itemName.Contains ("on ")) {
						itemName = itemName.Replace ("on ", "");
					}
				}

				Sit (itemName);
			}

			return;
		case "off":
		case "kill":
			if (itemName.Contains ("self") || itemName.Contains ("me") || itemName.Contains ("myself")) {
				KillSelf ();
			}
			return;
		case "commit":
			if (itemName.Contains ("suicide")) {
				KillSelf ();

			} else if (itemName.Contains ("seppuku")) {
				if (IsInInv (23) || IsInInv (74) || IsInInv (75)) {
					KillSelf ("Feeling like the noble Japanese samurai of which you only know a vague ammount, you disembowl yourself. Neat!");
				}
				else {
					AddText ("Hmm, I would need a sharp instrument to do that");
				}
			}
			return;
		case "scream":
		case "yell":
			Scream ();
			return;
		case "turn":
			Turn (itemName);
			return;
		case "climb":
			command = "Climb";
			break;
		case "break":
		case "smash":
			command = "Break";
			break;
		case "vault":
		case "leap":
		case "jump":

			if (itemName.Contains (" over") || itemName.Contains ("over ")) {
				if (itemName.Contains (" over"))
					itemName = itemName.Replace (" over", "");
				else
					itemName = itemName.Replace ("over ", "");
			}

			var jumpItem = GetObjectByName (itemName);
			if (currentRoom.Index == 7 && jumpItem.Index == 118) {
				AddText ("Are you serious? I think you got winded walking out here, you're definitely not jumping over that.");
			} else {
				AddText ("I have no idea what you're talking about");
			}
			return;
		case "sleep":
			if (itemName == "") {
				if (currentRoom.Index == 4) {
					AddText ("You take a quick lil nap");
				} else if (currentRoom.Index == 0) {
					AddText ("You take a lil snooze in your comfy arm chair");
				} else {
					AddText ("Eh, there's nowhere really comfortable to sleep in here");
				}
			} else {
				AddText ("I have no idea what you're talking about");
			}
			return;
		case "drop":
			Drop (itemName);
			return;
		case "unplug":
			Turn (("off ") + itemName);
			return;
		case "lock":
			command = "Lock";
			break;
		default:
			AddText ("I don't know how to do that");
            return;
        }
        var item = GetObjectByName(itemName);
		if (item == null) {
			item = GetObjectFromInv (itemName);
			if (item == null) {

				switch (command) {

				case "Call":
					if (currentRoom.Index == 0) {
						AddText ("Hm, you don't seem to have the number written down for that.");
						return;
					}
					else {
						AddText ("There's no phone in here. And you heard somewhere that cell phones give off harmful radiation. Or was that microwaves?");
						return;
					}
					break;
				default:
					break;
				}

				AddText ("I don't know how to do that");
				return;
			}
		}

		SpecialResponse specResponse = null;

		foreach (var specialResponse in specialResponses) {
			if (specialResponse.ItemIndex == item.Index && specialResponse.Command == command && specialResponse.ItemState == item.State)
				specResponse = specialResponse;
		}
			
		object[] parameters = new object[2];
		parameters [0] = item.Index;
		if (specResponse == null) {
			parameters [1] = -1;
		} else {
			parameters [1] = specialResponses.IndexOf (specResponse);
		}
		MethodInfo mInfo = typeof(HouseManager).GetMethod (command);
		mInfo.Invoke (this, parameters);

       /* for (int j = 0; j < specialResponses.Count; ++j)
        {
			if (specialResponses [j].Command == command && specialResponses[j].) {

			}
        }*/
    }

	public void Sit(string text){
		var obj = GetObjectByName (text);
		bool roomImage = false;

		if (obj == null) {
			AddText ("I don't know what you want to sit on");
			return;
		}

		var sitResponses = specialResponses
			.Where (x => x.ItemIndex == obj.Index)
			.Where (x => x.Command == "Sit");

		if (sitResponses.Count () == 0) {
			
			AddText ("I'm not sitting on that.");

			UpdateItemGroup (obj.Index);

			if (obj.currentState.Image != "") {
				ImageCheckAndShow (obj.Index, obj.State, obj.currentState.Image);
				ResetOverlay ();
			}

			return;
		}
		else {
			foreach (var response in sitResponses) {
				foreach (KeyValuePair<int, int> actions in response.Actions) {
					if (ChangeState (actions.Key, actions.Value) == 1)
						break;
				}

				if (response.Image != "") {
					ImageCheckAndShow (response.ItemIndex, response.ItemState, response.Image);
					ResetOverlay ();
					roomImage = false;
				} else {
					if (image.sprite.name == currentRoom.currentState.Image) {
						roomImage = false;
					}
				}

				UpdateItemGroup (obj.Index);
				UpdateRoomState (roomImage);
				AddText (response.Response);

				return;
			}
		}

	}

	public void Drop(string text){
		var obj = GetObjectFromInv (text);

		if (obj != null) {
			SetImage (GetImageByName ("invbig-" + obj.Name));
			lockMask = true;
			ResetOverlay ();
			SetGasMaskOverlay (false);
			AddText ("Nah, I think I might need that later");
		} else {
			AddText ("Not sure what you want me to drop.");
		}
	}

	public void KillSelf(string overrideText = ""){
		SetImage (GetImageByName (GetTombstone ()));
		if (overrideText == "")
			AddText ("You decide that life isn't worth living and kill yourself. Neat.");
		else
			AddText (overrideText);

		AddAdditionalText ("\n\nPress [ENTER] to Restart.");
		ResetOverlay ();
		SetGasMaskOverlay (false);
		SetBasementOverlay (4, false);
		health = 0;
	}

	public void Turn(string text){

		bool turnOff = false;
		bool turnOn = false;
		bool roomImage = false;

		var turnText = text.Shlex ();

		if (turnText [0] == "on")
			turnOn = true;
		if (turnText [0] == "off")
			turnOff = true;

		if (turnText [0] == "around" || turnText [0] == "back") {
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
			return;
		}

		int itemNameStart = (turnText[0] != "on" && turnText[0] != "off") ? 0 : 1;
		string objName = string.Join(" ", turnText.Skip(itemNameStart).ToArray());

		var obj = GetObjectByName (objName);
		if (obj == null) {
			obj = GetObjectFromInv (objName);
		}

		if (obj == null) {
			if (turnOn) {
				AddText ("I can't turn on what I can't see.");
			}
			else if (turnOff) {
				AddText ("I can't turn off what I can't see.");
			}
			else {
				AddText ("I'm not sure what you want to do.");
			}
			return;
		}

		var turnOnResponses = specialResponses
			.Where (x => x.ItemIndex == obj.Index)
			.Where (x => x.Command == "Turn On");
		
		var turnOffResponses = specialResponses
			.Where(x => x.ItemIndex == obj.Index)
			.Where(x => x.Command == "Turn Off");

		if (turnOnResponses.Count() == 0 && turnOffResponses.Count() == 0) {
			if (turnOn) {
				AddText ("I don't think I can turn that on.");
			} 
			else if (turnOff) {
				AddText ("I don't think that's something I can turn off.");	
			}

			UpdateItemGroup (obj.Index);

			if (obj.currentState.Image != "") {
				ImageCheckAndShow (obj.Index, obj.State, obj.currentState.Image);
				ResetOverlay ();
			}

			return;
		}

		if (turnOn) {
			foreach (var response in turnOnResponses) {
				foreach (KeyValuePair<int, int> actions in response.Actions) {
					if (ChangeState (actions.Key, actions.Value) == 1)
						break;
				}
				
				if (response.Image != "") {
					ImageCheckAndShow (response.ItemIndex, response.ItemState, response.Image);
					ResetOverlay ();
					roomImage = false;
				} else {
					if (image.sprite.name == currentRoom.currentState.Image) {
						roomImage = false;
					}
				}

				UpdateItemGroup (obj.Index);
				UpdateRoomState (roomImage);
				AddText (response.Response);

				return;
			}
		} 
		else {
			foreach (var response in turnOffResponses) {
				foreach (KeyValuePair<int, int> actions in response.Actions) {
					if (ChangeState (actions.Key, actions.Value) == 1)
						break;
				}

				if (response.Image != "") {
					ImageCheckAndShow (response.ItemIndex, response.ItemState, response.Image);
					ResetOverlay ();
					roomImage = false;
				} else {
					if (image.sprite.name == currentRoom.currentState.Image) {
						roomImage = false;
					}
				}

				UpdateItemGroup (obj.Index);
				UpdateRoomState (roomImage);
				AddText (response.Response);

				return;
			}
		}
	}

	public void Scream(){

		if (currentRoom.Index == 5) {
			HideNoItem ("scream");
		} 
		else {
			List<string> imageList = deathImages [5];
			ResetOverlay ();
			SetGasMaskOverlay (false);
			SetBasementOverlay (4, false);
			string imageName = imageList [UnityEngine.Random.Range (0, imageList.Count)];
			AddText ("You yell at the top of your lungs and the killer gets ya.\n\nPress [ENTER] to restart.");
			SetImage (GetImageByName (imageName));
			health = 0;
			return;
		}
	}

	public void Help()
	{
		SetImage (GetImageByName ("blankoverlay"));
		AddText ("");

		SetGasMaskOverlay (false);
		SetOverlay (GetImageByName ("blankoverlay"));
		helpScreenUp = true;

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
			} else if (source == "basement") {
				AddText ("As you descend the stairs, you hear the killer closely behind you. You hear da bad boy start to come down the stairs. Press [ENTER]");
			} 
			else {
				if (bearTrapMade || fireTrapMade || shitOnStairs || bucketTrapMade) {
					AddText ("You hide and wait for the killer to show up. You hear him start to come down the stairs. Press [ENTER]");
				}
				else {
					OtherCommands ("hide boxes");
					return;
				}
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

			return;
		}

		if ((currentRoom.Index == 7 || currentRoom.Index == 8) && dummyAssembled) {
			killerInShack = true;
			room = 7;
			SetImage (GetImageByName ("outside2"));
			List<string> imageList = deathOverlays [9];
			string imageName = imageList[UnityEngine.Random.Range(0, imageList.Count)];
			currOverlay = imageName;
			SetOverlay(GetImageByName(imageName));
			AddText ("you hide, looks like the killer's INSPECTING");
			currLockdownOption = 6;
			inputLockdown = true;
			return;
		}

		if (currentRoom.Index == 4 ) {
			OtherCommands ("hide under bed");
			return;
		}

		AddText ("There's not a good place to hide here.");
	}

	public void Climb(int i, int j){
		var item = itemsList [i];
		bool roomImage = true;
		if (j != -1) {
			AddText (specialResponses [j].Response);

			foreach (KeyValuePair<int, int> actions in specialResponses[j].Actions) {
				if (ChangeState (actions.Key, actions.Value) == 1)
					break;
			}

			if (specialResponses [j].Image != "") {
				ResetOverlay ();
				ImageCheckAndShow (item.Index, item.State, specialResponses [j].Image);
				roomImage = false;
			}

			UpdateItemGroup (item.Index);
			UpdateRoomState (roomImage);

			return;
		}
		else {
			if (item.currentState.Image != "") {
				ResetOverlay ();
				ImageCheckAndShow (item.Index, item.State, item.currentState.Image);
			}

			AddText ("Can't climb that.");
			return;
		}
	}

	public void Break(int i, int j){
		var item = itemsList [i];
		bool roomImage = true;
		if (j != -1) {
			AddText (specialResponses [j].Response);

			foreach (KeyValuePair<int, int> actions in specialResponses[j].Actions) {
				if (ChangeState (actions.Key, actions.Value) == 1)
					break;
			}

			if (specialResponses [j].Image != "") {
				ResetOverlay ();
				ImageCheckAndShow (item.Index, item.State, specialResponses [j].Image);
				roomImage = false;
			}

			UpdateItemGroup (item.Index);
			//UpdateRoomState (roomImage);

			return;
		}
		else {
			if (item.currentState.Image != "") {
				ResetOverlay ();
				ImageCheckAndShow (item.Index, item.State, item.currentState.Image);
			}

			AddText ("Can't break that.");
			return;
		}
	}

	public void Lock(int i, int j){
		var item = itemsList [i];
		bool roomImage = true;
		if (j != -1) {
			AddText(specialResponses[j].Response);

			foreach (KeyValuePair<int, int> actions in specialResponses[j].Actions)
			{
				if (ChangeState(actions.Key, actions.Value) == 1)
					break;
			}

			if (specialResponses[j].Image != "")
			{
				ResetOverlay ();
				ImageCheckAndShow (item.Index, item.State, specialResponses [j].Image);
				roomImage = false;
			}

			UpdateItemGroup (item.Index);
			UpdateRoomState(roomImage);

			return;
		}
		else {
			if (item.currentState.Image != "") {
				ResetOverlay ();
				ImageCheckAndShow (item.Index, item.State, item.currentState.Image);
			}

			AddText ("Can't lock that.");
			return;
		}
	}

	public void Drink(int i, int j){
		var item = itemsList [i];
		bool roomImage = true;
		if (j != -1) {
			AddText(specialResponses[j].Response);

			foreach (KeyValuePair<int, int> actions in specialResponses[j].Actions)
			{
				if (ChangeState(actions.Key, actions.Value) == 1)
					break;
			}

			if (specialResponses[j].Image != "")
			{
				ResetOverlay ();
				ImageCheckAndShow (item.Index, item.State, specialResponses [j].Image);
				roomImage = false;
			}

			UpdateItemGroup (item.Index);
			UpdateRoomState(roomImage);

			return;
		}
		else {
			if (item.currentState.Image != "") {
				ResetOverlay ();
				ImageCheckAndShow (item.Index, item.State, item.currentState.Image);
			}

			AddText ("Can't drink that.");
			return;
		}
	}

	public void Shit(int i, int j)
	{
		var item = itemsList [i];
		bool roomImage = true;
		if (j != -1) {
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
		else {
			if (item.currentState.Image != "") {
				ResetOverlay ();
				ImageCheckAndShow (item.Index, item.State, item.currentState.Image);
			}

			AddText ("Can't shit here.");
			return;
		}
	}

	public void Hide(int i, int j)
	{
		var item = itemsList [i];
		bool roomImage = true;
		if (j != -1) {
			AddText(specialResponses[j].Response);

			foreach (KeyValuePair<int, int> actions in specialResponses[j].Actions)
			{
				if (ChangeState(actions.Key, actions.Value) == 1)
					break;
			}

			if (item.Index == 121) {
				roomImage = false;
			}

			if (specialResponses[j].Image != "")
			{
				ImageCheckAndShow (item.Index, item.State, specialResponses [j].Image);
				roomImage = false;
			}

			playerHiding = true;

			UpdateItemGroup (item.Index);
			UpdateRoomState(roomImage);

			return;
		}
		else {
			if (item.currentState.Image != "") {
				ResetOverlay ();
				ImageCheckAndShow (item.Index, item.State, item.currentState.Image);
			}

			if (item.Index == 51) {
				AddText ("You'd have to open it first.");
				return;
			}

			AddText ("Doesn't look like a good place to hide.");
			return;
		}
	}

    public void Read(int i, int j)
    {
        var item = itemsList[i];
		bool roomImage = true;
		if (j != -1)
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
				toPlaySoundFX.Add (GetClip (2));
				//PlayClip (GetClip (2));
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
		else {
			if (item.currentState.Image != "") {
				ResetOverlay ();
				ImageCheckAndShow (item.Index, item.State, item.currentState.Image);
			}

			AddText ("Can't read that.");
			return;
		}
    }

    public void Call(int i, int j)
    {
        var item = itemsList[i];
		if (j != -1)
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
				AddText("You hear the dial tone, then suddenly, nothing.. That’s...not a good thing to have happen right now...");
				ImageCheckAndShow (item.Index, item.State, specialResponses[j].Image);

				UpdateItemGroup (item.Index);
				UpdateRoomState(false);

                return;
            }
        }
		else {
			if (item.currentState.Image != "") {
				ResetOverlay ();
				ImageCheckAndShow (item.Index, item.State, item.currentState.Image);
			}

			AddText ("No idea what you're trying to do.");
			return;
		}
    }


    public void Open(int i, int j)
    {
        var item = itemsList[i];
		bool roomImage = true;
		if (j != -1){
			int startState = item.State;
			if (item.Index == 55 && item.State == 1 && !playerKnowsCombo) {
				AddText ("ah man, if only I knew the dumb combo");
				SetImage(GetImageByName("safe"));
				return;
			}

			if (item.Index == 8 && pizzaTimer >= pizzaCap && pizzaTimer <= pizzaCap2) {
				multiSequence = true;
				currMultiSequence = 8;
                ResetOverlay();
                SetGasMaskOverlay(false);
				AddText ("The pizza mans gets the bad guy, whoa!");
				SetImage (GetImageByName ("pizzawin"));
				return;
			}

			if (item.Index == 8 && policeTimer >= policeCap) {
				AddText ("Oh no, that's not a cop!");
                SetGasMaskOverlay(false);
                SetImage (GetRandomDeathImage());
				health = 0;
				return;
			}

			if (item.Index == 8 && killerInKitchen) {
				AddText ("you make it as far as the front porch before the killer gets ya\n\nPress [ENTER] to restart.");
                SetGasMaskOverlay(false);
				ResetOverlay ();
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
						AddText ("You open the window and attempt to climb out. Just before you can get more than a leg out however, the man in the cleansuit drags you back inside, defenseless.\n\nPress [ENTER] to restart.");
					} else if (pizzaTimer >= pizzaCap && pizzaTimer <= pizzaCap2) {
						SetImage (GetImageByName ("windowdeath3"));
						AddText ("You open the window, but as you start to poke your head out, you feel a horrible pain as a blade of cold metal pierces your back. You fall to the floor, groping for the knife in your back and screaming. In your final moments, you notice that the pizza guy has come up to the window. Somehow clueless to your suffering, he attempts to deliver you the pizza by handing it to you through the window to your writhing form.\n\nPress [ENTER] to restart.");
					}
					else {
						SetImage (GetImageByName ("windowdeath"));
						AddText ("You open the window and attempt to climb out. Just before you can get more than a leg out however, the man in the cleansuit drags you back inside, defenseless.\n\nPress [ENTER] to restart.");
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

			if (item.Index == 60) {
				inputLockdown = true;
				currLockdownOption = 19;
			}

            if (item.Index == 96)
            {
				OtherCommands ("open drawer");
                return;
            }

			if (item.Index == 89)
			{
				OtherCommands ("open drawer");
				return;
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
        }
		else {
			if (item.currentState.Image != "") {
				ResetOverlay ();
				ImageCheckAndShow (item.Index, item.State, item.currentState.Image);
			}

			AddText ("Can't open that.");
		}
    }

    public void Close(int i, int j)
    {
        var item = itemsList[i];
		bool roomImage = true;
		if (j != -1)
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

			if (item.Index == 60) {
				inputLockdown = true;
				currLockdownOption = 20;
			}

            if (item.Index == 96)
            {
				OtherCommands ("close drawer");
                return;
            }

			if (item.Index == 89)
			{
				OtherCommands ("close drawer");
				return;
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
		else {
			if (item.currentState.Image != "") {
				ResetOverlay ();
				ImageCheckAndShow (item.Index, item.State, item.currentState.Image);
			}

			AddText ("Can't close that.");
			return;
		}
    }		

    public string GenericLook()
    {
        List<string> responses = new List<string>();
        responses.Add("Lookin' good.");
		responses.Add("Not sure what to look at.");

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

	public string GenericMove()
	{
		List<string> responses = new List<string>();
		responses.Add("You want to go where?");
		responses.Add("I don't think I can move there.");
		return responses[UnityEngine.Random.Range(0, responses.Count)];
	}

	public string GenericItemMove()
	{
		List<string> responses = new List<string>();
		responses.Add("It's fine where it is.");
		responses.Add("I think I'll leave it here.");
		return responses[UnityEngine.Random.Range(0, responses.Count)];
	}

    public string GenericUse()
    {
        List<string> responses = new List<string>();
		responses.Add("Reduce, reuse, no.");
		responses.Add("You can’t use that.");

        return responses[UnityEngine.Random.Range(0, responses.Count)];
    }

	public string GetResetText()
	{
		List<string> responses = new List<string> ();
		responses.Add("Don't mess up this time!\n\n");
		responses.Add("STOP DYING!\n\n");

		return responses[UnityEngine.Random.Range(0, responses.Count)];
	}

	public string GetTombstone(){
		List<string> responses = new List<string> ();
		responses.Add("tombstone");
		responses.Add("tombstone2");
		responses.Add("tombstone3");

		return responses[UnityEngine.Random.Range(0, responses.Count)];
	}

	public int GetAmbientNoise(){
		List<int> responses = new List<int> ();
		responses.Add(14);
		responses.Add(15);
		responses.Add(16);
		responses.Add(17);

		return responses[UnityEngine.Random.Range(0, responses.Count)];
	}
}