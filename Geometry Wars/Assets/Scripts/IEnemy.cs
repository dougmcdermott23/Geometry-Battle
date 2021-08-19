using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemy
{
    Transform transform { get; }
    Vector3 Velocity { get; }
    void Init(PlayerController2D target, Vector3 startPosition, GameController.GameBoundaries boundaries);
    void Damage();
    FactoryObject.ObjectType GetEnemyType();
}
