using System.Collections.Generic;
using UnityEngine;
public interface IPlayerController
{
    public void setActivePlayer(string getUserID, string getUsername, PlayerCanvas in_canvas);
    public void setOtherPlayer(string getUserID, string getUsername);
    public void serverControl(Dictionary<string, string> payload);
    public void serverControl(Payload in_payload);
    public GameObject getGameObject();
    public bool isMovable();
    public void accessMenu(bool in_bool);
    public void swapGun(bool in_bool);
    public void fireOne();
    public void toggleFlashLight();
    public void jump();
    public void reload(bool in_bool);
    public void crouching();
    public void fireTwo();
    public void Interact();
    public void useAbility(int in_num);
    public InfectionScript getInfectionScript();
    public void setSingleHandUse(bool in_bool);
    public void buildModeSwitch();
    public float getHealth();
    public LivingBeing getLivingBeing();
    public void listen(Payload in_payload);
    public Inventory getInventory();
    public bool pickupItem(string in_item);
    public void death();
}