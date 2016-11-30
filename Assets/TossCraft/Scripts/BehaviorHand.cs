using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CustomUtils;
using Leap;
using Leap.Unity;
using System;

namespace TossCraft
{
	[RequireComponent (typeof(Counter))]
	public class BehaviorHand : MonoBehaviour
	{

		GestureManager _gestureManager;
		bool _isBlock;

		public void UnBlockGesture ()
		{
			_isBlock = false;
		}

		protected GestureManager.GestureTypes _currentType;

		protected Counter _counterLoading;
		List<Hand> _listHands;

		protected Hand GetCurrent1Hand ()
		{
			if (_listHands.Count == 1)
				return _listHands [0];
			else
				return null;
		}

		protected List<Hand> GetCurrent2Hands ()
		{
			if (_listHands.Count == 2)
				return _listHands;
			else
				return null;
		}
			
		[Tooltip ("Delta degree to check 2 vectors same direction")]
		protected float handForwardDegree = 30;

		[Tooltip ("Grab hand strength, one for close hand(grab)")]
		protected float gradStrength = 0.9f;

		[Tooltip ("Velocity (m/s) move toward ")]
		protected float smallestVelocity = 0.4f;

		[Tooltip ("Velocity (m/s) move toward ")]
		protected float deltaVelocity = 0.7f;

		[Tooltip ("Angle when rotation vector is registered to change")]
		protected float angleChangeRot = 4;

		[Tooltip ("Opposite direction 2 vectors")]
		protected  float diffAngle2Hands = 130;

		[Tooltip ("Velocity opposite ")]
		protected  float diffAngle2Velocity = 150;

		[Tooltip ("Time (secs) during the user behavior checker")]
		public float CheckingTimeBeforeToggle = 1.5f;

		void Awake ()
		{
			_counterLoading = GetComponent<Counter> ();
			_isBlock = false;
		}
		protected void FixedUpdate ()
		{
			updateHands ();
			updateDebug ();
		}

		public void Init (GestureManager manager)
		{
			_gestureManager = manager;
		}

		void updateHands ()
		{
			Frame frame = _gestureManager.GetLeapHand ().CurrentFrame;
			_listHands = frame.Hands;
			if (!_isBlock) {
				if (_listHands.Count > 0) {
					if (checkConditionGesture ()) {
						if (_counterLoading.CurrentState == Counter.CounterState.STOP) {
							_counterLoading.StartTimerUpdatePercentage (CheckingTimeBeforeToggle, () => {
								callEvent ();
							}, (float percent) => {
								if (CheckingTimeBeforeToggle != 0)
									_gestureManager.LoadingGestureProgress (_currentType, percent);
							});
						}
					} else {
						_counterLoading.StopTimer ();
						_gestureManager.LoadingGestureProgress (_currentType, 0);
					}
				}
			}
		}
			
		protected virtual bool checkConditionGesture ()
		{
			return false;
		}

		protected Action specificEvent;

		protected void callEvent ()
		{
			bool eventSuccess = _gestureManager.ReceiveEvent (_currentType);
			if (eventSuccess) {
				_isBlock = true;
				if (specificEvent != null)
					specificEvent ();
			}
		}

		protected bool isMoveLeft (Hand hand)
		{
			return hand.PalmVelocity.x < -deltaVelocity && !isStationary (hand);
		}

		protected bool isMoveRight (Hand hand)
		{
			return hand.PalmVelocity.x > deltaVelocity && !isStationary (hand);
		}

		protected bool isMoveUp (Hand hand)
		{
			return hand.PalmVelocity.y > deltaVelocity && !isStationary (hand);
		}

		protected bool isMoveDown (Hand hand)
		{
			return hand.PalmVelocity.y < -deltaVelocity && !isStationary (hand);
		}

		protected bool isStationary (Hand hand)
		{
			return hand.PalmVelocity.Magnitude < smallestVelocity;
		}

		protected bool isHandConfidence (Hand hand)
		{
			return hand.Confidence > 0.5f;
		}

		protected bool isGrabHand (Hand hand)
		{
			return hand.GrabStrength > 0.8f;
		}

