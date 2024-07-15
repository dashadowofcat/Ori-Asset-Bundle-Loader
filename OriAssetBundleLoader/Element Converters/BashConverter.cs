using Il2Cpp;
using System.Linq;
using UnityEngine;
using UniverseLib;


public class BashConverter : ElementConverter
{
    public override void ConvertElement(GameObject Asset)
    {
        SpiritLantern lantern = Asset.AddComponent<SpiritLantern>();

        lantern.OnBashSoundProvider = RuntimeHelper.FindObjectsOfTypeAll<Varying2DSoundProvider>().Where(S => S.name == "startDashForward").FirstOrDefault();
    }
}