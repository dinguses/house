﻿using UnityEngine;
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
<<<<<<< HEAD
	public static Dictionary<string, string> altNames;

	// Use this for initialization
	void Start () {
=======
    public static int health = 100;
    static XmlDocument house;
    static  XMLParser xmlParser;
    // Use this for initialization
    void Start () {
>>>>>>> refs/remotes/origin/master

		// Rooms
		xmlParser = gameObject.GetComponent (typeof(XMLParser)) as XMLParser;
		TextAsset text = xmlParser.xmlDocument;
		house = new XmlDocument();
		house.LoadXml(text.text);
		rooms = xmlParser.ReadXML (house);

		// Inventory
		inventory = new List<int>();

		// Alt Names
		AltNamesParser altNamesParser = gameObject.GetComponent (typeof(AltNamesParser)) as AltNamesParser;
		TextAsset altNamesText = altNamesParser.xmlDocument;
		XmlDocument altNamesDoc = new XmlDocument();
		altNamesDoc.LoadXml(altNamesText.text);
		altNames = altNamesParser.ReadXML (altNamesDoc);
	}
	
	// Update is called once per rame
	void Update () {
		
	}

    public static void ResetHouse()
    {
        rooms = xmlParser.ReadXML(house);

        // Inventory
        inventory = new List<int>();
        health = 100;
        xmlParser.AddText("Type some shit here. Good luck, and don't fuck up this time!");
    }
}
