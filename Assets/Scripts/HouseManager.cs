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
using System.IO;
using UnityEngine.EventSystems;


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
	bool bearTrapMade, fireTrapMade, bucketTrapMade, shitOnStairs, blenderTrapMade;
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
	bool handsWashed;
	bool timeoutDeath;
	bool makingTraps, trapsDone;
	int trapsAtOnce;
	string fileName = "Playtest.txt";
	UnityEngine.GameObject selectedField;
	bool unlockingWindow;
	bool unlockingDoor;
	bool doorBlocked;
	List<int> audioTracks;
	List<int> trackNumbers;
	int trackInterludeTime;
	bool decrementInterlude;
	bool ambienceInProg;
	int ambientSubTimer;
	float trackLength;
	List<string> killerResponseGenerics;
	bool phoneLooked;
	bool politePlayer;
	bool trapStep1, trapStep2, trapStep3;
	int trapItem1, trapItem2, trapItem3;
	bool hideKillerInKitchen;
	string storedImage;
	string storedOverlay;
	string storedText;
	bool fadeActionTrack, fadeMusicTrack;
	bool sleepDeath;
	bool confrontPause;
	bool tapePlaced;

	public Image image;
	public Image overlayImage;
	public Image gasMaskOverlay;
	public Image basementOverlay;
	public Image basementOverlay2;
	public Image basementOverlay3;
	public Image basementOverlay4;
	public Image basementOverlay5;
	public Image workbenchOverlay;
	public Image workbenchOverlay2;
	public Image workbenchOverlay3;
	public Image inv0, inv1, inv2, inv3, inv4, inv5, inv6, inv7, inv8, inv9, inv10, inv11, inv12, inv13, inv14, inv15, inv16, inv17, inv18, inv19; 
	public GradualTextRevealer invText0, invText1, invText2, invText3, invText4, invText5, invText6, invText7, invText8, invText9, invText10, invText11, invText12, invText13, invText14, invText15, invText16, invText17, invText18, invText19, inventoryTopText;
	public GradualTextRevealer helpText;
	public AudioSource audioSource;
	public AudioSource loopingAudioSource;
	public AudioSource musicTrack;
	public AudioSource ambientSource;
	public AudioSource knockingSource;
	public AudioSource actionTrack;
	public AudioSource lossTrack;
	public EventSystem es;
	public InputField inputText;

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
		bearTrapMade = fireTrapMade = bucketTrapMade = shitOnStairs = blenderTrapMade = false;
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
		soundFXTimer = UnityEngine.Random.Range(1800, 2400);
		lockMask = false;
		handsWashed = false;
		timeoutDeath = false;
		makingTraps = false;
		trapsDone = false;
		trapsAtOnce = 0;
		RecordPlaytest ("New run started");
		unlockingWindow = false;
		unlockingDoor = false;
		doorBlocked = false;
		trackInterludeTime = UnityEngine.Random.Range(1800, 2400);
		ambientSubTimer = UnityEngine.Random.Range(900, 1200);
		decrementInterlude = false;
		ambienceInProg = false;
		currOverlay = "blankoverlay";
		phoneLooked = false;
		politePlayer = false;
		trapStep1 = trapStep2 = trapStep3 = false;
		trapItem1 = trapItem2 = trapItem3 = 0;
		hideKillerInKitchen = false;

		killerResponseGenerics = new List<string> ();

		killerResponseGenerics.Add ("run");
		killerResponseGenerics.Add ("run away");
		killerResponseGenerics.Add ("flee");
		killerResponseGenerics.Add ("escape");
		killerResponseGenerics.Add ("escaping");

		killerResponseGenerics.Add ("make friends");
		killerResponseGenerics.Add ("making friends");
		killerResponseGenerics.Add ("befriend");

		killerResponseGenerics.Add ("fight");
		killerResponseGenerics.Add ("attack");

		killerResponseGenerics.Add ("talk");
		killerResponseGenerics.Add ("reason");
		killerResponseGenerics.Add ("plead");
		killerResponseGenerics.Add ("speak");

		storedImage = "";
		storedOverlay = "";
		storedText = "";

		fadeActionTrack = fadeMusicTrack = false;
		sleepDeath = false;
		confrontPause = false;
		tapePlaced = true;
	}

	void Start()
	{
		parsedHouseXml = XElement.Parse(xmlDocument.text);

		RecordPlaytest ("\nNew Playtest\n\n");

		RandomizeTracks ();

		// Alt Names
		AltNamesParser altNamesParser = gameObject.GetComponent(typeof(AltNamesParser)) as AltNamesParser;
		TextAsset altNamesText = altNamesParser.xmlDocument;
		XmlDocument altNamesDoc = new XmlDocument();
		altNamesDoc.LoadXml(altNamesText.text);
		altNames = altNamesParser.ReadXML(altNamesDoc);

		SetupHouse();
		SetupCommands();
		PlayMusicTrack (GetClip (audioTracks.First()  ));
		trackLength = GetClip (audioTracks.First ()).length * 60;
		PlayKnockingClip (GetClip (23));
		AddAdditionalText ("You recline in your easy chair. It is late and your living room is lit only by harsh fluorescent light from the lamp behind you. There is a slight draft in the room that chills you, and the thought of your warm bed begins to form in your mind. Suddenly, there is a pounding at the front door, feet from you, causing you to jolt. As your heart races, you think, \"Who could that be?\" You suppose you had better take a look.\n\nType HELP and press [ENTER] for some guidance.");
	}

	void RecordPlaytest(string line){
		System.IO.File.AppendAllText(fileName, "\n"+ line);
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
		dict.Add (11, new List<string> { "death-gun", "death-gun2" });
		return dict;
	}

	Dictionary<int, List<string>> GetLockdownOptions()
	{
		Dictionary<int, List<string>> dict = new Dictionary<int, List<string>> ();

		List<string> killerNames = new List<string> {"killer", "intruder", "shape", "villain", "guy", "man", "dude", "person", "thing", "figure", "murderer", "him", "them",
			"killer in clean suit", "intruder in clean suit", "shape in clean suit", "villain in clean suit", "guy in clean suit", "man in clean suit", "dude in clean suit", "person in clean suit", "thing in clean suit", "figure in clean suit", "murderer in clean suit",
			"killer in clean suit", "intruder in cleansuit", "shape in cleansuit", "villain in cleansuit", "guy in cleansuit", "man in cleansuit", "dude in cleansuit", "person in cleansuit", "thing in cleansuit", "figure in cleansuit", "murderer in cleansuit",
			"bad killer", "bad intruder", "bad shape", "bad villain", "bad guy", "bad man", "bad dude", "bad person", "bad thing", "bad figure", "bad murderer",
			"evil killer", "evil intruder", "evil shape", "evil villain", "evil guy", "evil man", "evil dude", "evil person", "evil thing", "evil figure", "evil murderer",
			"bad killer in clean suit", "bad intruder in clean suit", "bad shape in clean suit", "bad villain in clean suit", "bad guy in clean suit", "bad man in clean suit", "bad dude in clean suit", "bad person in clean suit", "bad thing in clean suit", "bad figure in clean suit", "bad murderer in clean suit",
			"bad killer in clean suit", "bad intruder in cleansuit", "bad shape in cleansuit", "bad villain in cleansuit", "bad guy in cleansuit", "bad man in cleansuit", "bad dude in cleansuit", "bad person in cleansuit", "bad thing in cleansuit", "bad figure in cleansuit", "bad murderer in cleansuit",
			"evil killer in clean suit", "evil intruder in clean suit", "evil shape in clean suit", "evil villain in clean suit", "evil guy in clean suit", "evil man in clean suit", "evil dude in clean suit", "evil person in clean suit", "evil thing in clean suit", "evil figure in clean suit", "evil murderer in clean suit",
			"evil killer in clean suit", "evil intruder in cleansuit", "evil shape in cleansuit", "evil villain in cleansuit", "evil guy in cleansuit", "evil man in cleansuit", "evil dude in cleansuit", "evil person in cleansuit", "evil thing in cleansuit", "evil figure in cleansuit", "evil murderer in cleansuit"};
		dict.Add (0, new List<string> { "use [0]", "get [0]", "stab [killer]", "use [0] on [killer]", "stab [killer] with [0]", "attack [killer] with [0]",
			"using [0]", "getting [0]", "stabbing [killer]", "using [0] on [killer]", "stabbing [killer] with [0]", "attacking [killer] with [0]",
			"use [2]", "get [2]", "use [2] on [killer]", "stab [killer] with [2]", "attack [killer] with [2]",
			"using [2]", "getting [2]", "using [2] on [killer]", "stabbing [killer] with [2]", "attacking [killer] with [2]",
			"pistol whip [killer]", "hit [killer] with [1]", "attack [killer] with [1]", "smack [killer] with [1]", "whack [killer] with [1]",
			"pistol whipping [killer]", "hitting [killer] with [1]", "attacking [killer] with [1]", "smacking [killer] with [1]", "whacking [killer] with [1]",
			"use [1]" , "shoot [1]", "fire [1]", "use [1] on [killer]", "fire [1] at [killer]", "shoot [1] at [killer]", "shoot [killer]", "shoot [killer] with [1]", "shoot [killer] using [1]", "shoot at [killer]", "fire at [killer]",
			"using [1]" , "shooting [1]", "firing [1]", "using [1] on [killer]", "firing [1] at [killer]", "shooting [1] at [killer]", "shooting [killer]", "shooting [killer] with [1]", "shooting [killer] using [1]", "shooting at [killer]", "firing at [killer]",
			"shoot at [killer] with [1]", "shoot at [killer] using [1]", "fire at [killer] with [1]", "fire at [killer] using [1]", "shoot", "fire",
			"shooting at [killer] with [1]", "shooting at [killer] using [1]", "firing at [killer] with [1]", "firing at [killer] using [1]", "shooting", "firing",
			"use [1] again" , "shoot [1] again", "fire [1] again", "use [1] on [killer] again", "fire [1] at [killer] again", "shoot [1] at [killer] again", "shoot [killer] again", "shoot [killer] with [1] again", "shoot [killer] using [1] again", "shoot at [killer] again", "fire at [killer] again",
			"using [1] again" , "shooting [1] again", "firing [1] again", "using [1] on [killer] again", "firing [1] at [killer] again", "shooting [1] at [killer] again", "shooting [killer] again", "shooting [killer] with [1] again", "shooting [killer] using [1] again", "shooting at [killer] again", "firing at [killer] again",
			"shoot at [killer] with [1] again", "shoot at [killer] using [1] again", "fire at [killer] with [1] again", "fire at [killer] using [1] again", "shoot again", "fire again",
			"shooting at [killer] with [1] again", "shooting at [killer] using [1] again", "firing at [killer] with [1] again", "firing at [killer] using [1] again", "shooting", "firing again",
			"run", "run away", "flee", "escape", "running", "running away", "fleeing", "escaping",
			"talk", "speak", "plead", "talk to [killer]", "talk with [killer]", "speak to [killer]", "speak with [killer]", "plead with [killer]", "reason with [killer]",
			"talking", "speaking", "pleading", "talking to [killer]", "talking with [killer]", "speaking to [killer]", "speaking with [killer]", "pleading with [killer]", "reasoning with [killer]",
			"make friends", "make friends with [killer]", "befriend", "befriend [killer]",
			"making friends", "making friends with [killer]", "befriending", "befriending [killer]",
			"fight", "fight [killer]", "fight with [killer]", "fight back", "attack", "attack [killer]", "slap", "slap [killer]", "smack", "smack [killer]", "hit [killer]", "hit", "punch", "punch [killer]", "tussle", "tussle with [killer]", "beat [killer]", "beat [killer] up",
			"fighting", "fighting [killer]", "fighting with [killer]", "fighting back", "attacking", "attacking [killer]", "slapping", "slapping [killer]", "smacking", "smacking [killer]", "hitting [killer]", "hitting", "punching", "punching [killer]", "tussling", "tussling with [killer]", "beating [killer]", "beating [killer] up"});
			dict.Add (1, new List<string> { "use [0]" , "shoot [0]", "fire [0]", "use [0] on [killer]", "fire [0] at [killer]", "shoot [0] at [killer]", "shoot [killer]", "shoot [killer] with [0]",
			"using [0]" , "shooting [0]", "firing [0]", "using [0] on [killer]", "firing [0] at [killer]", "shooting [0] at [killer]", "shooting [killer]", "shooting [killer] with [0]",
			"shoot [killer] using [0]", "shoot at [killer]", "fire at [killer]", "shoot at [killer] with [0]", "shoot at [killer] using [0]", "fire at [killer] with [0]", "fire at [killer] using [0]", "shoot", "fire",
			"shooting [killer] using [0]", "shooting at [killer]", "firing at [killer]", "shooting at [killer] with [0]", "shooting at [killer] using [0]", "firing at [killer] with [0]", "firing at [killer] using [0]", "shooting", "firing",
			"run", "run away", "flee", "escape", "running", "running away", "fleeing", "escaping",
			"talk", "speak", "plead", "talk to [killer]", "talk with [killer]", "speak to [killer]", "speak with [killer]", "plead with [killer]", "reason with [killer]",
			"talking", "speaking", "pleading", "talking to [killer]", "talking with [killer]", "speaking to [killer]", "speaking with [killer]", "pleading with [killer]", "reasoning with [killer]",
			"make friends", "make friends with [killer]", "befriend", "befriend [killer]",
			"making friends", "making friends with [killer]", "befriending", "befriending [killer]",
			"stab [killer]", "use [1]", "use [1] on [killer]", "stab [killer] with [1]",
			"stabbing [killer]", "using [1]", "using [1] on [killer]", "stabbing [killer] with [1]",
			"fight", "fight [killer]", "fight with [killer]", "fight back", "attack", "attack [killer]", "slap", "slap [killer]", "smack", "smack [killer]", "hit [killer]", "hit", "punch", "punch [killer]", "tussle", "tussle with [killer]", "beat [killer]", "beat [killer] up",
			"fighting", "fighting [killer]", "fighting with [killer]", "fighting back", "attacking", "attacking [killer]", "slapping", "slapping [killer]", "smacking", "smacking [killer]", "hitting [killer]", "hitting", "punching", "punching [killer]", "tussling", "tussling with [killer]", "beating [killer]", "beating [killer] up"});
		dict.Add (2, new List<string> { "use [0]", "threaten [0]", "threaten [0] with [1]", "threaten [0] with [2]", "threaten [0] with [3]", "threaten [0] with [4]", "threaten [0] with [5]",
			"using [0]", "threatening [0]", "threatening [0] with [1]", "threatening [0] with [2]", "threatening [0] with [3]", "threatening [0] with [4]", "threatening [0] with [5]",
			"hold [0] hostage", "hold [0] hostage with [1]", "hold [0] hostage with [2]", "hold [0] hostage with [3]", "hold [0] hostage with [4]", "hold [0] hostage with [5]",
			"holding [0] hostage", "holding [0] hostage with [1]", "holding [0] hostage with [2]", "holding [0] hostage with [3]", "holding [0] hostage with [4]", "holding [0] hostage with [5]",
			"take [0] hostage", "take [0] hostage with [1]", "take [0] hostage with [2]", "take [0] hostage with [3]", "take [0] hostage with [4]", "take [0] hostage with [5]",
			"taking [0] hostage", "taking [0] hostage with [1]", "taking [0] hostage with [2]", "taking [0] hostage with [3]", "taking [0] hostage with [4]", "taking [0] hostage with [5]",
			"use [1] with [0]", "use [1] on [0]", "use [1]", "use [1] and [0]", "use [0] with [1]", "use [0] and [1]", "use [0] on [1]",
			"using [1] with [0]", "using [1] on [0]", "using [1]", "using [1] and [0]", "using [0] with [1]", "using [0] and [1]", "using [0] on [1]", 
			"use [2] with [0]", "use [2] on [0]", "use [2]", "use [2] and [0]", "use [0] with [2]", "use [0] and [2]", "use [0] on [2]", 
			"using [2] with [0]", "using [2] on [0]", "using [2]", "using [2] and [0]", "using [0] with [2]", "using [0] and [2]", "using [0] on [2]",
			"use [3] with [0]", "use [3] on [0]", "use [3]", "use [3] and [0]", "use [0] with [3]", "use [0] and [3]", "use [0] on [3]", 
			"using [3] with [0]", "using [3] on [0]", "using [3]", "using [3] and [0]", "using [0] with [3]", "using [0] and [3]", "using [0] on [3]", 
			"use [4] with [0]", "use [4] on [0]", "use [4]", "use [4] and [0]", "use [0] with [4]", "use [0] and [4]", "use [0] on [4]", 
			"using [4] with [0]", "using [4] on [0]", "using [4]", "using [4] and [0]", "using [0] with [4]", "using [0] and [4]", "using [0] on [4]", 
			"use [5] with [0]", "use [5] on [0]", "use [5]", "use [5] and [0]", "use [0] with [5]", "use [0] and [5]", "use [0] on [5]", 
			"using [5] with [0]", "using [5] on [0]", "using [5]", "using [5] and [0]", "using [0] with [5]", "using [0] and [5]", "using [0] on [5]",
			"use [6]", "hack [6]",
			"using [6]", "hacking [6]",
			"run", "run away", "flee", "escape", "running", "running away", "fleeing", "escaping",
			"talk", "speak", "plead", "talk to [killer]", "talk with [killer]", "speak to [killer]", "speak with [killer]", "plead with [killer]", "reason with [killer]",
			"talking", "speaking", "pleading", "talking to [killer]", "talking with [killer]", "speaking to [killer]", "speaking with [killer]", "pleading with [killer]", "reasoning with [killer]",
			"make friends", "make friends with [killer]", "befriend", "befriend [killer]",
			"making friends", "making friends with [killer]", "befriending", "befriending [killer]",
			"stab [killer]", "use [5]", "use [5] on [killer]", "stab [killer] with [5]",
			"stabbing [killer]", "using [5]", "using [5] on [killer]", "stabbing [killer] with [5]",
			"fight", "fight [killer]", "fight with [killer]", "fight back", "attack", "attack [killer]", "slap", "slap [killer]", "smack", "smack [killer]", "hit [killer]", "hit", "punch", "punch [killer]", "tussle", "tussle with [killer]", "beat [killer]", "beat [killer] up",
			"fighting", "fighting [killer]", "fighting with [killer]", "fighting back", "attacking", "attacking [killer]", "slapping", "slapping [killer]", "smacking", "smacking [killer]", "hitting [killer]", "hitting", "punching", "punching [killer]", "tussling", "tussling with [killer]", "beating [killer]", "beating [killer] up"});		dict.Add (3, new List<string> { "[0]", "with [0]", "use with [0]", "use it with [0]", "use [1] with [0]", "combine [0] and [1]", "combine [1] and [0]" });
		dict.Add (4, new List<string> { "[0]" , "with [0]", "use with [0]", "use it with [0]", "[1]", "with [1]", "use with [1]", "use it with [1]", "[2]", "with [2]", "use with [2]", "use it with [2]", "[1] and [2]", "use with [1] and [2]", "use it with [1] and [2]", "with [1] and [2]" });
		dict.Add (5, new List<string> { "[0]" , "with [0]", "use with [0]", "use it with [0]", "[1]", "with [1]", "use with [1]", "use it with [1]", "[2]", "with [2]", "use with [2]", "use it with [2]", "[1] and [2]", "use with [1] and [2]", "use it with [1] and [2]", "with [1] and [2]" });
		dict.Add (6, new List<string> { "lock [0]", "bolt [0]", "latch [0]", "close [0]", "shut [0]", "use [1]", "lock [1]", "lock [3]", "bolt [3]", "latch [3]", "close [3]", "shut [3]",
			"locking [0]", "bolting [0]", "latching [0]", "closing [0]", "shutting [0]", "using [1]", "locking [1]", "locking [3]", "bolting [3]", "latching [3]", "closing [3]", "shutting [3]",
			"lock door", "bolt door", "latch door", "close door", "shut door", "lock shed", "lock shack",
			"locking door", "bolting door", "latching door", "closing door", "shutting door", "locking shed", "locking shack",
			"lock [killer] in", "lock [killer] inside", "lock [killer] in [3]", "lock [killer] inside [3]",
			"locking [killer] in", "locking [killer] inside", "locking [killer] in [3]", "locking [killer] inside [3]",
			"close [killer] in", "close [killer] inside", "close [killer] in [3]", "close [killer] inside [3]",
			"closing [killer] in", "closing [killer] inside", "closing [killer] in [3]", "closing [killer] inside [3]",
			"shut [killer] in", "shut [killer] inside", "shut [killer] in [3]", "shut [killer] inside [3]",
			"shutting [killer] in", "shutting [killer] inside", "shutting [killer] in [3]", "shutting [killer] inside [3]",
			"run", "run away", "flee", "escape", "running", "running away", "fleeing", "escaping",
			"talk", "speak", "plead", "talk to [killer]", "talk with [killer]", "speak to [killer]", "speak with [killer]", "plead with [killer]", "reason with [killer]",
			"talking", "speaking", "pleading", "talking to [killer]", "talking with [killer]", "speaking to [killer]", "speaking with [killer]", "pleading with [killer]", "reasoning with [killer]",
			"make friends", "make friends with [killer]", "befriend", "befriend [killer]",
			"making friends", "making friends with [killer]", "befriending", "befriending [killer]",
			"stab [killer]", "use [2]", "use [2] on [killer]", "stab [killer] with [2]",
			"stabbing [killer]", "using [2]", "using [2] on [killer]", "stabbing [killer] with [2]",
			"fight", "fight [killer]", "fight with [killer]", "fight back", "attack", "attack [killer]", "slap", "slap [killer]", "smack", "smack [killer]", "hit [killer]", "hit", "punch", "punch [killer]", "tussle", "tussle with [killer]", "beat [killer]", "beat [killer] up",
			"fighting", "fighting [killer]", "fighting with [killer]", "fighting back", "attacking", "attacking [killer]", "slapping", "slapping [killer]", "smacking", "smacking [killer]", "hitting [killer]", "hitting", "punching", "punching [killer]", "tussling", "tussling with [killer]", "beating [killer]", "beating [killer] up"});		dict.Add (7, new List<string> { "[0]", "[1]", "[2]", "[3]", "[4]", "call [0]", "call [1]", "call [2]", "call [3]", "call [4]", "dial [0]", "dial [1]", "dial [2]", "dial [3]", "dial [4]" });
		dict.Add (8, new List<string> { "1", "2", "3", "4", "666", "[0]", "[1]", "[2]", "[3]", "[4]", "read [0]", "read [1]", "read [2]", "read [3]", "read [5]", "look [0]", "look [1]", "look [2]", "look [3]", "look [4]"
			,"read 1", "read 2", "read 3", "read 4", "read 666", "look 1", "look 2", "look 3", "look 4", "look 666", "use 1", "use 2", "use 3", "use 4", "use 666", "use [0]", "use [1]", "use [2]", "use [3]", "use [4]",
			"one", "two", "three", "four", "six six six", "six hundred sixty six", "six hundred and sixty six",
			"number 1", "number 2", "number 3", "number 4", "number 666", "no 1", "no 2", "no 3" , "no 4", "no 666" , "no1", "no2", "no3" , "no4", "no666"});
		dict.Add (9, new List<string> { "[0]", "[1]", "basement", "back", "look [0]", "look [1]" });
		dict.Add (10, new List<string> { "[0]", "[1]", "basement", "back", "open [0]", "open [1]" });
		dict.Add (11, new List<string> { "[0]", "[1]", "basement", "back", "close [0]", "close [1]" });
		dict.Add (12, new List<string> { "[0]", "[1]", "basement", "back", "use [0]", "use [1]" });
		dict.Add (13, new List<string> { "[0]", "[1]", "bedroom", "bathroom", "look [0]", "look [1]" });
		dict.Add (14, new List<string> { "[0]", "[1]", "bedroom", "bathroom", "open [0]", "open [1]" });
		dict.Add (15, new List<string> { "[0]", "[1]", "bedroom", "bathroom", "close [0]", "close [1]" });
		dict.Add (16, new List<string> { "[0]", "[1]", "bedroom", "bathroom", "use [0]", "use [1]" });
		dict.Add (17, new List<string> { "[0]", "with [0]", "use with [0]", "use it with [0]", "use [1] with [0]", "combine [0] and [1]", "combine [1] and [0]" });
		dict.Add (18, new List<string> { "[0]", "[1]", "shed", "back", "look [0]", "look [1]" });
		dict.Add (19, new List<string> { "[0]", "[1]", "shed", "back", "open [0]", "open [1]" });
		dict.Add (20, new List<string> { "[0]", "[1]", "shed", "back", "close [0]", "close [1]" });
		dict.Add (21, new List<string> { "[0]", "[1]", "shed", "back", "use [0]", "use [1]" });
		dict.Add (22, new List<string> { "lock [0]", "bolt [0]", "latch [0]", "use [1]", "lock [1]", "latch [1]", "lock [2]", "bolt [2]", "latch [2]",
			"locking [0]", "bolting [0]", "latching [0]", "using [1]", "locking [1]", "latching [1]",  "locking [2]", "bolting [2]", "latching [2]",
			"lock door", "bolt door", "latch door", "lock it", "bolt it", "latch it",
			"locking door", "bolting door", "latching door", "locking it", "bolting it", "latching it",
			"lock [killer] in", "lock [killer] inside", "lock [killer] in [2]", "lock [killer] inside [2]",
			"locking [killer] in", "locking [killer] inside", "locking [killer] in [2]", "locking [killer] inside [2]",
			"bolt [killer] in", "bolt [killer] inside", "bolt [killer] in [2]", "bolt [killer] inside [2]",
			"bolting [killer] in", "bolting [killer] inside", "bolting [killer] in [2]", "bolting [killer] inside [2]",
			"latch [killer] in", "latch [killer] inside", "latch [killer] in [2]", "latch [killer] inside [2]",
			"latching [killer] in", "latching [killer] inside", "latching [killer] in [2]", "latching [killer] inside [2]"});
		dict.Add (23, new List<string> { "[0]", "on [0]", "use [1] on [0]", "use [1] with [0]", "with [0]", "use with [0]", "use it with [0]", "use [0]" });
		dict.Add (24, new List<string> { "[0]", "on [0]", "use [1] on [0]", "use [2] with [0]", "with [0]", "use with [0]", "use it with [0]", "use [0]"  });
		dict.Add (25, new List<string> { "[0]", "on [0]", "use [1] on [0]", "use [3] with [0]", "with [0]", "use with [0]", "use it with [0]", "use [0]"  });
		dict.Add (26, new List<string> { "[0]", "on [0]", "use [1] on [0]", "use [4] with [0]", "with [0]", "use with [0]", "use it with [0]", "use [0]"  });
		dict.Add (27, new List<string> { "[0]", "on [0]", "use [1] on [0]", "use [5] with [0]", "with [0]", "use with [0]", "use it with [0]", "use [0]"  });
		dict.Add (28, new List<string> { "[0]", "[1]", "basement", "back", "lock [0]", "lock [1]" });
		dict.Add (29, new List<string> { "[0]", "[1]", "basement", "back", "unlock [0]", "unlock [1]" });
		dict.Add (30, new List<string> { "[0]", "[1]", "bedroom", "bathroom", "lock [0]", "lock [1]" });
		dict.Add (31, new List<string> { "[0]", "[1]", "bedroom", "bathroom", "unlock [0]", "unlock [1]" });
		dict.Add (32, new List<string> { "[0]", "[1]", "shed", "back", "lock [0]", "lock [1]" });
		dict.Add (33, new List<string> { "[0]", "[1]", "shed", "back", "unlock [0]", "unlock [1]" });
		dict.Add (34, new List<string> { "[1]", "[2]", "[3]", "[4]", "[5]", "use [1]", "use [2]", "use [3]", "use [4]", "use [5]",
			"with [1]", "with [2]", "with [3]", "with [4]", "with [5]", 
			"use [1] with [0]", "use [1] on [0]", "use [1]", "use [0] with [1]", "use [0] on [1]", "use it with [1]", "use it on [1]",
			"use [2] with [0]", "use [2] on [0]", "use [2]", "use [0] with [2]", "use [0] on [2]", "use it with [2]", "use it on [2]",
			"use [3] with [0]", "use [3] on [0]", "use [3]", "use [0] with [3]", "use [0] on [3]", "use it with [3]", "use it on [3]",
			"use [4] with [0]", "use [4] on [0]", "use [4]", "use [0] with [4]", "use [0] on [4]", "use it with [4]", "use it on [4]",
			"use [5] with [0]", "use [5] on [0]", "use [5]", "use [0] with [5]", "use [0] on [5]", "use it with [5]", "use it on [5]"});
		dict.Add (35, new List<string> { "[1]", "use [1]", "with [1]",
			"use [1] with [0]", "use [1] on [0]", "use [1]", "use [0] with [1]", "use [0] on [1]", "use it with [1]", "use it on [1]"});
		dict.Add (36, new List<string> { "[0]", "[1]", "hallway", "hall", "fireplace", "fire place", "furnace", "hearth", "look [0]", "look [1]" });
		dict.Add (37, new List<string> { "[0]", "[1]", "hallway", "hall", "fireplace", "fire place", "furnace", "hearth", "use [0]", "use [1]" });

		for (int i = 0; i < dict.Count; ++i) {
			string itemName = "";

			List<string> items = new List<string>();

			switch (i) {
			case 0:
				items.Add ("knife");
				items.Add ("revolver");
				items.Add ("knife block");
				break;
			case 1:
				items.Add ("revolver");
				items.Add ("knife");
				break;
			case 2:
				items.Add ("teddy bear");
				items.Add ("scalpel");
				items.Add ("spoon");
				items.Add ("scissors");
				items.Add ("knife");
				items.Add ("blender");
				items.Add ("computer");
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
				items.Add ("knife");
				items.Add ("shed");
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
				items.Add ("backyard door");
				items.Add ("basement door");
				break;
			case 10:
				items.Add ("backyard door");
				items.Add ("basement door");
				break;
			case 11:
				items.Add ("backyard door");
				items.Add ("basement door");
				break;
			case 12:
				items.Add ("backyard door");
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
				items.Add ("shed");
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
			case 28:
				items.Add ("basement door");
				items.Add ("backyard door");
				break;
			case 29:
				items.Add ("basement door");
				items.Add ("backyard door");
				break;
			case 30:
				items.Add ("bedroom door");
				items.Add ("bathroom door");
				break;
			case 31:
				items.Add ("bedroom door");
				items.Add ("bathroom door");
				break;
			case 32:
				items.Add ("back door");
				items.Add ("shed door");
				break;
			case 33:
				items.Add ("back door");
				items.Add ("shed door");
				break;
			case 34:
				items.Add ("teddy bear");
				items.Add ("scalpel");
				items.Add ("spoon");
				items.Add ("scissors");
				items.Add ("knife");
				items.Add ("blender");
				break;
			case 35:
				items.Add ("toaster");
				items.Add ("shower");
				break;
			case 36:
				items.Add ("living room staircase");
				items.Add ("secret staircase");
				break;
			case 37:
				items.Add ("living room staircase");
				items.Add ("secret staircase");
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
					|| dictItem.Contains("[5]") || dictItem.Contains("[6]")) {
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
						|| toAddItem.Contains ("[5]") || toAddItem.Contains("[6]")) {
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
					|| toAddItem.Contains ("[5]") || toAddItem.Contains("[6]")) {
					toRemove.Add (toAddItem);
				}
			}

			foreach (var toAddItem in toAddToo) {
				if (toAddItem.Contains ("[0]") || toAddItem.Contains ("[1]") || toAddItem.Contains ("[2]") || toAddItem.Contains ("[3]") || toAddItem.Contains ("[4]")
					|| toAddItem.Contains ("[5]") || toAddItem.Contains("[6]")) {
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

	List<string> GetAltNames(string altNameToFind){
		foreach (KeyValuePair<string, List<string>> entry in altNames)
		{
			if (entry.Key == altNameToFind) {
				List<string> altNamesToReturn = entry.Value;
				altNamesToReturn.Add (entry.Key);
				return altNamesToReturn;
			}       
		}

		return new List<string> ();
	}

	Texture2D GetImageByName(string name)
	{
		return Resources.Load(name) as Texture2D;
	}

	Texture2D GetRandomDeathImage()
	{
		int currRoom = currentRoom.Index;
		if (currRoom == 0 && policeTimer >= policeCap && killerTimer < killerCap4) {
			currRoom = 10;
		}

		if (currRoom == 5 && (shitOnStairs || bucketTrapMade || bearTrapMade || fireTrapMade || blenderTrapMade)){
			currRoom = 11;
		}

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

		if (currOverlay != null) {
			SetOverlay (GetImageByName (currOverlay));
		}
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
		else if (whatToScrub.Contains (toScrub))
			return (whatToScrub.Replace (toScrub, ""));
		else
			return whatToScrub;
	}

	public string ScrubSymbols(string toScrub, string whatToScrub){
			return (whatToScrub.Replace (toScrub, ""));
	}

	void KillerResponseGeneric (string generic){
		if (generic.Contains ("run") || generic.Contains ("flea") || generic.Contains ("escape")) {

			if (currentRoom.Index == 4) {
				var overlays = deathImages [currentRoom.Index];
				SetImage (GetImageByName (overlays.ElementAt (UnityEngine.Random.Range (2, 4))));
				AddText ("You turn around to the window behind you and attempt to prise it open. Before you can crack it more than a few inches however, the killer scrambles frantically across the room and sinks his knife into your back.\n\nPress [ENTER] to try again.");
				GameOverAudio (-1, true);
			}

			if (currentRoom.Index == 1) {
				//var overlays = deathImages [currentRoom.Index];
				//SetImage (GetImageByName (overlays.ElementAt (UnityEngine.Random.Range (0, 2))));
				SetImage (GetImageByName("porchdeath"));
				AddText ("You quickly conclude that the killer is surely no longer in a state to follow you, and you decide to flee. You rush into the living room and throw open the front door. Before your feet can even cross the threshold however, you experience an unpleasant dying sensation. The killer had evidently hobbled out of the kitchen in order to intercept you, and threw a knife square into the back of your neck.\n\nPress [ENTER] to try again.");

				GameOverAudio (-1, true);
			}

			if (currentRoom.Index == 6) {
				string weapon = currOverlay.Split ('-').Last ();
				var overlays = deathImages [currentRoom.Index];
				if (weapon == "gun") {
					SetImage (GetImageByName (overlays.ElementAt (UnityEngine.Random.Range (0, 2))));
					AddText("There isn't anywhere for you to run, as the killer stands in the way of the only exit. You run at him with the intent of pushing past him, but you are shot dead before you take your third step.\n\nPress [ENTER] to try again.");
				} 
				else if (weapon == "katana") {
					SetImage (GetImageByName (overlays.ElementAt (UnityEngine.Random.Range (2, 4))));
					AddText("There isn't anywhere for you to run, as the killer stands in the way of the only exit. You run at him with the intent of pushing past him, but as you do you are separated from a leg with a deft strike of his sword. At this point, you decide to fall over and take a little permanent nap.\n\nPress [ENTER] to try again.");
				}
				else if (weapon == "knife") {
					SetImage (GetImageByName (overlays.ElementAt (UnityEngine.Random.Range (4, 6))));
					AddText("There isn't anywhere for you to run, as the killer stands in the way of the only exit. You run at him with the intent of pushing past him, but you are wounded fatally by a quick stab as you pass.\n\nPress [ENTER] to try again.");
				}
				else {
					SetImage (GetImageByName (overlays.ElementAt (UnityEngine.Random.Range (6, 8))));
					AddText("There isn't anywhere for you to run, as the killer stands in the way of the only exit. You run at him with the intent of pushing past him, but he heaves his mace squarely into your chest, shattering your ribs like they were porcelain. As you perish, you amuse yourself by ridiculing the killer for clearly being a major LARPing nerd. This happens only in your head however, as all you are able to muster are a few bloody gurgles.\n\nPress [ENTER] to try again.");
				}

				GameOverAudio (-1, true);
			}

			if (currentRoom.Index == 7) {
				string weapon = currOverlay.Split ('-').Last ();
				var overlays = deathImages [currentRoom.Index];
				if (weapon == "knife") {
					SetImage (GetImageByName (overlays.ElementAt (UnityEngine.Random.Range (0, 2))));
					AddText ("You flee, as quietly as you can, towards the back door since the fence blocks any other exit. Despite these efforts, the man in the cleansuit detects you, and with nothing impeding his way, he runs towards you. He is unexpectedly fast, and you are quickly overrun. Though you struggle, the killer is still able to carve you like a Christmas ham.\n\nPress [ENTER] to try again.");
				} 
				else {
					SetImage (GetImageByName (overlays.ElementAt (UnityEngine.Random.Range (2, 4))));
					AddText ("You flee, as quietly as you can, towards the back door since the fence blocks any other exit. Despite these efforts, the man in the cleansuit detects you, and with nothing impeding his way, he runs towards you. He is unexpectedly fast, and you are quickly overrun. Though you struggle, the killer is still able to bash you to bits with an honest-to-goodness mace.\n\nPress [ENTER] to try again.");
				}

				GameOverAudio (-1, true);
			}
		}

		if (generic.Contains ("friends") || generic.Contains ("befriend")) {			
			SetImage (GetImageByName ("friends"));
			AddText ("You call out to the intruder and exclaim, \"Hey, can't we just be friends?\" He appears in front of you, out from the shadows, as intimidating as ever. You smile at him. He smiles back, and produces some yarn from inside of his briefcase. Over the next hour you and the man in the cleansuit braid each other friendship bracelets, stealing furtive looks at each other. When you're both done, you exchange them with giddy excitement. BFFs forever.\n\nPress [ENTER] to play again.");

			GameOverAudio (-1, false);
		}

		if (generic.Contains ("attack") || generic.Contains ("fight")) {
			SetImage (GetImageByName ("knockout"));
			AddText ("You run forward, winding your trusty punching arm up for a good strike. You take aim at his stupid face, but before your knuckles make contact, he grabs your fist out of the air with a crushing grip. He brings down his other arm in a powerful blow to your lower neck, causing your knees to buckle in shock and pain. You consider the possibility that trying to fight this man hand-to-hand may have been an enormous mistake as he sends you into permanent unconsciousness.\n\nPress [ENTER] to try again.");

			GameOverAudio (-1, true);
		}

		if (generic.Contains ("talk") || generic.Contains ("reason") || generic.Contains ("plead") || generic.Contains("speak")) {

			if (currentRoom.Index == 4) {
				var overlays = deathImages [currentRoom.Index];
				SetImage (GetImageByName (overlays.ElementAt (UnityEngine.Random.Range (2, 4))));
			}

			if (currentRoom.Index == 1) {
				var overlays = deathImages [currentRoom.Index];
				SetImage (GetImageByName (overlays.ElementAt (UnityEngine.Random.Range (0, 2))));
			}

			if (currentRoom.Index == 6) {
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
			}

			if (currentRoom.Index == 7) {
				string weapon = currOverlay.Split ('-').Last ();
				var overlays = deathImages [currentRoom.Index];
				if (weapon == "knife") {
					SetImage (GetImageByName (overlays.ElementAt (UnityEngine.Random.Range (0, 2))));
				} 
				else {
					SetImage (GetImageByName (overlays.ElementAt (UnityEngine.Random.Range (2, 4))));
				}
			}

			AddText ("You ask the killer who he is, and he does not reply. You ask him what he wants. No reply. You assure him there is no reason to kill you. Still, nothing. As you speak, he glides towards you. You quail. Before he snuffs you out for good, you offer him a cold drink in a last-ditch attempt to calm him down. You think he must be offended by your offer, because he then grabs your right arm and slowly but irresistibly bends it back until it snaps. So much for being hospitable to your guest! Oh yeah, also he kills you.\n\nPress [ENTER] to try again.");
			GameOverAudio (-1, true);
		}

		health = 0;
		ResetOverlay ();
		SetGasMaskOverlay (false);
		killerInKitchen = false;
		killerInBedroom = false;
		killerInLair = false;
		killerInShack = false;
	}

	public void ReadInput(string text)
	{
		RecordPlaytest (text);

		if (inventoryUp) {
			ResetInventory ();

			if (gasMaskOn) {
				SetGasMaskOverlay (true);
			}

			SetOverlay (GetImageByName (currOverlay));
			//SetImage (GetImageByName(currentRoom.currentState.Image));
			inventoryUp = false;
		}

		if (helpScreenUp) {

			if (text == "help") {
				//AddHelpText ("");
				if (!doubleHelp) {
					AddAdditionalHelpText ("\n\nI already helped you!");
					doubleHelp = true;
				}

				return;
			} 
			else {
				AddHelpText ("");

				if (gasMaskOn) {
					SetGasMaskOverlay (true);
				}

				//SetImage (GetImageByName(currentRoom.currentState.Image));

				SetOverlay (GetImageByName (currOverlay));
				doubleHelp = false;
				helpScreenUp = false;
			}
		}

		if (makingTraps) {

			if (!trapsDone) {

				if (text.Contains ("back") || text.Contains ("stop")) {
					makingTraps = false;
					trapItem1 = trapItem2 = trapItem3 = 0;
					trapStep1 = trapStep2 = trapStep3 = false;
					AddText ("");
					Look (null);
					return;
				}

				var toasterNames = GetAltNames ("toaster");
				var knifeNames = GetAltNames ("knife");
				var lighterNames = GetAltNames ("lighter");
				var gasNames = GetAltNames ("gas can");
				var fluidNames = GetAltNames ("lighter fluid");
				var juiceNames = GetAltNames ("orange juice");
				var bucketNames = GetAltNames ("bucket");
				var ammoniaNames = GetAltNames ("ammonia");
				var bleachNames = GetAltNames ("bleach");
				var blenderNames = GetAltNames ("blender");
				var springNames = GetAltNames ("spring");

				int trapItem = 0;

				foreach (var blenderName in blenderNames) {
					if (text.Contains (blenderName)) {
						if (IsInInv (35)) {
							trapItem = 35;
						}
					}
				}

				foreach (var toasterName in toasterNames) {
					if (text.Contains (toasterName)) {
						if (IsInInv (34)) {
							trapItem = 34;
						}
					}
				}

				foreach (var knifeName in knifeNames) {
					if (text.Contains (knifeName)) {
						if (IsInInv (23)) {
							trapItem = 23;
						}
					}
				}

				foreach (var lighterName in lighterNames) {
					if (text.Contains (lighterName)) {
						if (IsInInv (43)) {
							trapItem = 43;
						}
					}
				}

				foreach (var gasName in gasNames) {
					if (text.Contains (gasName) && !text.Contains("mask") && !text.Contains("breather")) {
						if (IsInInv (64)) {
							trapItem = 64;
						}
					}
				}

				foreach (var fluidName in fluidNames) {
					if (text.Contains (fluidName)) {
						if (IsInInv (15)) {
							trapItem = 15;
						}
					}
				}

				foreach (var juiceName in juiceNames) {
					if (text.Contains (juiceName)) {
						if (IsInInv (28)) {
							trapItem = 28;
						}
					}
				}

				foreach (var bucketName in bucketNames) {
					if (text.Contains (bucketName)) {
						if (IsInInv (31)) {
							trapItem = 31;
						}
					}
				}

				foreach (var ammoniaName in ammoniaNames) {
					if (text.Contains (ammoniaName)) {
						if (IsInInv (65)) {
							trapItem = 65;
						}
					}
				}

				foreach (var bleachName in bleachNames) {
					if (text.Contains (bleachName)) {
						if (IsInInv (47)) {
							trapItem = 47;
						}
					}
				}

				foreach (var springName in springNames) {
					if (text.Contains (springName)) {
						if (IsInInv (148)) {
							trapItem = 148;
						}
					}
				}

				WorkbenchStep (trapItem, 0, 0);
				return;
			} else {
				AddText ("You set up your creation on the stairs. Now, you suppose, it's just a matter of hiding and waiting for that bastard to try and come down here to get you.\n\nWhat would you like to do next?");
				var staircase = GetObjectByName ("basement staircase");
				ImageCheckAndShow (staircase.Index, staircase.State, staircase.currentState.Image);
				makingTraps = false;
				trapsDone = false;
				return;
			}

			/*foreach (var x in inventory) {
				if (x.Index == 23 || x.Index == 34 || x.Index == 28 || x.Index == 43 || x.Index == 64 || x.Index == 15 || x.Index == 31 || x.Index == 47 || x.Index == 65) {
					AddAdditionalText (x.Name+"        ");
				}
			}


			if (!trapsDone) {



				Use ("use workbench".Shlex ());
				return;
			}
			else {
				if (trapsAtOnce > 1) {
					AddText ("You set your creations up on the stairs. Now, you suppose, it's just a matter of hiding and waiting for that bastard to try and come down here to get you.");
				}
				else {
					AddText ("You set up your creation on the stairs. Now, you suppose, it's just a matter of hiding and waiting for that bastard to try and come down here to get you.");
				}
				var staircase = GetObjectByName ("basement staircase");
				ImageCheckAndShow (staircase.Index, staircase.State, staircase.currentState.Image);
				makingTraps = false;
				return;
			}*/
		}

		if (!multiSequence) {
			if (killerInBedroom) {
				if (!playerBedroomShot) {
					SetImage (GetImageByName (currentRoom.currentState.Image));
					SetOverlay (GetImageByName("bedr-knife"));

					string killerText = "Next moment, you catch something moving out of the corner of your eye. You jerk your head around and leap backwards, as the form of the man in the cleansuit becomes outlined by the light from your bedside lamp.\n\nWhat do you do next?";

					storedText = killerText;

					AddText (killerText);

					storedText = killerText;

					fadeMusicTrack = true;

					actionTrack.clip = GetClip (65);
					actionTrack.Play ();

					playerBedroomShot = true;
					currLockdownOption = 1;
					inputLockdown = true;
				} else {

					if (killerInKitchen) {
						AddText ("With ringing in your ears, you inch forward, searching for any sign of him, and see a trail of blood on the floor.\n\nWhat will you do next?");
						UpdateRoomState ();
						killerInBedroom = false;
						inputLockdown = false;
						currLockdownOption = 0;
						SetOverlay (GetImageByName ("bedroomblood"));
						if (gasMaskOn) {
							SetGasMaskOverlay (true);
						}
						return;
					}

					if (text == "") {
						return;
					}

					var options = lockdownOptions [currLockdownOption];

					text = ScrubSymbols ("-", text);
					text = ScrubSymbols (".", text);
					text = ScrubSymbols ("'", text);
					text = ScrubSymbols ("#", text);
					text = ScrubSymbols (":", text);
					text = ScrubSymbols (";", text);

					text = ScrubInput ("try and", text);
					text = ScrubInput ("try to", text);
					text = ScrubInput ("attempt to", text);
					text = ScrubInput ("try", text);
					text = ScrubInput ("attempt", text);

					var theRemovalTokens = text.Shlex ();
					for (int i = 0; i < theRemovalTokens.Count; i++) {
						if (theRemovalTokens [i] == "the") {
							theRemovalTokens.Remove (theRemovalTokens [i]);
						}
					}
					//theRemovalTokens.Remove ("the");
					text = String.Join (" ", theRemovalTokens.ToArray());

					if (ConfrontationPause (text)) {
						return;
					}

					foreach (var option in options) {
						if (text == option) {

							foreach (var killerResponseGen in killerResponseGenerics) {
								if (text.Contains (killerResponseGen)) {
									KillerResponseGeneric (killerResponseGen);
									return;
								}
							}

							if (text.Contains ("stab") || text.Contains ("knife")) {
								if (IsInInv (23)) {
									var stabOverlays = deathImages [currentRoom.Index];
									SetImage (GetImageByName (stabOverlays.ElementAt (UnityEngine.Random.Range (2, 4))));
									ResetOverlay ();
									SetGasMaskOverlay (false);
									AddText ("You draw the knife and hold it out in front of you in what you imagine might be a fighting stance. You lurch forward and strike, but the murderer slides effortlessly out of the way. In the same motion, he counters, plunging the knife between your ribs.\n\nThe killer moves fast; you'll only have one opportunity to act decisively in a situation like that.\n\nPress [ENTER] to try again.");

									GameOverAudio (-1, true);

									health = 0;
									killerInBedroom = false;
									return;
								}
							} else {
								BedroomGunshot ();
								return;
							}
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

					GameOverAudio (69, true);

					AddText ("You start to move, but the man in the cleansuit rushes forward. You struggle to hold his arms back but his strength is brutal. He stabs you in the chest with several knives, leaving each one in its place and producing an additional one from inside his suit in turn. It's really a pretty neat pattern, but you aren't around to appreciate it for long.\n\nThe killer moves fast; you'll only have one opportunity to act decisively in a situation like that.\n\nPress [ENTER] to try again.");
					health = 0;
					killerInBedroom = false;
				}
			} else if (killerInKitchen && currentRoom.Index == 1) {

				if (text == "") {
					return;
				}

				var options = lockdownOptions [currLockdownOption];

				text = ScrubSymbols ("-", text);
				text = ScrubSymbols (".", text);
				text = ScrubSymbols ("'", text);
				text = ScrubSymbols ("#", text);
				text = ScrubSymbols (":", text);
				text = ScrubSymbols (";", text);

				text = ScrubInput ("try and", text);
				text = ScrubInput ("try to", text);
				text = ScrubInput ("attempt to", text);
				text = ScrubInput ("try", text);
				text = ScrubInput ("attempt", text);

				var theRemovalTokens = text.Shlex ();
				for (int i = 0; i < theRemovalTokens.Count; i++) {
					if (theRemovalTokens [i] == "the") {
						theRemovalTokens.Remove (theRemovalTokens [i]);
					}
				}
				text = String.Join (" ", theRemovalTokens.ToArray());

				if (ConfrontationPause (text)) {
					return;
				}

				foreach (var option in options) {
					if (text == option) {

						foreach (var killerResponseGen in killerResponseGenerics) {
							if (text.Contains (killerResponseGen)) {
								KillerResponseGeneric (killerResponseGen);
								return;
							}
						}

						KitchenStab (text);
						return;
					}
				}

				string weapon = currOverlay.Split ('-').Last ();
				var overlays = deathImages [currentRoom.Index];
				SetImage (GetImageByName (overlays.ElementAt (UnityEngine.Random.Range (0, 2))));
				ResetOverlay ();
				SetGasMaskOverlay (false);
				AddText ("As you grapple with the assailant, you start to fatigue. Despite his wounded state, the man in the cleansuit still possesses surprising strength. He breaks a hand free of yours, and sticks his knife into your side. The shock grips you and there is a gurgling in your stomach. As you start to black out, you grin wickedly; a fetid gas escapes you. The rank stench reaches your nostrils. You catch a contorted expression on the killer's face before you pass on.\n\nPress [ENTER] to try again.");

				GameOverAudio (-1, true);

				health = 0;
				killerInKitchen = false;
			} 
			else if (killerInLair) {
				if (!playerLairThreaten) {
					SetImage (GetImageByName (currentRoom.currentState.Image));
					SetOverlay (GetRandomDeathOverlay ());

					string killerText = "Suddenly, you hear a loud grinding and sliding, which startles you into hyperarousal. The false panel of the fireplace has moved. You back away towards the other exit, as, to your horror, you see the the feet of the clean suit begin to descend the stairs into the room, followed by the rest of the murderer.\n\nWhat do you do?";

					AddText (killerText);

					storedText = killerText;

					fadeMusicTrack = true;

					actionTrack.clip = GetClip (65);
					actionTrack.Play ();

					playerLairThreaten = true;
					currLockdownOption = 2;
					inputLockdown = true;
				} else {

					if (text == "") {
						return;
					}

					var options = lockdownOptions [currLockdownOption];

					var bearNames = GetAltNames ("teddy bear");

					text = ScrubSymbols ("-", text);
					text = ScrubSymbols (".", text);
					text = ScrubSymbols ("'", text);
					text = ScrubSymbols ("#", text);
					text = ScrubSymbols (":", text);
					text = ScrubSymbols (";", text);

					text = ScrubInput ("try and", text);
					text = ScrubInput ("try to", text);
					text = ScrubInput ("attempt to", text);
					text = ScrubInput ("try", text);
					text = ScrubInput ("attempt", text);

					var theRemovalTokens = text.Shlex ();
					for (int i = 0; i < theRemovalTokens.Count; i++) {
						if (theRemovalTokens [i] == "the") {
							theRemovalTokens.Remove (theRemovalTokens [i]);
						}
					}
					text = String.Join (" ", theRemovalTokens.ToArray());

					if (ConfrontationPause (text)) {
						return;
					}

					foreach (var option in options) {
						if (text == option) {

							if (currLockdownOption == 2) {

								foreach (var killerResponseGen in killerResponseGenerics) {
									if (text.Contains (killerResponseGen)) {
										KillerResponseGeneric (killerResponseGen);
										return;
									}
								}

								if (text.Contains ("scalpel")) {
									if (IsInInv (74)) {
										if (text == "use scalpel" || text == "using scalpel") {
											currLockdownOption = 23;
											AddText ("What do you want to use it with?");
										} else {
											LairThreaten ("scalpel");
										}
									} else {
										break;
									}
								} else if (text.Contains ("spoon")) {
									if (IsInInv (76)) {
										if (text == "use spoon" || text == "using spoon") {
											currLockdownOption = 24;
											AddText ("What do you want to use it with?");
										} else {
											LairThreaten ("spoon");
										}
									} else {
										break;
									}
								} else if (text.Contains ("scissors")) {
									if (IsInInv (75)) {
										if (text == "use scissors" || text == "using scissors") {
											currLockdownOption = 25;
											AddText ("What do you want to use it with?");
										} else {
											LairThreaten ("scissors");
										}
									} else {
										break;
									}
								} else if (text.Contains ("blender")) {
									if (IsInInv (35)) {
										if (text == "use blender" || text == "using blender") {
											currLockdownOption = 26;
											AddText ("What do you want to use it with?");
										} else {
											LairThreaten ("blender");
										}
									} else {
										break;
									}
								} else if (text.Contains ("knife")) {
									if (IsInInv (23)) {
										if (text == "use knife" || text == "using knife") {
											currLockdownOption = 27;
											AddText ("What do you want to use it with?");
										} else {

											foreach (var bearName in bearNames) {
												if (text.Contains (bearName)) {
													LairThreaten ("knife");
												}
											}

											if (IsInInv (23)) {
												var stabOverlays = deathImages [currentRoom.Index];
												SetImage (GetImageByName (stabOverlays.ElementAt (UnityEngine.Random.Range (2, 4))));
												ResetOverlay ();
												SetGasMaskOverlay (false);
												AddText ("You draw the knife and hold it out in front of you in what you imagine might be a fighting stance. You lurch forward and strike, but the murderer slides effortlessly out of the way. In the same motion, he counters, bending your arm down and plunging the knife between your ribs. The man in the cleansuit grabs the teddy bear from your possession and strokes it mechanically as the scene fades from your mind.\n\nPress [ENTER] to try again.");

												GameOverAudio (-1, true);

												health = 0;
												killerInLair = false;
												return;
											}
										}
									} else {
										break;
									}
								} else if (text.Contains ("stab")) {

									if (IsInInv (23)) {
										AddText ("You draw the knife and hold it out in front of you in what you imagine might be a fighting stance. You lurch forward and strike, but the murderer slides effortlessly out of the way. In the same motion, he counters, bending your arm down and plunging the knife between your ribs. The man in the cleansuit grabs the teddy bear from your possession and strokes it mechanically as the scene fades from your mind.\n\nPress [ENTER] to try again.");

										string weaponImg = currOverlay.Split ('-').Last ();
										var stabOverlays = deathImages [currentRoom.Index];
										if (weaponImg == "gun") {
											SetImage (GetImageByName (stabOverlays.ElementAt (UnityEngine.Random.Range (0, 2))));
										} else if (weaponImg == "katana") {
											SetImage (GetImageByName (stabOverlays.ElementAt (UnityEngine.Random.Range (2, 4))));
										} else if (weaponImg == "knife") {
											SetImage (GetImageByName (stabOverlays.ElementAt (UnityEngine.Random.Range (4, 6))));
										} else {
											SetImage (GetImageByName (stabOverlays.ElementAt (UnityEngine.Random.Range (6, 8))));
										}
										ResetOverlay ();
										SetGasMaskOverlay (false);

										GameOverAudio (-1, true);

										health = 0;
										killerInLair = false;
										return;
									}

								} else {

									var computerNames = GetAltNames ("computer");

									foreach (var computerName in computerNames) {
										if (text.Contains (computerName)) {
											LairThreaten ("computer");
											return;
										}
									}

									if (text.Contains ("use") || text.Contains("using")) {
										currLockdownOption = 34;
										AddText ("What do you want to use it with?"); 
									} else {
										LairThreaten ("");
									}
								}
								return;
							}

							else {

								if (currLockdownOption != 34) {
									foreach (var bearName in bearNames) {
										if (text.Contains (bearName)) {

											switch (currLockdownOption) {

											case 23:
												LairThreaten ("scalpel");
												break;
											case 24:
												LairThreaten ("spoon");
												break;
											case 25:
												LairThreaten ("scissors");
												break;
											case 26:
												LairThreaten ("blender");
												break;
											case 27:
												LairThreaten ("knife");
												break;
											}

											return;
										}
									} 
								}
								else 
								{
									if (text.Contains ("scalpel")) {
										if (IsInInv (74)) {
											LairThreaten ("scalpel");
											return;
										}
									} else if (text.Contains ("spoon")) {
										if (IsInInv (76)) {
											LairThreaten ("spoon");
											return;
										}
									} else if (text.Contains ("scissors")) {
										if (IsInInv (75)) {
											LairThreaten ("scissors");
											return;
										}
									} else if (text.Contains ("blender")) {
										if (IsInInv (35)) {
											LairThreaten ("blender");
											return;
										}
									} else if (text.Contains ("knife")) {
										if (IsInInv (23)) {
											LairThreaten ("knife");
											return;
										}
									} 
								}
							}
						}
					}

					string weapon = currOverlay.Split ('-').Last ();
					var overlays = deathImages [currentRoom.Index];
					if (weapon == "gun") {
						SetImage (GetImageByName (overlays.ElementAt (UnityEngine.Random.Range (0, 2))));
						AddText("Before you can get a chance to do that, the man in the cleansuit notices that you are holding the teddy bear. You see an expression of rage pass over his face as he advances towards you, weapon drawn. Falling further into a state of panic, you cast around wildly for some means of defending yourself. Nothing occurs to you except to fling the bear into the killer's face, which bounces uselessly to the floor. The man's expression does not change, except for perhaps a further furrowing of the brow. For your lack of respect to Teddy, the man in the cleansuit shoots you once in each limb before finishing you off.\n\nPress [ENTER] to try again.");
					} 
					else if (weapon == "katana") {
						SetImage (GetImageByName (overlays.ElementAt (UnityEngine.Random.Range (2, 4))));
						AddText("Before you can get a chance to do that, the man in the cleansuit notices that you are holding the teddy bear. You see an expression of rage pass over his face as he advances towards you, weapon drawn. Falling further into a state of panic, you cast around wildly for some means of defending yourself. Nothing occurs to you except to fling the bear into the killer's face, which bounces uselessly to the floor. The man's expression does not change, except for perhaps a further furrowing of the brow. For your lack of respect to Teddy, the man in the cleansuit slices chunks of you off with his katana like you are a rotating shawarma spit.\n\nPress [ENTER] to try again.");
					}
					else if (weapon == "knife") {
						SetImage (GetImageByName (overlays.ElementAt (UnityEngine.Random.Range (4, 6))));
						AddText("Before you can get a chance to do that, the man in the cleansuit notices that you are holding the teddy bear. You see an expression of rage pass over his face as he advances towards you, weapon drawn. Falling further into a state of panic, you cast around wildly for some means of defending yourself. Nothing occurs to you except to fling the bear into the killer's face, which bounces uselessly to the floor. The man's expression does not change, except for perhaps a further furrowing of the brow. For your lack of respect to Teddy, the man in the cleansuit saws your head off with his knife.\n\nPress [ENTER] to try again.");
					}
					else {
						SetImage (GetImageByName (overlays.ElementAt (UnityEngine.Random.Range (6, 8))));
						AddText("Before you can get a chance to do that, the man in the cleansuit notices that you are holding the teddy bear. You see an expression of rage pass over his face as he advances towards you, weapon drawn. Falling further into a state of panic, you cast around wildly for some means of defending yourself. Nothing occurs to you except to fling the bear into the killer's face, which bounces uselessly to the floor. The man's expression does not change, except for perhaps a further furrowing of the brow. For your lack of respect to Teddy, the man in the cleansuit collapses various parts of your flesh with ferocious impacts of his mace.\n\nPress [ENTER] to try again.");
					}
					ResetOverlay ();
					SetGasMaskOverlay (false);

					GameOverAudio (-1, true);

					health = 0;
					killerInLair = false;
				}
			} 
			else if (killerInShack) {

				if (text == "") {
					return;
				}

				var options = lockdownOptions [currLockdownOption];

				text = ScrubSymbols ("-", text);
				text = ScrubSymbols (".", text);
				text = ScrubSymbols ("'", text);
				text = ScrubSymbols ("#", text);
				text = ScrubSymbols (":", text);
				text = ScrubSymbols (";", text);

				text = ScrubInput ("try and", text);
				text = ScrubInput ("try to", text);
				text = ScrubInput ("attempt to", text);
				text = ScrubInput ("try", text);
				text = ScrubInput ("attempt", text);

				var theRemovalTokens = text.Shlex ();
				for (int i = 0; i < theRemovalTokens.Count; i++) {
					if (theRemovalTokens [i] == "the" || theRemovalTokens[i] == "to") {
						theRemovalTokens.Remove (theRemovalTokens [i]);
					}
				}
				text = String.Join (" ", theRemovalTokens.ToArray());

				if (ConfrontationPause (text)) {
					return;
				}

				foreach (var option in options) {
					if (text == option) {

						foreach (var killerResponseGen in killerResponseGenerics) {
							if (text.Contains (killerResponseGen)) {
								KillerResponseGeneric (killerResponseGen);
								return;
							}
						}

						if (text.Contains ("stab") || text.Contains ("knife")) {
							if (IsInInv (23)) {
								var stabOverlays = deathImages [currentRoom.Index];
								SetImage (GetImageByName ("shedstab"));
								ResetOverlay ();
								SetGasMaskOverlay (false);
								AddText ("While the killer's attention is focused on your clever ruse, you silently prepare your knife. You slink carefully forward towards his back and raise the blade to strike. Suddenly, his head moves to the right and his body tenses still, evidently having heard you or caught movement in his peripheral vision. At the same time, you act instinctively and plunge the blade hard into his back. The man in the cleansuit collapses to the floor in pain, gasping and flailing, with the knife stuck firmly in place. After a few moments of horror, his movements cease. After a few more \"just to be sure\" stabs, you clap the dust off your hands and return to your armchair.\n\nPress [ENTER] to play again.");

								GameOverAudio (-1, false);

								health = 0;
								killerInShack = false;
								return;
							}
						} else {
							ShackLock (text);
							return;
						}
					}
				}

				if (text == "move shed" || text == "move shack" || text == "go shed" || text == "go shack" || text == "enter shed" || text == "enter shack") {
					List<string> imageList = deathImages [1];
					ResetOverlay ();
					SetGasMaskOverlay (false);
					SetBasementOverlay (5, false);
					string imageName = imageList [UnityEngine.Random.Range (0, imageList.Count)];
					AddText("You bumble foolishly straight into the shed. The killer has realized that the dummy you constructed is not you and is scanning around the dark looking for you. He stops when he sees you standing there in the shed with him and pauses, in awe of your audacity. He fells you with one slash of his knife.\n\nPress [ENTER] to try again.");
					SetImage (GetImageByName (imageName));
					killerInShack = false;

					GameOverAudio (-1, true);

					health = 0;
					return;
				}

				// Player closed door, didn't lock
				if (currLockdownOption == 22) {
					SetImage (GetImageByName ("shedloss"));
					AddText ("As you begin, you think you notice the door of the shed shift slightly. You attempt to brace the door shut, but the struggle you expect does not arrive. With a loud, \"BANG!\" the killer slams his body full into the shed door, causing it to separate from its hinges. Their combined mass crashes into you, pinning you to the ground. The killer takes pleasure in meticulously carving you up for incurring his wrath.\n\nPress [ENTER] to restart.");

					GameOverAudio (-1, true);
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
					AddText ("Before you can act, you find that the stranger has emerged from the shed, having noticed that the bizarre homunculus you made is not you. He snuffs you out without hesitation.\n\nYou might want to try and do something to keep the killer inside the shed next time.\n\nPress [ENTER] to try again.");

					GameOverAudio (-1, true);
				}

				ResetOverlay ();
				SetGasMaskOverlay (false);

				health = 0;
				killerInShack = false;
			}
			else if (playerHiding) {
				string hideText = "";
				if (image.sprite.name == "bedhide") {
					hideText = "You wait for a few minutes and then decide you've spent enough time under here.";
				} else if (image.sprite.name == "closethide") {
					hideText = "After waiting a few minutes, you decide you've spent enough time in this musty box.";
				} else if (image.sprite.name == "boxes") {
					hideText = "After a few wasted minutes, your back starts to get sore from all this crouching and you leave your hiding place.";
				} else if (image.sprite.name == "shower2") {
					hideText = "After a few minutes of being dripped on by your shower, you exit your hiding spot.";
				}
				Look(null);
				UpdateTimers();
				AddText(hideText+"\n\n");
				AddAdditionalText(textWaiting);
				textWaiting = "";
				playerHiding = false;
				UpdateRoomState ();

				if (playerOutOfTime) {

					if (killerInKitchen) {
						hideKillerInKitchen = true;
					}

					Look (null);
				}

				return;
			}
			else {
				if (health > 0) {
					if (inputLockdown) {
						var options = lockdownOptions [currLockdownOption];

						text = ScrubSymbols ("-", text);
						text = ScrubSymbols (".", text);
						text = ScrubSymbols ("'", text);
						text = ScrubSymbols ("#", text);
						text = ScrubSymbols (":", text);
						text = ScrubSymbols (";", text);
					
						text = ScrubInput ("try and", text);
						text = ScrubInput ("try to", text);
						text = ScrubInput ("attempt to", text);
						text = ScrubInput ("try", text);
						text = ScrubInput ("attempt", text);

						var theRemovalTokens = text.Shlex ();
						for (int i = 0; i < theRemovalTokens.Count; i++) {
							if (theRemovalTokens [i] == "the") {
								theRemovalTokens.Remove (theRemovalTokens [i]);
							}
						}		

						text = String.Join (" ", theRemovalTokens.ToArray());

						foreach (var option in options) {
							if (text == option) {

								if (currLockdownOption == 7 || currLockdownOption == 8 || currLockdownOption == 9 || currLockdownOption == 10
									|| currLockdownOption == 11 || currLockdownOption == 12 || currLockdownOption == 13 || currLockdownOption == 14
									|| currLockdownOption == 15 || currLockdownOption == 16 || currLockdownOption == 18  || currLockdownOption == 19
									|| currLockdownOption == 20 || currLockdownOption == 21 || currLockdownOption == 28 || currLockdownOption == 29
									|| currLockdownOption == 30 || currLockdownOption == 31 || currLockdownOption == 32 || currLockdownOption == 33
									|| currLockdownOption == 34 || currLockdownOption == 35 || currLockdownOption == 36 || currLockdownOption == 37) {
									LockdownResponse (text, true);
									return;
								}

								CombineItems ();
								return;
							}
						}

						if (currLockdownOption == 7 || currLockdownOption == 8 || currLockdownOption == 9 || currLockdownOption == 10
							|| currLockdownOption == 11 || currLockdownOption == 12 || currLockdownOption == 13 || currLockdownOption == 14
							|| currLockdownOption == 15 || currLockdownOption == 16 || currLockdownOption == 18 || currLockdownOption == 19
							|| currLockdownOption == 20 || currLockdownOption == 21 || currLockdownOption == 28 || currLockdownOption == 29
							|| currLockdownOption == 30 || currLockdownOption == 31 || currLockdownOption == 32 || currLockdownOption == 33
							|| currLockdownOption == 34 || currLockdownOption == 35 || currLockdownOption == 36 || currLockdownOption == 37) {
							LockdownResponse (text, false);
							return;
						} else {
							AddText ("Don't think those things go together.");
							inputLockdown = false;
							return;
						}
					} 
					else {
						if (text != "") {	

							if (gasMaskOn && !lockMask) {
								SetGasMaskOverlay (true);
							}
							//Debug.LogFormat ("Running command: {0}", text);

							if (text.Contains ("fuck")) {
								AddText (GenericSwearResponse (true));
								return;
							}

							if (text.Contains ("cunt")) {
								AddText (GenericSwearResponse ());
								return;
							}

							if (IsAllUpper (text)) {
								AddText (GenericCapsResponse ());
								return;
							}

							if (text.Contains("please")) {

								if (text.Contains ("please ")) {
									text = text.Replace ("please ", "");
								} else if (text.Contains (" please")) {
									text = text.Replace(" please", "");
								}

								politePlayer = true;
							}

							if (text.Contains ("make friends") || text.Contains ("befriend") || text.Contains("making friends")) {
								ResetOverlay ();
								SetGasMaskOverlay (false);
								SetImage (GetImageByName ("friends"));
								AddText("You call out to the intruder and exclaim, \"Hey, can't we just be friends?\" He appears in front of you, out from the shadows, as intimidating as ever. You smile at him. He smiles back, and produces some yarn from inside of his briefcase. Over the next hour you and the man in the cleansuit braid each other friendship bracelets, stealing furtive looks at each other. When you're both done, you exchange them with giddy excitement. BFFs forever.\n\nPress [ENTER] to play again.");

								GameOverAudio (-1, false);

								health = 0;
								return;
							}

							text = text.ToLower ();

							text = ScrubSymbols ("-", text);
							text = ScrubSymbols (".", text);
							text = ScrubSymbols ("'", text);
							text = ScrubSymbols ("#", text);
							text = ScrubSymbols (":", text);
							text = ScrubSymbols (";", text);

							var tokens = text.Shlex ();

							//tokens.RemoveAll ("the");

							for (int i = 0; i < tokens.Count; i++) {
								if (tokens [i] == "the") {
									tokens.Remove (tokens [i]);
								}
							}

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

	bool ConfrontationPause (string text) {
		if (text.Contains ("help")) {
			confrontPause = true;
			Help ();
			return true;
		}

		if (text.Contains ("inv") || text.Contains ("pocket")) {
			confrontPause = true;
			ListInventory ();
			return true;
		}

		if (confrontPause) {
			if (text.Contains ("back")) {

				string savedOverlay = storedOverlay;

				SetImage (GetImageByName (storedImage));
				SetOverlay (GetImageByName (savedOverlay));

				confrontPause = false;
				AddText (storedText);

				return true;
			}
		}

		return false;
	}

	void GameOverAudio(int lossToPlay, bool loss) {
		fadeActionTrack = true;
		fadeMusicTrack = true;

		if (lossToPlay != -1) {
			PlayClip (GetClip (lossToPlay));
		}

		if (loss) {

			if (lossToPlay == 61) {
				lossTrack.clip = GetClip (70);
			} 

			else if (lossToPlay == 71 || lossToPlay == 74) {
				lossTrack.clip = GetClip (72);
			}

			else {
				lossTrack.clip = GetClip (66);
			}

			lossTrack.Play ();
		}
	}

	void WorkbenchStep(int itemIndex, int itemIndex2, int itemIndex3) {

		var workbenchItem = itemsList [itemIndex];

		if (itemIndex != 0) {

			if (trapStep3) {

				GameObject workbenchItem3 = null;
				GameObject workbenchItem2 = null;

				if (itemIndex3 != 0) {
					trapItem1 = itemIndex;
					trapItem2 = itemIndex2;
					trapItem3 = itemIndex3;
					workbenchItem2 = itemsList [trapItem2];
					workbenchItem3 = itemsList [trapItem3];
				} else {
					if (itemIndex == trapItem1 || itemIndex == trapItem2) {

						if (itemIndex == trapItem1) {
							var trapObj = itemsList [trapItem1];
							AddText ("You already put the " + trapObj.Name + " on the bench. Your remaining odds and ends are as follows:\n\n");
						} else {
							var trapObj = itemsList [trapItem2];
							AddText ("You already put the " + trapObj.Name + " on the bench. Your remaining odds and ends are as follows:\n\n");
						}

						AddBenchItems (true, true);

						AddAdditionalText ("\n\nWhat is the third item you would like to use?");

						return;
					}

					trapItem3 = itemIndex;
					workbenchItem3 = itemsList [itemIndex];
				}

				if ( (trapItem1 == 65 && trapItem2 == 47 && trapItem3 == 31) || (trapItem1 == 65 && trapItem2 == 31 && trapItem3 == 47) || (trapItem1 == 47 && trapItem2 == 65 && trapItem3 == 31)
					|| (trapItem1 == 47 && trapItem2 == 31 && trapItem3 == 65) || (trapItem1 == 31 && trapItem2 == 65 && trapItem3 == 47) || (trapItem1 == 31 && trapItem2 == 47 && trapItem3 == 65)
					|| (trapItem1 == 65 && trapItem2 == 47 && trapItem3 == 35) || (trapItem1 == 65 && trapItem2 == 35 && trapItem3 == 47) || (trapItem1 == 47 && trapItem2 == 65 && trapItem3 == 35)
					|| (trapItem1 == 47 && trapItem2 == 35 && trapItem3 == 65) || (trapItem1 == 35 && trapItem2 == 65 && trapItem3 == 47) || (trapItem1 == 35 && trapItem2 == 47 && trapItem3 == 65)) {

					trapsDone = true;

					bool trapInBucket = false;

					RemoveFromInv (65);
					RemoveFromInv (47);

					if (trapItem1 == 31 || trapItem2 == 31 || trapItem3 == 31) {
						trapInBucket = true;
						RemoveFromInv (31);
					}
					if (trapItem1 == 35 || trapItem2 == 35 || trapItem3 == 35) {
						RemoveFromInv (35);
					}

					trapItem1 = trapItem2 = trapItem3 = 0;
					trapStep1 = trapStep2 = trapStep3 = false;

					PlayClip (GetClip (78));

					if (gasMaskOn) {

						if (trapInBucket) {
							AddText ("You pour the entire bottle of bleach into the bucket, and pause briefly to confirm to yourself you really want to do this. Then, holding it at an arm's length, you open the bottle of ammonia and empty it into the bleach-filled bucket.\n\nThe gas spreads up into your face but your breathing appears unaffected. You guess this gas mask is working.\n\nPress [ENTER] to continue.");
							ChangeState (99, 1);
							bucketTrapMade = true;
							SetImage (GetImageByName ("buckettrap"));
						}

						else {
							AddText ("You pour the entire bottle of bleach into the blender, and pause briefly to confirm to yourself you really want to do this. Then, holding it at an arm's length, you open the bottle of ammonia and empty it into the bleach-filled blender.\n\nThe gas spreads up into your face but your breathing appears unaffected. You guess this gas mask is working.\n\nPress [ENTER] to continue.");
							ChangeState (140, 1);
							blenderTrapMade = true;
							SetImage (GetImageByName ("blendertrap"));
						}

						return;
					} else {

						if (trapInBucket) {
							AddText ("You pour the entire bottle of bleach into the bucket, and pause briefly to confirm to yourself you really want to do this. Then, holding it at an arm's length, you open the bottle of ammonia and empty it into the bleach-filled bucket.\n\nPress [ENTER] to continue.");
							blenderTrapMade = true;
							SetImage (GetImageByName ("buckettrap"));
							currMultiSequence = 29;
						}

						else {
							AddText ("You pour the entire bottle of bleach into the blender, and pause briefly to confirm to yourself you really want to do this. Then, holding it at an arm's length, you open the bottle of ammonia and empty it into the bleach-filled blender.\n\nPress [ENTER] to continue.");
							blenderTrapMade = true;
							SetImage (GetImageByName ("blendertrap"));
							currMultiSequence = 39;
						}

						ChangeState (-1, 100);
						multiSequence = true;
						makingTraps = false;
						return;
					}
				}

				if ( (trapItem1 == 28 && trapItem2 == 43 && trapItem3 == 64) || (trapItem1 == 28 && trapItem2 == 64 && trapItem3 == 43) || (trapItem1 == 43 && trapItem2 == 28 && trapItem3 == 64)
					|| (trapItem1 == 43 && trapItem2 == 64 && trapItem3 == 28) || (trapItem1 == 64 && trapItem2 == 28 && trapItem3 == 43) || (trapItem1 == 64 && trapItem2 == 43 && trapItem3 == 28)
					|| (trapItem1 == 28 && trapItem2 == 43 && trapItem3 == 15) || (trapItem1 == 28 && trapItem2 == 15 && trapItem3 == 43) || (trapItem1 == 43 && trapItem2 == 28 && trapItem3 == 15)
					|| (trapItem1 == 43 && trapItem2 == 15 && trapItem3 == 28) || (trapItem1 == 15 && trapItem2 == 28 && trapItem3 == 43) || (trapItem1 == 15 && trapItem2 == 43 && trapItem3 == 28)) {

					RemoveFromInv (28);
					RemoveFromInv (43);
					if (trapItem1 == 15 || trapItem2 == 15 || trapItem3 == 15)
						RemoveFromInv (15);
					if (trapItem1 == 64 || trapItem2 == 64 || trapItem3 == 64)
						RemoveFromInv (64);

					trapItem1 = trapItem2 = trapItem3 = 0;
					trapStep1 = trapStep2 = trapStep3 = false;

					trapsDone = true;

					PlayClip (GetClip (77));

					if (playerKnowsFiretrap) {
						AddText ("Okay, in the movie, Johnny Knifeblaster mixed the fuel and orange juice in a can and then fashioned a pressure detonator out of his lighter. Even through your panicked state and intense revere for the Knifeblaster Quadrilogy, you experience a shimmer of just how ridiculous this idea is. Regardless, you push on and assemble the appropriate ingredients.");
					}
					else {
						AddText ("In one of your favorite movies, Glory In His Bosom, protagonist Johnny Knifeblaster made a bomb by mixing fuel and orange juice in a can and then fashioned a pressure detonator out of his lighter. Even through your panicked state and intense revere for the Knifeblaster Quadrilogy, you experience a shimmer of just how ridiculous this idea is. Regardless, you push on and assemble the appropriate ingredients.");
					}

					ChangeState (100, 1);
					fireTrapMade = true;
					SetImage (GetImageByName ("firetrap"));
					AddAdditionalText ("\n\nPress [ENTER] to continue.");
					return;
				}

				if (itemIndex3 != 0) {

					bool twoMatch = false;
					bool noMatch = false;

					switch (trapItem1) {
					case 23:
						if ((trapItem2 == 34 || trapItem2 == 148 || trapItem3 == 34 || trapItem3 == 148) ||
							((trapItem2 == 15 || trapItem3 == 15) && ((trapItem2 == 43 || trapItem3 == 43) || (trapItem2 == 28 || trapItem3 == 28))) || 
							((trapItem2 == 64 || trapItem3 == 64) && ((trapItem2 == 43 || trapItem3 == 43) || (trapItem2 == 28 || trapItem3 == 28))) || 
							((trapItem2 == 28 || trapItem3 == 28) && (trapItem2 == 43 || trapItem3 == 43)) ||
							((trapItem2 == 47 || trapItem3 == 47) && ((trapItem2 == 31 || trapItem3 == 31) || (trapItem2 == 35 || trapItem3 == 35))) || 
							((trapItem2 == 65 || trapItem3 == 65) && ((trapItem2 == 31 || trapItem3 == 31) || (trapItem2 == 35 || trapItem3 == 35))) || 
							((trapItem2 == 65 || trapItem3 == 65) && (trapItem2 == 47 || trapItem3 == 47))) {
							twoMatch = true;
						} else {
							noMatch = true;
						}
						break;
					case 148:
						if ((trapItem2 == 23 || trapItem3 == 23) ||
							((trapItem2 == 15 || trapItem3 == 15) && ((trapItem2 == 43 || trapItem3 == 43) || (trapItem2 == 28 || trapItem3 == 28))) || 
							((trapItem2 == 64 || trapItem3 == 64) && ((trapItem2 == 43 || trapItem3 == 43) || (trapItem2 == 28 || trapItem3 == 28))) || 
							((trapItem2 == 28 || trapItem3 == 28) && (trapItem2 == 43 || trapItem3 == 43)) ||
							((trapItem2 == 47 || trapItem3 == 47) && ((trapItem2 == 31 || trapItem3 == 31) || (trapItem2 == 35 || trapItem3 == 35))) || 
							((trapItem2 == 65 || trapItem3 == 65) && ((trapItem2 == 31 || trapItem3 == 31) || (trapItem2 == 35 || trapItem3 == 35))) || 
							((trapItem2 == 65 || trapItem3 == 65) && (trapItem2 == 47 || trapItem3 == 47))) {
							twoMatch = true;
						} else {
							noMatch = true;
						}
						break;
					case 34:
						if ((trapItem2 == 23 || trapItem3 == 23) ||
							((trapItem2 == 15 || trapItem3 == 15) && ((trapItem2 == 43 || trapItem3 == 43) || (trapItem2 == 28 || trapItem3 == 28))) || 
							((trapItem2 == 64 || trapItem3 == 64) && ((trapItem2 == 43 || trapItem3 == 43) || (trapItem2 == 28 || trapItem3 == 28))) || 
							((trapItem2 == 28 || trapItem3 == 28) && (trapItem2 == 43 || trapItem3 == 43)) ||
							((trapItem2 == 47 || trapItem3 == 47) && ((trapItem2 == 31 || trapItem3 == 31) || (trapItem2 == 35 || trapItem3 == 35))) || 
							((trapItem2 == 65 || trapItem3 == 65) && ((trapItem2 == 31 || trapItem3 == 31) || (trapItem2 == 35 || trapItem3 == 35))) || 
							((trapItem2 == 65 || trapItem3 == 65) && (trapItem2 == 47 || trapItem3 == 47))) {
							twoMatch = true;
						} else {
							noMatch = true;
						}
						break;
					case 28:
						if ((trapItem2 == 43 || trapItem2 == 15 || trapItem2 == 64 || trapItem3 == 43 || trapItem3 == 15 || trapItem3 == 64) ||
							((trapItem2 == 47 || trapItem3 == 47) && ((trapItem2 == 31 || trapItem3 == 31) || (trapItem2 == 35 || trapItem3 == 35))) || 
							((trapItem2 == 65 || trapItem3 == 65) && ((trapItem2 == 31 || trapItem3 == 31) || (trapItem2 == 35 || trapItem3 == 35))) || 
							((trapItem2 == 65 || trapItem3 == 65) && (trapItem2 == 47 || trapItem3 == 47)) ||
							((trapItem2 == 23 || trapItem3 == 23) && ((trapItem2 == 148 || trapItem3 == 148) || (trapItem2 == 34 || trapItem3 == 34)))) {
							twoMatch = true;
						} else {
							noMatch = true;
						}
						break;
					case 43:
						if ((trapItem2 == 28 || trapItem2 == 15 || trapItem2 == 64 || trapItem3 == 28 || trapItem3 == 15 || trapItem3 == 64) ||
							((trapItem2 == 47 || trapItem3 == 47) && ((trapItem2 == 31 || trapItem3 == 31) || (trapItem2 == 35 || trapItem3 == 35))) || 
							((trapItem2 == 65 || trapItem3 == 65) && ((trapItem2 == 31 || trapItem3 == 31) || (trapItem2 == 35 || trapItem3 == 35))) || 
							((trapItem2 == 65 || trapItem3 == 65) && (trapItem2 == 47 || trapItem3 == 47)) ||
							((trapItem2 == 23 || trapItem3 == 23) && ((trapItem2 == 148 || trapItem3 == 148) || (trapItem2 == 34 || trapItem3 == 34)))) {
							twoMatch = true;
						} else {
							noMatch = true;
						}
						break;
					case 15:
						if ((trapItem2 == 28 || trapItem2 == 43 || trapItem2 == 28 || trapItem3 == 43) ||
							((trapItem2 == 47 || trapItem3 == 47) && ((trapItem2 == 31 || trapItem3 == 31) || (trapItem2 == 35 || trapItem3 == 35))) || 
							((trapItem2 == 65 || trapItem3 == 65) && ((trapItem2 == 31 || trapItem3 == 31) || (trapItem2 == 35 || trapItem3 == 35))) || 
							((trapItem2 == 65 || trapItem3 == 65) && (trapItem2 == 47 || trapItem3 == 47)) ||
							((trapItem2 == 23 || trapItem3 == 23) && ((trapItem2 == 148 || trapItem3 == 148) || (trapItem2 == 34 || trapItem3 == 34)))) {
							twoMatch = true;
						} else {
							noMatch = true;
						}
						break;
					case 64:
						if ((trapItem2 == 28 || trapItem2 == 43 || trapItem3 == 28 || trapItem3 == 43) ||
							((trapItem2 == 47 || trapItem3 == 47) && ((trapItem2 == 31 || trapItem3 == 31) || (trapItem2 == 35 || trapItem3 == 35))) || 
							((trapItem2 == 65 || trapItem3 == 65) && ((trapItem2 == 31 || trapItem3 == 31) || (trapItem2 == 35 || trapItem3 == 35))) || 
							((trapItem2 == 65 || trapItem3 == 65) && (trapItem2 == 47 || trapItem3 == 47)) ||
							((trapItem2 == 23 || trapItem3 == 23) && ((trapItem2 == 148 || trapItem3 == 148) || (trapItem2 == 34 || trapItem3 == 34)))) {
							twoMatch = true;
						} else {
							noMatch = true;
						}
						break;
					case 31:
						if ((trapItem2 == 47 || trapItem2 == 65 || trapItem3 == 47 || trapItem3 == 65) ||
							((trapItem2 == 15 || trapItem3 == 15) && ((trapItem2 == 43 || trapItem3 == 43) || (trapItem2 == 28 || trapItem3 == 28))) || 
							((trapItem2 == 64 || trapItem3 == 64) && ((trapItem2 == 43 || trapItem3 == 43) || (trapItem2 == 28 || trapItem3 == 28))) || 
							((trapItem2 == 28 || trapItem3 == 28) && (trapItem2 == 43 || trapItem3 == 43)) ||
							((trapItem2 == 23 || trapItem3 == 23) && ((trapItem2 == 148 || trapItem3 == 148) || (trapItem2 == 34 || trapItem3 == 34)))) {
							twoMatch = true;
						} else {
							noMatch = true;
						}
						break;
					case 35:
						if ((trapItem2 == 47 || trapItem2 == 65 || trapItem3 == 47 || trapItem3 == 65) ||
							((trapItem2 == 15 || trapItem3 == 15) && ((trapItem2 == 43 || trapItem3 == 43) || (trapItem2 == 28 || trapItem3 == 28))) || 
							((trapItem2 == 64 || trapItem3 == 64) && ((trapItem2 == 43 || trapItem3 == 43) || (trapItem2 == 28 || trapItem3 == 28))) || 
							((trapItem2 == 28 || trapItem3 == 28) && (trapItem2 == 43 || trapItem3 == 43)) ||
							((trapItem2 == 23 || trapItem3 == 23) && ((trapItem2 == 148 || trapItem3 == 148) || (trapItem2 == 34 || trapItem3 == 34)))) {
							twoMatch = true;
						} else {
							noMatch = true;
						}
						break;
					case 47:
						if ((trapItem2 == 31 || trapItem2 == 65 || trapItem2 == 35 || trapItem3 == 31 || trapItem3 == 65 || trapItem3 == 35) ||
							((trapItem2 == 15 || trapItem3 == 15) && ((trapItem2 == 43 || trapItem3 == 43) || (trapItem2 == 28 || trapItem3 == 28))) || 
							((trapItem2 == 64 || trapItem3 == 64) && ((trapItem2 == 43 || trapItem3 == 43) || (trapItem2 == 28 || trapItem3 == 28))) || 
							((trapItem2 == 28 || trapItem3 == 28) && (trapItem2 == 43 || trapItem3 == 43)) ||
							((trapItem2 == 23 || trapItem3 == 23) && ((trapItem2 == 148 || trapItem3 == 148) || (trapItem2 == 34 || trapItem3 == 34)))) {
							twoMatch = true;
						} else {
							noMatch = true;
						}
						break;
					case 65:
						if ((trapItem2 == 31 || trapItem2 == 47 || trapItem2 == 35 || trapItem3 == 31 || trapItem3 == 47 || trapItem3 == 35) ||
							((trapItem2 == 15 || trapItem3 == 15) && ((trapItem2 == 43 || trapItem3 == 43) || (trapItem2 == 28 || trapItem3 == 28))) || 
							((trapItem2 == 64 || trapItem3 == 64) && ((trapItem2 == 43 || trapItem3 == 43) || (trapItem2 == 28 || trapItem3 == 28))) || 
							((trapItem2 == 28 || trapItem3 == 28) && (trapItem2 == 43 || trapItem3 == 43)) ||
							((trapItem2 == 23 || trapItem3 == 23) && ((trapItem2 == 148 || trapItem3 == 148) || (trapItem2 == 34 || trapItem3 == 34)))) {
							twoMatch = true;
						} else {
							noMatch = true;
						}
						break;
					}

					if (twoMatch) {
						AddText ("Hmm, one of those items doesn't seem to go with the other two.");
					}

					if (noMatch) {
						AddText ("It doesn't look like any of these items go together.");
					}

					workbenchOverlay.sprite = Sprite.Create (GetImageByName ("left-" + workbenchItem.Name), workbenchOverlay.sprite.rect, workbenchOverlay.sprite.pivot);
					workbenchOverlay2.sprite = Sprite.Create (GetImageByName ("center-" + workbenchItem2.Name), workbenchOverlay2.sprite.rect, workbenchOverlay2.sprite.pivot);
				} else {
					AddText ("Nah, that last item doesn't seem like it goes with the first two.");
				}

				trapItem1 = trapItem2 = trapItem3 = 0;
				trapStep1 = trapStep2 = trapStep3 = false;

				workbenchOverlay3.sprite = Sprite.Create (GetImageByName ("right-" + workbenchItem3.Name), workbenchOverlay3.sprite.rect, workbenchOverlay3.sprite.pivot);

				makingTraps = false;
				return;

			} 

			else if (trapStep2) {

				GameObject workbenchItem2 = null;

				if (itemIndex2 != 0) {

					trapItem1 = itemIndex;
					trapItem2 = itemIndex2;
					workbenchItem2 = itemsList [itemIndex2];
				} else {		
					if (itemIndex == trapItem1) {

						AddText ("You already put the " + workbenchItem.Name + " on the bench. Your remaining odds and ends are as follows:\n\n");

						//workbenchOverlay.sprite = Sprite.Create (GetImageByName ("left-" + workbenchItem.Name), workbenchOverlay.sprite.rect, workbenchOverlay.sprite.pivot);

						AddBenchItems (true, false);

						AddAdditionalText ("\n\nWhat is the second item you would like to use?");

						return;
					}

					trapItem2 = itemIndex;
					workbenchItem2 = itemsList [itemIndex];
				}

				trapStep3 = true;

				bool wrongItems = false;
				bool bearTrap = false;
				bool bedSpring = false;
				bool springBucket = false;

				switch (trapItem1) {
				case 23:
					if (trapItem2 == 31) {
						springBucket = true;
					}

					else if (trapItem2 == 34 || trapItem2 == 148) {
						if (trapItem2 == 148) {
							bedSpring = true;
						}
						bearTrap = true;
					} else {
						wrongItems = true;
					}
					break;
				case 34:

					if (trapItem2 == 31) {
						springBucket = true;
					}
					else if (trapItem2 == 23) {
						bearTrap = true;
					} else {
						wrongItems = true;
					}
					break;
				case 148:
					if (trapItem2 == 31) {
						springBucket = true;
					}

					else if (trapItem2 == 23) {
						bedSpring = true;
						bearTrap = true;
					} else {
						wrongItems = true;
					}
					break;
				case 28:
					if (trapItem2 != 43 && trapItem2 != 15 && trapItem2 != 64) {
						wrongItems = true;
					}
					break;
				case 43:
					if (trapItem2 != 28 && trapItem2 != 15 && trapItem2 != 64) {
						wrongItems = true;
					}
					break;
				case 15:
					if (trapItem2 != 28 && trapItem2 != 43) {
						wrongItems = true;
					}
					break;
				case 64:
					if (trapItem2 != 28 && trapItem2 != 43) {
						wrongItems = true;
					}
					break;
				case 31:
					if (trapItem2 == 148 || trapItem2 == 34) {
						springBucket = true;
					}

					else if (trapItem2 != 47 && trapItem2 != 65) {
						wrongItems = true;
					}
					break;
				case 35:
					if (trapItem2 != 47 && trapItem2 != 65) {
						wrongItems = true;
					}
					break;
				case 47:
					if (trapItem2 != 31 && trapItem2 != 65 && trapItem2 != 35) {
						wrongItems = true;
					}
					break;
				case 65:
					if (trapItem2 != 31 && trapItem2 != 47 && trapItem2 != 35) {
						wrongItems = true;
					}
					break;
				}

				if (bearTrap) {

					trapItem1 = trapItem2 = 0;
					trapStep1 = trapStep2 = trapStep3 = false;

					trapsDone = true;

					if (playerKnowsBeartrap) {
						AddText ("You bash the toaster against the counter, separating it into many pieces. Using what you learned from the home bear survival guide, you get to work. You extract pieces of the frame and springing mechanism and firmly attach the knife to it, creating a trap...of sorts. You aren't sure if it would work against a bear, but it might mangle that man's leg.");
					} else {
						AddText ("You bash the toaster against the counter, separating it into many pieces. Using what you learned from a bear attack home survival guide, you get to work. You extract pieces of the frame and springing mechanism and firmly attach the knife to it, creating a trap...of sorts. You aren't sure if it would work against a bear, but it might mangle that man's leg.");
					}

					if (bedSpring) {
						RemoveFromInv (148);
					} else {
						RemoveFromInv (34);
					}

					PlayClip (GetClip (77));

					RemoveFromInv (23);
					ChangeState (98, 1); 
					bearTrapMade = true;
					SetImage (GetImageByName ("beartrap"));

					AddAdditionalText ("\n\nPress [ENTER] to continue.");
					return;
				}

				if (springBucket) {
					trapItem1 = trapItem2 = 0;
					trapStep1 = trapStep2 = trapStep3 = false;

					makingTraps = false;

					AddText ("Maybe if you lived in a cartoon you could launch a pie at the killer’s face with this thing, but sadly it just doesn’t work that way.");
					SetImage (GetImageByName ("springbucket"));

					return;
				}

				if (wrongItems) {

					trapItem1 = trapItem2 = 0;
					trapStep1 = trapStep2 = trapStep3 = false;

					makingTraps = false;

					// FIX THIS
					if (workbenchOverlay.sprite.name == "" && (workbenchItem.Name != workbenchItem2.Name)) {
						workbenchOverlay.sprite = Sprite.Create (GetImageByName ("left-" + workbenchItem.Name), workbenchOverlay.sprite.rect, workbenchOverlay.sprite.pivot);
					}

					workbenchOverlay2.sprite = Sprite.Create (GetImageByName ("center-" + workbenchItem2.Name), workbenchOverlay2.sprite.rect, workbenchOverlay2.sprite.pivot);

					AddText ("No, I don't think that combination of items will be of much use.");
					return;
				}

				int numItems = 0;

				foreach (var x in inventory) {
					if (x.Index == 23 || x.Index == 34 || x.Index == 148 || x.Index == 28 || x.Index == 43 || x.Index == 64 || x.Index == 15 || x.Index == 31 || x.Index == 47 || x.Index == 65 || x.Index == 35) {
						if (IsInInv(x.Index) && x.Index != trapItem1 && x.Index != trapItem2) {
							numItems++;
						}
					}
				}

				if (numItems >= 1) {

					if (itemIndex2 != 0) {
						AddText ("You add the " + workbenchItem.Name + " and the " + workbenchItem2.Name + " to the workbench. Your remaining odds and ends are as follows:\n\n");
						workbenchOverlay.sprite = Sprite.Create (GetImageByName ("left-" + workbenchItem.Name), workbenchOverlay.sprite.rect, workbenchOverlay.sprite.pivot);
						workbenchOverlay2.sprite = Sprite.Create (GetImageByName ("center-" + workbenchItem2.Name), workbenchOverlay2.sprite.rect, workbenchOverlay2.sprite.pivot);
					} else {
						AddText ("You add the " + workbenchItem.Name + " to the workbench. Your remaining odds and ends are as follows:\n\n");
						workbenchOverlay2.sprite = Sprite.Create (GetImageByName ("center-" + workbenchItem.Name), workbenchOverlay2.sprite.rect, workbenchOverlay2.sprite.pivot);
					}

					AddBenchItems (true, true);

					AddAdditionalText ("\n\nWhat is the third item you would like to use?");
				} 
				else {
					trapItem1 = trapItem2 = 0;
					trapStep1 = trapStep2 = trapStep3 = false;

					makingTraps = false;

					if (itemIndex2 != 0) {
						AddText ("You add the " + workbenchItem.Name + " and the " + workbenchItem2.Name + " to the workbench.\n\nHmm, not enough components to finish this trap.");
						workbenchOverlay.sprite = Sprite.Create (GetImageByName ("left-" + workbenchItem.Name), workbenchOverlay.sprite.rect, workbenchOverlay.sprite.pivot);
						workbenchOverlay2.sprite = Sprite.Create (GetImageByName ("center-" + workbenchItem2.Name), workbenchOverlay2.sprite.rect, workbenchOverlay2.sprite.pivot);
					} else {
						AddText ("You add the " + workbenchItem.Name + " to the workbench.\n\nHmm, not enough components to finish this trap.");
						workbenchOverlay2.sprite = Sprite.Create (GetImageByName ("center-" + workbenchItem.Name), workbenchOverlay2.sprite.rect, workbenchOverlay2.sprite.pivot);
					}

					return;
				}
			} 
			else {
				trapItem1 = itemIndex;
				trapStep2 = true;

				AddText ("You add the " + workbenchItem.Name + " to the workbench. Your remaining odds and ends are as follows:\n\n");

				workbenchOverlay.sprite = Sprite.Create (GetImageByName ("left-" + workbenchItem.Name), workbenchOverlay.sprite.rect, workbenchOverlay.sprite.pivot);

				AddBenchItems (true, false);

				AddAdditionalText ("\n\nWhat is the second item you would like to use?");
			}
		}


		else {
			AddText ("That's not one of the things you can use. Your remaining odds and ends are as follows:\n\n");

			AddBenchItems (true, true);

			if (trapItem2 != 0) {
				AddAdditionalText ("\n\nWhat is the third item you would like to use?");
			} 
			else {
				if (trapItem1 != 0) {
					AddAdditionalText ("\n\nWhat is the second item you would like to use?");
				} else {
					AddAdditionalText ("\n\nWhat is the first item you would like to use?");
				}
			}
		}
	}

	void AddBenchItems(bool item1, bool item2) {

		int numPrintedItems = 0;
		int spaceLength = 0;
		string line = "";
		string spaces = "";
		bool addSpaces = false;

		foreach (var x in inventory) {
			if (x.Index == 23 || x.Index == 34 || x.Index == 148 || x.Index == 28 || x.Index == 43 || x.Index == 64 || x.Index == 15 || x.Index == 31 || x.Index == 47 || x.Index == 65 || x.Index == 35) {
				if (!item1 && !item2) {			
					if (addSpaces) {
						addSpaces = false;
						AddAdditionalText ("\n");
					}

					if (numPrintedItems == 1) {
						spaceLength = 20 - line.Length;
					}

					if (numPrintedItems == 2) {
						spaceLength = 40 - line.Length;
					}

					if (numPrintedItems == 3) {
						spaceLength = 60 - line.Length;
					}

					if (numPrintedItems == 4) {
						spaceLength = 80 - line.Length;
					}

					for (int i = 0; i < spaceLength; ++i) {
						spaces += " ";
					}

					if (numPrintedItems == 0) {
						AddAdditionalText (x.Name);
						line += x.Name;
					}

					if (numPrintedItems == 1 || numPrintedItems == 2 || numPrintedItems == 3 || numPrintedItems == 4) {
						AddAdditionalText (spaces + x.Name);
						line += spaces + x.Name;
					}

					numPrintedItems++;
					spaces = "";

					if (numPrintedItems == 5) {
						addSpaces = true;
						numPrintedItems = 0;
						line = "";
					}
				}

				if (item1 && !item2) {
					if (x.Index != trapItem1) {
						if (addSpaces) {
							addSpaces = false;
							AddAdditionalText ("\n");
						}

						if (numPrintedItems == 1) {
							spaceLength = 20 - line.Length;
						}

						if (numPrintedItems == 2) {
							spaceLength = 40 - line.Length;
						}

						if (numPrintedItems == 3) {
							spaceLength = 60 - line.Length;
						}

						if (numPrintedItems == 4) {
							spaceLength = 80 - line.Length;
						}

						for (int i = 0; i < spaceLength; ++i) {
							spaces += " ";
						}

						if (numPrintedItems == 0) {
							AddAdditionalText (x.Name);
							line += x.Name;
						}

						if (numPrintedItems == 1 || numPrintedItems == 2 || numPrintedItems == 3 || numPrintedItems == 4) {
							AddAdditionalText (spaces + x.Name);
							line += spaces + x.Name;
						}

						numPrintedItems++;
						spaces = "";

						if (numPrintedItems == 5) {
							addSpaces = true;
							numPrintedItems = 0;
							line = "";
						}

					}
				}

				if (item1 && item2) {
					if (x.Index != trapItem1 && x.Index != trapItem2) {
						if (addSpaces) {
							addSpaces = false;
							AddAdditionalText ("\n");
						}

						if (numPrintedItems == 1) {
							spaceLength = 20 - line.Length;
						}

						if (numPrintedItems == 2) {
							spaceLength = 40 - line.Length;
						}

						if (numPrintedItems == 3) {
							spaceLength = 60 - line.Length;
						}

						if (numPrintedItems == 4) {
							spaceLength = 80 - line.Length;
						}

						for (int i = 0; i < spaceLength; ++i) {
							spaces += " ";
						}

						if (numPrintedItems == 0) {
							AddAdditionalText (x.Name);
							line += x.Name;
						}

						if (numPrintedItems == 1 || numPrintedItems == 2 || numPrintedItems == 3 || numPrintedItems == 4) {
							AddAdditionalText (spaces + x.Name);
							line += spaces + x.Name;
						}

						numPrintedItems++;
						spaces = "";

						if (numPrintedItems == 5) {
							addSpaces = true;
							numPrintedItems = 0;
							line = "";
						}
					}
				}
			}
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

				GameOverAudio (-1, false);

				health = 0;
				AddAdditionalText ("\n\nPress [ENTER] to play again.");
				return;
			} else {
				// LOSE
				if (killerTimer >= killerCap4 && !killerInKitchen) {
					AddAdditionalText ("\n\n"+GetTimeOutText());
				}
				multiSequence = false;

				GameOverAudio (-1, true);

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

		if (!ambienceInProg) {
			soundFXTimer -= 1;
		}

		if (fadeMusicTrack) {

			if (musicTrack.volume > 0) {
				musicTrack.volume -= .02f;
			} else {
				musicTrack.Stop ();
				musicTrack.volume = 1;
				fadeMusicTrack = false;
			}
		}

		if (fadeActionTrack) {

			if (actionTrack.volume > 0) {
				actionTrack.volume -= .02f;
			} else {
				actionTrack.Stop ();
				actionTrack.volume = 1;
				fadeActionTrack = false;
			}
		}

		if (soundFXTimer <= 0) {

			int playSound = UnityEngine.Random.Range(0, 2);

			if (playSound == 1) {
				PlayAmbientClip (GetClip (GetAmbientNoise ()));
			}

			soundFXTimer = UnityEngine.Random.Range(1800, 2400);
		}

		/*if (!musicTrack.isPlaying && !decrementInterlude && !ambienceInProg && es.currentSelectedGameObject != null && selectedField != null) {
			RecordPlaytest ("TIME FOR AMBIENCE BITCH");
			int lastTrack = audioTracks.First ();
			audioTracks.Remove (lastTrack);
			audioTracks.Add (lastTrack);

			decrementInterlude = true;
			ambienceInProg = true;
		}*/

		if (trackLength < 0 && trackLength != null && !decrementInterlude && !ambienceInProg) {
			int lastTrack = audioTracks.First ();
			audioTracks.Remove (lastTrack);
			audioTracks.Add (lastTrack);

			decrementInterlude = true;
			ambienceInProg = true;
		}

		if (decrementInterlude) {
			trackInterludeTime -= 1;
			ambientSubTimer -= 1;
		}

		if (ambientSubTimer <= 0) {

			int playSound = UnityEngine.Random.Range(0, 2);

			if (playSound == 1) {
				PlayAmbientClip (GetClip (GetRealAmbientNoise ()));
			}

			ambientSubTimer = UnityEngine.Random.Range(1200, 1600);
		}

		if (trackInterludeTime <= 0 && ambienceInProg && !ambientSource.isPlaying) {
			PlayMusicTrack (GetClip(audioTracks.First ()));
			trackLength = GetClip (audioTracks.First ()).length * 60;
			ambienceInProg = false;
		}

		if (trackInterludeTime <= 0 && !ambienceInProg && !ambientSource.isPlaying) {
			decrementInterlude = false;
			trackInterludeTime = UnityEngine.Random.Range (1800, 2400);
		}

		if (musicTrack.isPlaying && !actionTrack.isPlaying) {
			trackLength--;
		}

		if (es.currentSelectedGameObject == null && selectedField != null) {
			es.SetSelectedGameObject (selectedField);
		} else {
			selectedField = es.currentSelectedGameObject;
		}

		inputText.MoveTextEnd (false);
	}

	public void RandomizeTracks(){
		audioTracks = new List<int> ();
		trackNumbers = new List<int> ();
		trackNumbers.Add (13);
		trackNumbers.Add (30);
		trackNumbers.Add (31);
		trackNumbers.Add (63);
		trackNumbers.Add (64);

		for (int x = 0; x < trackNumbers.Count; ++x) {
			int numToGet = trackNumbers[UnityEngine.Random.Range (0, trackNumbers.Count)];
			audioTracks.Add (numToGet);
			trackNumbers.Remove (numToGet);
		}

		audioTracks.Add (trackNumbers.First ());
	}

	public void ResetHouse()
	{
		AltNamesParser altNamesParser = gameObject.GetComponent(typeof(AltNamesParser)) as AltNamesParser;
		TextAsset altNamesText = altNamesParser.xmlDocument;
		XmlDocument altNamesDoc = new XmlDocument();
		altNamesDoc.LoadXml(altNamesText.text);
		altNames = altNamesParser.ReadXML(altNamesDoc);

		SetupHouse();
		//AddText (GetResetText ());

		actionTrack.Stop ();

		AddText("");
		PlayKnockingClip (GetClip (UnityEngine.Random.Range (27, 30)));
		RandomizeTracks ();
		PlayMusicTrack (GetClip (audioTracks.First()  ));
		trackLength = GetClip (audioTracks.First ()).length * 60;
		AddAdditionalText ("You recline in your easy chair. It is late and your living room is lit only by harsh fluorescent light from the lamp behind you. There is a slight draft in the room that chills you, and the thought of your warm bed begins to form in your mind. Suddenly, there is a pounding at the front door, feet from you, causing you to jolt. As your heart races, you think, \"Who could that be?\" You suppose you had better take a look.\n\nType HELP and press [ENTER] for some guidance.");
	}

	public void AddText(string txt)
	{
		string textToAdd = "";

		if (politePlayer && !inventoryUp && !helpScreenUp) {
			textToAdd = GenericPlease () + "\n\n" + txt;
		} else {
			textToAdd = txt;
		}

		politePlayer = false;

		//text.StartReveal("\n" + txt + "\n");

		textToAdd = textToAdd.Replace ("[NEWLINE]", "\r");
		textToAdd = textToAdd.Replace ("[DOUBLENEWLINE]", "\n\n");
		textToAdd = textToAdd.Replace("HELP", "<color=#08FF00>help</color>");
		textToAdd = textToAdd.Replace("[ENTER]", "<color=#ed716d>[ENTER]</color>");


		text.AppendText(textToAdd);
	}

	public void AddAdditionalText(string txt)
	{
		txt = txt.Replace ("[NEWLINE]", "\r");
		txt = txt.Replace ("[DOUBLENEWLINE]", "\n\n");
		txt = txt.Replace("HELP", "<color=#08FF00>help</color>");
		txt = txt.Replace("[ENTER]", "<color=#ed716d>[ENTER]</color>");
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
		workbenchOverlay.sprite = Sprite.Create (GetImageByName ("blankoverlay"), workbenchOverlay.sprite.rect, workbenchOverlay.sprite.pivot);
		workbenchOverlay2.sprite = Sprite.Create (GetImageByName ("blankoverlay"), workbenchOverlay2.sprite.rect, workbenchOverlay2.sprite.pivot);
		workbenchOverlay3.sprite = Sprite.Create (GetImageByName ("blankoverlay"), workbenchOverlay3.sprite.rect, workbenchOverlay3.sprite.pivot);


		if (currentRoom.Index == 5 && currentRoom.currentState.Image == tex.name) {
			if (bearTrapMade || bucketTrapMade || fireTrapMade || shitOnStairs || blenderTrapMade) {
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

				if (blenderTrapMade) {
					SetBasementOverlay (4, true);
				}
			}
		}
		else {
			SetBasementOverlay (5, false);
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

		if (tex.name == "orangejuice" || tex.name == "icecubetrays" || tex.name == "flashlight" || tex.name == "bleach" || tex.name == "bucket"
			|| tex.name == "phone3" || tex.name == "phone4" || tex.name == "phone5" || tex.name == "bedrtable5" || tex.name == "bedrtable6" || tex.name == "gun") {
			twoLayerLook = true;
		} else {
			twoLayerLook = false;
		}

		if (tex.name.Contains ("invbig")) {
			if (image.sprite.name != currentRoom.currentState.Image) {
				storedImage = image.sprite.name;
				storedOverlay = currOverlay;
			}
		}

		if (image.sprite.name.Contains ("invbig")) {
			if (!tex.name.Contains ("invbig")) {
				lockMask = false;

				/*storedImage = "";
				storedOverlay = "";*/

				if (gasMaskOn && !inventoryUp && !helpScreenUp) {
					SetGasMaskOverlay (true);
				}
			}
		}

		if ((image.sprite.name == "phone4" || image.sprite.name == "phone5") && (tex.name != "phone4" && tex.name != "phone5")) {
			/*AudioClip clipToPlay = GetClip (49);
			if (!toPlaySoundFX.Contains (clipToPlay) ) {
				toPlaySoundFX.Add (clipToPlay);
			}*/

			PlayClip (GetClip (49));
		}

		if (tex.name == "trapstairs") {
			PlayClip (GetClip (60));
		}

		if (tex.name == "trapstairs3") {
			PlayClip (GetClip (75));
		}

		if (tex.name == "bedrtable5" || tex.name == "bedrtable6") {
			PlayClip (GetClip (51));
		}

		if (tex.name == "safe2" && killerInKitchen) {
			tex = GetImageByName ("safe3");
		}

		var necromicon = itemsList [126];

		if (image.sprite.name != "undercushion" && necromicon.State == 1) {
			ChangeState (126, 0);
		}

		if (tex.name == "lrdeath") {
			if (!sleepDeath) {
				AddText ("You unlock and open the front door. Almost instantaneously, the sound of a motor starting greets your ears. You move away from the door in shock, just before the door is blasted back. A man in a cleansuit stands, foot extended, wielding a revving chainsaw. You start to turn to run, but you don't get very far before the saw chain starts to work its way through your yielding flesh.\n\nPress [ENTER] to try again.");
			}
		}

		var oldImageName = image.sprite.name;

		image.sprite = Sprite.Create(tex, image.sprite.rect, image.sprite.pivot);
		image.sprite.name = tex.name;

		if ((oldImageName.Contains("invbig") || oldImageName == "blankoverlay" ) && !tex.name.Contains ("invbig") && !tex.name.Contains ("blankoverlay")) {
			if (!confrontPause) {
				storedImage = "";
				storedOverlay = "";
			}
		}

		MapArrow ();

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
		helpText.textComponent.text = helpText.textComponent.text.Replace("LOOK", "<color=#08FF00>look</color>");
		helpText.textComponent.text = helpText.textComponent.text.Replace("GET", "<color=#08FF00>get</color>");
		helpText.textComponent.text = helpText.textComponent.text.Replace("BACK", "<color=#08FF00>back</color>");
		helpText.textComponent.text = helpText.textComponent.text.Replace("USE", "<color=#08FF00>use</color>");
		helpText.textComponent.text = helpText.textComponent.text.Replace("MOVE", "<color=#08FF00>move</color>");
		helpText.textComponent.text = helpText.textComponent.text.Replace("OPEN", "<color=#08FF00>open</color>");
		helpText.textComponent.text = helpText.textComponent.text.Replace("CLOSE", "<color=#08FF00>close</color>");
		helpText.textComponent.text = helpText.textComponent.text.Replace("INVENTORY", "<color=#08FF00>inventory</color>");
		helpText.textComponent.text = helpText.textComponent.text.Replace("INV", "<color=#08FF00>inv</color>");
		helpText.textComponent.text = helpText.textComponent.text.Replace("READ", "<color=#08FF00>read</color>");
		helpText.textComponent.text = helpText.textComponent.text.Replace("CALL", "<color=#08FF00>call</color>");
		helpText.textComponent.text = helpText.textComponent.text.Replace("HIDE", "<color=#08FF00>hide</color>");
		helpText.textComponent.text = helpText.textComponent.text.Replace("HALLWAY", "<color=#BCC5FF>hallway</color>");
		helpText.textComponent.text = helpText.textComponent.text.Replace("STAIRCASE", "<color=#BCC5FF>staircase</color>");
		helpText.textComponent.text = helpText.textComponent.text.Replace("KNIFE", "<color=#BCC5FF>knife</color>");
		helpText.textComponent.text = helpText.textComponent.text.Replace("ROOM", "<color=#BCC5FF>room</color>");
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
		overlayImage.sprite.name = tex.name;
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
				basementOverlay5.sprite = Sprite.Create (GetImageByName ("basementoverlay5"), basementOverlay.sprite.rect, basementOverlay.sprite.pivot);
			} else {
				basementOverlay5.sprite = Sprite.Create (GetImageByName ("blankoverlay"), basementOverlay.sprite.rect, basementOverlay.sprite.pivot);
			}
			break;
		case 5:
			if (setImage) {
				basementOverlay.sprite = Sprite.Create (GetImageByName ("basementoverlay"), basementOverlay.sprite.rect, basementOverlay.sprite.pivot);
				basementOverlay2.sprite = Sprite.Create (GetImageByName ("basementoverlay2"), basementOverlay.sprite.rect, basementOverlay.sprite.pivot);
				basementOverlay3.sprite = Sprite.Create (GetImageByName ("basementoverlay3"), basementOverlay.sprite.rect, basementOverlay.sprite.pivot);
				basementOverlay4.sprite = Sprite.Create (GetImageByName ("basementoverlay4"), basementOverlay.sprite.rect, basementOverlay.sprite.pivot);
				basementOverlay5.sprite = Sprite.Create (GetImageByName ("basementoverlay5"), basementOverlay.sprite.rect, basementOverlay.sprite.pivot);
			} else {
				basementOverlay.sprite = Sprite.Create (GetImageByName ("blankoverlay"), basementOverlay.sprite.rect, basementOverlay.sprite.pivot);
				basementOverlay2.sprite = Sprite.Create (GetImageByName ("blankoverlay"), basementOverlay.sprite.rect, basementOverlay.sprite.pivot);
				basementOverlay3.sprite = Sprite.Create (GetImageByName ("blankoverlay"), basementOverlay.sprite.rect, basementOverlay.sprite.pivot);
				basementOverlay4.sprite = Sprite.Create (GetImageByName ("blankoverlay"), basementOverlay.sprite.rect, basementOverlay.sprite.pivot);
				basementOverlay5.sprite = Sprite.Create (GetImageByName ("blankoverlay"), basementOverlay.sprite.rect, basementOverlay.sprite.pivot);
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

			if (currOverlay == "br-gun") {
				currMultiSequence = 40;
			} else {
				currMultiSequence = 28;
			}
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
		AddText ("You swing the revolver upwards, pointing it at the killer. He tenses, and a look of determined rage is visible on his face. You hesitate for a moment, and the killer starts to move. Now or never. You pull the trigger and involuntarily shut your eyes. Next second, you re-open them to find the killer with a gaping hole in his leg, lurching quickly out of the room.\n\nPress [ENTER] to continue.");
		//SetGasMaskOverlay (false);
		ResetOverlay ();
		//killerInBedroom = false;

		PlayClip (GetClip (36));

		ChangeState (134, 1);
		ChangeState (135, 1);
		ChangeState (136, 1);

		killerInKitchen = true;
	}

	void KitchenStab (string text) {

		if (text.Contains ("knife") || text.Contains("stab")) {
			SetImage (GetImageByName ("knifeaction"));

			if (IsInInv (23)) {
				AddText ("You draw the knife and grip it tightly in your hand. The killer notices you and raises his head. He lunges at you and grabs your weapon arm. He grins wickedly as he pushes you against the stove. Your paunch bounces comically in your periphery. \"Shouldn't have had that second Quesarritalupa,\" you think, as you bring your knee up into the killer's groin. " +
					"He loses his grip and you bring the knife down into the back of his neck. He crumples instantly, his neck clinging to the blade of the knife.\n\nBut you did it. You survived.\n\nPress [ENTER] to play again.");

				GameOverAudio (-1, false);
			} else {
				AddText ("You sprint across the room towards the knife block and grab out a knife before the killer has a chance to steady himself. He lunges at you and grabs your weapon arm. He grins wickedly as he pushes you against the stove. Your paunch bounces comically in your periphery. \"Shouldn't have had that second Quesarritalupa,\" you think, as you bring your knee up into the killer's groin. " +
					"He loses his grip and you bring the knife down into the back of his neck. He crumples instantly, his neck clinging to the blade of the knife.\n\nBut you did it. You survived.\n\nPress [ENTER] to play again.");

				GameOverAudio (-1, false);
			}
		} else if (text.Contains ("fire") || (text == "shoot" || text.Contains("shoot ")) ||
			text == "use gun" || text == "use pistol" || text == "use firearm" || text == "use handgun" || text == "use hand gun" ||
			text == "use sixshooter" || text == "use six shooter" || text == "use fire arm" || text == "use revolver") {
			AddText ("You take aim at the killer, brace yourself, and pull the trigger. To your horror, instead of the powerful blast you expect, you hear a click as the gun's hammer hits an empty chamber. The killer moves with surprising speed given his injury, and drives his knife into your chest. You continue to squeeze the trigger in disbelief, despite your arm falling away, your body sinking to the floor, and your lungs spluttering. Presently, you become deader than a doornail.\n\nPress [ENTER] to play again.");
			string weapon = currOverlay.Split ('-').Last ();
			var overlays = deathImages [currentRoom.Index];
			SetImage (GetImageByName (overlays.ElementAt (UnityEngine.Random.Range (0, 2))));

			PlayClip (GetClip (37));

			ResetOverlay ();
			SetGasMaskOverlay (false);

			GameOverAudio (-1, true);

			killerInKitchen = false;
		}
		else {
			SetImage (GetImageByName ("whipaction"));
			AddText ("The killer lunges at you and grabs your weapon arm. He pushes you against the stove. Your paunch bounces comically in your periphery. \"Shouldn’t have had that second Quesarritalupa,\" you think, as you bring your knee up into the killer's groin. "+
				"He loses his grip and you bring the butt of the pistol down into the back of his head. He falls unconscious to the ground. You nudge him cautiously a few times with your foot to guarantee that he is truly no longer a threat. It seems you are safe at last.\n\nPress [ENTER] to play again.");

			GameOverAudio (-1, false);
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
			AddText("The killer sees that you are holding his bear, and his eyes flash with fear and rage. He begins to lunge towards you, but as you raise the scalpel to the bear, the killer recognizes your intention and freezes. You attempt to regulate your breathing and thudding heart. The man in the clean suit looks up from the bear and into your eyes while straightening up to his full height. \"Back away,\" you say. The man says nothing, but after a moment's hesitation, capitulates. You try to gather your thoughts. \"Why-...what is this? How long have you been here?,\" you say. He remains utterly silent, communicating nothing other than cold, predatory malice through his gaze. You decide to press your advantage.\n\nPress [ENTER] to continue.");
			multiSequence = true;
			currMultiSequence = 24;
			break;
		case "spoon":
			SetImage (GetImageByName ("bearspoon"));
			AddText ("You hold out the bear in front of you, and the killer falters mid step. However, as you bring the spoon to bear on the bear, he furrows his brow momentarily. He blinks and continues to press forward. You fumble stupidly.\n\nOver the next few days before your death, the killer makes a few things clear to you, namely what he uses the spoon for. You decide you don't very much like it.\n\nPress [ENTER] to try again.");            
			GameOverAudio (-1, true);

			health = 0;
			break;
		case "scissors":
			SetImage (GetImageByName ("bearscissors"));
			AddText("The killer sees that you are holding his bear, and his eyes flash with fear and rage. He begins to lunge towards you, but as you raise the scissors to the bear, the killer recognizes your intention and freezes. You attempt to regulate your breathing and thudding heart. The man in the clean suit looks up from the bear and into your eyes while straightening up to his full height. \"Back away,\" you say. The man says nothing, but after a moment's hesitation, capitulates. You try to gather your thoughts. \"Why-...what is this? How long have you been here?,\" you say. He remains utterly silent, communicating nothing other than cold, predatory malice through his gaze. You decide to press your advantage.\n\nPress [ENTER] to continue.");			
			multiSequence = true;
			currMultiSequence = 24;
			break;
		case "knife":
			SetImage (GetImageByName ("bearknife"));
			AddText("The killer sees that you are holding his bear, and his eyes flash with fear and rage. He begins to lunge towards you, but as you raise the knife to the bear, the killer recognizes your intention and freezes. You attempt to regulate your breathing and thudding heart. The man in the clean suit looks up from the bear and into your eyes while straightening up to his full height. \"Back away,\" you say. The man says nothing, but after a moment's hesitation, capitulates. You try to gather your thoughts. \"Why-...what is this? How long have you been here?,\" you say. He remains utterly silent, communicating nothing other than cold, predatory malice through his gaze. You decide to press your advantage.\n\nPress [ENTER] to continue.");
			multiSequence = true;
			currMultiSequence = 24;
			break;
		case "blender":
			SetImage (GetImageByName ("bearblender"));
			AddText("The killer sees that you are holding his bear, and his eyes flash with fear and rage. He begins to lunge towards you, but as stuff the bear into the blender, the killer recognizes your intention and freezes. You attempt to regulate your breathing and thudding heart. The man in the clean suit looks up from the bear and into your eyes while straightening up to his full height. \"Back away,\" you say. The man says nothing, but after a moment's hesitation, capitulates. You try to gather your thoughts. \"Why-...what is this? How long have you been here?,\" you say. He remains utterly silent, communicating nothing other than cold, predatory malice through his gaze. You decide to press your advantage.\n\nPress [ENTER] to continue.");
			multiSequence = true;
			currMultiSequence = 24;
			break;
		case "computer":
			SetImage (GetImageByName ("hack2"));
			AddText ("You try to hack the computer and die.\n\nPress [ENTER] to restart.");

			GameOverAudio (-1, true);

			health = 0;
			break;
			/*case "stab":
			List<string> bearItemsStab = new List<string> ();
			if (IsInInv (23))
				bearItemsStab.Add ("bearknife");
			if (IsInInv (74))
				bearItemsStab.Add ("bearscalpel");
			if (IsInInv (75))
				bearItemsStab.Add ("bearscissors");
			if (IsInInv (76))
				bearItemsStab.Add ("bearspoon");

			string imageNameStab = bearItemsStab [UnityEngine.Random.Range (0, bearItemsStab.Count)];

			switch (imageNameStab) {
			case "bearscalpel":
				AddText ("You draw the scalpel and hold it out in front of you in what you imagine might be a fighting stance. You lurch forward and strike, but the murderer slides effortlessly out of the way. In the same motion, he counters, plunging the knife between your ribs.\n\nPress [ENTER] to restart.");
				break;
			case "bearspoon":
				AddText ("You draw the spoon and hold it out in front of you in what you imagine might be a fighting stance. You lurch forward and strike, but the murderer slides effortlessly out of the way. In the same motion, he counters, plunging the knife between your ribs.\n\nPress [ENTER] to restart.");
				break;
			case "bearscissors":
				AddText ("You draw the scissors and hold it out in front of you in what you imagine might be a fighting stance. You lurch forward and strike, but the murderer slides effortlessly out of the way. In the same motion, he counters, plunging the knife between your ribs.\n\nPress [ENTER] to restart.");
				break;
			case "bearknife":
				AddText ("You draw the knife and hold it out in front of you in what you imagine might be a fighting stance. You lurch forward and strike, but the murderer slides effortlessly out of the way. In the same motion, he counters, plunging the knife between your ribs.\n\nPress [ENTER] to restart.");
				break;
			}

			string weaponImg = currOverlay.Split ('-').Last ();
			var overlays = deathImages [currentRoom.Index];
			if (weaponImg == "gun") {
				SetImage (GetImageByName (overlays.ElementAt (UnityEngine.Random.Range (0, 2))));
			} 
			else if (weaponImg == "katana") {
				SetImage (GetImageByName (overlays.ElementAt (UnityEngine.Random.Range (2, 4))));
			}
			else if (weaponImg == "knife") {
				SetImage (GetImageByName (overlays.ElementAt (UnityEngine.Random.Range (4, 6))));
			}
			else {
				SetImage (GetImageByName (overlays.ElementAt (UnityEngine.Random.Range (6, 8))));
			}
			ResetOverlay ();
			SetGasMaskOverlay (false);
			health = 0;
			killerInLair = false;
			return;*/
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
				GameOverAudio (-1, true);

				health = 0;
			} else {
				multiSequence = true;
				currMultiSequence = 24;
			}

			if (multiSequence) {
				switch (imageName) {
				case "bearscalpel":
					AddText("The killer sees that you are holding his bear, and his eyes flash with fear and rage. He begins to lunge towards you, but as you raise the scalpel to the bear, the killer recognizes your intention and freezes. You attempt to regulate your breathing and thudding heart. The man in the clean suit looks up from the bear and into your eyes while straightening up to his full height. \"Back away,\" you say. The man says nothing, but after a moment's hesitation, capitulates. You try to gather your thoughts. \"Why-...what is this? How long have you been here?,\" you say. He remains utterly silent, communicating nothing other than cold, predatory malice through his gaze. You decide to press your advantage.\n\nPress [ENTER] to continue.");
					break;
				case "bearspoon":
					AddText ("You hold out the bear in front of you, and the killer falters mid step. However, as you bring the spoon to bear on the bear, he furrows his brow momentarily. He blinks and continues to press forward. You fumble stupidly.\n\nOver the next few days before your death, the killer makes a few things clear to you, namely what he uses the spoon for. You decide you don't very much like it.\n\nPress [ENTER] to try again.");
					break;
				case "bearscissors":
					AddText("The killer sees that you are holding his bear, and his eyes flash with fear and rage. He begins to lunge towards you, but as you raise the scissors to the bear, the killer recognizes your intention and freezes. You attempt to regulate your breathing and thudding heart. The man in the clean suit looks up from the bear and into your eyes while straightening up to his full height. \"Back away,\" you say. The man says nothing, but after a moment's hesitation, capitulates. You try to gather your thoughts. \"Why-...what is this? How long have you been here?,\" you say. He remains utterly silent, communicating nothing other than cold, predatory malice through his gaze. You decide to press your advantage.\n\nPress [ENTER] to continue.");			
					break;
				case "bearknife":
					AddText("The killer sees that you are holding his bear, and his eyes flash with fear and rage. He begins to lunge towards you, but as you raise the knife to the bear, the killer recognizes your intention and freezes. You attempt to regulate your breathing and thudding heart. The man in the clean suit looks up from the bear and into your eyes while straightening up to his full height. \"Back away,\" you say. The man says nothing, but after a moment's hesitation, capitulates. You try to gather your thoughts. \"Why-...what is this? How long have you been here?,\" you say. He remains utterly silent, communicating nothing other than cold, predatory malice through his gaze. You decide to press your advantage.\n\nPress [ENTER] to continue.");
					break;
				case "bearblender":
					AddText("The killer sees that you are holding his bear, and his eyes flash with fear and rage. He begins to lunge towards you, but as stuff the bear into the blender, the killer recognizes your intention and freezes. You attempt to regulate your breathing and thudding heart. The man in the clean suit looks up from the bear and into your eyes while straightening up to his full height. \"Back away,\" you say. The man says nothing, but after a moment's hesitation, capitulates. You try to gather your thoughts. \"Why-...what is this? How long have you been here?,\" you say. He remains utterly silent, communicating nothing other than cold, predatory malice through his gaze. You decide to press your advantage.\n\nPress [ENTER] to continue.");
					break;
				}

				if (pizzaTimer >= 0 && pizzaTimer <= pizzaCap2) {
					currMultiSequence = 25;
					SetGasMaskOverlay (false);
					ResetOverlay ();
					SetImage (GetImageByName (imageName));
					killerInLair = false;
					return;
				}

				if (policeTimer >= 0) {
					currMultiSequence = 26;
					SetGasMaskOverlay (false);
					ResetOverlay ();
					SetImage (GetImageByName (imageName));
					killerInLair = false;
					return;
				}
			} else {
				AddText ("You hold out the bear in front of you, and the killer falters mid step. However, as you bring the spoon to bear on the bear, he furrows his brow momentarily. He blinks and continues to press forward. You fumble stupidly.\n\nOver the next few days before your death, the killer makes a few things clear to you, namely what he uses the spoon for. You decide you dont very much like it.\n\nPress [ENTER] to try again.");
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

		if (text.Contains ("close") || text.Contains ("shut") || text.Contains ("closing")) {
			SetImage (GetImageByName ("lock2"));
			PlayClip (GetClip (55));

			string killerText = "You slink out from your hiding place and attempt to ease the door closed as soundlessly as possible. Surely the stranger will notice your ploy any second now.\n\nHow do you proceed?";

			AddText (killerText);

			storedText = killerText;

			ResetOverlay ();
			currLockdownOption = 22;
		}

		else {
			if (currLockdownOption == 22) {
				PlayClip (GetClip(54));
				AddText ("You click the lock into place, entombing the man in the cleansuit in your makeshift prison. You hear shifting around inside of the shed. The killer slams himself against the door, causing you to jump back in fear. The latch holds, however, despite his repeated attempts. The bastard is trapped in there - you're safe, for now.\n\nPress [ENTER] to play again.");
			}
			else {
				PlayClip (GetClip (56));
				AddText ("You quickly slam the door shut and click the lock into place before the man in the cleansuit has a chance to react. You hear shifting around inside of the shed. The killer slams himself against the door, causing you to jump back in fear. The latch holds, however, despite his repeated attempts. The bastard is trapped in there - you're safe, for now.\n\nPress [ENTER] to play again.");
			}
			SetImage (GetImageByName ("shackwin"));

			killerInShack = false;
			ResetOverlay ();
			SetGasMaskOverlay (false);

			GameOverAudio (-1, false);

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
				AddText ("Hm, you don't seem to have the number written down for that.");
				return;
			}
		case 8:
			if (valid) {
				if (text.Contains ("1") || text.Contains ("one")) {
					OtherCommands ("read book 1");
					return;
				} else if (text.Contains ("2") || text.Contains ("two")) {
					OtherCommands ("read book 2");
					return;
				} else if (text.Contains ("3") || text.Contains ("three")) {
					OtherCommands ("read book 3");
					return;
				} else if (text.Contains ("4") || text.Contains ("four")) {
					OtherCommands ("read book 4");
					return;
				}
				else if (text.Contains ("666") || text.Contains ("six six six") || text.Contains("six hundred sixty six") || text.Contains("six hundred and sixty six")) {
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
				AddText ("Not sure which door you're talking about.");
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
				AddText ("Not sure which door you're talking about.");
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
				AddText ("Not sure which door you're talking about.");
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
				AddText ("Not sure which door you're talking about.");
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
				AddText ("Not sure which door you're talking about.");
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
				AddText ("Not sure which door you're talking about.");
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
				AddText ("Not sure which door you're talking about.");
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
				AddText ("Not sure which door you're talking about.");
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
				AddText ("Not sure which door you're talking about.");
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
				AddText ("Not sure which door you're talking about.");
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
				AddText ("Not sure which door you're talking about.");
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
				AddText ("Not sure which door you're talking about.");
				return;
			}
		case 28:
			if (valid) {
				if (text.Contains ("back") || text.Contains ("yard")) {
					OtherCommands ("lock back door");
					return;
				} else if (text.Contains ("basement") || text.Contains ("cellar")) {
					OtherCommands ("lock basement door");
					return;
				} else {
					break;
				}
			} else {
				AddText ("Not sure which door you're talking about.");
				return;
			}
		case 29:
			if (valid) {
				if (text.Contains ("back") || text.Contains ("yard")) {
					OtherCommands ("unlock back door");
					return;
				} else if (text.Contains ("basement") || text.Contains ("cellar")) {
					OtherCommands ("unlock basement door");
					return;
				} else {
					break;
				}
			} else {
				AddText ("Not sure which door you're talking about.");
				return;
			}
		case 30:
			if (valid) {
				if (text.Contains ("bedroom")) {
					OtherCommands ("lock bedroom door");
					return;
				} else if (text.Contains ("bathroom") || text.Contains ("rest") || text.Contains ("wash") || text.Contains ("powder") || text.Contains ("lavatory")) {
					OtherCommands ("lock bathroom door");
					return;
				} else {
					break;
				}
			} else {
				AddText ("Not sure which door you're talking about.");
				return;
			}
		case 31:
			if (valid) {
				if (text.Contains ("bedroom")) {
					OtherCommands ("unlock bedroom door");
					return;
				} else if (text.Contains ("bathroom") || text.Contains ("rest") || text.Contains ("wash") || text.Contains ("powder") || text.Contains ("lavatory")) {
					OtherCommands ("unlock bathroom door");
					return;
				} else {
					break;
				}
			} else {
				AddText ("Not sure which door you're talking about.");
				return;
			}
		case 32:
			if (valid) {
				if (text.Contains ("back")) {
					OtherCommands ("lock back door");
					return;
				} else if (text.Contains ("shed") || text.Contains ("shack")) {
					OtherCommands ("lock shed door");
					return;
				} else {
					break;
				}
			} else {
				AddText ("Not sure which door you're talking about.");
				return;
			}
		case 33:
			if (valid) {
				if (text.Contains ("back")) {
					OtherCommands ("unlock back door");
					return;
				} else if (text.Contains ("shed") || text.Contains ("shack")) {
					OtherCommands ("unlock shed door");
					return;
				} else {
					break;
				}
			} else {
				AddText ("Not sure which door you're talking about.");
				return;
			}
		case 34:
			if (valid) {
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
				}
				return;
			} else {
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
				AddText ("You died. Maybe you could have saved yourself.\n\nPress [ENTER] to try again.");

				GameOverAudio (-1, true);

				health = 0;
				killerInLair = false;
				return;
			}
		case 35:
			if (valid) {
				Use ("use toaster with shower".Shlex ());
				return;
			} else {
				AddText ("Don't think those things can be used together.");
				return;
			}
		case 36:
			if (valid) {
				if (text.Contains ("hall") || text.Contains("up") || text.Contains("second") || text.Contains("2nd")) {
					Look ("look hallway staircase".Shlex());
					return;
				} else {
					Look ("look secret staircase".Shlex());
					return;
				}
			} else {
				AddText ("Not sure which staircase you're talking about.");
				return;
			}
		case 37:
			if (valid) {
				if (text.Contains ("hall") || text.Contains("up") || text.Contains("second") || text.Contains("2nd")) {
					Use ("use hallway staircase".Shlex());
					return;
				} else {
					Use ("use secret staircase".Shlex());
					return;
				}
			} else {
				AddText ("Not sure which staircase you're talking about.");
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
			AddText ("You wrap the tarp around the rake. Hmm, this almost looks like a human-shaped dummy. If only it had a head.");
			ChangeState (80, 1);
			ChangeState (82, 1);
			ChangeState (115, 1);

			PlayClip (GetClip (59));

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
				AddText ("You crank up the volume as loud as possible on the tape recorder, set your recording to repeat, and place it on the floor behind the dummy. It still needs a head, though.");
				tapePlaced = true;
				SetImage (GetImageByName ("dummystep1"));
			}
			else {
				AddText ("You crank up the volume as loud as possible on the tape recorder, set your recording to repeat, and place it on the floor behind the dummy. Now just to hide and wait for the killer to take the bait.");
				SetImage (GetImageByName ("dummystep2"));
			}
			RemoveFromInv (57);
			updateRoom = false;
			dummyStepsCompleted++;
			break;
		case 17:
			AddText ("You wrap the tarp around the rake. Hmm, this almost looks like a human-shaped dummy. If only it had a head.");
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
		inventoryUp = true;

		storedImage = image.sprite.name;
		storedOverlay = currOverlay;

		SetOverlay (GetImageByName ("blankoverlay"));
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
	}

	[Command("peep")]
	[Command("inspect")]
	[Command("examine")]
	[Command("check")]
	[Command]
	public void Look(List<string> argv = null)
	{
		if (argv == null) {

			if (!playerOutOfTime) {

				ResetOverlay ();

				if (killerInKitchen && currentRoom.Index == 1) {
					SetOverlay (GetImageByName ("kinjuredoverlay"));

					string killerText = "You find the man in the cleansuit hunched over, knife in hand, waiting to strike.\n\nWhat do you do?";

					AddText (killerText);

					storedText = killerText;
				} 
				else if (killerInKitchen && currentRoom.Index == 0 && !bloodSeen3) {
					AddText ("There is a stream of blood trailing through the living room into the kitchen.");
					bloodSeen3 = true;
				} 
				else if (killerInKitchen && currentRoom.Index == 2 && !bloodSeen2) {
					AddText ("The trail of blood continues downstairs to the living room.");
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
				if (currentRoom.Index == 5 && (bearTrapMade || bucketTrapMade || fireTrapMade || shitOnStairs || blenderTrapMade)) {
					HideNoItem ("basement");
					return;
				}
				else {
					ResetItemGroup ();
					SetImage (GetImageByName (currentRoom.currentState.Image));
					UpdateRoomState ();
					SetOverlay (GetRandomDeathOverlay ());
					PlayDeathSequence ();

					fadeMusicTrack = true;

					actionTrack.clip = GetClip (73);
					actionTrack.Play ();

					if (hideKillerInKitchen) {
						AddText ("As you emerge from your hiding spot, you see that the killer has returned. He appears to have tended to his leg, at least temporarily, but you don't get much time to examine it before the murderer exacts his revenge on you for the pain you caused him.\n\nPress [ENTER] to continue.");

					} else {
						if (overlayImage.sprite.name.Contains ("katana")) {
							AddText ("You find the killer in the next room; he has come for you at last.\n\nPress [ENTER] to continue.");
						} else if (overlayImage.sprite.name.Contains ("knife")) {
							AddText ("Just as you turn the corner, you find the killer standing poised with a knife - ready to strike.\n\nPress [ENTER] to continue.");
						} else if (overlayImage.sprite.name.Contains ("gun")) {
							AddText ("As you enter the room, your heart skips a beat as you see the man in the cleansuit already beginning to take aim at your face with a silenced pistol.\n\nPress [ENTER] to continue.");
						} else if (overlayImage.sprite.name.Contains ("mace")) {
							AddText ("You recoil in shock as you see that, in the next room, the imposing figure of the man in the cleansuit stands, wielding a mace.\n\nPress [ENTER] to continue.");
						}
					}

					timeoutDeath = true;
					return;
				}
			}
		}

		else if (argv.Count == 1)
		{
			if (twoLayerLook)
			{
				TwoLayerLook ();
				return;
			}
			else {
				ResetItemGroup();

				if (killerInKitchen && currentRoom.Index == 4) {
					AddText ("You see a trail of blood starting from near the doorway where you shot the man in the cleansuit and leading into the hallway. Shit, he must not be dead.\n\nWhat do you want to do next?");
				} else {
					AddText (currentRoom.currentState.Description);
				}

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
					ResetOverlay ();
					AddText ("You life up the corner of the rug to check underneath it. There is nothing, naturally. That was pointless.");
				} else if (underObj.Index == 50) {
					SetImage (GetImageByName ("underbed"));
					ResetOverlay ();
					AddText ("Lots of dust down there and not much else. You could probably fit under there.");
				} else if (underObj.Index == 125) {
					SetImage (GetImageByName ("undercushion"));
					ChangeState (126, 1);
					ResetOverlay ();
					AddText ("You lift the seat cushion and a strange book is revealed to be hiding underneath. You have no recollection of putting it - whatever it is - here.");
				} 
				else if (underObj.Index == 29) {
					Look ("look cabinet".Shlex());
					return;
				}
				else if (underObj.Index == 44) {
					Look ("look cabinet".Shlex());
					return;
				}
				else {
					AddText ("You don't need to look under that.");
				}
			}
			else {
				AddText ("Not sure what you're trying to look under.");
			}

			return;
		}

		if (argv [1] == "behind") {

			itemName = itemName.Replace ("behind ", "");

			var underObj = GetObjectByName (itemName);
			if (underObj == null) {
				underObj = GetObjectFromInv (itemName);
			}

			if (underObj != null) {
				if (underObj.Index == 54) {
					Move ("move painting".Shlex ());
					return;
				}
				else {
					AddText ("You don't need to look behind that.");
				}
			}
			else {
				AddText ("Not sure what you're trying to look behind.");
			}

			return;
		}

		bool invItem = false;

		//Debug.LogFormat("Looking at ({0})", itemName);

		if (itemName == "inventory" || itemName == "pockets" || itemName == "inv") {
			ListInventory ();
			return;
		}

		if (itemName == "room" || itemName == "around") {
			Look ("look".Shlex());
			return;
		}

		if (itemName == "outside" && currentRoom.Index == 0) {
			Look ("look window".Shlex ());
			return;
		}

		if (itemName == "book" && currentRoom.Index == 0 && (image.sprite.name == "phone2" || image.sprite.name == "phone3" || image.sprite.name == "phone5")) {
			Look ("look address book".Shlex ());
			return;
		}

		if (itemName == "back") {
			OtherCommands ("back");
			return;
		}

		var lookRoom = rooms.Find(x => itemName.Contains(x.Name) || AltNameCheck(itemName, "lookRoom") == x.Index);
		if (lookRoom != null) {

			if (lookRoom.Name == currentRoom.Name) {
				Look ("look".Shlex());
				return;
			}
		}

		if (currentRoom.Index == 0) {
			if (itemName == "1" || itemName == "one" || itemName == "number one" || itemName == "number 1") {
				OtherCommands ("read book 1");
				return;
			} else if (itemName == "2" || itemName == "two" || itemName == "number two" || itemName == "number 2") {
				OtherCommands ("read book 2");
				return;
			} else if (itemName == "3" || itemName == "three" || itemName == "number three" || itemName == "number 3") {
				OtherCommands ("read book 3");
				return;
			} else if (itemName == "4" || itemName == "four" || itemName == "number four" || itemName == "number 4") {
				OtherCommands ("read book 4");
				return;
			} else if (itemName == "666" || itemName == "six six six" || itemName == "six hundred sixty six" || itemName == "six hundred and sixty six"  || itemName == "number 666" || itemName == "number six six six"
				|| itemName == "number six hundred sixty six" || itemName == "number six hundred and sixty six" ) {
				OtherCommands ("read book 666");
				return;
			}
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
			if (obj.Index == 89) {
				var drawer = GetObjectByName ("drawer");
				if (drawer.State == 1) {
					AddAdditionalText ("\n\nThe drawer is open, and inside you see a little slip of paper.");
				}
			}

			if (obj.Index == 34 && playerKnowsBeartrap) {
				AddAdditionalText ("\n\nThe springs from the toaster might be just what you need to build a bear trap like that book said.");
			}

			if (obj.Index == 23 && playerKnowsBeartrap) {
				AddAdditionalText ("\n\nThat survival book said that this might be useful for a trap.");
			}

			if (obj.Index == 22 && playerKnowsBeartrap) {
				AddAdditionalText ("\n\nThat survival book said that one of these might be useful for a trap.");
			}

			if (obj.Index == 55 && !playerKnowsCombo) {
				AddAdditionalText (" If only you could remember the damn combination.");
			}

			if (obj.Index == 139 && obj.State == 1) {
				ResetOverlay ();
				inputLockdown = true;
				currLockdownOption = 36;
			}

			if (obj.Index == 111) {
				ResetOverlay ();
				UpdateItemGroup (111);
				UpdateRoomState (false);
				inputLockdown = true;
				currLockdownOption = 9;
			}

			if (obj.Index == 112) {
				ResetOverlay ();
				inputLockdown = true;
				currLockdownOption = 13;
			}

			if (obj.Index == 60) {
				ResetOverlay ();
				inputLockdown = true;
				currLockdownOption = 18;
			}

			if (obj.Index == 132) {
				PlayClip (GetClip(35));
			}

			if (obj.Index == 10) {
				PlayClip (GetClip(53));
			}

			if (invItem) {
				SetImage (GetImageByName ("invbig-" + obj.Name));
				lockMask = true;
				ResetOverlay ();
				MapArrow ();
				SetGasMaskOverlay (false);
				return;
			}

			if (obj.currentState.Image != "")
			{
				ResetOverlay ();
				ImageCheckAndShow (obj.Index, obj.State, obj.currentState.Image);
				PlayClips ();
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

				if (loopingAudioSource.clip != null) {
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
				}

				if (currentItemGroup.BaseItemIndex == 33) {
					var obj = GetObjectByName ("oven");

					if (obj.State != 0) {
						AudioClip clipToPlay = GetClip (1);
						toPlaySoundFX.Add (clipToPlay);
					}
				}

				if (currentItemGroup.BaseItemIndex == 30) {
					var obj = GetObjectByName ("k cabinet");

					if (obj.State != 0) {
						AudioClip clipToPlay = GetClip (21);
						toPlaySoundFX.Add (clipToPlay);
					}
				}

				if (currentItemGroup.BaseItemIndex == 16) {
					var obj = GetObjectByName ("drawer");

					var phoneImage = image.sprite.name;

					if (obj.State != 0) {
						AudioClip clipToPlay = GetClip (50);
						if (!toPlaySoundFX.Contains (clipToPlay)) {
							toPlaySoundFX.Add (clipToPlay);
						}
					}
				}

				if (currentItemGroup.BaseItemIndex == 0) {
					var obj = GetObjectByName ("drawer");

					if (obj.State != 0 && !toPlaySoundFX.Contains(GetClip(58))) {
						AudioClip clipToPlay = GetClip (58);
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

	void PlayKnockingClip (AudioClip audioClip) {
		knockingSource.loop = false;
		knockingSource.clip = audioClip;
		knockingSource.Play ();
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

		if (currentRoom.Index == 0 && (specificRoom == 1 || specificRoom == 2 || specificRoom == 6 || specificRoom == 0)) {
			if (currOverlay == "windowoverlay" || currOverlay == "windowoverlay2" || currOverlay == "windowoverlay3"
				|| currOverlay == "peepholeoverlay" || currOverlay == "peepholeoverlay2" || currOverlay == "peepholeoverlay3") 
			{
				//ResetOverlay ();
			}
		}

		if (currentRoom.Index == 6) {
			if (currOverlay == "journal") {
				ResetOverlay ();
			}
		}

		if (!killerInKitchen && currOverlay == "blankoverlay") {
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
			if (killerInKitchen) {
				killerTimer = killerCap4;
			}
			else {

				updateAllTimers = false;
				if (pizzaTimer >= 0 && pizzaTimer < pizzaCap) {
					killerTimer += (pizzaCap - pizzaTimer);
					pizzaTimer = pizzaCap;
				} else if (pizzaTimer >= pizzaCap && pizzaTimer < pizzaCap2) {
					killerTimer += (pizzaCap2 - pizzaTimer);
					pizzaTimer = pizzaCap2;
				} else if (policeTimer >= 0 && policeTimer < policeCap) {
					killerTimer += (policeCap - policeTimer);
					policeTimer = policeCap;
				} else {
					if (killerTimer < killerCap2) {
						killerTimer = killerCap2;
					} else if (killerTimer >= killerCap2 && killerTimer < killerCap3) {
						killerTimer = killerCap3;
					} else if (killerTimer >= killerCap3 && killerTimer < killerCap4) {
						killerTimer = killerCap4;
					}
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
		if (killerTimer == 1)
		{
			ChangeState(94, 1);
			ChangeState(95, 1);
		}
		else if (killerTimer == killerCap2)
		{
			textWaiting += "You think you hear a distant creaking of the floorboards.\n\n";
		}
		else if (killerTimer == killerCap3)
		{
			textWaiting += "There is a soft thud that sounds awfully close...\n\n";
		}

		else if (killerTimer >= killerCap4)
		{
			//ChangeState (-1, 100);
			//SetImage (GetRandomDeathImage ());
			//AddText ("You died! \n\n");
			playerOutOfTime = true;
		}

		if (pizzaTimer == pizzaCap)
		{
			textWaiting += "You hear a knock downstairs at what must be the front door.\n\n";
			toPlaySoundFX.Add (GetClip (26));
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
			textWaiting += "You hear a police siren, distantly at first, which grows louder and louder. You wait for a moment, anticipating a knock at the front door, but it does not come. Instead, you hear a shout, a gunshot, and a struggle. Hopefully, whatever happened, it at least bought you some time.\n\n";
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
							obj = invItem;
						}
					}
				}


				if (obj != null) return obj.Index;

			}
			break;
		case "lookRoom":
			foreach (KeyValuePair<string, List<string>> entry in altNames)
			{
				if (!entry.Value.Any(x => x == nameToCheck)) continue;
				var key = 0;

				if (int.TryParse (entry.Key, out key)) {
					return key;
				}       
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

	void MapArrow(){
		if (image.sprite.name == "invbig-map") {
			switch (currentRoom.Index) {
			case 0:
				SetOverlay (GetImageByName ("lr-location"));
				break;
			case 1:
				SetOverlay (GetImageByName ("k-location"));
				break;
			case 2:
				SetOverlay (GetImageByName ("hall-location"));
				break;
			case 3:
				SetOverlay (GetImageByName ("br-location"));
				break;
			case 4:
				SetOverlay (GetImageByName ("bedr-location"));
				break;
			case 5:
				SetOverlay (GetImageByName ("bment-location"));
				break;
			case 7:
				SetOverlay (GetImageByName ("outside-location"));
				break;
			case 8:
				SetOverlay (GetImageByName ("shed-location"));
				break;
			}
		}
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
		if (argv.Count == 1) {
			AddText (GenericMove ());
			return;
		}

		bool firstWordTo = false;
		bool firstWordGo = false;

		// ENTER CODE
		if (argv [0] == "enter" && currentRoom.Index == 4) {
			if (argv [1] == "code" || argv [1] == "combination") {

				var painting = GetObjectByName ("painting");

				if (painting.State == 0) {
					AddText (GenericDontKnow());
				} 
				else {
					ImageCheckAndShow (painting.Index, painting.State, painting.currentState.Image);
					if (playerKnowsCombo) {
						OtherCommands ("open safe");
					}
					else 
					{
						AddText ("You would enter the code if you could remember it!");
					}
				}

				return;
			}
		}

		if (argv [1] == "to") {
			firstWordTo = true;
		}

		if (argv [1] == "back") {
			OtherCommands ("back");
			return;
		}

		if (argv [0] == "go") {
			firstWordGo = true;
		}

		int newRoom = room;
		bool isRoom = false;
		bool roomImage = true;
		string roomName = string.Join(" ", argv.Skip((argv[1] != "to") ? 1 : 2).ToArray());

		if ((roomName == "porch" || roomName == "outside" || roomName == "front yard" || roomName == "front lawn") && currentRoom.Index == 0) {
			var frontDoor = GetObjectByName ("front door");
			ImageCheckAndShow (8, frontDoor.State, frontDoor.currentState.Image);
			ResetOverlay ();
			AddText ("You'd need to open the front door first.");
			return;
		}

		if ((roomName == "front yard" || roomName == "front lawn") && currentRoom.Index == 7){
			AddText("You can't, there is a fence in the way.");
			return;
		}

		if (currentRoom.Index == 4 && !roomName.Contains("under") && firstWordGo && (roomName.Contains("sleep") || roomName.Contains("bed") )) {
			OtherCommands ("sleep");
			return;
		}

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

				// If going to the secret lair, lock the living room
				if (rooms [newRoom].Index == 6) {
					currentRoom.States [currentRoom.State].Gettable = 0;
					var lairAltName = altNames.Where (w => w.Key == "6").FirstOrDefault ();
					lairAltName.Value.Remove ("fireplace");
					lairAltName.Value.Remove ("fire place");
					lairAltName.Value.Remove ("furnace");
					lairAltName.Value.Remove ("hearth");

					var livingRoomAltName = altNames.Where (w => w.Key == "0").FirstOrDefault ();
					livingRoomAltName.Value.Add ("upstairs");
					livingRoomAltName.Value.Add ("up stairs");
					livingRoomAltName.Value.Add ("up");
				}

				if (currentRoom.Index == 8 && newRoom == 1 && (dummyAssembled || tapePlaced)) {
					room = 7;
					Move ("move kitchen".Shlex ());
					return;
				}

				if (currentRoom.Index == 5 && (bearTrapMade || fireTrapMade || bucketTrapMade || shitOnStairs || blenderTrapMade)) {
					int numTraps = 0;
					if (bearTrapMade)
						numTraps++;
					if (fireTrapMade)
						numTraps++;
					if (bucketTrapMade)
						numTraps++;
					if (shitOnStairs)
						numTraps++;
					if (blenderTrapMade)
						numTraps++;

					if (numTraps == 1) {
						textWaiting += ("As you ascend the stairs, you take special care to avoid the trap on the stairs.\n\n");
					}

					if (numTraps > 1) {
						textWaiting += ("As you ascend the stairs, you take special care to avoid the traps on the stairs.\n\n");
					}
				}

				if (currentRoom.Index == 7 && (dummyAssembled || tapePlaced)) {
					// TODO: kill player if they don't try to hide/wait or close door

					if (newRoom == 8) {
						List<string> imageList = deathImages [5];
						ResetOverlay ();
						SetGasMaskOverlay (false);
						SetBasementOverlay (5, false);
						string imageName = imageList [UnityEngine.Random.Range (0, imageList.Count)];
						AddText ("You wander back into the shed. Just as you arrive, the man in the cleansuit appears, drawn by the sounds. His gaze passes back and forth once between you, and the dummy you made of yourself next to you. The corner of his mouth twitches. The killer adds some very unwelcome additional holes to your torso.\n\nPress [ENTER] to try again.");
						SetImage (GetImageByName (imageName));

						GameOverAudio (-1, true);

						health = 0;
						return;
					} else {
						List<string> imageList = deathImages [5];
						ResetOverlay ();
						SetGasMaskOverlay (false);
						SetBasementOverlay (5, false);
						string imageName = imageList [UnityEngine.Random.Range (0, imageList.Count)];
						AddText("You start to wander back into your house. As your cross the threshold, the man in the cleansuit appears, drawn by the sounds coming from the shed. Your dummy might have fooled him had he seen it, but I guess you'll never know now.\n\nPress [ENTER] to try again.");
						SetImage (GetImageByName (imageName));

						GameOverAudio (-1, true);

						health = 0;
						return;
					}
				}

				if (currentRoom.Index == 7 && newRoom == 8 && !dummyAssembled) {
					killerTimer--;
				}

				if (currentRoom.Index == 8 && newRoom == 7) {
					killerTimer--;
				}

				UpdateTimers ();
				ResetOverlay ();


				if (newRoom == 6) {
					textWaiting = ("As you descend down into the hidden room, you hear the entrance seal itself behind you with more sliding.\n\n");
				}

				if (textWaiting != "") {
					AddText (textWaiting);
					textWaiting = "";
				} else {
					AddText ("");
				}

				if (health <= 0)
					return;


				ResetItemGroup ();
				room = newRoom;

				Look (null);
				return;
			} else {
				switch (currentRoom.Index) {
				case 1:
					if (newRoom == 7) {
						var doorObj = GetObjectByName ("backyard door");
						if (doorObj.State == 0) {
							AddText ("You would need to open the door first.\n\n");
						} else if (doorObj.State == 1) {
							AddText ("Beyond is impenetrable darkness. Without a source of light you won't be able to see what you are doing.\n\n");
						}
						ImageCheckAndShow (doorObj.Index, doorObj.State, doorObj.currentState.Image);
					}

					if (newRoom == 8) {

						var doorObj = GetObjectByName ("backyard door");

						if (doorObj.State == 0) {
							AddText ("You would need to open the door first.\n\n");
							ImageCheckAndShow (doorObj.Index, doorObj.State, doorObj.currentState.Image);
						}
						else if (doorObj.State == 1 && !IsInInv(52)) {
							AddText ("Beyond is impenetrable darkness. Without a source of light you won't be able to see what you are doing.\n\n");
							ImageCheckAndShow (doorObj.Index, doorObj.State, doorObj.currentState.Image);
						} else {
							textWaiting = "You would have to open the shed door first.";
							Move ("move yard".Shlex ());
							return;
						}
					}
					break;
				case 0:

					if (firstWordGo || firstWordTo) {

						var lairNames = GetAltNames ("6");

						foreach (var lairName in lairNames) {
							if (roomName == lairName) {
								AddText (GenericDontKnow());
								return;
							}
						}

						AddText ("Maybe try looking at it?");
						return;
					}

					var fireplace = GetObjectByName ("fireplace");
					AddText (GenericItemMove ());
					ImageCheckAndShow (fireplace.Index, fireplace.State, fireplace.currentState.Image);
					ResetOverlay ();
					break;
				case 6:
					var door = itemsList [69];
					ImageCheckAndShow (door.Index, door.State, door.currentState.Image);
					AddText ("The secret panel sealed behind you and, try as you might, there doesn't seem to be a way to open it again.");
					break;
				case 7:
					AddText ("You would have to open the shed door first.");
					break;
				default:
					break;
				}
			}

		} else {
			AddText (GenericWrongRoom ());
			SetOverlay (GetImageByName (currOverlay));
		}

		if (!isRoom /*&& argv [0] == "move"*/) {
			var obj = GetObjectByName (roomName, (x, y) => x.Contains (y));
			if (obj == null) 
			{
				AddText (GenericMove ());
				return;
			}

			if (firstWordTo || firstWordGo) {
				if (obj.Index != 9 && obj.Index != 133 && obj.Index != 59 && obj.Index != 38 && obj.Index != 61 && obj.Index != 129) {
					AddText ("Maybe try looking at it?");
					return;
				}
			}

			if (obj.Index == 51) {
				if (obj.State == 0) {
					OtherCommands ("open closet");
				}
				else {
					OtherCommands ("hide closet");
				}

				return;
			}

			if (obj.Index == 9) {
				Move ("move hallway".Shlex ());
				return;
			}

			if (obj.Index == 133) {
				Move ("move lair".Shlex ());
				return;
			}

			if (obj.Index == 59) {
				Move ("move basement".Shlex ());
				return;
			}

			if (obj.Index == 38) {
				Move ("move living room".Shlex ());
				return;
			}

			if (obj.Index == 61) {
				Move ("move kitchen".Shlex ());
				return;
			}

			if (obj.Index == 129) {
				Move ("move living room".Shlex ());
				return;
			}

			if (obj.Index == 125) {
				Look ("look under cushion".Shlex ());
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
					ResetOverlay ();
					ImageCheckAndShow (obj.Index, obj.State, obj.currentState.Image);
				}

				return;
			}

			foreach (var response in moveResponses) {
				foreach (KeyValuePair<int, int> actions in response.Actions) {
					if (ChangeState (actions.Key, actions.Value) == 1)
						break;
				}

				if (obj.Index == 3) {
					if (obj.State == 1) {
						doorBlocked = true;
						killerCap += 5;
					} else {
						doorBlocked = false;
						killerCap -= 5;
					}
				}

				if (response.Image != "") {
					ResetOverlay ();
					ImageCheckAndShow (response.ItemIndex, response.ItemState, response.Image);
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

	[Command("grab")]
	[Command]
	public void Get(List<string> argv)
	{
		string itemName = string.Join(" ", argv.Skip(1).ToArray());
		bool roomImage = true;
		bool getIN = false;
		bool invItem = false;

		if (itemName.Contains ("in ")) {
			itemName = itemName.Replace ("in ", "");
			getIN = true;
		}

		if (itemName == "book" && currentRoom.Index == 0 && (image.sprite.name == "phone2" || image.sprite.name == "phone3" || image.sprite.name == "phone5")) {
			Look ("look address book".Shlex ());
			return;
		}

		if (currentRoom.Index == 1 && IsInInv(23) && (itemName == "another knife" || itemName == "more knives" || itemName == "rest of knives" 
			|| itemName == "second knife" || itemName == "2nd knife" || itemName == "a second knife" || itemName == "a 2nd knife")) {
			Get ("get knife block".Shlex());
			return;
		}

		if (currentRoom.Index == 0) {
			if (itemName == "1" || itemName == "one" || itemName == "number one" || itemName == "number 1") {
				OtherCommands ("read book 1");
				return;
			} else if (itemName == "2" || itemName == "two" || itemName == "number two" || itemName == "number 2") {
				OtherCommands ("read book 2");
				return;
			} else if (itemName == "3" || itemName == "three" || itemName == "number three" || itemName == "number 3") {
				OtherCommands ("read book 3");
				return;
			} else if (itemName == "4" || itemName == "four" || itemName == "number four" || itemName == "number 4") {
				OtherCommands ("read book 4");
				return;
			} else if (itemName == "666" || itemName == "six six six" || itemName == "six hundred sixty six" || itemName == "six hundred and sixty six"  || itemName == "number 666" || itemName == "number six six six"
				|| itemName == "number six hundred sixty six" || itemName == "number six hundred and sixty six" ) {
				OtherCommands ("read book 666");
				return;
			}
		}

		var item = GetObjectByName(itemName);
		if (item == null) {
			item = GetObjectFromInv (itemName);
			if (item == null) {
				AddText (GenericGet ());
				return;
			} else {
				invItem = true;
			}
		} else {
			if (IsInInv (item.Index)) {
				invItem = true;
			}
		}


		if (getIN) {
			if (item.Index == 48) {
				OtherCommands ("hide shower");
				return;
			} else if (item.Index == 51) {
				OtherCommands ("hide closet");
				return;
			} else if (item.Index == 7) {
				Use ("use fireplace".Shlex ());
				return;
			} else if (item.Index == 50) {
				OtherCommands ("sleep");
				return;
			}
		}

		if (invItem) {

			if (item.Index == 23 && currentRoom.Index == 1) {
				Get ("get knife block".Shlex ());
				return;
			}

			SetImage (GetImageByName ("invbig-" + item.Name));
			lockMask = true;
			ResetOverlay ();
			MapArrow ();
			SetGasMaskOverlay (false);
			AddText ("You already have it!");
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



			// Gun
			if (item.Index == 58) {
				killerInBedroom = true;
			}

			if (item.Index == 28) {
				var ojNames = altNames.Where (w => w.Key == "orange juice").FirstOrDefault();
				var ojNamesPurged = ojNames;
				ojNamesPurged.Value.Remove ("can");
				altNames.Remove (ojNames.Key);
				altNames.Add (ojNamesPurged.Key, ojNamesPurged.Value);
			}

			// Teddy Bear
			if (item.Index == 87) {
				if (IsInInv (74) || IsInInv (75) || IsInInv (76)) {
					killerInLair = true;
				}

			}

			if (item.Index == 47) {
				//item.State = 0;
				var bleachAltNames = altNames.Where (w => w.Key == "bleach").FirstOrDefault();
				var bleachAltNamesPurged = bleachAltNames;
				bleachAltNamesPurged.Value.Remove ("bottle");
				bleachAltNamesPurged.Value.Remove ("cleaner bottle");
				bleachAltNamesPurged.Value.Remove ("bottle of cleaner");
				bleachAltNamesPurged.Value.Remove ("disinfectant bottle");
				bleachAltNamesPurged.Value.Remove ("bottle of disinfectant");
				bleachAltNamesPurged.Value.Remove ("cleaning solution bottle");
				bleachAltNamesPurged.Value.Remove ("bottle of cleaning solution");
				bleachAltNamesPurged.Value.Remove ("cleaning solution");
				bleachAltNamesPurged.Value.Remove ("cleaner");
				bleachAltNamesPurged.Value.Remove ("disinfectant");
				altNames.Remove (bleachAltNames.Key);
				altNames.Add (bleachAltNamesPurged.Key, bleachAltNamesPurged.Value);
			}

			if (item.Index == 65) {
				var bleachAltNames = altNames.Where (w => w.Key == "ammonia").FirstOrDefault();
				var bleachAltNamesPurged = bleachAltNames;
				bleachAltNamesPurged.Value.Remove ("bottle");
				bleachAltNamesPurged.Value.Remove ("cleaner bottle");
				bleachAltNamesPurged.Value.Remove ("bottle of cleaner");
				bleachAltNamesPurged.Value.Remove ("disinfectant bottle");
				bleachAltNamesPurged.Value.Remove ("bottle of disinfectant");
				bleachAltNamesPurged.Value.Remove ("cleaning solution bottle");
				bleachAltNamesPurged.Value.Remove ("bottle of cleaning solution");
				bleachAltNamesPurged.Value.Remove ("cleaning solution");
				bleachAltNamesPurged.Value.Remove ("cleaner");
				bleachAltNamesPurged.Value.Remove ("disinfectant");
				altNames.Remove (bleachAltNames.Key);
				altNames.Add (bleachAltNamesPurged.Key, bleachAltNamesPurged.Value);
			}

			if (item.Index == 74 || item.Index == 75 || item.Index == 76) {

				ResetOverlay ();
				ImageCheckAndShow (73, 0, "checkitem");
				roomImage = false;

				if (IsInInv (87)) {
					killerInLair = true;
				}
			}

			if (twoLayerLook) {
				twoLayerLook = false;
			}

			UpdateItemGroup (item.Index);

			if (item.currentState.Image != "") {
				ResetOverlay ();
				ImageCheckAndShow (item.Index, item.State, item.currentState.Image);
				roomImage = false;
			} else {
				if (currOverlay != "blankoverlay") {
					ResetOverlay ();
				}
			}

			if (item.Index == 65 && (image.sprite.name == "washer" || image.sprite.name == "washer2" || image.sprite.name == "washer3" || image.sprite.name == "washer4" || 
				image.sprite.name == "washer5" || image.sprite.name == "washer6" || image.sprite.name == "washer7" || image.sprite.name == "washer8")) {
				var washer = itemsList [62];
				ImageCheckAndShow (washer.Index, washer.State, washer.currentState.Image);
				roomImage = false;
			}

			if (item.Index == 57 && (image.sprite.name == "bedrtable" || image.sprite.name == "bedrtable2" || image.sprite.name == "bedrtable3" || image.sprite.name == "bedrtable4" || 
				image.sprite.name == "bedrtable5" || image.sprite.name == "bedrtable6")) {
				var table = itemsList [89];
				ImageCheckAndShow (table.Index, table.State, table.currentState.Image);
				roomImage = false;
			}

			if (item.Index == 43 && (image.sprite.name == "stove" || image.sprite.name == "stove2")) {
				var stove = itemsList [32];
				ImageCheckAndShow (stove.Index, stove.State, stove.currentState.Image);
				roomImage = false;
			}

			if (item.Index == 15 && (image.sprite.name == "fireplace" || image.sprite.name == "fireplace2" || image.sprite.name == "fireplace3" || image.sprite.name == "fireplace4")) {
				var fireplace = itemsList [7];
				ImageCheckAndShow (fireplace.Index, fireplace.State, fireplace.currentState.Image);
				roomImage = false;
			}

			UpdateRoomState (roomImage);


		} else {

			if (item.Index == 54) {
				AddText ("You pick the painting up and attempt to jam it into your pockets. Having no luck, you set it down and see the safe embedded in the wall");
				ChangeState (54, 1);
				ChangeState (55, 1);
			} else if (item.Index == 93 && item.State == 1) {
				AddText ("Eh, this piece of paper's just gonna weigh you down. You do read it though: Safe Combination: 1-2-4-3. Really original, pal.");
				playerKnowsCombo = true;
			} else if (item.Index == 123) {
				AddText (GenericGet ());
				return;
			} else if (item.Index == 126) {
				OtherCommands ("read necronomicon");
				return;
			} else if (item.Index == 125) {
				SetImage (GetImageByName ("undercushion"));
				ChangeState (126, 1);
				AddText ("You lift the seat cushion and a strange book is revealed to be hiding underneath. You have no recollection of putting it - whatever it is - here.");
				ResetOverlay ();
				roomImage = true;
				UpdateItemGroup (item.Index);
				return;
			} else if (item.Index == 22 && item.State == 0) {
				var knife = GetObjectByName ("knife");
				ChangeState (22, 1);
				ChangeState (23, 1);
				inventory.Add (knife);
				toPlaySoundFX.Add (GetClip (12));
			} else if (item.Index == 17) {
				OtherCommands ("read book 1");
				return;
			} else if (item.Index == 18) {
				OtherCommands ("read book 2");
				return;
			} else if (item.Index == 19) {
				OtherCommands ("read book 3");
				return;
			} else if (item.Index == 20) {
				OtherCommands ("read book 4");
				return;
			} else if (item.Index == 122) {
				OtherCommands ("read book 666");
				return;
			} else if (item.Index == 132) {
				Look ("look journal.txt".Shlex ());
				return;
			} else if (item.Index == 141) {
				Use ("use dryer".Shlex ());
				return;
			} else if (item.Index == 148) {
				if (item.State == 0) {
					OtherCommands ("stab bed");
					return;
				}
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
				ResetOverlay ();
				ImageCheckAndShow (item.Index, item.State, item.currentState.Image);
				UpdateItemGroup (item.Index);
				roomImage = false;
			} else {
				if (image.sprite.name == currentRoom.currentState.Image) {
					roomImage = false;
				}
			}

			UpdateRoomState (roomImage);
		}


	}

	[Command]
	public void Use(List<string> argv)
	{
		string itemName = string.Join(" ", argv.Skip(1).ToArray());
		bool roomImage = true;
		bool invItem = false;

		if (itemName == "book" && currentRoom.Index == 0 && (image.sprite.name == "phone2" || image.sprite.name == "phone3" || image.sprite.name == "phone5")) {
			Look ("look address book".Shlex ());
			return;
		}

		if (argv.Contains ("with") || argv.Contains("and") || argv.Contains("on") || argv.Contains("at") || argv.Contains("together")) {
			UseWith (itemName);
			return;
		}

		if (currentRoom.Index == 0) {
			if (itemName == "1" || itemName == "one" || itemName == "number one" || itemName == "number 1") {
				OtherCommands ("read book 1");
				return;
			} else if (itemName == "2" || itemName == "two" || itemName == "number two" || itemName == "number 2") {
				OtherCommands ("read book 2");
				return;
			} else if (itemName == "3" || itemName == "three" || itemName == "number three" || itemName == "number 3") {
				OtherCommands ("read book 3");
				return;
			} else if (itemName == "4" || itemName == "four" || itemName == "number four" || itemName == "number 4") {
				OtherCommands ("read book 4");
				return;
			} else if (itemName == "666" || itemName == "six six six" || itemName == "six hundred sixty six" || itemName == "six hundred and sixty six"  || itemName == "number 666" || itemName == "number six six six"
				|| itemName == "number six hundred sixty six" || itemName == "number six hundred and sixty six" ) {
				OtherCommands ("read book 666");
				return;
			}
		}

		//In Room
		var item = GetObjectByName(itemName);
		if (item == null) {
			item = GetObjectFromInv (itemName);

			if (item == null) {
				AddText (GenericUse ());
				return;
			} else {
				invItem = true;
			}
		} else {
			var invObj = GetObjectFromInv (itemName);
			if (invObj != null) {
				invItem = true;
			}
		}

		var useResponses = specialResponses
			.Where(x => x.ItemIndex == item.Index)
			.Where(x => x.Command == "Use");

		if (useResponses.Count() == 0) {
			AddText(GenericUse ());
			UpdateItemGroup (item.Index);

			if (item.currentState.Image != "") {
				ResetOverlay ();
				ImageCheckAndShow (item.Index, item.State, item.currentState.Image);
			}

			return;
		}

		foreach (var response in useResponses) {

			if (invItem) {
				//SetImage (GetImageByName ("invbig-" + item.Name));
				lockMask = true;
				SetGasMaskOverlay (false);

				if (item.Index != 41) {
					ResetOverlay ();
				}
				MapArrow ();
				response.Image = "invbig-" + item.Name;
			}

			if (response.ItemIndex == 66) {
				bool someParts = false;
				bool allParts = false;

				if (!trapStep1) {
					int numItems = 0;

					foreach (var x in inventory) {
						if (x.Index == 23 || x.Index == 34 || x.Index == 148 || x.Index == 28 || x.Index == 43 || x.Index == 64 || x.Index == 15 || x.Index == 31 || x.Index == 47 || x.Index == 65 || x.Index == 35) {
							numItems++;
						}
					}

					if (numItems >= 2) {

						makingTraps = true;
						trapStep1 = true;

						ResetOverlay ();
						SetImage (GetImageByName ("workbench"));

						AddText ("You rummage through your pockets and pull out all the bits and bobs you think might help you make a trap to stop the killer:\n\n");

						AddBenchItems (false, false);

						AddAdditionalText ("\n\nWhat is the first item you would like to use?");
					}
					else {
						ResetOverlay ();
						SetImage (GetImageByName ("workbench"));
						AddText ("You appraise the contents of your pockets and decide you don't have enough components to make anything of value. Maybe you can come back once you have more stuff.");
						return;
					}
				}

				return;

				/*if (IsInInv (23) || IsInInv (34)) {

					if (IsInInv (23) && IsInInv (34)) {
						if (playerKnowsBeartrap) {
							AddText ("You bash the toaster against the counter, separating it into many pieces. Using what you learned from the home bear survival guide, you extract pieces of the frame and the springing mechanism and combine them with your knife.");
							RemoveFromInv (23);
							RemoveFromInv (34);
							ChangeState (98, 1);
							bearTrapMade = true;
							SetImage (GetImageByName ("beartrap"));
							++trapsAtOnce;
							AddAdditionalText ("\n\nPress [ENTER] to continue.");

							if (!((IsInInv (28) && IsInInv (43)) && (IsInInv (64) || IsInInv (15)) && playerKnowsFiretrap) && !(IsInInv (31) && IsInInv (47) && IsInInv (65))) {
								trapsDone = true;
							}

							return;
						} 
						else {
							allParts = true;
						}
					}
					else {
						someParts = true;
					}
				} 

				if (IsInInv (28) || IsInInv (43) || IsInInv (64) || IsInInv (15)) {
					if ((IsInInv (28) && IsInInv (43)) && (IsInInv (64) || IsInInv (15))) {
						if (playerKnowsFiretrap) {
							AddText ("Okay, in the movie, Johnny Knifeblaster mixed the fuel and orange juice and then used his lighter to set it off. Even through your panicked state and intense revere for the Knifeblaster Quadrilogy, you experience a shimmer of just how ridiculous this idea is. Regardless, you push on and assemble the appropriate ingredients.");
							RemoveFromInv (28);
							RemoveFromInv (43);
							if (IsInInv (15))
								RemoveFromInv (15);
							if (IsInInv (64))
								RemoveFromInv (64);
							ChangeState (100, 1);
							fireTrapMade = true;
							SetImage (GetImageByName ("firetrap"));
							++trapsAtOnce;
							AddAdditionalText ("\n\nPress [ENTER] to continue.");

							if (!(IsInInv (31) && IsInInv (47) && IsInInv (65))) {
								trapsDone = true;
							}

							return;
						}
						else {
							allParts = true;
						}
					}
					else {
						someParts = true;
					}
				} 
				if (IsInInv (31) || IsInInv (47) || IsInInv (65)) {
					if (IsInInv (31) && IsInInv (47) && IsInInv (65)) {
						if (gasMaskOn) {
							AddText ("You pour the entire bottle of bleach into the bucket, and pause briefly to confirm to yourself you really want to do this. Then, holding it at an arm's length, you open the bottle of ammonia and empty it into the bleach-filled bucket.\n\nPress [ENTER] to continue.");
							RemoveFromInv (31);
							RemoveFromInv (47);
							RemoveFromInv (65);
							ChangeState (99, 1);
							bucketTrapMade = true;
							SetImage (GetImageByName ("buckettrap"));
							++trapsAtOnce;
							trapsDone = true;

							return;
						}
						else {
							ChangeState (-1, 100);
							AddText ("You pour the entire bottle of bleach into the bucket, and pause briefly to confirm to yourself you really want to do this. Then, holding it at an arm's length, you open the bottle of ammonia and empty it into the bleach-filled bucket.\n\nPress [ENTER] to continue.");
							SetImage (GetImageByName ("buckettrap"));
							multiSequence = true;
							currMultiSequence = 29;
							makingTraps = false;
							return;
						}
					}
					else {
						someParts = true;
					}
				}

				var workbench = GetObjectByName ("workbench");
				makingTraps = false;
				ImageCheckAndShow (workbench.Index, workbench.State, workbench.currentState.Image);

				if (allParts) {
					AddText ("You're sure that with all of the stuff you have on you, some combination of things would help you slow or stop the killer, but you can't think of anything. Maybe there's something around the house that could give you an idea.");
				}
				else if (someParts) {
					AddText ("You wrack your brains for some creative uses, but no combination of the items you have appears like it would accomplish anything.");
				}
				else {
					AddText ("You don't have anything on you that you can work with.");
				}*/
			}


			if (response.ItemIndex == 57) {
				if (IsInInv(57)) {
					if (currentRoom.Index == 5) {
						if (tapeRecorderUsed) {
							HideNoItem ("tape");
							return;
						} else {
							tapeRecorderUsed = true;
							AddText ("You hypothesize that maybe you could distract or lure the killer with sounds of yourself coming from this thing. You record yourself making some sniffles and moans of terror. Hopefully your acting is convincing enough. You were once a narrating rat for a school play in elementary school.\n\nWhat do you do next?");
							SetImage (GetImageByName ("invbig-tape recorder"));
							roomImage = false;
							UpdateRoomState (roomImage);
							UpdateItemGroup (item.Index);
							return;
						}
					} 
					else {
						if (tapeRecorderUsed) {
							if (currentRoom.Index == 8) {

								if (dummyStepsCompleted == 0) {
									AddText ("There's nothing you can use that with...yet.");
									return;
								} else {
									AddText ("What do you want to use it with?");
									inputLockdown = true;
									currLockdownOption = 5;
									return;
								}
							}
							else {
								AddText ("You lower the volume and playback the wailing. This might come in handy to lure the killer in the right spot.");
								SetImage (GetImageByName ("invbig-tape recorder"));
								roomImage = false;
								UpdateRoomState (roomImage);
								UpdateItemGroup (item.Index);
								return;
							}
						}
						else {
							tapeRecorderUsed = true;
							AddText ("You hypothesize that maybe you could distract or lure the killer with sounds of yourself coming from this thing. You record yourself making some sniffles and moans of terror. Hopefully your acting is convincing enough. You were once a narrating rat for a school play in elementary school.");
							UpdateRoomState (roomImage);
							SetImage (GetImageByName ("invbig-tape recorder"));
							roomImage = false;
							UpdateItemGroup (item.Index);
							return;
						}
					}
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
					AddText ("You don't really feel like eating candy right now. Your heart is just not in it.");
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
			}

			if (response.ItemIndex == 115)
			{
				var dummy = itemsList [115];
				var PINATAPLACEHOLDER = itemsList [102];
				if (dummy.State == 0) {
					AddText (GenericUse ());
					return;
				}

				if (dummyStepsCompleted == 2 && PINATAPLACEHOLDER.State == 1) {
					AddText ("Now if only it sounded like you too; if it made some sound so that the killer would be drawn to it...");
					ImageCheckAndShow (dummy.Index, dummy.State, dummy.currentState.Image);
					return;
				}

				if (dummyStepsCompleted == 3) {
					AddText ("It's perfect. Now just to hide and wait for the killer to take the bait.");
					ImageCheckAndShow (dummy.Index, dummy.State, dummy.currentState.Image);
					return;
				}
			}

			if (response.ItemIndex == 63)
			{
				ResetOverlay ();
				SetImage (GetImageByName ("dryerdeath"));
				AddText ("You get your clothes out of the dryer and try to delude yourself that your life is not in immediate danger. Everything's going pretty well until you try to carry a massive armload of clothes upstairs and fall down the stairs, breaking your neck.\n\nPress [ENTER] to try again.");

				GameOverAudio (-1, true);

				health = 0;
				return;
			}

			if (response.ItemIndex == 67)
			{
				if (currentRoom.Index == 8) {
					if (dummyStepsCompleted == 0) {
						AddText ("There's nothing you can use that with...yet.");
						return;
					} else {

						response.Response = "What do you want to use it with?";
						response.Image = "";
						inputLockdown = true;
						currLockdownOption = 4;
					}
				}
			}

			if (response.ItemIndex == 114)
			{
				Look ("look map".Shlex ());
				return;
			}

			if (response.ItemIndex == 7) {

				if (item.State == 0) {

					ResetOverlay ();
					if (IsInInv (15) || IsInInv (64)) {
						if (!IsInInv (43)) {
							AddText ("You don't have anything to light it with.");
							ImageCheckAndShow (response.ItemIndex, response.ItemState, "showitem");
							return;
						} else {
							GameOverAudio (-1, true);
						}
					} else {
						if (IsInInv (43)) {
							AddText ("You don't have anything flammable on you.");
							ImageCheckAndShow (response.ItemIndex, response.ItemState, "showitem");
							return;
						} else {
							AddText ("You don't have the means of starting a fire on you.");
							ImageCheckAndShow (response.ItemIndex, response.ItemState, "showitem");
							return;
						}
					}
				} else {
					Move ("move lair".Shlex ());
					return;
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
						AddText ("You pick up the gas mask and fit it over your face.");
						//UpdateRoomState ();
						//inventory.Add (item);
					} else {
						AddText ("You fit the gas mask over your face.");
					}

					if (image.sprite.name.Contains("invbig") || image.sprite.name == "blankoverlay") {
						UpdateRoomState ();
					}

					SetGasMaskOverlay (true);
					gasMaskOn = true;
					return;
				} 
				else {
					AddText ("You remove the gas mask");
					SetGasMaskOverlay (false);
					gasMaskOn = false;
					return;
				}
			}

			if (response.ItemIndex == 2) {
				OtherCommands ("open window");
				return;
			}

			if (response.ItemIndex == 125) {
				Look ("look under cushion".Shlex());
				return;
			}

			if (response.ItemIndex == 145) {
				Use ("use sink".Shlex());
				return;
			}

			if (response.ItemIndex == 146) {
				Use ("use sink".Shlex());
				return;
			}

			if (response.ItemIndex == 147) {
				Use ("use sink".Shlex());
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

			if (response.ItemIndex == 71) {
				if (item.State == 0){
					OtherCommands ("turn on computer");
				}
				else{
					OtherCommands ("turn off computer");
				}
				return;
			}

			if (response.ItemIndex == 132) {
				if (item.State == 0){
					AddText (GenericUse ());
					return;
				}
			}

			if (response.ItemIndex == 54) {
				Move ("move painting".Shlex ());
				return;
			}

			if (response.ItemIndex == 55) {
				if (item.State == 1){
					OtherCommands ("open safe");
				}
				else if (item.State == 2) {
					OtherCommands ("close safe");
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

			if (response.ItemIndex == 25) {
				if (item.State == 0){
					OtherCommands ("open refrigerator");
				}
				else{
					OtherCommands ("close refrigerator");
				}
				return;
			}

			if (response.ItemIndex == 26) {
				if (item.State == 0){
					OtherCommands ("open freezer");
				}
				else{
					OtherCommands ("close freezer");
				}
				return;
			}

			if (response.ItemIndex == 46) {
				if (item.State == 0){
					OtherCommands ("open cabinet");
				}
				else{
					OtherCommands ("close cabinet");
				}
				return;
			}

			if (response.ItemIndex == 51) {
				if (item.State == 0){
					OtherCommands ("open closet");
				}
				else{
					OtherCommands ("close closet");
				}
				return;
			}

			if (response.ItemIndex == 30) {
				if (item.State == 0) {
					OtherCommands ("open cabinet");
				}
				else{
					OtherCommands ("close cabinet");
				}
				return;
			}

			if (response.ItemIndex == 49) {
				if (item.State == 0){
					OtherCommands ("close curtain");
				}
				else{
					OtherCommands ("open curtain");
				}
				return;
			}

			if (response.ItemIndex == 31) {
				if (item.State == 0 && !IsInInv(31)) {
					AddText (GenericUse ());
					return;
				}
			}

			if (response.ItemIndex == 141) {

				var clothes = itemsList [141];

				if (clothes.State == 1) {
					Use ("use dryer".Shlex ());
					return;
				}
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
			if (response.ItemIndex == 69) {
				Move ("move living room".Shlex());
				return;
			}
			if (response.ItemIndex == 129) {
				Move ("move living room".Shlex());
				return;
			}
			if (response.ItemIndex == 133) {
				Move ("move lair".Shlex());
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
			if (response.ItemIndex == 122) {
				OtherCommands ("read book 666");
				return;
			}

			if (response.ItemIndex == 50) {
				OtherCommands ("sleep bed");
				return;
			}

			if (response.ItemIndex == 132)
			{
				Look ("look journal.txt".Shlex());
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

			if (response.ItemIndex == 139) {
				var lrstairs = itemsList [139];
				if (lrstairs.State == 1) {
					if (image.sprite.name == "fireplace3" || image.sprite.name == "fireplace4") {
						Use ("use fireplace stairs".Shlex ());
						return;
					}

					if (image.sprite.name == "staircase") {
						Use ("use living room staircase".Shlex ());
						return;
					}

					ResetOverlay ();
					inputLockdown = true;
					currLockdownOption = 37;
				} else {
					Use ("use living room staircase".Shlex ());
					return;
				}
			}

			if (response.ItemIndex == 6) {
				inputLockdown = true;
				currLockdownOption = 8;
			}

			if (response.ItemIndex == 111) {

				if (image.sprite.name == "backdoor" || image.sprite.name == "backdoor2" || image.sprite.name == "backdoor3" || image.sprite.name == "backdoor4") {
					Use ("use back door".Shlex ());
					return;
				}

				if (image.sprite.name == "basementdoor") {
					Use ("use basement door".Shlex ());
					return;
				}

				ResetOverlay ();
				inputLockdown = true;
				currLockdownOption = 12;
			}

			if (response.ItemIndex == 112) {
				ResetOverlay ();
				inputLockdown = true;
				currLockdownOption = 16;
			}

			if (response.ItemIndex == 60) {
				ResetOverlay ();
				inputLockdown = true;
				currLockdownOption = 21;
			}

			if (response.ItemIndex == 5) {
				Look ("look peephole".Shlex());
				return;
			}

			if (response.ItemIndex == 48) {
				OtherCommands ("take shower");
				return;
			}

			if (response.ItemIndex == 96)
			{
				Use("use drawer".Shlex());
				return;
			}

			if (response.ItemIndex == 58)
			{
				var gun = itemsList[58];

				if (IsInInv (58)) {
					SetImage (GetImageByName ("invbig-revolver"));
					lockMask = true;
					ResetOverlay ();
					SetGasMaskOverlay (false);

					PlayClip (GetClip (37));

					AddText ("It clicks uselessly as the hammer hits the empty chamber. Only had the one bullet.");
					return;
				} else if (currentRoom.Index == 4 && gun.State == 0) {
					AddText (GenericUse ());
					return;
				}
			}

			if (response.ItemIndex == 87)
			{
				var bear = itemsList[87];

				if (IsInInv (87)) {
					SetImage (GetImageByName ("invbig-teddy bear"));
					lockMask = true;
					ResetOverlay ();
					SetGasMaskOverlay (false);
					AddText ("You hug it and disappear for a moment into its fur, your happy childhood.\n\n...Back to reality.");
					return;
				}
			}

			if (response.ItemIndex == 43)
			{
				var lighter = itemsList[43];

				if (IsInInv (43)) {
					SetImage (GetImageByName ("invbig-lighter"));
					lockMask = true;
					ResetOverlay ();
					SetGasMaskOverlay (false);
					AddText ("You pull the trigger and the flame, being longer than you expect, extends up into your face and singes your eyebrows. I guess this thing really goes off.");
					return;
				}
			}

			if (response.ItemIndex == 28)
			{
				var juice = itemsList[28];

				if (juice.State == 0) {
					AddText (GenericUse ());
					return;
				}

				if (IsInInv (28)) {
					SetImage (GetImageByName ("invbig-orange juice"));
					lockMask = true;
					ResetOverlay ();
					SetGasMaskOverlay (false);
					AddText ("While you aren't above opening the container and eating the frozen orange slush, you feel as though you should probably save it for helping you get out of this predicament.");
					return;
				}
			}

			if (response.ItemIndex == 23)
			{
				var knife = itemsList[23];

				if (IsInInv (23)) {
					SetImage (GetImageByName ("invbig-knife"));
					lockMask = true;
					ResetOverlay ();
					SetGasMaskOverlay (false);
					AddText ("You swish the knife through the air. Take that, air!");
					return;
				}
			}

			if (response.ItemIndex == 47)
			{
				var bleach = itemsList[47];

				if (bleach.State == 0) {
					AddText (GenericUse ());
					return;
				}
			}

			if (response.ItemIndex == 97)
			{
				var poster = itemsList[97];

				if (poster.State == 0) {
					AddText (GenericUse ());
					return;
				}
			}

			if (response.ItemIndex == 10)
			{
				var addressBook = itemsList[10];

				if (addressBook.State == 0) {
					AddText (GenericUse ());
					return;
				}
			}

			if (response.ItemIndex == 52)
			{
				var flashlight = itemsList[52];

				if (IsInInv (52)) {
					SetImage (GetImageByName ("invbig-flashlight"));
					lockMask = true;
					ResetOverlay ();
					SetGasMaskOverlay (false);
					AddText ("You can see perfectly well in here - you decide to turn it on only when you need it so as not to alert the killer to your presence.");
					return;
				} else if (currentRoom.Index == 4 && flashlight.State == 0) {
					AddText (GenericUse ());
					return;
				}
			}

			if (response.ItemIndex == 89)
			{
				Use("use drawer".Shlex());
				return;
			}

			if (response.ItemIndex == 22)
			{
				Get("get knife block".Shlex());
				return;
			}

			if (response.ItemIndex == 93 )
			{
				var combination = itemsList [93];
				if (combination.State == 1) {
					Look ("look combination".Shlex ());
					return;
				} else {
					AddText (GenericUse());
					return;
				}
			}

			if (response.ItemIndex == 126 && item.State == 0)
			{
				if (item.State == 0) {
					AddText (GenericUse ());
					return;
				} else {
					GameOverAudio (-1, true);
				}
			}

			if (response.ItemIndex == 34)
			{
				if (currentRoom.Index == 3) {
					inputLockdown = true;
					currLockdownOption = 35;
					AddText ("What do you want to use it with?");
					return;
				}
			}

			AddText (response.Response);

			foreach (KeyValuePair<int, int> actions in response.Actions) {
				if (ChangeState (actions.Key, actions.Value) == 1)
					break;
			}

			if (response.Image != "") {
				ResetOverlay ();
				ImageCheckAndShow (response.ItemIndex, response.ItemState, response.Image);
				roomImage = false;
			}

			UpdateItemGroup (item.Index);
			UpdateRoomState (roomImage);
			return;
		}
	}

	void GetDummyItems(){
		room = 1;
		inventory.Add (GetObjectByName ("knife"));
		room = 4;
		inventory.Add (GetObjectByName("tape recorder"));
		inventory.Add (GetObjectByName("flashlight"));
		room = 5;
		inventory.Add (GetObjectByName ("pinata"));
		room = 7;
		/*inventory.Add (52);
		inventory.Add (67);*/
	}

	bool IsInAltNames(string toCheck, string itemName){
		bool isInAltNames = false;

		var itemAltNames = GetAltNames (toCheck);

		foreach (var itemAltName in itemAltNames) {
			if (itemName.Contains (itemAltName)) {
				isInAltNames = true;
			}
		}

		return isInAltNames;
	}

	void UseWith (string itemName){

		if (currentRoom.Index != 5) {
			var toasterNames = GetAltNames ("toaster");
			var knifeNames = GetAltNames ("knife");
			var lighterNames = GetAltNames ("lighter");
			var gasNames = GetAltNames ("gas can");
			var fluidNames = GetAltNames ("lighter fluid");
			var juiceNames = GetAltNames ("orange juice");
			var bucketNames = GetAltNames ("bucket");
			var ammoniaNames = GetAltNames ("ammonia");
			var bleachNames = GetAltNames ("bleach");
			var blenderNames = GetAltNames ("blender");
			var springNames = GetAltNames ("spring");

			var sinkNames = GetAltNames ("sink");
			var ksinkNames = GetAltNames ("kitchen sink");
			var showerNames = GetAltNames ("shower");
			var toiletNames = GetAltNames ("toilet");

			bool isToaster = false;
			bool isKnife = false;
			bool isLighter = false;
			bool isGas = false;
			bool isFluid = false;
			bool isJuice = false;
			bool isBucket = false;
			bool isAmmonia = false;
			bool isBleach = false;
			bool isBlender = false;
			bool isSpring = false;

			bool isSink = false;
			bool isShower = false;
			bool isToilet = false;

			int trapItems = 0;

			foreach (var toasterName in toasterNames) {
				if (itemName.Contains (toasterName)) {
					if (IsInInv (34) || currentRoom.Index == 1) {
						isToaster = true;
					}
				}
			}
			foreach (var knifeName in knifeNames) {
				if (itemName.Contains (knifeName)) {
					if (IsInInv (23) || currentRoom.Index == 1) {
						isKnife = true;
					}
				}
			}
			foreach (var lighterName in lighterNames) {
				if (itemName.Contains (lighterName)) {
					if (IsInInv (43) || currentRoom.Index == 1) {
						isLighter = true;
					}
				}
			}
			foreach (var gasName in gasNames) {
				if (itemName.Contains (gasName)) {
					if (IsInInv (64) || currentRoom.Index == 5) {
						isGas = true;
					}
				}
			}
			foreach (var fluidName in fluidNames) {
				if (itemName.Contains (fluidName)) {
					if (IsInInv (15) || currentRoom.Index == 0) {
						isFluid = true;
					}
				}
			}
			foreach (var juiceName in juiceNames) {
				if (itemName.Contains (juiceName)) {
					var oj = itemsList [28];

					if (IsInInv (28) || (currentRoom.Index == 1 && oj.State != 0)) {
						isJuice = true;
					}
				}
			}
			foreach (var bucketName in bucketNames) {
				if (itemName.Contains (bucketName)) {

					var bucket = itemsList [31];

					if (IsInInv (31) || (currentRoom.Index == 1 && bucket.State != 0)) {
						isBucket = true;
					}
				}
			}
			foreach (var ammoniaName in ammoniaNames) {
				if (itemName.Contains (ammoniaName)) {
					if (IsInInv (65) || currentRoom.Index == 5) {
						isAmmonia = true;
					}
				}
			}
			foreach (var bleachName in bleachNames) {
				if (itemName.Contains (bleachName)) {
					var bleach = itemsList [47];

					if (IsInInv (47) || (currentRoom.Index == 3 && bleach.State != 0)) {
						isBleach = true;
					}
				}
			}
			foreach (var blenderName in blenderNames) {
				if (itemName.Contains (blenderName)) {
					if (IsInInv (35) || currentRoom.Index == 1) {
						isBlender = true;
					}
				}
			}
			foreach (var springName in springNames) {
				if (itemName.Contains (springName)) {
					if (IsInInv (148)) {
						isSpring = true;
					}
				}
			}

			foreach (var ksinkName in ksinkNames) {
				if (itemName.Contains (ksinkName) && (currentRoom.Index == 1 || currentRoom.Index == 3 || currentRoom.Index == 6)) {
					isSink = true;
				}
			}

			foreach (var sinkName in sinkNames) {
				if (itemName.Contains (sinkName) && (currentRoom.Index == 1 || currentRoom.Index == 3 || currentRoom.Index == 6)) {
					isSink = true;
				}
			}

			foreach (var showerName in showerNames) {
				if (itemName.Contains (showerName) && currentRoom.Index == 3) {
					isShower = true;
				}
			}

			foreach (var toiletName in toiletNames) {
				if (itemName.Contains (toiletName) && currentRoom.Index == 3) {
					isToilet = true;
				}
			}

			if (isToaster)
				trapItems++;
			if (isKnife)
				trapItems++;
			if (isLighter)
				trapItems++;
			if (isGas)
				trapItems++;
			if (isFluid)
				trapItems++;
			if (isJuice)
				trapItems++;
			if (isBucket)
				trapItems++;
			if (isAmmonia)
				trapItems++;
			if (isBleach)
				trapItems++;
			if (isBlender)
				trapItems++;
			if (isSpring)
				trapItems++;

			if (trapItems > 1) {

				if (isBleach || isAmmonia) {
					if (isSink || isShower || isToilet) {
						if (isSink) {
							var sinkName = "sink";
							if (currentRoom.Index == 1) {
								sinkName = "kitchen sink";
							}
							var sinkObj = GetObjectByName (sinkName);
							ResetOverlay ();
							ImageCheckAndShow (sinkObj.Index, sinkObj.State, sinkObj.currentState.Image);
							AddText ("You should probably mix your chemicals in a more portable receptacle.");
							return;
						} 

						if (isShower) {
							var showerObj = GetObjectByName ("shower");
							ResetOverlay ();
							ImageCheckAndShow (showerObj.Index, showerObj.State, showerObj.currentState.Image);
							AddText ("You should probably mix your chemicals in a more portable receptacle.");
							return;
						}
						if (isToilet) {
							var toiletObj = GetObjectByName ("toilet");
							ResetOverlay ();
							ImageCheckAndShow (toiletObj.Index, toiletObj.State, toiletObj.currentState.Image);
							AddText ("You should probably mix your chemicals in a more portable receptacle.");
							return;
						}
					} else if (isBucket || isBlender) {
						if (isBleach && isAmmonia) {
							if (isBucket) {
								AddText ("You could mix these together in the bucket, but you’d rather do it in the basement, where you do all of your chemical mixing.");
							} else {
								AddText ("You could mix these together in the blender, but you’d rather do it in the basement, where you do all of your chemical mixing.");
							}
						} else {
							if (isBucket) {
								AddText ("You could put that in the bucket, but you’d rather do it in the basement, where you do all of your chemical mixing.");
							} else {
								AddText ("You could put that in the blender, but you’d rather do it in the basement, where you do all of your chemical mixing.");
							}
						}
						return;
					} else {
						if (isBleach && isAmmonia) {
							AddText ("You could mix these together, but you'd need a recepticle.");
							return;
						}
					}
				}

				AddText ("Combining these items might be easier with a flat workspace and some additional tools.");
				return;
			}

			if (trapItems == 1){
				if (isBleach || isAmmonia) {
					if (isSink) {
						var sinkName = "sink";
						if (currentRoom.Index == 1) {
							sinkName = "kitchen sink";
						}
						var sinkObj = GetObjectByName (sinkName);
						ResetOverlay ();
						ImageCheckAndShow (sinkObj.Index, sinkObj.State, sinkObj.currentState.Image);
						AddText ("You should probably mix your chemicals in a more portable receptacle.");
						return;

					} 

					if (isShower) {
						var showerObj = GetObjectByName ("shower");
						ResetOverlay ();
						ImageCheckAndShow (showerObj.Index, showerObj.State, showerObj.currentState.Image);
						AddText ("You should probably mix your chemicals in a more portable receptacle.");
						return;
					}
					if (isToilet) {
						var toiletObj = GetObjectByName ("toilet");
						ResetOverlay ();
						ImageCheckAndShow (toiletObj.Index, toiletObj.State, toiletObj.currentState.Image);
						AddText ("You should probably mix your chemicals in a more portable receptacle.");
						return;
					}
				} 
			}
		}

		if (currentRoom.Index == 0) {
			var doorNames = GetAltNames ("front door");
			var chairNames = GetAltNames ("arm chair");
			var bookcaseNames = GetAltNames ("bookcase");
			var tableNames = GetAltNames ("table");
			var lampNames = GetAltNames ("lamp");

			var fireplaceNames = GetAltNames ("fireplace");
			var fluidNames = GetAltNames ("lighter fluid");
			var gasNames = GetAltNames ("gas can");

			bool isDoor = false;
			bool isChair = false;
			bool isBookcase = false;
			bool isTable = false;
			bool isLamp = false;

			bool isFireplace = IsInAltNames ("fireplace", itemName);
			bool isFluid = IsInAltNames ("lighter fluid", itemName);
			bool isGas = IsInAltNames ("gas can", itemName);
			bool isLighter = IsInAltNames ("lighter", itemName);

			foreach (var doorName in doorNames) {
				if (itemName.Contains(doorName)) {
					isDoor = true;
				}
			}

			foreach (var chairName in chairNames) {
				if (itemName.Contains (chairName)) {
					isChair = true;
				}
			}

			foreach (var bookcaseName in bookcaseNames) {
				if (itemName.Contains(bookcaseName)) {
					isBookcase = true;
				}
			}

			foreach (var tableName in tableNames) {
				if (itemName.Contains (tableName)) {
					isTable = true;
				}
			}

			foreach (var lampName in lampNames) {
				if (itemName.Contains (lampName)) {
					isLamp = true;
				}
			}

			if (isDoor) {

				if (isChair) {
					OtherCommands ("block door");
					return;
				}

				if (isBookcase) {
					Move("move bookcase".Shlex());
					return;
				}

				if (isTable) {
					Move("move table".Shlex());
					return;
				}

				if (isLamp) {
					Move("move lamp".Shlex());
					return;
				}

			}

			if (isFireplace) {

				if (isFluid) {
					if (IsInInv (15)) {
						var fireplaceObj = GetObjectByName ("fireplace");

						if (fireplaceObj.State == 0) {
							Use ("use fireplace".Shlex ());
							return;
						} else {
							ResetOverlay ();
							ImageCheckAndShow (fireplaceObj.Index, fireplaceObj.State, fireplaceObj.currentState.Image);
							AddText ("Now that a passage has opened in the back of the fireplace, it's probably not a great time to light a fire in it.");
							return;
						}
					}

					else {
						var fluid = itemsList [15];
						ResetOverlay ();
						ImageCheckAndShow (fluid.Index, fluid.State, fluid.currentState.Image);
						AddText ("You would have to grab it first.");
						return;
					}
				}

				if (isGas || isLighter) {
					var fireplaceObj = GetObjectByName ("fireplace");

					if (fireplaceObj.State == 0) {
						Use ("use fireplace".Shlex ());
						return;
					} else {
						ResetOverlay ();
						ImageCheckAndShow (fireplaceObj.Index, fireplaceObj.State, fireplaceObj.currentState.Image);
						AddText ("Now that a passage has opened in the back of the fireplace, it's probably not a great time to light a fire in it.");
						return;
					}
				}
			}

		}

		if (currentRoom.Index == 1) {
			var bucketNames = GetAltNames ("bucket");
			var sinkNames = GetAltNames ("sink");
			var faucetNames = GetAltNames ("faucet");
			var blenderNames = GetAltNames ("blender");

			bool isBucket = false;
			bool isSink = false;
			bool isFaucet = false;
			bool isBlender = false;

			foreach (var bucketName in bucketNames) {
				if (itemName.Contains(bucketName)) {
					isBucket = true;
				}
			}

			foreach (var blenderName in blenderNames) {
				if (itemName.Contains(blenderName)) {
					isBlender = true;
				}
			}

			foreach (var sinkName in sinkNames) {
				if (itemName.Contains(sinkName)) {
					isSink = true;
				}
			}

			foreach (var faucetName in faucetNames) {
				if (itemName.Contains(faucetName)) {
					isFaucet = true;
				}
			}

			if (isBucket) {
				if ((isSink || isFaucet) || itemName.Contains("water")) {
					if (IsInInv(31)) {
					SetImage (GetImageByName ("ksink"));
					AddText ("You'd really rather not carry a big heavy bucket of water around, plus you're already exceptionally hydrated.");
					return;
					}
				}
			}

			if (isBlender) {
				if ((isSink || isFaucet) || itemName.Contains("water")) {
					if (IsInInv(35)) {
						SetImage (GetImageByName ("ksink"));
						AddText ("You'd really rather not carry a heavy blender full of water around, plus you're already exceptionally hydrated.");
						return;
					}
				}
			}
		}

		if (currentRoom.Index == 3) {
			var toasterNames = GetAltNames ("toaster");
			var showerNames = GetAltNames ("shower");
			var bucketNames = GetAltNames ("bucket");
			var sinkNames = GetAltNames ("sink");
			var faucetNames = GetAltNames ("faucet");
			var blenderNames = GetAltNames ("blender");

			bool isBucket = false;
			bool isSink = false;
			bool isFaucet = false;
			bool isBlender = false;
			bool isToaster = false;
			bool isShower = false;

			foreach (var bucketName in bucketNames) {
				if (itemName.Contains (bucketName)) {
					isBucket = true;
				}
			}

			foreach (var blenderName in blenderNames) {
				if (itemName.Contains (blenderName)) {
					isBlender = true;
				}
			}

			foreach (var sinkName in sinkNames) {
				if (itemName.Contains (sinkName)) {
					isSink = true;
				}
			}

			foreach (var faucetName in faucetNames) {
				if (itemName.Contains (faucetName)) {
					isFaucet = true;
				}
			}

			foreach (var showerName in showerNames) {
				if (itemName.Contains (showerName)) {
					isShower = true;
				}
			}

			foreach (var toasterName in toasterNames) {
				if (itemName.Contains(toasterName)) {
					isToaster = true;
				}
			}

			if (isBucket) {
				if ((isSink || isFaucet) || itemName.Contains ("water")) {
					if (IsInInv (31)) {
						var bathroomSink = GetObjectByName ("sink");
						ImageCheckAndShow (bathroomSink.Index, bathroomSink.State, bathroomSink.currentState.Image);
						AddText ("You'd really rather not carry a big heavy bucket of water around, plus you're already exceptionally hydrated.");
						return;
					}
				}
			}

			if (isBlender) {
				if ((isSink || isFaucet) || itemName.Contains ("water")) {
					if (IsInInv (35)) {
						var bathroomSink = GetObjectByName ("sink");
						ImageCheckAndShow (bathroomSink.Index, bathroomSink.State, bathroomSink.currentState.Image);
						AddText ("You'd really rather not carry a heavy blender full of water around, plus you're already exceptionally hydrated.");
						return;
					}
				}
			}

			if (isShower) {
				if (isToaster) {
					if (IsInInv (34)) {
						ResetOverlay ();
						SetGasMaskOverlay (false);
						SetImage (GetImageByName ("toasterdeath"));
						AddText ("You throw open the taps on your bathtub and plug the toaster into the wall. The man at the door is not going to have the satisfaction of killing you. You climb into the tepid water and arm the toaster's toasting mechanism. There is a \"thunk\" as the toaster drops into the bath and soon you are a wet, fried corpse.\n\nPress [ENTER] to try again.");

						GameOverAudio (-1, true);

						health = 0;
						return;
					}
				} else if (isBucket) {
					if (IsInInv (31)) {
						var shower = GetObjectByName ("shower");
						ImageCheckAndShow (shower.Index, shower.State, shower.currentState.Image);
						AddText ("You'd really rather not carry a big heavy bucket of water around, plus you're already exceptionally hydrated.");
						return;
					}
				} else if (isBlender) {
					if (IsInInv (35)) {
						var shower = GetObjectByName ("shower");
						ImageCheckAndShow (shower.Index, shower.State, shower.currentState.Image);
						AddText ("You'd really rather not carry a heavy blender full of water around, plus you're already exceptionally hydrated.");
						return;
					}
				} else if (itemName.Contains ("fill")) {
					var shower = GetObjectByName ("shower");
					ImageCheckAndShow (shower.Index, shower.State, shower.currentState.Image);
					AddText ("Look, if you didn't have time for a bath this morning, you certainly don't have time for one now.");
					return;
				}
			}
		}

		if (currentRoom.Index == 4) {
			var safeNames = GetAltNames ("safe");
			var combinationNames = GetAltNames ("combination");
			var knifeNames = GetAltNames ("knife");
			var bedNames = GetAltNames ("bed");

			bool isSafe = false;
			bool isCombo = false;
			bool isKnife = false;
			bool isBed = false;

			foreach (var safeName in safeNames) {
				if (itemName.Contains(safeName)) {
					isSafe = true;
				}
			}

			foreach (var combinationName in combinationNames) {
				if (itemName.Contains (combinationName)) {
					isCombo = true;
				}
			}

			foreach (var knifeName in knifeNames) {
				if (itemName.Contains (knifeName) && IsInInv(23)) {
					isKnife = true;
				}
			}

			foreach (var bedName in bedNames) {
				if (itemName.Contains (bedName)) {
					isBed = true;
				}
			}

			if (isSafe && isCombo) {
				OtherCommands ("open safe");
				return;
			}

			if (isKnife && isBed) {

				var springObj = GetObjectByName ("spring");

				if (springObj.State == 0) {
					ChangeState (148, 1);
					inventory.Add (GetObjectByName ("spring"));
					toPlaySoundFX.Add (GetClip (12));
					AddText ("You grab the spinrgus.");
				} else {
					AddText ("Stop, stop, he's already dead!");
				}

				return;
			}
		}

		if (currentRoom.Index == 8) {
			var rakeNames = GetAltNames ("rake");
			var tarpNames = GetAltNames ("tarp");
			var pinataNames = GetAltNames ("pinata");
			var tapeNames = GetAltNames ("tape recorder");
			var dummyNames = GetAltNames ("dummy");

			bool isRake = false;
			bool isTarp = false;
			bool isPinata = false;
			bool isTape = false;
			bool isDummy = false;

			foreach (var rakeName in rakeNames) {
				if (itemName.Contains(rakeName)) {
					isRake = true;
				}
			}

			foreach (var tarpName in tarpNames) {
				if (itemName.Contains (tarpName)) {
					isTarp = true;
				}
			}

			foreach (var pinataName in pinataNames) {
				if (itemName.Contains (pinataName)) {
					isPinata = true;
				}
			}

			foreach (var pinataName in pinataNames) {
				if (itemName.Contains (pinataName)) {
					isPinata = true;
				}
			}

			foreach (var tapeName in tapeNames) {
				if (itemName.Contains (tapeName)) {
					isTape = true;
				}
			}

			foreach (var dummyName in dummyNames) {
				if (itemName.Contains (dummyName)) {
					isDummy = true;
				}
			}

			if (isRake && isTarp) {

				if (dummyStepsCompleted == 0) {
					currLockdownOption = 3;
					CombineItems ();
					return;
				} 
				else {
					AddText ("Already did that.");
					return;
				}
			}

			if (isPinata && (isTarp || isRake || isDummy)) {

				if (dummyStepsCompleted != 0) {

					if (IsInInv (67)) {
						currLockdownOption = 4;
						CombineItems ();
						return;
					}
					else {
						AddText ("Already did that.");
						return;
					}
				}
			}

			if (isTape && IsInInv(57) && (isTarp || isRake || isDummy)) {

				if (dummyStepsCompleted != 0) {

					if (!tapeRecorderUsed) {
						AddText ("Hmm, you might need to record something on it first.");
						return;
					} 

					else {
						if (IsInInv (57)) {
							currLockdownOption = 5;
							CombineItems ();
							return;
						}
						else {
							AddText ("Already did that.");
							return;
						}
					}
				}
			}
		}

		if (currentRoom.Index == 6) {
			var bucketNames = GetAltNames ("bucket");
			var sinkNames = GetAltNames ("sink");
			var faucetNames = GetAltNames ("faucet");
			var blenderNames = GetAltNames ("blender");

			bool isBucket = false;
			bool isSink = false;
			bool isFaucet = false;
			bool isBlender = false;

			foreach (var bucketName in bucketNames) {
				if (itemName.Contains(bucketName)) {
					isBucket = true;
				}
			}

			foreach (var blenderName in blenderNames) {
				if (itemName.Contains(blenderName)) {
					isBlender = true;
				}
			}

			foreach (var sinkName in sinkNames) {
				if (itemName.Contains(sinkName)) {
					isSink = true;
				}
			}

			foreach (var faucetName in faucetNames) {
				if (itemName.Contains(faucetName)) {
					isFaucet = true;
				}
			}

			if (isBucket) {
				if ((isSink || isFaucet) || itemName.Contains("water")) {
					if (IsInInv(31)) {
						ResetOverlay ();
						SetImage (GetImageByName ("labsink"));
						AddText ("You'd really rather not carry a big heavy bucket of water around, plus you're already exceptionally hydrated.");
						return;
					}
				}
			}

			if (isBlender) {
				if ((isSink || isFaucet) || itemName.Contains("water")) {
					if (IsInInv(35)) {
						ResetOverlay ();
						SetImage (GetImageByName ("labsink"));
						AddText ("You'd really rather not carry a heavy blender full of water around, plus you're already exceptionally hydrated.");
						return;
					}
				}
			}
		}

		if (currentRoom.Index == 5) {
			var workbenchNames = GetAltNames ("workbench");

			bool isWorkbench = false;

			foreach (var workbenchName in workbenchNames) {
				if (itemName.Contains (workbenchName)) {
					isWorkbench = true;
				}
			}

			var toasterNames = GetAltNames ("toaster");
			var knifeNames = GetAltNames ("knife");
			var lighterNames = GetAltNames ("lighter");
			var gasNames = GetAltNames ("gas can");
			var fluidNames = GetAltNames ("lighter fluid");
			var juiceNames = GetAltNames ("orange juice");
			var bucketNames = GetAltNames ("bucket");
			var ammoniaNames = GetAltNames ("ammonia");
			var bleachNames = GetAltNames ("bleach");
			var blenderNames = GetAltNames ("blender");
			var springNames = GetAltNames ("spring");

			bool isToaster = false;
			bool isKnife = false;
			bool isLighter = false;
			bool isGas = false;
			bool isFluid = false;
			bool isJuice = false;
			bool isBucket = false;
			bool isAmmonia = false;
			bool isBleach = false;
			bool isBlender = false;
			bool isSpring = false;

			int trapItems = 0;
			int trapItemsInInv = 0;

			if (IsInInv(34)) { trapItemsInInv++; }
			if (IsInInv(23)) { trapItemsInInv++; }
			if (IsInInv(43)) { trapItemsInInv++; }
			if (IsInInv(64)) { trapItemsInInv++; }
			if (IsInInv(15)) { trapItemsInInv++; }
			if (IsInInv(28) ) { trapItemsInInv++; }
			if (IsInInv(31)) { trapItemsInInv++; }
			if (IsInInv(65) ) { trapItemsInInv++; }
			if (IsInInv(47) ) { trapItemsInInv++; }
			if (IsInInv(35)) { trapItemsInInv++; }
			if (IsInInv(148)) { trapItemsInInv++; }

			foreach (var toasterName in toasterNames) {
				if (itemName.Contains (toasterName)) {
					if (IsInInv (34)) {
						isToaster = true;
					}
				}
			}
			foreach (var knifeName in knifeNames) {
				if (itemName.Contains (knifeName)) {
					if (IsInInv (23)) {
						isKnife = true;
					}
				}
			}
			foreach (var lighterName in lighterNames) {
				if (itemName.Contains (lighterName)) {
					if (IsInInv (43)) {
						isLighter = true;
					}
				}
			}
			foreach (var gasName in gasNames) {
				if (itemName.Contains (gasName)) {
					if (IsInInv (64)) {
						isGas = true;
					}
				}
			}
			foreach (var fluidName in fluidNames) {
				if (itemName.Contains (fluidName)) {
					if (IsInInv (15)) {
						isFluid = true;
					}
				}
			}
			foreach (var juiceName in juiceNames) {
				if (itemName.Contains (juiceName)) {
					if (IsInInv (28)) {
						isJuice = true;
					}
				}
			}
			foreach (var bucketName in bucketNames) {
				if (itemName.Contains (bucketName)) {
					if (IsInInv (31)) {
						isBucket = true;
					}
				}
			}
			foreach (var ammoniaName in ammoniaNames) {
				if (itemName.Contains (ammoniaName)) {
					if (IsInInv (65)) {
						isAmmonia = true;
					}
				}
			}
			foreach (var bleachName in bleachNames) {
				if (itemName.Contains (bleachName)) {
					if (IsInInv (47)) {
						isBleach = true;
					}
				}
			}
			foreach (var blenderName in blenderNames) {
				if (itemName.Contains (blenderName)) {
					if (IsInInv (35)) {
						isBlender = true;
					}
				}
			}
			foreach (var springName in springNames) {
				if (itemName.Contains (springName)) {
					if (IsInInv (148)) {
						isSpring = true;
					}
				}
			}

			if (isToaster)
				trapItems++;
			if (isKnife)
				trapItems++;
			if (isLighter)
				trapItems++;
			if (isGas)
				trapItems++;
			if (isFluid)
				trapItems++;
			if (isJuice)
				trapItems++;
			if (isBucket)
				trapItems++;
			if (isAmmonia)
				trapItems++;
			if (isBleach)
				trapItems++;
			if (isBlender)
				trapItems++;
			if (isSpring)
				trapItems++;

			if (isWorkbench) {
				if (trapItems == 1) {

					makingTraps = true;
					trapStep1 = true;

					ResetOverlay ();

					SetImage (GetImageByName ("workbench"));

					int step1 = 0;

					if (isToaster) { step1 = 34; }
					if (isKnife) { step1 = 23; }
					if (isLighter) { step1 = 43; }
					if (isGas) { step1 = 64; }
					if (isFluid) { step1 = 15; }
					if (isJuice) { step1 = 28; }
					if (isBucket) { step1 = 31; }
					if (isAmmonia) { step1 = 65; }
					if (isBleach) { step1 = 47; }
					if (isBlender) { step1 = 35; }
					if (isSpring) { step1 = 148; }

					if (trapItemsInInv == 1) {
						var lonelyBenchItem = itemsList [step1];
						AddText ("You go to add the " + lonelyBenchItem.Name + " to the bench, but notice you don't have any other items to use with it. Maybe you can come back once you have more stuff.");
						makingTraps = false;
						trapStep1 = false;
						return;
					}

					WorkbenchStep (step1, 0, 0);
					return;
				}

				if (trapItems == 2) {
					makingTraps = true;
					trapStep1 = trapStep2 = true;

					ResetOverlay ();

					SetImage (GetImageByName ("workbench"));

					int step1 = 0;
					int step2 = 0;

					if (isToaster) { 
						if (step1 == 0)
							step1 = 34;
						else
							step2 = 34;
					}
					if (isKnife) { 
						if (step1 == 0)
							step1 = 23;
						else
							step2 = 23;
					}
					if (isLighter) { 
						if (step1 == 0)
							step1 = 43;
						else
							step2 = 43;
					}
					if (isGas) { 
						if (step1 == 0)
							step1 = 64;
						else
							step2 = 64;
					}
					if (isFluid) { 
						if (step1 == 0)
							step1 = 15;
						else
							step2 = 15;
					}
					if (isJuice) { 
						if (step1 == 0)
							step1 = 28;
						else
							step2 = 28;
					}
					if (isBucket) { 
						if (step1 == 0)
							step1 = 31;
						else
							step2 = 31;
					}
					if (isAmmonia) { 
						if (step1 == 0)
							step1 = 65;
						else
							step2 = 65;
					}
					if (isBleach) { 
						if (step1 == 0)
							step1 = 47;
						else
							step2 = 47;
					}
					if (isBlender) { 
						if (step1 == 0)
							step1 = 35;
						else
							step2 = 35;
					}
					if (isSpring) { 
						if (step1 == 0)
							step1 = 148;
						else
							step2 = 148;
					}

					WorkbenchStep (step1, step2, 0);
					return;
				}

				if (trapItems == 3) {
					makingTraps = true;
					trapStep1 = trapStep2 = trapStep3 = true;

					ResetOverlay ();

					SetImage (GetImageByName ("workbench"));

					int step1 = 0;
					int step2 = 0;
					int step3 = 0;

					if (isToaster) { 
						if (step1 == 0)
							step1 = 34;
						else if (step2 == 0)
							step2 = 34;
						else
							step3 = 34;
					}
					if (isKnife) { 
						if (step1 == 0)
							step1 = 23;
						else if (step2 == 0)
							step2 = 23;
						else
							step3 = 23;
					}
					if (isLighter) { 
						if (step1 == 0)
							step1 = 43;
						else if (step2 == 0)
							step2 = 43;
						else
							step3 = 43;
					}
					if (isGas) { 
						if (step1 == 0)
							step1 = 64;
						else if (step2 == 0)
							step2 = 64;
						else
							step3 = 64;
					}
					if (isFluid) { 
						if (step1 == 0)
							step1 = 15;
						else if (step2 == 0)
							step2 = 15;
						else
							step3 = 15;
					}
					if (isJuice) { 
						if (step1 == 0)
							step1 = 28;
						else if (step2 == 0)
							step2 = 28;
						else
							step3 = 28;
					}
					if (isBucket) { 
						if (step1 == 0)
							step1 = 31;
						else if (step2 == 0)
							step2 = 31;
						else
							step3 = 31;
					}
					if (isAmmonia) { 
						if (step1 == 0)
							step1 = 65;
						else if (step2 == 0)
							step2 = 65;
						else
							step3 = 65;
					}
					if (isBleach) { 
						if (step1 == 0)
							step1 = 47;
						else if (step2 == 0)
							step2 = 47;
						else
							step3 = 47;
					}
					if (isBlender) { 
						if (step1 == 0)
							step1 = 35;
						else if (step2 == 0)
							step2 = 35;
						else
							step3 = 35;
					}
					if (isSpring) { 
						if (step1 == 0)
							step1 = 148;
						else if (step2 == 0)
							step2 = 148;
						else
							step3 = 148;
					}

					WorkbenchStep (step1, step2, step3);
					return;
				}
			}
			else {
				if (itemName.Contains ("fill")) {
					AddText ("Hmm, not sure how you want to fill that.");
				} else {
					AddText ("Combining these items might be easier with a flat workspace and some additional tools.");
				}
				return;
			}
		}

		if (itemName.Contains ("fill")) {
			AddText ("Hmm, not sure how you want to fill that.");
		} else {
			AddText ("Hmm, don't think you can use those things together");
		}
	}

	void TwoLayerLook() {
		string imgName = image.sprite.name;

		if (currentItemGroup != null) {
			var obj = itemsList [currentItemGroup.BaseItemIndex];
			Look (("look " + obj.Name).Shlex ());
			twoLayerLook = false;
			AddText ("");
		}

		if (imgName == "phone4") {
			SetImage (GetImageByName ("phone"));
			//PlayClip (GetClip (49));
			//AddText ("You set the phone down");
			AddText ("");
		} else if (imgName == "phone5") {
			SetImage (GetImageByName ("phone2"));
			//PlayClip (GetClip (49));
			//AddText ("You set the phone down");
			AddText ("");
		} else if (imgName == "phone3") {
			SetImage (GetImageByName ("phone2"));
			//AddText ("You put the address book back in the drawer.");
			AddText ("");
		} else if (imgName == "bleach") {
			SetImage (GetImageByName ("sink2"));
			AddText ("");
		} else if (imgName == "gun") {
			SetImage (GetImageByName("safe2"));

			AddText ("");
		}

		twoLayerLook = false;
	}

	bool IsAllUpper(string toCheck)
	{
		for (int i = 0; i < toCheck.Length; i++)
		{
			if (!Char.IsUpper(toCheck[i]) && !Char.IsWhiteSpace(toCheck[i]) && !Char.IsPunctuation(toCheck[i]))
				return false;
		}

		return true;
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
		case "follow":
			if (killerInKitchen) {
				if (itemName.Contains ("blood") || itemName.Contains("trail") || itemName.Contains("stream")) {
					if (currentRoom.Index == 4) {
						Move ("move hallway".Shlex ());
						return;
					}

					if (currentRoom.Index == 2) {
						Move ("move living room".Shlex ());
						return;
					}

					if (currentRoom.Index == 0) {
						Move ("move kitchen".Shlex ());
						return;
					}
				}
			}

			AddText (GenericDontKnow());
			break;
		case "read":

			if (itemName == "1" || itemName == "one" || itemName == "number one" || itemName == "number 1") {
				OtherCommands ("read book 1");
				return;
			}
			else if (itemName == "2" || itemName == "two" || itemName == "number two" || itemName == "number 2") {
				OtherCommands ("read book 2");
				return;
			}
			else if (itemName == "3" || itemName == "three" || itemName == "number three" || itemName == "number 3") {
				OtherCommands ("read book 3");
				return;
			}
			else if (itemName == "4" || itemName == "four" || itemName == "number four" || itemName == "number 4") {
				OtherCommands ("read book 4");
				return;
			}
			else if (itemName == "666" || itemName == "six six six" || itemName == "six hundred sixty six" || itemName == "six hundred and sixty six"  || itemName == "number 666" || itemName == "number six six six"
				|| itemName == "number six hundred sixty six" || itemName == "number six hundred and sixty six" ) {
				OtherCommands ("read book 666");
				return;
			}

			if (itemName == "book" && currentRoom.Index == 0 && (image.sprite.name == "phone2" || image.sprite.name == "phone3" || image.sprite.name == "phone5")) {
				Look ("look address book".Shlex ());
				return;
			}

			command = "Read";
			break;
		case "dial":
		case "call":
			command = "Call";
			break;
		case "open":

			if (itemName == "book" && currentRoom.Index == 0 && (image.sprite.name == "phone2" || image.sprite.name == "phone3" || image.sprite.name == "phone5")) {
				Look ("look address book".Shlex ());
				return;
			}

			if (itemName == "inventory" || itemName == "pockets" || itemName == "inv") {
				ListInventory ();
				return;
			}

			command = "Open";
			break;
		case "dig":

			if (itemName == "" || itemName == "hole" || itemName == "a hole") {

				if (currentRoom.Index != 7) {
					AddText ("The floor is a little hard to just start digging around.");
				} else {
					AddText ("You get on your knees and claw at the ground for a little bit and make a small little hole. You feel a rush of pride at this great accomplishment. It's pointless.");
				}
			} else {
				AddText (GenericDontKnow());
			}

			return;
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
					itemName = itemName.Replace ("under ", "");
				}

				if (itemName.Contains ("in ")) {
					itemName = itemName.Replace ("in ", "");
				}

				if (itemName.Contains ("behind ")) {
					itemName = itemName.Replace ("behind ", "");
				}

				command = "Hide";
			}
			break;
		case "help":
			Help ();
			return;
		case "lift":

			if (itemName.Contains ("up ")) {
				itemName = itemName.Replace ("up ", "");
			}

			command = "Lift";
			break;
		case "leave":
			if (currentRoom.Index == 8 && (itemName.Contains ("shed") || itemName.Contains ("shack") || itemName.Contains ("room"))) {
				Move ("move yard".Shlex ());
				return;
			}

			OtherCommands ("back");
			return;
		case "back":
			if (!twoLayerLook) {
				//AddText ("You step back");

				if (storedImage != "") {

					var buStoredOverlay = storedOverlay;

					SetImage (GetImageByName (storedImage));

					if (buStoredOverlay != "") {
						SetOverlay (GetImageByName (buStoredOverlay));
						storedOverlay = "";
						buStoredOverlay = "";
					} else {
						ResetOverlay ();
					}

					storedImage = "";
					storedOverlay = "";
					buStoredOverlay = "";

					AddText ("");
				} else {
					AddText ("");
					Look (null);
				}
			} else {
				TwoLayerLook ();
				return;
			}
			return;
		case "poop":
		case "crap":
		case "defecate":
		case "shit":
			if (itemName == "") {
				ShitNoItem ();
				return;
			} else {

				if (itemName.Contains("on ")){
					itemName = itemName.Replace ("on ", "");
				}

				if (itemName.Contains("in ")){
					itemName = itemName.Replace ("in ", "");
				}

				if (itemName.Contains("into ")){
					itemName = itemName.Replace ("into ", "");
				}

				if (itemName.Contains("onto ")){
					itemName = itemName.Replace ("onto ", "");
				}

				command = "Shit";
				break;
			}
		case "sip":
		case "taste":
		case "sample":
		case "chug":
		case "imbibe":
		case "drink":
			command = "Drink";
			break;
		case "reset":
			ResetHouse ();
			UpdateRoomState ();
			return;
		case "inv":
		case "inventory":
			ListInventory ();
			return;
		case "record":

			if (currentRoom.Index == 4 || IsInInv (57)) {
				var tapeNames = GetAltNames ("tape recorder");

				foreach (var tapeName in tapeNames) {
					if (itemName.Contains (tapeName) || itemName.Contains ("voice") || itemName == "myself") {
						Use ("use tape recorder".Shlex ());
						return;
					}
				}
			} 

			AddText (GenericDontKnow());
			return;
		case "place":
		case "put":
		case "equip":
		case "wear":
			tempTokens = itemName.Shlex ();

			if (IsInInv (57)) {
				var tapeNames = GetAltNames ("tape recorder");

				foreach (var tapeName in tapeNames) {
					if (itemName.Contains(tapeName)) {
						if (tapeRecorderUsed) {
							if (currentRoom.Index == 8) {
								var dummyNames = GetAltNames ("dummy");

								//foreach (var dummyName in dummyNames) {
								//	if (itemName.Contains (dummyName)) {
										Use ("use tape recorder with dummy".Shlex ());
										return;
									//}
								//}
							}

							AddText ("Good idea, but not here.");
							SetImage (GetImageByName ("invbig-tape recorder"));
							return;
						} else {
							AddText ("The tape is blank, so putting it there won't do anything.");
							SetImage (GetImageByName ("invbig-tape recorder"));
							return;
						}
					}
				}
			}


			for (int i = 0; i < tempTokens.Count; ++i) {
				if (tempTokens [i] == "with" || tempTokens [i] == "and" || tempTokens [i] == "at" || tempTokens [i] == "in" || tempTokens [i] == "together") {
					UseWith (itemName);
					return;
				}

				if (tempTokens [i] == "on") {
					tempTokens.Remove (tempTokens [i]);
					if (itemName.Contains ("on "))
						itemName = itemName.Replace ("on ", "");
					else if (itemName.Contains (" on"))
						itemName = itemName.Replace (" on", "");
				}
			}


			if (currentRoom.Index == 0) {
				if ((itemName.Contains ("down") || itemName.Contains ("away") || itemName.Contains ("back")) && (image.sprite.name == "phone4" || image.sprite.name == "phone5" || image.sprite.name == "phone3")) {
					OtherCommands ("back");
					return;
				}
			}
				
			if (currentRoom.Index == 4) {
				if ((itemName.Contains("down") || itemName.Contains("away") || itemName.Contains("back")) && (image.sprite.name == "bedrtable5" || image.sprite.name == "bedrtable6")) {
					OtherCommands ("back");
					return;
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
				} else if (useItem.Index == 67) {
					ResetOverlay ();

					if (IsInInv (67)) {
						SetImage (GetImageByName ("invbig-pinata"));
					} else {
						SetImage (GetImageByName ("pinata"));
					}

					AddText ("Now that would just look plain silly.");
					UpdateItemGroup (useItem.Index);
					UpdateRoomState (false);
				} else if (useItem.Index == 72) {
					var tokens = itemName.Shlex ();
					Use (tokens);
				}
				else {
					if (useItem.currentState.Image != "") {
						ResetOverlay ();
						SetImage (GetImageByName (useItem.currentState.Image));
					}

					AddText (GenericDontKnow ());
				}
			} else {
				AddText (GenericDontKnow ());
			}

			return;
		case "take":
			tempTokens = itemName.Shlex ();

			bool takeOff = false;
			for (int i = 0; i < tempTokens.Count; ++i) {
				if (tempTokens [i] == "off") {
					tempTokens.Remove (tempTokens [i]);
					if (itemName.Contains ("off "))
						itemName = itemName.Replace ("off ", "");
					else if (itemName.Contains (" off"))
						itemName = itemName.Replace (" off", "");
					takeOff = true;
				}
			}

			if (itemName.Contains ("down")) {
				var paintingNames = GetAltNames ("painting");

				foreach (var paintingName in paintingNames) {
					if (itemName.Contains (paintingName)) {
						Move ("move painting".Shlex ());
						return;
					}
				}
			}

			if (itemName == "a seat") {
				if (currentRoom.Index == 0) {
					Use ("use arm chair".Shlex ());	
					return;
				} else {
					AddText ("There's not a good place to sit down here.");
					return;
				}
			}

			if (itemName == "nap" || itemName == "a nap") {
				OtherCommands ("sleep");
				return;
			}

			if (itemName.Contains ("look") || itemName.Contains ("gander")) {
				if (itemName.Contains ("a look")) {
					itemName = itemName.Replace ("a look", "");
				}

				if (itemName.Contains ("a gander")) {
					itemName = itemName.Replace ("a gander", "");
				}

				if (itemName.Contains ("look")) {
					itemName = itemName.Replace ("look", "");
				}

				if (itemName.Contains ("gander")) {
					itemName = itemName.Replace ("gander", "");
				}

				Look (("look " + itemName).Shlex ());
				return;
			}

			if (currentRoom.Index == 3 && (itemName.Contains ("shower") || itemName.Contains ("bath"))) {
				AddText ("Look, if you didn't have time for one this morning, you certainly don't have time for one now.");
				return;
			}

			var takeItem = GetObjectByName (itemName);
			if (takeItem == null) {
				takeItem = GetObjectFromInv (itemName);

				if (takeItem == null) {
					AddText (GenericDontKnow ());
					return;
				}
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
						AddText (GenericDontKnow ());
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
				AddText (GenericDontKnow());
			}
			return;
		case "tidy":
		case "wash":
		case "scrub":
		case "clean":
			if (itemName.Contains("hands") || itemName.Contains("fingers") || itemName.Contains("cuticles")) {
				if (currentRoom.Index == 3 || currentRoom.Index == 6 || currentRoom.Index == 1) {
					var sink = GetObjectByName ("sink");
					ImageCheckAndShow (sink.Index, sink.State, sink.currentState.Image);
					if (!handsWashed) {
						if (currentRoom.Index == 3) {
							toPlaySoundFX.Add (GetClip (24));
						}
						else if (currentRoom.Index == 6) {
							toPlaySoundFX.Add (GetClip (25));
						}
						else {
							toPlaySoundFX.Add (GetClip (18));
						}
						handsWashed = true;
						AddText ("You scrub your hands with fervor. Your cuticles have never looked better. ‘Bout time you cleaned those suckers.");
					} else {
						AddText ("Your hands are already plenty clean.");
					}

					UpdateRoomState (false);
				} else {
					AddText ("There's no place to do that here");
				}
				return;
			}

			if (currentRoom.Index == 1 && (itemName.Contains ("up") || itemName.Contains ("kitchen") || itemName.Contains ("room") || itemName == "")) {
				AddText ("You attempt to clean up for a second. You are uncessessful.");
				return;
			}
			command = "Wash";
			break;
		case "pour":
		case "mix":
		case "fill":
		case "combine":
			UseWith (command + " " + itemName);
			return;
		case "fart":
			Fart ();
			return;
		case "burp":
			Burp ();
			return;
		case "stab":
			if (itemName.Contains ("self")) {
				OtherCommands ("commit seppuku");
				return;
			}

			if (currentRoom.Index == 4) {
				var bedNames = GetAltNames ("bed");

				bool isBed = false;

				foreach (var bedName in bedNames) {
					if (itemName.Contains (bedName)) {
						if (IsInInv (23)) {
							Use ("use knife with bed".Shlex ());
							return;
						} else {
							SetImage (GetImageByName ("bed"));
							AddText ("Hmm, you would need a sharp instrument to do that.");
							return;
						}
					}
				}
			}

			AddText (GenericDontKnow());
			return;	
		case "hang":
			if (currentRoom.Index == 0) {
				if (itemName.Contains("up") && (image.sprite.name == "phone4" || image.sprite.name == "phone5")) {
					OtherCommands ("back");
					return;
				}
			}
			break;
		case "perish":
		case "expire":
		case "die":
			KillSelf ();
			return;
		case "add":
			if (currentRoom.Index == 4 || IsInInv(57)) {
				var tapeNames = GetAltNames ("tape recorder");

				foreach (var tapeName in tapeNames) {
					if (itemName.Contains (tapeName)) {
						OtherCommands("record tape recorder");
						return;
					}
				}
			}
			break;
		case "lie":
		case "lay":
			if (currentRoom.Index == 4) {
				if (itemName.Contains ("down") || itemName.Contains ("bed")) {
					OtherCommands ("sleep");
					return;
				}
			}
			break;
		case "remove":
			if (currentRoom.Index == 4 || currentRoom.Index == 0) {
				var paintingNames = GetAltNames ("painting");

				foreach (var paintingName in paintingNames) {
					if (itemName.Contains (paintingName)) {
						Move ("move painting".Shlex ());
						return;
					}
				}

			}
			break;
		case "throw":
		case "toss":
			if (currentRoom.Index == 3) {
				if (currentRoom.Index == 3) {
					var toasterNames = GetAltNames ("toaster");
					var showerNames = GetAltNames ("shower");

					bool isToaster = false;
					bool isShower = false;

					if (IsInInv (34)) {

						foreach (var toasterName in toasterNames) {
							if (itemName.Contains(toasterName)) {
								isToaster = true;
							}
						}

						foreach (var showerName in showerNames) {
							if (itemName.Contains (showerName)) {
								isShower = true;
							}
						}

						if (isToaster && isShower) {
							Use ("use toaster with shower".Shlex ());
							return;
						}
					}
				}
			}

			AddText (GenericDontKnow());
			return;
		case "light":
		case "start":
		case "set":
			if (currentRoom.Index == 0) {
				var fireplaceNames = GetAltNames ("fireplace");
				fireplaceNames.Add ("fire");

				foreach (var fireplaceName in fireplaceNames) {
					if (itemName.Contains (fireplaceName)) {

						var fireplaceObj = GetObjectByName ("fireplace");

						if (fireplaceObj.State == 0) {
							Use ("use fireplace".Shlex ());
							return;
						} else {
							ResetOverlay ();
							ImageCheckAndShow (fireplaceObj.Index, fireplaceObj.State, fireplaceObj.currentState.Image);
							AddText ("Now that a passage has opened in the back of the fireplace, it's probably not a great time to light a fire in it.");
							return;
						}
					}
				}

			}

			AddText (GenericDontKnow());
			return;
		case "dance":
			AddText (GetRandomDanceText ());
			return;
		case "hack":
			if (currentRoom.Index == 6) {
				var computerNames = GetAltNames ("computer");

				foreach (var computerName in computerNames) {
					if (itemName.Contains (computerName)) {
						ResetOverlay ();
						SetGasMaskOverlay (false);
						SetImage (GetImageByName ("hack"));
						AddText ("You crack your knuckles and begin to type, your fingers flying across the keyboard. Zeros and ones flash across the screen with blinding speed. Just as you succeed in accessing the recycle bin, you catch movement out of the corner of your eye. The man in the cleansuit grabs the keyboard out from under your hands and impales it directly into your chest. Mess with the best, die like the rest.\n\nPress [ENTER] to try again");

						GameOverAudio (74, true);

						health = 0;
						killerInLair = false;
						return;
					}
				}
			}

			AddText (GenericDontKnow());
			return;
		case "wait":
			if (currentRoom.Index == 5) {
				if (bearTrapMade || fireTrapMade || shitOnStairs || bucketTrapMade || blenderTrapMade) {
					OtherCommands ("hide");
					return;
				}
			}

			if ((currentRoom.Index == 7 || currentRoom.Index == 8) && dummyAssembled) {
				OtherCommands ("hide");
				return;
			}

			if (currentRoom.Index == 4) {
				OtherCommands ("hide");
				return;
			}

			if (currentRoom.Index == 3) {
				OtherCommands ("hide");
				return;
			}

			AddText ("You don't want to wait out in the open.");
			return;

		case "breathe":
			KillSelf ("As you go to take the deepest breath of your adult life, a massive buzzing fly soars down your throat. You choke and sputter, reaching all around you for support, but find none. Lungs empty, gasping for air, your oxygen-starved brain manages one last cohesive thought: \"This is what I get for not trusting my autonomic nervous system to properly regulate my respiration!\"");
			return;
		case "flush":
			if (currentRoom.Index == 3) {
				var toiletNames = GetAltNames ("toilet");
				toiletNames.Add ("toilet");
				foreach (var toiletName in toiletNames) {
					if (itemName == toiletName) {
						var toilet = GetObjectByName ("toilet");
						AddText ("Maybe you can regain control over your life by sending gallons of water rushing into the sewer. Hmm, I guess not. Still felt pretty good though.");

						PlayClip (GetClip (52));

						ImageCheckAndShow (toilet.Index, toilet.State, toilet.currentState.Image);
						return;
					}
				}
			}

			AddText (GenericDontKnow());
			return;
		case "give":
			if (itemName.Contains ("up")) {
				KillSelf ("You decide to give up living. Great!");
				return;
			} else {
				AddText (GenericDontKnow());
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
					KillSelf ("Feeling like the noble Japanese samurai of which you only know a vague amount, you disembowel yourself. Neat!");
				}
				else {
					AddText ("Hmm, you would need a sharp instrument to do that.");
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
		case "click":
			command = "Click";
			break;
		case "break":
		case "punch":
		case "smash":
			if (itemName.Contains ("wind")) {
				Fart ();
				return;
			}

			command = "Break";
			break;
		case "pass":
			if (itemName.Contains ("gas")) {
				Fart ();
				return;
			}
			break;
		case "continue":
		case "keep":
			if (itemName.Contains ("reading") && image.sprite.name == "book") {
				Turn ("page");
				return;
			}

			if (killerInKitchen && itemName.Contains ("following")) {
				OtherCommands ("follow blood");
				return;
			}

			break;
		case "hop":
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
				AddText (GenericDontKnow ());
			}
			return;
		case "sleep":
			if (itemName == "") {
				if (currentRoom.Index == 4) {
					OtherCommands ("sleep bed");
					return;
				} else if (currentRoom.Index == 0) {
					OtherCommands ("sleep arm chair");
					return;
				}
				else {
					AddText ("Eh, there's nowhere really comfortable to sleep in here");
				}
			} else {
				if (currentRoom.Index == 4) {
					if (itemName.Contains ("bed")) {
						SetImage (GetImageByName ("bed"));
						ResetOverlay ();
						SetGasMaskOverlay (false);
						AddText ("You get in bed and try to sleep.\n\nPress [ENTER] to continue.");
						multiSequence = true;
						currMultiSequence = 30;
						return;
					}	
				} else if (currentRoom.Index == 0) {
					if (itemName.Contains ("chair") || itemName.Contains ("recliner") || itemName.Contains("reclining")) {
						sleepDeath = true;

						var chairObj = GetObjectByName ("arm chair");
						ResetOverlay ();
						ImageCheckAndShow (chairObj.Index, chairObj.State, chairObj.currentState.Image);
						SetGasMaskOverlay (false);
						AddText ("Amazingly, you are able to nod off after a few minutes of meditative thought on bees.\n\nPress [ENTER] to continue.");

						multiSequence = true;

						if (policeTimer >= policeCap) {
							currMultiSequence = UnityEngine.Random.Range(43, 45);
						}
						else {
							currMultiSequence = UnityEngine.Random.Range(41, 43);
						}
						return;
					}
				}
				AddText ("No idea what you're talking about");
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
		case "unlock":
			command = "Unlock";
			break;
		case "drag":
		case "push":

			if (currentRoom.Index == 0) {
				var chairNames = GetAltNames ("arm chair");

				foreach (var chairName in chairNames) {
					if (itemName.Contains (chairName)) {
						Move ("move arm chair".Shlex ());
						return;
					}
				}
			}

			AddText (GenericDontKnow());
			return;				
		case "block":
		case "bar":
		case "barricade":
			if (itemName.Contains ("with") || itemName.Contains ("using")) {
				if (currentRoom.Index == 0) {
					var chairNames = GetAltNames ("arm chair");
					var bookcaseNames = GetAltNames ("bookcase");
					var doorNames = GetAltNames ("front door");
					var tableNames = GetAltNames ("table");
					var lampNames = GetAltNames ("lamp");
					chairNames.Add ("arm chair");
					bookcaseNames.Add ("bookcase");
					doorNames.Add ("front door");
					tableNames.Add ("table");
					lampNames.Add ("lamp");

					var chair = false;
					var door = false;
					var bookcase = false;
					var table = false;
					var lamp = false;

					foreach (var chairName in chairNames) {
						if (itemName.Contains (chairName))
							chair = true;
					}

					foreach (var doorName in doorNames) {
						if (itemName.Contains (doorName))
							door = true;
					}

					foreach (var bookcaseName in bookcaseNames) {
						if (itemName.Contains (bookcaseName))
							bookcase = true;
					}

					foreach (var tableName in tableNames) {
						if (itemName.Contains (tableName))
							table = true;
					}

					foreach (var lampName in lampNames) {
						if (itemName.Contains (lampName))
							lamp = true;
					}

					if (chair && door) {
						Move ("move arm chair".Shlex ());
						return;
					}

					if (bookcase && door) {
						Move ("move bookcase".Shlex ());
						return;
					}

					if (table && door) {
						Move ("move table".Shlex ());
						return;
					}

					if (lamp && door) {
						Move ("move lamp".Shlex ());
						return;
					}
				}
			}

			command = "Block";
			break;
		case "unblock":
		case "unbar":
		case "unbarricade":
			command = "Unblock";
			break;
		case "play":
			command = "Play";
			break;
		case "admire":
			command = "Admire";
			break;
		default:
			AddText (GenericDontKnow());
			return;
		}
		var item = GetObjectByName(itemName);
		if (item == null) {
			item = GetObjectFromInv (itemName);
			if (item == null) {

				switch (command) {

				case "Call":
					if (currentRoom.Index == 0) {
						AddText ("Hmm, that's not a phone number you recognize. Maybe you should try one of the ones in your address book.");
						return;
					}
					else {
						AddText ("There's no phone in here. And you heard somewhere that cell phones give off harmful radiation. Or was that microwaves?");
						return;
					}
				default:
					break;
				}

				AddText (GenericDontKnow());
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

	void Fart() {
		AddText ("You crack off a fart with such force that it reverberates around the room slightly. You worry that it might have been so loud that it gave away your location to the intruder.");
		PlayClip (GetClip (GetFart()));
	}

	void Burp() {
		AddText ("You burp and it's loud.");
		PlayClip (GetClip (76));
	}

	public void Sit(string text){
		var obj = GetObjectByName (text);
		bool roomImage = false;

		if (obj == null) {
			AddText ("Not sure what you want to sit on");
			return;
		}

		var sitResponses = specialResponses
			.Where (x => x.ItemIndex == obj.Index)
			.Where (x => x.Command == "Sit");

		if (sitResponses.Count () == 0) {
			if (obj.currentState.Image != "") {
				ResetOverlay ();
				ImageCheckAndShow (obj.Index, obj.State, obj.currentState.Image);
				roomImage = false;
			}

			if (IsInInv (obj.Index)) {
				SetImage (GetImageByName ("invbig-" + obj.Name));
			} else {
				UpdateItemGroup (obj.Index);
				UpdateRoomState (roomImage);
			}

			AddText ("Not sitting on that.");
			return;
		}
		else {
			foreach (var response in sitResponses) {
				foreach (KeyValuePair<int, int> actions in response.Actions) {
					if (ChangeState (actions.Key, actions.Value) == 1)
						break;
				}

				if (obj.Index == 45) {
					Use ("use toilet".Shlex ());
					return;
				}


				if (response.Image != "") {
					ResetOverlay ();
					ImageCheckAndShow (response.ItemIndex, response.ItemState, response.Image);
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
			MapArrow ();
			SetGasMaskOverlay (false);
			AddText ("Nah, I think I might need that later");
		} else {
			AddText ("Not sure what you want me to drop.");
		}
	}

	public void KillSelf(string overrideText = ""){
		SetImage (GetImageByName (GetTombstone ()));
		if (overrideText == "")
			AddText ("You die. Great job.");
		else
			AddText (overrideText);

		GameOverAudio (-1, true);

		AddAdditionalText ("\n\nPress [ENTER] to Restart.");
		ResetOverlay ();
		SetGasMaskOverlay (false);
		SetBasementOverlay (5, false);
		health = 0;
	}

	public void Turn(string text){

		bool turnOff = false;
		bool turnOn = false;
		bool roomImage = false;

		var turnText = text.Shlex ();

		if (text.Contains ("page") && image.sprite.name == "book") {
			AddText ("This page seems like it has all of the information you'll need.");
			return;
		}

		if (turnText.Contains ("on")) {
			turnText.Remove ("on");
			turnOn = true;
		}
		if (turnText.Contains ("off")) {
			turnText.Remove ("off");
			turnOff = true;
		}

		if (turnText [0] == "around" || turnText [0] == "back") {
			if (!twoLayerLook) {
				//AddText ("You step back");
				AddText("");
				Look (null);
			} else {
				if (currentItemGroup != null) {
					var subObj = itemsList [currentItemGroup.BaseItemIndex];
					Look (("look " + subObj.Name).Shlex ());
					twoLayerLook = false;
					AddText ("");
				} else {
					if (image.sprite.name == "phone4") {
						SetImage (GetImageByName ("phone"));
						//PlayClip (GetClip (49));
						//AddText ("You set the phone down");
						AddText("");
						twoLayerLook = false;
					}
					else if (image.sprite.name == "phone5") {
						SetImage (GetImageByName ("phone2"));
						//PlayClip (GetClip (49));
						//AddText ("You set the phone down");
						AddText("");
						twoLayerLook = false;
					}
					else if (image.sprite.name == "phone3") {
						SetImage (GetImageByName ("phone2"));
						//AddText ("You put the address book back in the drawer.");
						AddText("");
						twoLayerLook = false;
					}
				}
			}
			return;
		}

		int itemNameStart = 0;
		string objName = string.Join(" ", turnText.Skip(itemNameStart).ToArray());

		var obj = GetObjectByName (objName);
		if (obj == null) {
			obj = GetObjectFromInv (objName);
		}

		if (obj == null) {
			if (turnOn) {
				AddText ("You can't turn on what you can't see.");
			}
			else if (turnOff) {
				AddText ("You can't turn off what you can't see.");
			}
			else {
				AddText ("Not sure what you want to do.");
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
				AddText ("Don't think you can turn that on.");
			} 
			else if (turnOff) {
				AddText ("Don't think that's something you can turn off.");	
			}


			if (obj.currentState.Image != "") {
				ResetOverlay ();
				ImageCheckAndShow (obj.Index, obj.State, obj.currentState.Image);
				roomImage = false;
			}

			if (IsInInv (obj.Index)) {
				SetImage (GetImageByName ("invbig-" + obj.Name));
			} else {
				UpdateItemGroup (obj.Index);
				UpdateRoomState (roomImage);
			}

			return;
		}

		if (turnOn) {
			foreach (var response in turnOnResponses) {
				foreach (KeyValuePair<int, int> actions in response.Actions) {
					if (ChangeState (actions.Key, actions.Value) == 1)
						break;
				}

				if (response.ItemIndex == 145) {
					Use ("use sink".Shlex());
					return;
				}

				if (response.ItemIndex == 146) {
					Use ("use sink".Shlex());
					return;
				}

				if (response.ItemIndex == 147) {
					Use ("use sink".Shlex());
					return;
				}

				if (response.ItemIndex == 81) {
					Use ("use hose".Shlex());
					return;
				}

				if (response.ItemIndex == 57) {
					Use ("use tape recorder".Shlex());
					return;
				}

				if (response.ItemIndex == 52) {
					var flashlight = itemsList [52];

					if (IsInInv (52)) {
						response.Image = "invbig-flashlight";

						if (currentRoom.Index == 7 || currentRoom.Index == 8) {
							response.Response = "It's already on!";
						}
					} 
					else {
						if (flashlight.State == 0) {
							AddText ("Can't turn on what you can't see.");
							return;
						} else {
							response.Response = "You would need to grab it first.";
						}
					}
				}

				if (response.Image != "") {
					ResetOverlay ();
					ImageCheckAndShow (response.ItemIndex, response.ItemState, response.Image);
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

				if (response.ItemIndex == 52) {
					var flashlight = itemsList [52];

					if (IsInInv (52)) {

						response.Image = "invbig-flashlight";

						if (currentRoom.Index == 7 || currentRoom.Index == 8) {
							response.Response = "Then you wouldn't be able to see!";
						}
					} 
					else {
						if (flashlight.State == 0) {
							AddText ("Can't turn off what you can't see.");
							return;
						}
					}
				}

				if (response.Image != "") {
					ResetOverlay ();
					ImageCheckAndShow (response.ItemIndex, response.ItemState, response.Image);
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
			SetBasementOverlay (5, false);
			string imageName = imageList [UnityEngine.Random.Range (0, imageList.Count)];
			AddText ("You let out a shriek of terror that would give Philip Glean a run for his money. Less than a minute passes before the intruder barges into the room, clearly lured in by irresistible call of despair. Seeking to really get his money's worth of screams, the man in the cleansuit butchers you before you can even attempt to flee or defend yourself.\n\nPress [ENTER] to restart.");
			SetImage (GetImageByName (imageName));

			GameOverAudio (-1, true);

			health = 0;
			return;
		}
	}

	public void Help()
	{
		storedImage = image.sprite.name;
		storedOverlay = currOverlay;

		helpScreenUp = true;
		SetOverlay (GetImageByName ("blankoverlay"));
		SetImage (GetImageByName ("blankoverlay"));
		AddText ("");

		SetGasMaskOverlay (false);
		AddHelpText ("In this game, you interact with the world around you by typing commands. The Four Basic Commands are LOOK, GET, USE, and MOVE. For example, if you wanted to move up the stairs, you might type MOVE HALLWAY or USE STAIRCASE.\n\n" +
			"You'll want to try other commands too - or you won't survive. Examples of other commands that are accepted include: READ, CALL, HIDE, OPEN, and CLOSE.\n\n" +
			"Along the way, you might find useful items that you can pick up. To pick up the knife, for example, you would type GET KNIFE. These will be stored in your inventory, which you can see by typing INV.\n\n" +
			"If you get disoriented, you have a map in your inventory. LOOK at it.\n\n" + 
			"The goal is to survive. There are multiple ways to fight, escape, or outwit the killer, but it may take some trial and error. Try LOOKing at everything, including LOOK ROOM for a description of the room you are in. There may be hints that put you on the right track.\n\n" +
			"Type BACK to return to the game.");
	}

	public void ShitNoItem(string source = "")
	{
		if (currentRoom.Index == 5) {
			OtherCommands ("shit stairs");
		}
		else if (currentRoom.Index == 3) {
			OtherCommands ("shit toilet");
		}
		else {
			if (!shitOnStairs) {
				AddText ("Somehow, you don't feel like it's the right time for that. You do feel a rumbling in your lower stomach though.");
			}
			else {
				AddText ("You're all spent.");
			}
		}
	}

	public void HideNoItem(string source = "")
	{
		if (currentRoom.Index == 5) {
			ImageCheckAndShow (61, 0, "checkitem");

			if (source == "scream") {
				AddText ("You wail at the top of your lungs. A minute passes before you hear the footsteps of the killer hurrying towards the basement and down the stairs.\n\nPress [ENTER] to continue.");
			} else if (source == "tape") {
				AddText ("You crank up the volume on the tape recorder and play back your recorded sniveling. After a few moments, the man in the cleansuit arrives at the top of the stairs to claim what he thinks is vulnerable prey. He starts to descend...\n\nPress [ENTER] to continue.");
			} else if (source == "basement") {
				AddText ("As you descend the stairs, you hear the killer closely behind you. You hear da bad boy start to come down the stairs. Press [ENTER]");
			} 
			else {
				if (bearTrapMade || fireTrapMade || shitOnStairs || bucketTrapMade || blenderTrapMade) {
					AddText ("You find a perch behind some boxes. Tense minutes pass until you hear a slight creaking of the floor from upstairs. In the dim light from the kitchen, you see your pursuer appear in the door frame. He is moving cautiously - gingerly lowering himself down each stair. You think you see him pause and squint, confused for a moment, before deciding to continue.\n\nPress [ENTER] to continue.");
				}
				else {
					OtherCommands ("hide boxes");
					return;
				}
			}

			multiSequence = true;

			if (itemsList [98].State == 0 && itemsList [99].State == 0 && itemsList [100].State == 0 && itemsList [101].State == 0 && itemsList[140].State == 0) {
				currMultiSequence = UnityEngine.Random.Range(0, 8);
			}
			else if (itemsList [98].State == 1 && itemsList [99].State == 0 && itemsList [100].State == 0 && itemsList [101].State == 0 && itemsList[140].State == 0) {
				currMultiSequence = 9;
			}
			else if (itemsList [98].State == 0 && itemsList [99].State == 1 && itemsList [100].State == 0 && itemsList [101].State == 0 && itemsList[140].State == 0) {
				currMultiSequence = 10;
			}
			else if (itemsList [98].State == 0 && itemsList [99].State == 0 && itemsList [100].State == 1 && itemsList [101].State == 0 && itemsList[140].State == 0) {
				currMultiSequence = 11;
			}
			else if (itemsList [98].State == 0 && itemsList [99].State == 0 && itemsList [100].State == 0 && itemsList [101].State == 1 && itemsList[140].State == 0) {
				currMultiSequence = 12;
			}
			else if (itemsList [98].State == 1 && itemsList [99].State == 1 && itemsList [100].State == 0 && itemsList [101].State == 0 && itemsList[140].State == 0) {
				currMultiSequence = 13;
			}
			else if (itemsList [98].State == 1 && itemsList [99].State == 0 && itemsList [100].State == 1 && itemsList [101].State == 0 && itemsList[140].State == 0) {
				currMultiSequence = 14;
			}
			else if (itemsList [98].State == 1 && itemsList [99].State == 0 && itemsList [100].State == 0 && itemsList [101].State == 1 && itemsList[140].State == 0) {
				currMultiSequence = 15;
			}
			else if (itemsList [98].State == 0 && itemsList [99].State == 1 && itemsList [100].State == 1 && itemsList [101].State == 0 && itemsList[140].State == 0) {
				currMultiSequence = 16;
			}
			else if (itemsList [98].State == 0 && itemsList [99].State == 0 && itemsList [100].State == 1 && itemsList [101].State == 1 && itemsList[140].State == 0) {
				currMultiSequence = 17;
			}
			else if (itemsList [98].State == 0 && itemsList [99].State == 1 && itemsList [100].State == 0 && itemsList [101].State == 1 && itemsList[140].State == 0) {
				currMultiSequence = 18;
			}
			else if (itemsList [98].State == 1 && itemsList [99].State == 1 && itemsList [100].State == 1 && itemsList [101].State == 0 && itemsList[140].State == 0) {
				currMultiSequence = 19;
			}
			else if (itemsList [98].State == 1 && itemsList [99].State == 1 && itemsList [100].State == 0 && itemsList [101].State == 1 && itemsList[140].State == 0) {
				currMultiSequence = 20;
			}
			else if (itemsList [98].State == 1 && itemsList [99].State == 0 && itemsList [100].State == 1 && itemsList [101].State == 1 && itemsList[140].State == 0) {
				currMultiSequence = 21;
			}
			else if (itemsList [98].State == 0 && itemsList [99].State == 1 && itemsList [100].State == 1 && itemsList [101].State == 1 && itemsList[140].State == 0) {
				currMultiSequence = 22;
			}
			else if (itemsList [98].State == 1 && itemsList [99].State == 1 && itemsList [100].State == 1 && itemsList [101].State == 1 && itemsList[140].State == 0) {
				currMultiSequence = 23;
			}
			else if (itemsList [98].State == 0 && itemsList [99].State == 0 && itemsList [100].State == 0 && itemsList [101].State == 0 && itemsList[140].State == 1) {
				currMultiSequence = 31;
			}
			else if (itemsList [98].State == 1 && itemsList [99].State == 0 && itemsList [100].State == 0 && itemsList [101].State == 0 && itemsList[140].State == 1) {
				currMultiSequence = 32;
			}
			else if (itemsList [98].State == 0 && itemsList [99].State == 0 && itemsList [100].State == 1 && itemsList [101].State == 0 && itemsList[140].State == 1) {
				currMultiSequence = 33;
			}
			else if (itemsList [98].State == 0 && itemsList [99].State == 0 && itemsList [100].State == 0 && itemsList [101].State == 1 && itemsList[140].State == 1) {
				currMultiSequence = 34;
			}
			else if (itemsList [98].State == 1 && itemsList [99].State == 0 && itemsList [100].State == 1 && itemsList [101].State == 0 && itemsList[140].State == 1) {
				currMultiSequence = 35;
			}
			else if (itemsList [98].State == 1 && itemsList [99].State == 0 && itemsList [100].State == 0 && itemsList [101].State == 1 && itemsList[140].State == 1) {
				currMultiSequence = 36;
			}
			else if (itemsList [98].State == 0 && itemsList [99].State == 0 && itemsList [100].State == 1 && itemsList [101].State == 1 && itemsList[140].State == 1) {
				currMultiSequence = 37;
			}
			else if (itemsList [98].State == 1 && itemsList [99].State == 0 && itemsList [100].State == 1 && itemsList [101].State == 1 && itemsList[140].State == 1) {
				currMultiSequence = 38;
			}

			return;
		}

		if (currentRoom.Index == 7 || currentRoom.Index == 8) {

			if (dummyAssembled) {
				killerInShack = true;
				room = 7;
				SetImage (GetImageByName ("outside2"));
				List<string> imageList = deathOverlays [9];
				string imageName = imageList [UnityEngine.Random.Range (0, imageList.Count)];
				currOverlay = imageName;
				SetOverlay (GetImageByName (imageName));

				string killerText = "You crouch behind some bushes and wait, passing the time by farting soundlessly from nervousness. Eventually, you see the killer sneaking out into your backyard in the light from the kitchen.\n\nWhat do you want to do?";

				AddText (killerText);

				storedText = killerText;

				fadeMusicTrack = true;

				actionTrack.clip = GetClip (65);
				actionTrack.Play ();


				currLockdownOption = 6;
				inputLockdown = true;
				return;
			} 
			else {
				AddText ("Not sure if you're quite ready for that yet.");
				return;
			}
		}

		if (currentRoom.Index == 4 ) {
			if (image.sprite.name == "closet2" || image.sprite.name == "closet2") {
				OtherCommands ("hide closet");
			} else if (image.sprite.name == "closet") {
				AddText ("Gotta open it first");
			} else {
				OtherCommands ("hide under bed");
			}
			return;
		}

		if (currentRoom.Index == 3) {
			OtherCommands ("hide shower");
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
				roomImage = false;
			}

			if (IsInInv (item.Index)) {
				SetImage (GetImageByName ("invbig-" + item.Name));
			} else {
				UpdateItemGroup (item.Index);
				UpdateRoomState (roomImage);
			}

			AddText (GenericCantDoThis ("climb"));
			return;
		}
	}

	public void Wash(int i, int j){
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
				roomImage = false;
			}

			if (IsInInv (item.Index)) {
				SetImage (GetImageByName ("invbig-" + item.Name));
			} else {
				UpdateItemGroup (item.Index);
				UpdateRoomState (roomImage);
			}

			AddText (GenericCantDoThis ("wash"));
			return;
		}
	}

	public void Click(int i, int j){
		var item = itemsList [i];
		bool roomImage = true;
		if (j != -1) {

			if (item.Index == 132) {
				Look ("look journal.txt".Shlex());
				return;
			}

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
				roomImage = false;
			}

			if (IsInInv (item.Index)) {
				SetImage (GetImageByName ("invbig-" + item.Name));
			} else {
				UpdateItemGroup (item.Index);
				UpdateRoomState (roomImage);
			}

			AddText (GenericCantDoThis ("click"));
			return;
		}
	}


	public void Lift(int i, int j){
		var item = itemsList [i];
		bool roomImage = true;
		if (j != -1) {
			AddText (specialResponses [j].Response);

			foreach (KeyValuePair<int, int> actions in specialResponses[j].Actions) {
				if (ChangeState (actions.Key, actions.Value) == 1)
					break;
			}

			if (item.Index == 125) {
				Look ("look under cushion".Shlex ());
				return;
			}
			if (item.Index == 85) {
				Look ("look under rug".Shlex ());
				return;
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
				roomImage = false;
			}

			if (IsInInv (item.Index)) {
				SetImage (GetImageByName ("invbig-" + item.Name));
			} else {
				UpdateItemGroup (item.Index);
				UpdateRoomState (roomImage);
			}

			AddText (GenericCantDoThis("lift"));
			return;
		}
	}

	public void Admire(int i, int j){
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
				roomImage = false;
			}

			if (IsInInv (item.Index)) {
				SetImage (GetImageByName ("invbig-" + item.Name));
			} else {
				UpdateItemGroup (item.Index);
				UpdateRoomState (roomImage);
			}

			AddText (GenericCantDoThis ("admire"));
			return;
		}
	}


	public void Play(int i, int j){
		var item = itemsList [i];
		bool roomImage = true;
		if (j != -1) {
			AddText (specialResponses [j].Response);

			if (item.Index == 57 && tapeRecorderUsed) {
				AddText ("You lower the volume and playback the wailing. This might come in handy to lure the killer in the right spot.");
				SetImage (GetImageByName ("invbig-tape recorder"));
				lockMask = true;
				ResetOverlay ();
				SetGasMaskOverlay (false);
				return;
			}

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
				roomImage = false;
			}

			if (IsInInv (item.Index)) {
				SetImage (GetImageByName ("invbig-" + item.Name));
			} else {
				UpdateItemGroup (item.Index);
				UpdateRoomState (roomImage);
			}

			AddText (GenericCantDoThis ("play"));
			return;
		}
	}

	public void Block(int i, int j){
		var item = itemsList [i];
		bool roomImage = true;
		if (j != -1) {
			AddText (specialResponses [j].Response);

			if (item.Index == 8 && item.State == 0) {
				Move ("move arm chair".Shlex ());
				return;
			}

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
				roomImage = false;
			}

			if (IsInInv (item.Index)) {
				SetImage (GetImageByName ("invbig-" + item.Name));
			} else {
				UpdateItemGroup (item.Index);
				UpdateRoomState (roomImage);
			}

			AddText (GenericCantDoThis ("block"));
			return;
		}
	}

	public void Unblock(int i, int j){
		var item = itemsList [i];
		bool roomImage = true;
		if (j != -1) {
			AddText (specialResponses [j].Response);

			if (item.Index == 8 && item.State == 1) {
				Move ("move arm chair".Shlex ());
				return;
			}

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
				roomImage = false;
			}

			if (IsInInv (item.Index)) {
				SetImage (GetImageByName ("invbig-" + item.Name));
			} else {
				UpdateItemGroup (item.Index);
				UpdateRoomState (roomImage);
			}

			AddText (GenericCantDoThis ("unblock"));
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
				roomImage = false;
			}

			if (IsInInv (item.Index)) {
				SetImage (GetImageByName ("invbig-" + item.Name));
			} else {
				UpdateItemGroup (item.Index);
				UpdateRoomState (roomImage);
			}

			AddText (GenericCantDoThis ("break"));
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

			if (item.Index == 111) {

				if (image.sprite.name == "backdoor" || image.sprite.name == "backdoor2" || image.sprite.name == "backdoor3" || image.sprite.name == "backdoor4") {
					OtherCommands ("lock back door");
					return;
				}

				if (image.sprite.name == "basementdoor") {
					OtherCommands ("lock basement door");
					return;
				}

				ResetOverlay ();
				inputLockdown = true;
				currLockdownOption = 28;
			}

			if (item.Index == 112) {
				ResetOverlay ();
				inputLockdown = true;
				currLockdownOption = 30;
			}

			if (item.Index == 77) {
				OtherCommands ("lock shed door");
				return;
			}

			if (item.Index == 144) {
				OtherCommands ("lock door");
				return;
			}

			if (item.Index == 55 && item.State == 2) {
				OtherCommands ("close safe");
				return;
			}

			if (item.Index == 60) {
				ResetOverlay ();
				inputLockdown = true;
				currLockdownOption = 32;
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
				roomImage = false;
			}

			if (IsInInv (item.Index)) {
				SetImage (GetImageByName ("invbig-" + item.Name));
			} else {
				UpdateItemGroup (item.Index);
				UpdateRoomState (roomImage);
			}

			AddText (GenericCantDoThis ("lock"));
			return;
		}
	}

	public void Unlock(int i, int j){
		var item = itemsList [i];
		bool roomImage = true;
		if (j != -1) {
			AddText(specialResponses[j].Response);

			foreach (KeyValuePair<int, int> actions in specialResponses[j].Actions)
			{
				if (ChangeState(actions.Key, actions.Value) == 1)
					break;
			}

			if (item.Index == 111) {
				if (image.sprite.name == "backdoor" || image.sprite.name == "backdoor2" || image.sprite.name == "backdoor3" || image.sprite.name == "backdoor4") {
					OtherCommands ("unlock back door");
					return;
				}

				if (image.sprite.name == "basementdoor") {
					OtherCommands ("unlock basement door");
					return;
				}

				ResetOverlay ();
				inputLockdown = true;
				currLockdownOption = 29;
			}

			if (item.Index == 112) {
				ResetOverlay ();
				inputLockdown = true;
				currLockdownOption = 31;
			}

			if (item.Index == 55 && item.State == 1) {
				OtherCommands ("open safe");
				return;
			}

			if (item.Index == 77) {
				OtherCommands ("unlock shed door");
				return;
			}

			if (item.Index == 60) {
				ResetOverlay ();
				inputLockdown = true;
				currLockdownOption = 33;
			}

			if (item.Index == 2) {
				unlockingWindow = true;
				Use ("use window".Shlex ());
				return;
			}

			if (item.Index == 8) {
				unlockingDoor = true;
				Use ("use front door".Shlex ());
				return;
			}

			if (item.Index == 144) {
				OtherCommands ("unlock door");
				return;
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
				roomImage = false;
			}

			if (IsInInv (item.Index)) {
				SetImage (GetImageByName ("invbig-" + item.Name));
			} else {
				UpdateItemGroup (item.Index);
				UpdateRoomState (roomImage);
			}

			AddText (GenericCantDoThis ("unlock"));
			return;
		}
	}

	public void Drink(int i, int j){
		var item = itemsList [i];
		bool roomImage = true;
		if (j != -1) {
			AddText(specialResponses[j].Response);

			if (item.Index == 65) {
				GameOverAudio (61, true);
			}

			if (item.Index == 47) {
				GameOverAudio (71, true);
			}

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
				roomImage = false;
			}

			if (IsInInv (item.Index)) {
				SetImage (GetImageByName ("invbig-" + item.Name));
			} else {
				UpdateItemGroup (item.Index);
				UpdateRoomState (roomImage);
			}

			AddText (GenericCantDoThis ("drink"));
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

				if (!shitOnStairs) {
					shitOnStairs = true;
				}
				else {
					AddText ("You're all spent.");
					return;
				}
			}

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
				roomImage = false;
			}

			if (IsInInv (item.Index)) {
				SetImage (GetImageByName ("invbig-" + item.Name));
			} else {
				UpdateItemGroup (item.Index);
				UpdateRoomState (roomImage);
			}

			AddText ("Doesn't seem like a great place to take a shit.");
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

			if (item.Index == 48) {
				var curtain = GetObjectByName ("curtain");
				if (curtain.State == 0) {
					AddText ("You crouch gingerly in the slippery tub and pull the curtain closed behind you.\n\nPress [ENTER] to continue.");
				}
				else {
					AddText ("You crouch gingerly in the slippery tub.\n\nPress [ENTER] to continue.");
				}
			}

			if (specialResponses[j].Image != "")
			{
				ResetOverlay ();
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
				roomImage = false;
			}

			if (IsInInv (item.Index)) {
				SetImage (GetImageByName ("invbig-" + item.Name));
			} else {
				UpdateItemGroup (item.Index);
				UpdateRoomState (roomImage);
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

			if (item.Index == 17)
			{
				var lrStairNames = GetAltNames ("living room staircase");
				lrStairNames.Remove ("stairs");
				lrStairNames.Remove ("stair case");
				lrStairNames.Remove ("staircase");
				lrStairNames.Remove ("stairway");
				lrStairNames.Remove ("stair way");
				lrStairNames.Remove ("stair");
			}

			if (item.Index == 114)
			{
				Look ("look map".Shlex ());
				return;
			}

			if (item.Index == 132) {
				Look ("look journal.txt".Shlex ());
				return;
			}

			if (item.Index == 126) {
				GameOverAudio (-1, true);
			}

			if (item.Index == 122) {
				GameOverAudio (-1, true);
			}

			if (item.Index == 20) {
				multiSequence = true;
				currMultiSequence = 27;
				AddText("\"First, don't take your mind off the situation, especially by reading a How To manual.\" Wow, that's really good advice. Maybe the silhouette behind you would appreciate that advice. Wait a minute... You don't know any silhouettes!\n\nPress [ENTER] to continue.");
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
				ResetOverlay ();
				ImageCheckAndShow (item.Index, item.State, specialResponses [j].Image);
				roomImage = false;
			}

			if (!killerInKitchen) {
				ResetOverlay ();
			}

			UpdateItemGroup (item.Index);
			UpdateRoomState(roomImage);
		}
		else {
			if (item.currentState.Image != "") {
				ResetOverlay ();
				ImageCheckAndShow (item.Index, item.State, item.currentState.Image);
				roomImage = false;
			}

			if (IsInInv (item.Index)) {
				SetImage (GetImageByName ("invbig-" + item.Name));
			} else {
				UpdateItemGroup (item.Index);
				UpdateRoomState (roomImage);
			}

			AddText (GenericCantDoThis ("read"));
			return;
		}
	}

	public void Call(int i, int j)
	{
		bool roomImage = true;
		var item = itemsList[i];
		if (j != -1)
		{
			ResetOverlay ();


			if (item.State == 0 && !killerInKitchen) {
				AddText (specialResponses [j].Response);

				if (item.Index == 123) {
					GameOverAudio (-1, true);
				}

				int state = item.State;
				ImageCheckAndShow (item.Index, item.State, specialResponses [j].Image);

				foreach (KeyValuePair<int, int> actions in specialResponses[j].Actions) {
					if (ChangeState (actions.Key, actions.Value) == 1)
						break;
				}

				UpdateItemGroup (item.Index);
				UpdateRoomState (false);

				if (item.Index == 11) {
					pizzaTimer = 0;

					if (image.sprite.name != "phone4" && image.sprite.name != "phone5") {
						PlayClip (GetClip (42));
					} else {
						PlayClip (GetClip (41));
					}

				} 

				if (item.Index == 12) {

					if (image.sprite.name != "phone4" && image.sprite.name != "phone5") {
						PlayClip (GetClip (44));
					} else {
						PlayClip (GetClip (43));
					}

				} 

				if (item.Index == 13) {

					if (image.sprite.name != "phone4" && image.sprite.name != "phone5") {
						PlayClip (GetClip (46));
					} else {
						PlayClip (GetClip (45));
					}

				} 

				if (item.Index == 14) {
					policeTimer = 0;

					if (image.sprite.name != "phone4" && image.sprite.name != "phone5") {
						PlayClip (GetClip (48));
					} else {
						PlayClip (GetClip (47));
					}
				}

				return;
			}
			else
			{
				if (!phoneLooked && !killerInKitchen) {
					phoneLooked = true;

					if (image.sprite.name != "phone4" && image.sprite.name != "phone5") {
						PlayClip (GetClip (38));
					} else {
						PlayClip (GetClip (40));
					}

					AddText ("You hear the dial tone, a split second of harsh static, then suddenly, nothing. That's...not a good thing to have happen right now...");
				}
				else {

					if (image.sprite.name != "phone4" && image.sprite.name != "phone5") {
						PlayClip (GetClip (39));
					}

					AddText ("No use. The line's been cut and it's dead.");
				}

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
				roomImage = false;
			}

			if (IsInInv (item.Index)) {
				SetImage (GetImageByName ("invbig-" + item.Name));
			} else {
				UpdateItemGroup (item.Index);
				UpdateRoomState (roomImage);
			}

			AddText (GenericCantDoThis ("call"));
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
				AddText ("Well...this is embarrassing. You can't remember the code to get into this thing.");
				SetImage(GetImageByName("safe"));
				return;
			}

			if (item.Index == 8 && pizzaTimer >= pizzaCap && pizzaTimer <= pizzaCap2) {
				multiSequence = true;
				currMultiSequence = 8;
				ResetOverlay();
				SetGasMaskOverlay(false);
				AddText ("You open the door to greet the pizza delivery man. Mid-sentence however, he interrupts: \"Watch out behind you!\" You tense up and swing around as the pizza guy runs past you, pizza and all. Behind you, the killer had evidently been sneaking up to your back. The delivery guy slams the man in the cleansuit to the floor and begins pummeling him. There is a brief struggle before the killer is knocked out cold. Evidently, he did not expect a hostile delivery man to be waiting behind the door when you opened it.[DOUBLENEWLINE]Press [ENTER] to continue.");
				SetImage (GetImageByName ("pizzawin"));
				return;
			}

			if (item.Index == 8 && policeTimer >= policeCap) {
				if (killerInKitchen) {
					AddText ("You decide to make a break for it. You throw open the front door and start to run. Before your feet can even cross the threshold however, you experience an unpleasant dying sensation. The killer had evidently hobbled out of the kitchen in order to intercept you, and threw a knife square into the back of your neck.\n\nPress [ENTER] to restart.");
					SetImage (GetImageByName ("porchdeath"));
				} else {
					AddText ("You turn the handle of the door and ease it open. Before you can so much as look around, the man in the cleansuit appears to finish the job. The bloody cap of a presumably butchered police officer dons his head at a jaunty angle.");
					SetImage (GetRandomDeathImage());
				}

				SetGasMaskOverlay(false);

				GameOverAudio (-1, true);

				health = 0;
				return;
			}

			if (item.Index == 8 && killerInKitchen) {
				AddText ("You decide to make a break for it. You throw open the front door and start to run. Before your feet can even cross the threshold however, you experience an unpleasant dying sensation. The killer had evidently hobbled out of the kitchen in order to intercept you, and threw a knife square into the back of your neck.\n\nPress [ENTER] to restart.");             
				SetImage(GetImageByName("porchdeath"));
				SetGasMaskOverlay(false);
				ResetOverlay ();

				GameOverAudio (-1, true);

				health = 0;
				return;
			}

			if (item.Index == 8) {
				GameOverAudio (-1, true);
			}

			if (item.Index == 2) {
				ResetOverlay ();
				SetGasMaskOverlay (false);
				if (killerInKitchen) {
					if (pizzaTimer >= pizzaCap && pizzaTimer <= pizzaCap2) {
						AddText ("You unlock and open the window, but as you start to poke your head out, you feel a horrible pain as a blade of cold metal pierces your back. You fall to the floor, groping for the knife in your back and screaming. In your last seconds of life, you notice that the pizza guy has come up to the window. Somehow clueless to your suffering, he attempts to deliver you the pizza by handing it to you through the window to your writhing form.\n\nPress [ENTER] to restart.");

						GameOverAudio (-1, true);

						SetImage (GetImageByName ("windowdeath3"));
					} else {
						AddText ("You decide to make a break for it. You throw open the front door and start to run. Before your feet can even cross the threshold however, you experience an unpleasant dying sensation. The killer had evidently hobbled out of the kitchen in order to intercept you, and threw a knife square into the back of your neck.\n\nPress [ENTER] to restart.");

						GameOverAudio (-1, true);

						SetImage (GetImageByName ("porchdeath"));
					}
				} else {
					if (policeTimer >= policeCap) {
						SetImage (GetImageByName ("windowdeath2"));
						if (unlockingWindow) {
							AddText ("You unlock and lift the window and poke your head out to see if you can figure out what happened to the police officers and the stranger. Before you can so much as look around, the man in the cleansuit appears to finish the job. The bloody cap of a presumably butchered police officer dons his head at a jaunty angle.\n\nPress [ENTER] to restart.");

							GameOverAudio (-1, true);
						} else {
							AddText ("You lift the window and poke your head out to see if you can figure out what happened to the police officers and the stranger. Before you can so much as look around, the man in the cleansuit appears to finish the job. The bloody cap of a presumably butchered police officer dons his head at a jaunty angle.\n\nPress [ENTER] to restart.");

							GameOverAudio (-1, true);
						}
					} else if (pizzaTimer >= pizzaCap && pizzaTimer <= pizzaCap2) {
						SetImage (GetImageByName ("windowdeath3"));
						if (unlockingWindow) {
							AddText ("You unlock and open the window, but as you start to poke your head out, you feel a horrible pain as a blade of cold metal pierces your back. You fall to the floor, groping for the knife in your back and screaming. In your last seconds of life, you notice that the pizza guy has come up to the window. Somehow clueless to your suffering, he attempts to deliver you the pizza by handing it to you through the window to your writhing form. You only wish you could have tasted it before you died.\n\nPress [ENTER] to restart.");

							GameOverAudio (-1, true);
						} else {
							AddText ("You open the window, but as you start to poke your head out, you feel a horrible pain as a blade of cold metal pierces your back. You fall to the floor, groping for the knife in your back and screaming. In your last seconds of life, you notice that the pizza guy has come up to the window. Somehow clueless to your suffering, he attempts to deliver you the pizza by handing it to you through the window to your writhing form. You only wish you could have tasted it before you died.\n\nPress [ENTER] to restart.");

							GameOverAudio (-1, true);
						}
					}
					else {
						SetImage (GetImageByName ("windowdeath"));
						if (unlockingWindow) {
							AddText ("You unlock, open the window, and attempt to climb out. Just before you can get more than a leg out however, the man in the cleansuit appears from the shadows and pushes you bodily back into the house. You scramble to get to your feet, but the knife is already in your chest.\n\nPress [ENTER] to restart.");

							GameOverAudio (-1, true);
						} else {
							AddText ("You open the window and attempt to climb out. Just before you can get more than a leg out however, the man in the cleansuit appears from the shadows and pushes you bodily back into the house. You scramble to get to your feet, but the knife is already in your chest.\n\nPress [ENTER] to restart.");								

							GameOverAudio (-1, true);
						}
					}

				}


				health = 0;
				return;
			}

			if (item.Index == 111) {
				if (image.sprite.name == "backdoor" || image.sprite.name == "backdoor2" || image.sprite.name == "backdoor3" || image.sprite.name == "backdoor4") {
					OtherCommands ("open back door");
					return;
				}

				if (image.sprite.name == "basementdoor") {
					OtherCommands ("open basement door");
					return;
				}

				ResetOverlay ();
				inputLockdown = true;
				currLockdownOption = 10;
			}

			if (item.Index == 112) {
				ResetOverlay ();
				inputLockdown = true;
				currLockdownOption = 14;
			}

			if (item.Index == 60) {
				ResetOverlay ();
				inputLockdown = true;
				currLockdownOption = 19;
			}

			if (item.Index == 77) {
				OtherCommands ("open shed door");
				return;
			}

			if (item.Index == 114)
			{
				Look ("look map".Shlex ());
				return;
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

			/*if (item.Index == 62)
			{
				Use ("use washer".Shlex());
				return;
			}*/

			if (item.Index == 132)
			{
				Look ("look journal.txt".Shlex());
				return;
			}

			if (item.Index == 67)
			{
				Use ("use pinata".Shlex());
				return;
			}

			if (item.Index == 144)
			{
				OtherCommands ("open door");
				return;
			}

			AddText (specialResponses [j].Response);


			int state = item.State;
			foreach (KeyValuePair<int, int> actions in specialResponses[j].Actions)
			{
				if (ChangeState (actions.Key, actions.Value, 1) == 1) {
					ResetOverlay ();
					SetGasMaskOverlay (false);
					break;
				}

			}

			if (health > 0 && !inputLockdown && startState != item.State && item.Index != 49) {

				if (item.currentState.Description == "checkitem") {
					AddAdditionalText ("\n\n"+ (GetCheckItemDescription (item.Index)) );
				} else {
					AddAdditionalText ("\n\n" + item.currentState.Description);
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
		}
		else {
			if (item.currentState.Image != "") {
				ResetOverlay ();
				ImageCheckAndShow (item.Index, item.State, item.currentState.Image);
				roomImage = false;
			}

			if (IsInInv (item.Index)) {
				SetImage (GetImageByName ("invbig-" + item.Name));
			} else {
				UpdateItemGroup (item.Index);
				UpdateRoomState (roomImage);
			}

			AddText (GenericCantDoThis("open"));
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
				if (image.sprite.name == "backdoor" || image.sprite.name == "backdoor2" || image.sprite.name == "backdoor3" || image.sprite.name == "backdoor4") {
					OtherCommands ("close back door");
					return;
				}

				if (image.sprite.name == "basementdoor") {
					OtherCommands ("close basement door");
					return;
				}

				ResetOverlay ();
				inputLockdown = true;
				currLockdownOption = 11;
			}

			if (item.Index == 112) {
				ResetOverlay ();
				inputLockdown = true;
				currLockdownOption = 15;
			}

			if (item.Index == 60) {
				ResetOverlay ();
				inputLockdown = true;
				currLockdownOption = 20;
			}

			if (item.Index == 77) {
				OtherCommands ("close shed door");
				return;
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

			if (item.Index == 144)
			{
				OtherCommands ("close door");
				return;
			}

			if (item.Index == 83 && dummyAssembled)
			{
				AddText ("If you close the door, it might stifle your recording and spoil the trap you've made.");
				return;
			}

			if (item.Index == 16)
			{
				PlayClip (GetClip (50));
			}

			if (item.Index == 55)
			{
				if (!IsInInv (58)) {
					ChangeState (58, 0);
				}
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
				roomImage = false;
			}

			if (IsInInv (item.Index)) {
				SetImage (GetImageByName ("invbig-" + item.Name));
			} else {
				UpdateItemGroup (item.Index);
				UpdateRoomState (roomImage);
			}

			AddText (GenericCantDoThis("close"));
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
		responses.Add("Don't think you can move there.");
		return responses[UnityEngine.Random.Range(0, responses.Count)];
	}

	public string GenericItemMove()
	{
		List<string> responses = new List<string>();
		responses.Add("It's fine where it is.");
		responses.Add("You decide it's best to leave it here.");
		return responses[UnityEngine.Random.Range(0, responses.Count)];
	}

	public string GenericUse()
	{
		List<string> responses = new List<string>();
		responses.Add("No way to operate that.");
		responses.Add("You can't use that.");

		return responses[UnityEngine.Random.Range(0, responses.Count)];
	}

	public string GenericDontKnow(){
		List<string> responses = new List<string>();
		responses.Add("Not sure what you're talking about.");
		responses.Add("Don't know how to do that.");
		responses.Add("No idea what you mean.");

		return responses[UnityEngine.Random.Range(0, responses.Count)];
	}

	public string GenericPlease()
	{
		List<string> responses = new List<string>();
		responses.Add("That was VERY polite!");
		responses.Add("Wow, great manners!");

		return responses[UnityEngine.Random.Range(0, responses.Count)];
	}

	public string GenericWrongRoom()
	{
		List<string> responses = new List<string>();
		responses.Add("Never did get your teleportation license, did you? You'll just have to move one room at a time.");
		responses.Add("That room isn't next to this one.");

		return responses[UnityEngine.Random.Range(0, responses.Count)];
	}

	public string GenericCapsResponse()
	{
		List<string> responses = new List<string>();
		responses.Add("You don't have to yell.");
		responses.Add("Whoa, chill out.");

		return responses[UnityEngine.Random.Range(0, responses.Count)];
	}

	public string GenericSwearResponse(bool fuck = false)
	{
		List<string> responses = new List<string>();
		responses.Add("That's not happening until you stop cursing.");
		responses.Add("You don't have to take that tone with me, I'm not the one trying to kill you.");

		if (fuck) {
			responses.Add ("Did you just say the \"fuck\" word?");
		}

		return responses[UnityEngine.Random.Range(0, responses.Count)];
	}

	public string GetRandomDanceText()
	{
		List<string> responses = new List<string>();
		responses.Add("You shuffle around awkwardly. This feels like the wrong time.");
		responses.Add("You pause what you are doing to do a perfect moonwalk and spin.");

		return responses[UnityEngine.Random.Range(0, responses.Count)];
	}

	public string GenericCantDoThis(string verb) {
		List<string> responses = new List<string>();
		responses.Add("Hmm, don't think you can " + verb + " that.");
		responses.Add("Doesn't seem like something you can " + verb + ".");
		responses.Add("Can't " + verb + " that.");

		return responses[UnityEngine.Random.Range(0, responses.Count)];
	}

	public int GetFart() {
		List<int> responses = new List<int>();
		responses.Add(67);
		responses.Add(68);

		return responses[UnityEngine.Random.Range(0, responses.Count)];
	}

	public string GetTimeOutText()
	{
		List<string> responses = new List<string>();

		responses.Add("You'll have to move through the house more efficiently if you want to have a chance.");

		if (!doorBlocked) {
			responses.Add ("Try and find a way to block the front door to give yourself more time.");
		}

		return responses[UnityEngine.Random.Range(0, responses.Count)];
	}

	/*public string GetResetText()
	{
		List<string> responses = new List<string> ();
		responses.Add("Don't mess up this time!\n\n");
		responses.Add("STOP DYING!\n\n");

		return responses[UnityEngine.Random.Range(0, responses.Count)];
	}*/

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


		return responses[UnityEngine.Random.Range(0, responses.Count)];
	}

	public int GetRealAmbientNoise(){
		List<int> responses = new List<int> ();
		responses.Add(32);
		responses.Add(33);
		responses.Add(34);
		responses.Add(17);

		return responses[UnityEngine.Random.Range(0, responses.Count)];
	}
}