using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBuilding
{
    public void buildingComplete();
    public void buildingInProgress();
    public void placeBuilding(PlayerController player);
    public void startBuilding();
    public void relocateBuilding(PlayerController player);
    public int toggleBuildable(bool getBool, Building getEmitter);
    public Building getEmitter();
    public int getBuildingCurrent();
    public List<InventoryMapping> getRequirement();
}
