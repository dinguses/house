using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Commander : MonoBehaviour
{
    public Text text;

    public void OnValidate()
    {
        if (text == null) text = GetComponent<Text>();
    }

    public void Start()
    {
      
    }

    public void Update()
    {
    }

    public void Clear()
    {
        text.text = "";
    }

    public void Append(string txt)
    {
        this.text.text += "\n" + txt + "\n" ;
    }

    public void RunCommand(string command)
    {
        Debug.Log("Running command: ", this);

        var tokens = command.Shlex();

        var sb = new StringBuilder();
        foreach (var item in tokens)
        {
            sb.Append(item);
            sb.Append(", ");
        }

        Debug.LogFormat("cmd: {0}", sb.ToString());
    }
}
