using UnityEngine;
using System.Collections;

namespace UXLib.UI {

	public class UXButton : MonoBehaviour {
		
		/// 
		public enum State {
			Normal,	/**< */
			Pressed, /**< */
			Disabled /**< */
		};
	
		public Sprite[] sprites;
		SpriteRenderer spriteRenderer;
		
		State state;
		bool isPressed;
		
		public delegate void ButtonClickHandler();
		public delegate void ButtonUpHandler();
		public delegate void ButtonDownHandler();
		public delegate void ButtonDragHandler();
		
		public event ButtonClickHandler Click;
		public event ButtonUpHandler Up;
		public event ButtonDownHandler Down;
		public event ButtonDragHandler Drag;
		
		void Start () {
			
			spriteRenderer = GetComponent<Renderer>() as SpriteRenderer;
			
			if (spriteRenderer == null) {
				Debug.Log ("SpriteRenderer not found");
				return;
			}
			
			spriteRenderer = GetComponent<Renderer>() as SpriteRenderer;
			
			SetState (State.Normal);
		}
		
		public void SetState(State s) {
			state = s;
			
			if (sprites == null || sprites.Length < 1) {
				return;
			}
			
			if (s == State.Normal) {
				spriteRenderer.sprite = sprites[0];
			} else if (s == State.Pressed) {
				if (sprites.Length > 1) {
					spriteRenderer.sprite = sprites[1];
				}
			} else if (s == State.Disabled) {
				if (sprites.Length > 2) {
					spriteRenderer.sprite = sprites[2];
				}
			}
		}
		
		void OnMouseDown() {
			if (state == State.Disabled) {
				return;
			}
			
			isPressed = true;
			
			SetState (State.Pressed);
			
			if (Down != null) {
				Down();
			}
		}
		
		void OnMouseUp() {
			if (state == State.Disabled) {
				return;
			}
	
			if (Up != null) {
				Up();
			}
			
			if (isPressed == false) {
				if (Click != null) {
					Click();
				}
			}
		SetState (State.Normal);
		isPressed = false;
	}
	
	void OnMouseDrag() {
		if (Drag != null) {
			Drag();
		}
	}
	
	void OnMouseExit() {
		isPressed = false;
	}
}
}
