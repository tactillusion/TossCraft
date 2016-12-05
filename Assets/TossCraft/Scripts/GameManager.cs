using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace TossCraft {
	
	/// <summary>
	/// Game manager. This class serves as the main controller for the game. It is divided into two
	/// sub tasks: Gesture Management and UI management. 
	/// </summary>
	public class GameManager : MonoBehaviour {

		public GestureManager gestureManager;
		public UIManager uiManager;

		public delegate void EndEvent (GestureManager.GestureTypes type);

		public enum GameMode {
			Desktop,
			VR
		}

		public GameMode CurrentMode;
		public Transform VRCamera;
		public Transform DesktopCamera;

		void Awake () {
			init ();
		}

		void Update () {}

		/// <summary>
		/// Setting up the camera depending on the mode you are playing in (VR, Desktop).
		/// Initializing the Gesture Manager and the UI.
		/// </summary>
		void init () {
			if (DesktopCamera == null) {
				CurrentMode = GameMode.VR;
				Debug.Log ("Current scene doesn't have Desktop mode");
			} else if (VRCamera == null) {
				CurrentMode = GameMode.Desktop;
				Debug.Log ("Current scene doesn't have VR mode");
			}

			if (DesktopCamera != null)
				DesktopCamera.gameObject.SetActive (CurrentMode == GameMode.Desktop);
			if (VRCamera != null)
				VRCamera.gameObject.SetActive (CurrentMode == GameMode.VR);

			gestureManager.InitGesture (this);
			uiManager.InitUI (this);
		}

		#region Gesture

		/// <summary>
		/// Gets the current active gestures.
		/// </summary>
		/// <returns>The current active gestures.</returns>
		public Dictionary<GestureManager.GestureTypes, object> GetCurrentActiveGestures () {
			return gestureManager.GetCurrentActiveGestures ();
		}

		/// <summary>
		/// Gets the transform gesture manager based mode. (VR vs. Desktop)
		/// </summary>
		/// <returns>The transform gesture manager based mode.</returns>
		public Transform GetTransformGestureManagerBasedMode () {
			return (CurrentMode == GameMode.Desktop) ? DesktopCamera : VRCamera;
		}

		#endregion

		#region UI

		public bool IsReadyUI () {
			return uiManager.IsReady ();
		}

		public void UpdateUIBlockingGesture (GestureManager.GestureTypes type, float timer, EndEvent endEvent) {
			uiManager.RegisterEventEndCountDown (endEvent);
			uiManager.UpdateSliderBlockingGesture (type, timer);
		}

		public void UpdateUILoadingGesture (GestureManager.GestureTypes type, float percent) {
			uiManager.UpdateTimerLoadingGesture (type, percent);
		}

		#endregion
	}
}