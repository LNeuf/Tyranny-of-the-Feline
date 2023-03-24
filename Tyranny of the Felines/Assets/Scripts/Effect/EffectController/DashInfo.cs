using System;
using System.Collections;
using System.Collections.Generic;
using Matryoshka.Effect.EffectDataStructure;
using Matryoshka.Entity;
using UnityEngine;

[Serializable]
public struct DashInfo
{
    public Entity entity;
    public Vector2 direction;
    public DashEffect dashEffect;

    public DashInfo(Entity entity, Vector2 direction, DashEffect dashEffect)
    {
        this.entity = entity;
        this.direction = direction;
        this.dashEffect = dashEffect;
    }
}
