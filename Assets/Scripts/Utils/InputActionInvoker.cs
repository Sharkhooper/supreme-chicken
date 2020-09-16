using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputActionInvoker : MonoBehaviour {
	public InputAction action;

	public UnityEvent<InputAction.CallbackContext> @callback;

	private void Start() {
		action.performed += Invoke;
	}

	private void OnEnable() {
		action.Enable();
	}

	private void OnDisable() {
		action.Disable();
	}

	public void Invoke(InputAction.CallbackContext c) {
		@callback.Invoke(c);
	}

	public void SaySomething(string s) {
		Debug.Log(s);
	}

}
