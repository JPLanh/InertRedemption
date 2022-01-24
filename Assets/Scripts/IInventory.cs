using System;
using System.Collections.Generic;

public interface IInventory : IConsoleOption
{
    public string getName();
    public Dictionary<string, int> getInventory();
    public bool recieveItem(string getName, int getAmt);
    public bool recieveAmmo(string getName, int getAmt);
    public void intervalAmmoIncrease(string getName, int getAmt);
    public Dictionary<string, int> getAmmo();
    public void getHoldAmt(string getName, int getAmt);
    public void convertAllHold();
    public void modifyAmount(int getAmt);
}
