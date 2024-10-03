using Il2Cpp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Il2CppMoon;
using UniverseLib;
using MelonLoader;

public class SlotDoorConverter : ElementConverter
{
    private DoorType doorType;

    public SlotDoorConverter(DoorType DoorType)
    {
        doorType = DoorType;
    }

    public override void ConvertElement(GameObject Asset)
    {
        MelonLogger.Warning("non-completed convertion (SlotDoorConverter), you can either ignore it or re-visit the development");

        return;


        GameObject Door = null;

        Door = SpawnDoor(Asset, doorType);

        Door.transform.localPosition = Vector3.zero;



        UberStateGroup Group = RuntimeHelper.FindObjectsOfTypeAll<UberStateGroup>().Where(G => G.name == "testUberStateGroupDescriptor").FirstOrDefault();

        NewSetupStateController StateController = Door.GetComponentInChildren<NewSetupStateController>();

        MoonDoorWithSlots DoorWithSlots = Door.GetComponentInChildren<MoonDoorWithSlots>();

        SerializedBooleanUberState State = new SerializedBooleanUberState();

        State.name = "TestDoorStateJAJAJA";

        State.Group = Group;

        State.Value = false;

        StateController.AffectingUberStates[0] = State.TryCast<IUberState>();
        DoorWithSlots.AffectingUberStates[0] = StateController.AffectingUberStates[0];

        DoorWithSlots.OpenedState.Descriptor = State;
    }


    private GameObject SpawnDoor(GameObject Parent, DoorType Type)
    {
        if(Type == DoorType.TwoSlot) return null;

        if(Type == DoorType.FourSlot) return GameObject.Instantiate(PrefabCachingManager.GetPrefab("FourSlotDoor"), Parent.transform);

        return null;
    }


    public enum DoorType
    {
        TwoSlot,
        FourSlot
    }
}
