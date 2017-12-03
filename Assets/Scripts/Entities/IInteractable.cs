using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    void Hover(bool bInRange);

    void Unhover();

    GameObject GetGameObject();

    void Interact(PlayerBehaviour player, int mouseButton);
}
