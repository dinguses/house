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

	public Dictionary<string, string> ReadXML(XmlDocument xml)
	{
		Dictionary<string,string> altNamesList = new Dictionary<string, string>();
		XmlDocument altNamesXML = xml;
		for (int i = 0; i < altNamesXML ["altnamedoc"].FirstChild.ChildNodes.Count; ++i) {
			string altNameKey = altNamesXML["altnamedoc"].FirstChild.ChildNodes [i]["alt"].InnerText;
			string altNameValue = altNamesXML["altnamedoc"].FirstChild.ChildNodes [i]["real"].InnerText;
			altNamesList.Add (altNameKey, altNameValue);
		}
		return altNamesList;

	}
}

