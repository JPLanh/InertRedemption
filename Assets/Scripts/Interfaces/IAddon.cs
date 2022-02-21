using System;
using System.Collections.Generic;

public interface IAddon
{
    public Dictionary<string, int> getRequirements();
    public int getLevel();
    public void setLevel(int in_level);
    public string getName();
    public void updateLevel(int getVal);
    public string getInfo();
    public string getUpgradeInfo();
}
