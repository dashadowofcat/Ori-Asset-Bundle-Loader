using UnityEngine;

public class ElementConverter
{
    public virtual void ConvertElement(GameObject Asset)
    {
    }

    public string GetString(GameObject Asset, string PropertyName)
    {
        Transform child = Asset.transform.Find(PropertyName);
        if(child != null && child.childCount > 0)
        {
            return child.GetChild(0).name;
        }

        return null;
    }

    public int GetInt(GameObject Asset, string PropertyName)
    {
        string value = GetString(Asset, PropertyName);
        if (value == null)
            return -1;

        return int.Parse(value);
    }

    public float GetFloat(GameObject Asset, string PropertyName)
    {
        string value = GetString(Asset, PropertyName);
        if (value == null)
            return -1f;

        return float.Parse(value);
    }

    public bool GetBool(GameObject Asset, string PropertyName)
    {
        Transform child = Asset.transform.Find(PropertyName);
        if (child != null)
        {
            return child.Find("true");
        }

        return false;
    }
}