		protected bool isCloseHand (Hand hand)
		{
			List<Finger> listOfFingers = hand.Fingers;
			int count = 0;
			for (int f = 0; f < listOfFingers.Count; f++) {
				Finger finger = listOfFingers [f];
				if ((finger.TipPosition - hand.PalmPosition).Magnitude < deltaCloseFinger) {
					count++;
//				if (finger.Type == Finger.FingerType.TYPE_THUMB)
//					Debug.Log ((finger.TipPosition - hand.PalmPosition).Magnitude);
				}
			}
			return (count == 5);
		}

		protected bool isOpenFullHand (Hand hand)
		{
			//Debug.Log (hand.GrabStrength + " " + hand.PalmVelocity + " " + hand.PalmVelocity.Magnitude);
			return hand.GrabStrength == 0;
		}

		protected bool isPalmNormalSameDirectionWith (Hand hand, Vector3 dir)
		{
			return isSameDirection (hand.PalmNormal, UnityVectorExtension.ToVector (dir));
		}

		protected bool isHandMoveForward (Hand hand)
		{
			return isSameDirection (hand.PalmNormal, hand.PalmVelocity) && !isStationary (hand);
		}

		float deltaAngleThumb = 30;
		// degree
		float deltaCloseFinger = 0.05f;

		protected bool checkPalmNormalInXZPlane (Hand hand)
		{
			float anglePalmNormal = angle2LeapVectors (hand.PalmNormal, UnityVectorExtension.ToVector (Vector3.up));

			return (anglePalmNormal > 70 && anglePalmNormal < 110);
		}

		// check thumb finger up/down
		protected bool isThumbDirection (Hand hand, Vector3 dir)
		{
			List<Finger> listOfFingers = hand.Fingers;
			for (int f = 0; f < listOfFingers.Count; f++) {
				Finger finger = listOfFingers [f];

				if (finger.Type == Finger.FingerType.TYPE_THUMB) {
					float angleThumbFinger = angle2LeapVectors (finger.Direction, 
						                        UnityVectorExtension.ToVector (dir));
					float angleThumbFinger2 = angle2LeapVectors (
						                         finger.StabilizedTipPosition - hand.PalmPosition, UnityVectorExtension.ToVector (dir));
					//Debug.Log (angleThumbFinger + " " + angleThumbFinger2);
					if (angleThumbFinger < deltaAngleThumb
					   || angleThumbFinger2 < deltaAngleThumb)
						return true;
					else
						return false;
				} 
			}
			return false;
		}

		// check 4 fingers tip close to palm position
		protected bool checkFingerCloseToHand (Hand hand)
		{
			List<Finger> listOfFingers = hand.Fingers;
			int count = 0;
			for (int f = 0; f < listOfFingers.Count; f++) {
				Finger finger = listOfFingers [f];
				if ((finger.TipPosition - hand.PalmPosition).Magnitude < deltaCloseFinger) {
					if (finger.Type == Finger.FingerType.TYPE_THUMB) {
						return false;
					} else {
						count++;
					}
				}
			}
			//Debug.Log (count);
			return (count == 4);
		}

		protected bool isOppositeDirection (Vector a, Vector b)
		{
			return angle2LeapVectors (a, b) > (180 - handForwardDegree);
		}

		protected bool isSameDirection (Vector a, Vector b)
		{
			//Debug.Log (angle2LeapVectors (a, b) + " " + b);
			return angle2LeapVectors (a, b) < handForwardDegree;
		}

		protected float angle2LeapVectors (Leap.Vector a, Leap.Vector  b)
		{
			return Vector3.Angle (UnityVectorExtension.ToVector3 (a), UnityVectorExtension.ToVector3 (b));
		}

		void updateDebug ()
		{
			if (_listHands.Count > 0 && _listHands.Count <= 2) {
				foreach (Hand hand in _listHands) {
					Debug.DrawRay (UnityVectorExtension.ToVector3 (hand.PalmPosition), UnityVectorExtension.ToVector3 (hand.PalmNormal) * 10, Color.green);
					if (!isStationary (hand)) {
						Debug.DrawRay (UnityVectorExtension.ToVector3 (hand.PalmPosition), UnityVectorExtension.ToVector3 (hand.PalmVelocity) * 10, Color.blue);

					}
				}
			}
		}
	}
}