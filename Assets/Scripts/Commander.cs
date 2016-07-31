using System;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Commander : MonoBehaviour
{
    public Text text;

    Dictionary<string, MethodInfo> commands;

    public void OnValidate()
    {
        if (text == null) text = GetComponent<Text>();
    }

    void SetupCommands()
    {
        commands = new Dictionary<string, MethodInfo>();
        var methods = typeof(Commander).GetMethods();
        foreach (var method in methods)
        {
            foreach (var attr in method.GetCustomAttributes(typeof(CommandAttribute), false).Cast<CommandAttribute>())
            {
                var cmdname = attr.Name;

                if (cmdname == null) cmdname = method.Name.ToLower();

                commands.Add(cmdname, method);
            }
        }
    }

    public void Start()
    {
        SetupCommands();
    }

    public void Update()
    {
    }

    [Command]
    public void Clear(List<string> _)
    {
        text.text = "";
    }

    [Command("echo")]
    public void Append(List<string> txt)
    {
        var sb = new StringBuilder(txt.Count * 2 + 1);
        sb.Append('\n');
        foreach (var word in txt.Skip(1))
        {
            sb.Append(word);
            sb.Append(' ');
        }

        sb.Append('\n');
        this.text.text += sb.ToString();
    }

    [Command]
    public void Log(List<string> txt)
    {
        var sb = new StringBuilder((txt.Count()-1)*2);

        foreach (var item in txt.Skip(1))
        {
            sb.Append(item);
            sb.Append(' ');
        }

        Debug.Log(sb.ToString());
    }

    public void RunCommand(string command)
    {
        Debug.LogFormat("Running command: {0}", command);

        var tokens = command.Shlex();

        var cmd = commands[tokens[0]];

        cmd.Invoke(this, new object[] { tokens });
    }
}
