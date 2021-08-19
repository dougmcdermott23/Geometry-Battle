using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    float InteractionRange { get; }
    void Init(Vector3 startPosition);
    void OnInteraction();
}
