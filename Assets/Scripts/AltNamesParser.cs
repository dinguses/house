using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Reflection;
using System.Collections;
using UnityEngine.UI;

class AltNamesParser : MonoBehaviour
{
	public TextAsset xmlDocument;
	public static XmlDocument altNamesDoc;

	void Start()
	{
		altNamesDoc = new XmlDocument();
		altNamesDoc.LoadXml(xmlDocument.text);
	}

	public Dictionary<string, List<string>> ReadXML(XmlDocument xml)
	{
		Dictionary<string,List<string>> altNamesList = new Dictionary<string, List<string>>();
		XmlDocument altNamesXML = xml;
		for (int i = 0; i < altNamesXML ["altnamedoc"].FirstChild.ChildNodes.Count; ++i) {
			string altNameKey = altNamesXML["altnamedoc"].FirstChild.ChildNodes [i]["real"].InnerText;

			List<string> altStringList = new List<string> ();

			for (int j = 0; j < altNamesXML ["altnamedoc"].FirstChild.ChildNodes [i] ["alts"].ChildNodes.Count; ++j) {
				string altNameValue = altNamesXML ["altnamedoc"].FirstChild.ChildNodes [i] ["alts"].ChildNodes [j].InnerText;
				altStringList.Add (altNameValue);
			}
				
			altNamesList.Add (altNameKey, altStringList);
		}
		return altNamesList;

	}
}

