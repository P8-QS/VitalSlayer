using UnityEngine;

public struct Damage
{
    public Vector3 origin;
    public int damageAmount;
    public float pushForce;
    public bool isCritical;
    // For determining damage text size
    public int minPossibleDamage;
    public int maxPossibleDamage;
    // Custom damage color - Det lidt scuffed men man kan ikke bruge struct field initializers i c# 9.0
    public bool useCustomColor;
    public Color customColor;
}