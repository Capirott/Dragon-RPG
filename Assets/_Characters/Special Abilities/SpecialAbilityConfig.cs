using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Characters;
using System;

public abstract class SpecialAbilityConfig : ScriptableObject
{
    [Header("Special Ability General")]
    [SerializeField] float energyCost = 10f;

    protected ISpecialAbility behaviour;

    public abstract void AttachComponentTo(GameObject gameObjectToAttachTo);

    public void Use()
    {
        behaviour.Use();
    }
}
