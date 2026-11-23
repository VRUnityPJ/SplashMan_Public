using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Method, Inherited = true)]
public class ButtonAttribute : PropertyAttribute
{
    public string Label { get; private set; }

    public ButtonAttribute()
    {
        Label = null;
    }

    public ButtonAttribute(string label)
    {
        Label = label;
    }
}