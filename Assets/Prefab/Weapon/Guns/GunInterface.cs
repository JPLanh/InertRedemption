using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface GunInterface
{
    public void shoot();
    public IEnumerator reloading();
    public void reload();
    public void aim();
}
