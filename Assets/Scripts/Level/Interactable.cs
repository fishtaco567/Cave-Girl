using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using Entities.Character;

public class Interactable : MonoBehaviour {

    [SerializeField]
    UnityEvent<Player> callback;

    public void OnInteract(Player player) {
        Debug.Log("Interact");
        callback?.Invoke(player);
    }

}
