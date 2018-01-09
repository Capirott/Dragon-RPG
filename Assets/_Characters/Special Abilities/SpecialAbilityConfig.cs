using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Characters;

public abstract class SpecialAbilityConfig : ScriptableObject
{
    [Header("Special Ability General")]
    [SerializeField] float energyCost = 10f;

    public abstract ISpecialAbility AddComponent(GameObject gameObjectToAttachTo);
}
