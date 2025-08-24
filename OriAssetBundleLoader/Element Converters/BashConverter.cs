using Il2Cpp;
using System.Linq;
using UnityEngine;


public class BashConverter : ElementConverter
{
    public override void ConvertElement(GameObject Asset)
    {
        GameObject bashGameObject = GameObject.Instantiate(PrefabCachingManager.GetPrefab("BashLantern"), Asset.transform.position, Quaternion.identity, Asset.transform);
        bashGameObject.SetActive(true);

        Transform spiritLanternTransform = bashGameObject.transform.Find("spiritLantern");
        GameObject spiritLanternObject = spiritLanternTransform.gameObject;

        Object.Destroy(spiritLanternObject.GetComponent<ConfigurableJoint>());
        Object.Destroy(spiritLanternObject.GetComponent<AttachToPhysicsSystem>());
        Object.Destroy(spiritLanternObject.GetComponent<PhysicalSoftInteraction>());
        Object.Destroy(spiritLanternObject.GetComponent<RigidbodyInteractionController>());
        Object.Destroy(spiritLanternObject.GetComponent<ApplyTurbulentForce>());
        Object.Destroy(spiritLanternObject.GetComponent<BoxCollider>());
        Object.Destroy(spiritLanternObject.GetComponent<ApplyForceOnDamage>());
        Object.Destroy(spiritLanternObject.GetComponent<Rigidbody>());

        for(int i = 0; i < bashGameObject.transform.childCount; ++i)
        {
            Transform child = bashGameObject.transform.GetChild(i);
            if(child.name != "spiritLantern")
            {
                Object.Destroy(child.gameObject);
            }
        }

        bool hasParticles = GetBool(Asset, "HasParticles");
        if (!hasParticles)
        {
            Object.Destroy(spiritLanternObject.GetComponent<InstantiateAction>());

            for(int i = 0; i < spiritLanternObject.transform.childCount; ++i)
            {
                Transform child = spiritLanternObject.transform.GetChild(i);
                Object.Destroy(child.gameObject);
            }
        }

        spiritLanternTransform.localPosition = new Vector3(0f, 0f, 0f);

        LevelManager.AddBashTransform(spiritLanternTransform);

        //SpiritLantern spiritLantern = Asset.AddComponent<SpiritLantern>();

        //InstantiateAction action = Asset.AddComponent<InstantiateAction>();
        //action.Parent = Asset.transform;
        //action.Position = Asset.transform;
        //action.ShouldSetParent = true;
        //action.SpawnWhenNotOnScreen = true;
        //spiritLantern.OnAttackAction = action;
    }
}