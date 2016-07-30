using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;

public class XmlTest : MonoBehaviour {

    public TextAsset testFile;

    public XElement root;

    public Text text;

	// Use this for initialization
	void Start () {
        Debug.Log("loading", this);
        root = XElement.Parse(testFile.text);
        Debug.Log(testFile.text, this);
        var rooms = from room in root.Elements("room")
                    select room.Attribute("name").Value;
        Debug.Log(rooms, this);
        var roomstr = string.Join(", ", rooms.ToArray());

        Debug.Log("rooms: " + roomstr, this);

        text.text = roomstr;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
