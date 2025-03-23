using UnityEngine;

public struct Damage
{
    public Vector3 origin;
    public int damageAmount;
    public float pushForce;
    public bool isCritical;
    // TODO : Find better way to pass min and max damage instead of sending it everytime -> maybe find the weapon used
    public int minPossibleDamage;
    public int maxPossibleDamage;
}
