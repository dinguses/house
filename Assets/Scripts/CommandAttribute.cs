using System;

/// <summary>
/// A Command marks a method to dispatch a command to.
/// There may be multiple, to have a command answer to many names.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class CommandAttribute : Attribute
{
    public string Name { get; private set; }

    public CommandAttribute(string name)
    {
        Name = name;
    }

    public CommandAttribute(): this(null)
    {
    }
}