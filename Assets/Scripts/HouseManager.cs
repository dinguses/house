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

	// Use this for initialization
	void Start () {
		XMLParser xmlParser = gameObject.GetComponent (typeof(XMLParser)) as XMLParser;
		TextAsset text = xmlParser.xmlDocument;
		XmlDocument house = new XmlDocument();
		house.LoadXml(text.text);
		rooms = xmlParser.ReadXML (house);

		for (int i = 0; i < rooms.Count; ++i) {
			//Debug.Log ("Room Index - " + rooms[i].Index);

			for (int j = 0; j < rooms [i].States.Count; ++j) {

				//Debug.Log ("State Image - " + rooms [i].States [j].Image);
				//Debug.Log ("State Desc - " + rooms [i].States [j].Description);
				//Debug.Log ("Conditional Type - " + rooms [i].States [j].ConditionalActions.Type);

				foreach (KeyValuePair<int, int> conditionals in rooms [i].States [j].ConditionalActions.ConditionalActions) {
					//Debug.Log ("Item - " + conditionals.Key);
					//Debug.Log ("Item state - " + conditionals.Value);
				}

			}

			for (int x = 0; x < rooms [i].Objects.Count; ++x) {
			}
		}

		Debug.Log (rooms [0].AdjacentRooms [0]);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
