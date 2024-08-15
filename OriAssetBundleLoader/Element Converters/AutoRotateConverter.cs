using Il2Cpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class AutoRotateConverter : ElementConverter
{
    public override void ConvertElement(GameObject Asset)
    {
        AutoRotate autoRotate = Asset.transform.Find("RotatingGameObject").gameObject.AddComponent<AutoRotate>();

        autoRotate.Speed = GetFloat(Asset ,"Speed");
    }
}