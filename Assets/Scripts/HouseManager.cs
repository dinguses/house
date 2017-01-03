using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Reflection;
using System.Collections;
using UnityEngine.UI;


public class HouseManager : MonoBehaviour {

	public static List<ObjectClass> rooms;
	public static List<int> inventory;
	public static List<ObjectClass> itemsList;
	public static List<SpecialResponseClass> specialResponses;
	public static List<String> commands;

	// Use this for initialization
	void Start () {

		// Rooms
		XMLParser xmlParser = gameObject.GetComponent (typeof(XMLParser)) as XMLParser;
		TextAsset text = xmlParser.xmlDocument;
		XmlDocument house = new XmlDocument();
		house.LoadXml(text.text);
		rooms = xmlParser.ReadXML (house);

		// Inventory
		inventory = new List<int>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
