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
    public static int health = 100;
    static XmlDocument house;
    static  XMLParser xmlParser;
    // Use this for initialization
    void Start () {

		// Rooms
		xmlParser = gameObject.GetComponent (typeof(XMLParser)) as XMLParser;
		TextAsset text = xmlParser.xmlDocument;
		house = new XmlDocument();
		house.LoadXml(text.text);
		rooms = xmlParser.ReadXML (house);

		// Inventory
		inventory = new List<int>();
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
