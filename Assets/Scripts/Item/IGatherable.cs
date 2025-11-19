using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGatherable
{
    void Gather(Vector3 hitPoint, Vector3 hitNormal, float damageAmount);
}
