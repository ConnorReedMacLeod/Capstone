using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This should control actions and logical flow of the game
// Map actions taken with the view to changes in the model
// Can maintain information about game state

abstract public class Controller : Element {

	abstract public void OnNotification (string eventType, Object target, params object[] args);
}
