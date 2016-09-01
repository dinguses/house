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
        Debug.Log(house["house"].FirstChild.ChildNodes[room]["description"].InnerText);
    }

    public void ReadInput(string text)
    {
        string[] stringArr = text.Split(' ');
        for(int i = 0; i < stringArr.Length; ++i)
        {
            for(int j = 0; j < house["house"].LastChild.ChildNodes.Count; ++j)
            {
                if(stringArr[i].ToLower() == house["house"].LastChild.ChildNodes[j].InnerText.ToLower())
                {
                    object[] parameters = new object[1];
                    parameters[0] = text;
                    MethodInfo mInfo = typeof(XMLParser).GetMethod(house["house"].LastChild.ChildNodes[j].InnerText);
                    mInfo.Invoke(this, parameters);
                    return;
                }
            }
        }
    }

    public void Look(string text = "")
    {
        string description;
        description = house["house"].FirstChild.ChildNodes[room]["description"].InnerText;
        Debug.Log(description);
        appender.AppendText(description);
    }

    public void Move(string text)
    {
        int newRoom = room;
        string[] stringArr = text.Split(' ');
        for (int i = 0; i < stringArr.Length; ++i)
        {
            for (int j = 0; j < house["house"].FirstChild.ChildNodes.Count; ++j)
            {
                if (house["house"].FirstChild.ChildNodes[j].Attributes.GetNamedItem("name").Value.ToLower().Contains(stringArr[i].ToLower()))
                {
                    newRoom = j;
                    break;
                }
            }
            if (newRoom != room)
                break;
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

