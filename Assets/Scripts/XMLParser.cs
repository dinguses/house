using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Reflection;

class XMLParser : MonoBehaviour
{
    public TextAsset xmlDocument;
    public TextAppender appender;

    public static XmlDocument house;
    int room = 0;


    void Start()
    {
        house = new XmlDocument();
        house.LoadXml(xmlDocument.text);
    }

    public void ReadInput(string text)
    {
        for(int j = 0; j < house["house"].LastChild.ChildNodes.Count; ++j)
        {
            if(text.ToLower().Contains(house["house"].LastChild.ChildNodes[j].InnerText.ToLower()))
            {
                object[] parameters = new object[1];
                parameters[0] = text;
                MethodInfo mInfo = typeof(XMLParser).GetMethod(house["house"].LastChild.ChildNodes[j].InnerText);
                mInfo.Invoke(this, parameters);
                return;
            }
        }
    }

    public void Look(string text = "")
    {
        string description;
        int state = int.Parse(house["house"].FirstChild.ChildNodes[room].Attributes.GetNamedItem("state").Value);
        description = house["house"].FirstChild.ChildNodes[room]["states"].ChildNodes[state]["description"].InnerText;
        Debug.Log(description);
        appender.AppendText(description);
        //going to the next state of the house, just for testing purposes
        state = (state + 1) % house["house"].FirstChild.ChildNodes[room]["states"].ChildNodes.Count;
        house["house"].FirstChild.ChildNodes[room].Attributes.GetNamedItem("state").Value = state.ToString();
    }

    public void Move(string text)
    {
        int newRoom = room;
        for (int j = 0; j < house["house"].FirstChild.ChildNodes.Count; ++j)
        {
            if (text.ToLower().Contains(house["house"].FirstChild.ChildNodes[j].Attributes.GetNamedItem("name").Value.ToLower()))
            {
                newRoom = j;
                break;
            }
        }

        for (int i = 0; i < house["house"].FirstChild.ChildNodes[room]["adjacent"].ChildNodes.Count; ++i)
        {
            if(newRoom.ToString() == house["house"].FirstChild.ChildNodes[room]["adjacent"].ChildNodes[i].InnerText)
            {
                room = newRoom;
                Look();
                return;
            }
        }
    }
}

