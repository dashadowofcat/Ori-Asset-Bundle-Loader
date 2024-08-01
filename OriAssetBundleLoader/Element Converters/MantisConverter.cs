using Il2Cpp;
using Il2CppMoon;
using MelonLoader;
using System.Linq;
using UnityEngine;
using UniverseLib;

public class MantisConverter : ElementConverter
{
    public override void ConvertElement(GameObject Asset)
    {
        GameObject mantis = GameObject.Instantiate(PrefabManager.hornBug, Asset.transform);

        mantis.transform.localPosition = Vector3.zero;

        MantisPlaceholder PlaceHolder = mantis.GetComponent<MantisPlaceholder>();

        PlaceHolder.SpawnOn = EntityPlaceholder.SpawnMode.Manual;

        GameObject.Destroy(PlaceHolder.PooledEntity);

        PlaceHolder.SpawnOnGround = false;

        // the delay is so it has enough time to setup
        PlaceHolder.Invoke("Spawn", .05f);
    }
}
