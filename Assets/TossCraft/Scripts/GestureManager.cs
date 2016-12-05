using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Leap.Unity;
using UnityEngine.UI;
using CustomUtils;

namespace TossCraft
{
	public class GestureManager : MonoBehaviour
	{

		public enum GestureTypes
		{
			Grab,
			Throw
		}

		GestureTypes _currentType;

		public GestureTypes GteCurrentGestureType ()
		{
			return _currentType;
		}

		public LeapProvider _leapHandProvider;

		public LeapProvider GetLeapHand ()
		{
			return _leapHandProvider;
		}

		public float TimeBetween2Gestures;

		Dictionary<GestureTypes, object> _listActiveGestures;

		public Dictionary<GestureTypes, object> GetCurrentActiveGestures ()
		{
			return _listActiveGestures;
		}

		GameManager _gameManager;

		void Start ()
		{
		}

		void Update ()
		{
	
		}

		public void InitGesture (GameManager manager)
		{
			_gameManager = manager;

			_leapHandProvider = _gameManager.GetTransformGestureManagerBasedMode ().GetComponentInChildren<LeapProvider> ();
			
			_listActiveGestures = new Dictionary<GestureTypes, object> ();
			foreach (Transform t in transform) {
				if (t.GetComponent<Gesture> () != null) {
					foreach (GestureTypes type in Enum.GetValues(typeof(GestureTypes))) {
						if (t.name.Equals (type.ToString ()))
							_listActiveGestures.Add (type, t.GetComponent<Gesture> () as object);
					}
					t.GetComponent<Gesture> ().Init (this);
				}
			}

		}

		public bool ReceiveEvent (GestureTypes type)
		{
			if (_gameManager.IsReadyUI ()) {
				_currentType = type;
				_gameManager.UpdateUIBlockingGesture (type, TimeBetween2Gestures, UnBlockGesture);
				return true;
			} 
			return false;
		}

		void UnBlockGesture (GestureTypes type)
		{
			Gesture behavior = (Gesture)_listActiveGestures [type];
			behavior.UnBlockGesture ();
		}

		public void LoadingGestureProgress (GestureTypes type, float percent)
		{
			_gameManager.UpdateUILoadingGesture (type, percent);
		}

		public GameManager.GameMode GetCurrentGameMode ()
		{
			return _gameManager.CurrentMode;
		}
	}
}