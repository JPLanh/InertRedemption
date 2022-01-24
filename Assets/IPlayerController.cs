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
    public void toggleCrouching();
    public void fireTwo();
    public void Interact();
    public void useAbility(int in_num);
    public InfectionScript getInfectionScript();
    public void setSingleHandUse(bool in_bool);
    public void buildModeSwitch();
}