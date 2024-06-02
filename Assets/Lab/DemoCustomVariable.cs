// CustomVariable.cs
using UnityEngine;

[System.Serializable]
public class DemoCustomVariable
{
    public string variableName;
    public DemoVariableType type;
    public int intValue;
    public float floatValue;
    public string stringValue;
    public bool boolValue;
}

public enum DemoVariableType
{
    Int,
    Float,
    String,
    Bool
}
