using System;
using Matryoshka.Abilities;
using UnityEngine;
using Matryoshka.Entity;
using UnityEngine.Serialization;

[Serializable]
public struct EntityInfo
{
    public AbilityType currentAttack;
    public Vector2 position;
    public Vector2 mouseDirection;
    public Vector2 moveDirection;
    public EntityType entityType;
    public Vector2 mousePosition;

    public EntityInfo(AbilityType currentAttack, Vector2 position, Vector2 mouseDirection, Vector2 moveDirection, EntityType entityType, Vector2 mousePosition)
    {
        this.currentAttack = currentAttack;
        this.position = position;
        this.mouseDirection = mouseDirection;
        this.moveDirection = moveDirection;
        this.entityType = entityType;
        this.mousePosition = mousePosition;
    }
}

