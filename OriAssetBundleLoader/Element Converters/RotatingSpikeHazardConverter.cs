using Il2Cpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class RotatingSpikeHazardConverter : ElementConverter
{
    public override void ConvertElement(GameObject Asset)
    {
        GameObject rotatingHazard = GameObject.Instantiate(PrefabManager.spring, Asset.transform);

        rotatingHazard.transform.localPosition = Vector3.zero;

        rotatingHazard.GetComponentInChildren<AutoRotate>().Speed = this.GetFloat(Asset, "Speed");

        //rotatingHazard.GetComponentInChildren<AutoRotate>().m_rotation = this.GetFloat(Asset, "Rotation");
    }
}

