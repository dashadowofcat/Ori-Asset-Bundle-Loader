using UnityEngine;

class ElementConverter
{
    public virtual void ConvertElement(GameObject Asset)
    {

    }

    public string GetString(GameObject Asset, string PropertyName)
    {
        return Asset.transform.Find(PropertyName).GetChild(0).name;
    }

    public int GetInt(GameObject Asset, string PropertyName)
    {
        return int.Parse(Asset.transform.Find(PropertyName).GetChild(0).name);
    }

    public float GetFloat(GameObject Asset, string PropertyName)
    {
        return float.Parse(Asset.transform.Find(PropertyName).GetChild(0).name);
    }

    public bool GetBool(GameObject Asset, string PropertyName)
    {
        return Asset.transform.Find(PropertyName).Find("true");
    }
}
