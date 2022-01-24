using System;
using UnityEngine;

public interface UsableItemInterface
{
	public void fireOne();
	public void fireTwo();
	public void reload();
	public void setOwner(LivingBeing getOwner);
	public bool isUsable();
	public void accessMenu(bool flag);
	public void swapItem(bool getBool);
	public int rechargeDurability(int getVal);
	public float getEnergy();

}
