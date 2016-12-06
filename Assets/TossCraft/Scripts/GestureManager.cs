using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Leap.Unity;
using UnityEngine.UI;
using CustomUtils;

namespace TossCraft
{
	public class GestureManager : MonoBehaviour {

		public enum GestureTypes {
			Grab,
			Throw
		}

		GestureTypes currentType;

		public GestureTypes GetCurrentGestureType () {
			return currentType;
		}

		public LeapProvider leapHandProvider;

		public LeapProvider GetLeapHand () {
			return leapHandProvider;
		}

		public float TimeBetween2Gestures;

		Dictionary<GestureTypes, object> listActiveGestures;

		public Dictionary<GestureTypes, object> GetCurrentActiveGestures () {
			return listActiveGestures;
		}

		GameManager gameManager;

		void Start () {}

		void Update () {}

		public void InitGesture (GameManager manager) {
			gameManager = manager;

			leapHandProvider = gameManager.GetTransformGestureManagerBasedMode ().GetComponentInChildren<LeapProvider> ();
			
			listActiveGestures = new Dictionary<GestureTypes, object> ();
			foreach (Transform t in transform) {
				if (t.GetComponent<Gesture> () != null) {
					foreach (GestureTypes type in Enum.GetValues(typeof(GestureTypes))) {
						if (t.name.Equals (type.ToString ()))
							listActiveGestures.Add (type, t.GetComponent<Gesture> () as object);
					}
					t.GetComponent<Gesture> ().Init (this);
				}
			}
		}

		public bool ReceiveEvent (GestureTypes type) {
			if (gameManager.IsReadyUI ()) {
				currentType = type;
				gameManager.UpdateUIBlockingGesture (type, TimeBetween2Gestures, UnBlockGesture);
				return true;
			} 
			return false;
		}

		void UnBlockGesture (GestureTypes type) {
			Gesture behavior = (Gesture)listActiveGestures [type];
			behavior.UnBlockGesture ();
		}

		public void LoadingGestureProgress (GestureTypes type, float percent) {
			gameManager.UpdateUILoadingGesture (type, percent);
		}

		public GameManager.GameMode GetCurrentGameMode () {
			return gameManager.CurrentMode;
		}
	}
}