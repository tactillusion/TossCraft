using UnityEngine;
using System.Collections;
using Leap;
using Leap.Unity;

namespace TossCraft {

	//TODO: The gesture is not working smoothly enough ^^

	public class Grab : Gesture {
	
		public GameObject CurrentHoldingObj;

		Vector3 savedBallPosition;
		bool isHoldingBall = false;

		void Start () {
			currentType = GestureManager.GestureTypes.Grab;
			specificEvent = grabBall;
			savedBallPosition = CurrentHoldingObj.transform.position;
		}
	
		void Update () {}

		protected new void FixedUpdate () {
			base.FixedUpdate ();
			updateBall ();
		}

		protected override bool checkConditionGesture () {
			Hand hand = GetCurrentHand ();
			if (hand != null) {
				if (isGrabHand (hand) && !isHoldingBall)
					return true;
			}

			return false;
		}


		void grabBall () {
			//Debug.Log ("Grab");
			isHoldingBall = true;
		}

		void releaseBall () {
			//Debug.Log ("Release");
			isHoldingBall = false;
			CurrentHoldingObj.transform.position = savedBallPosition;
		}

		void updateBall () {
			bool isUpdating = false;
			if (isHoldingBall) {
				Hand hand = GetCurrentHand ();
				if (hand != null) {
					if (isGrabHand (hand)) {
						if (CurrentHoldingObj != null) {
							CurrentHoldingObj.transform.position = UnityVectorExtension.ToVector3 (hand.PalmPosition + hand.PalmNormal.Normalized * 0.03f);
							isUpdating = true;
						}
					}
				}

				if (!isUpdating)
					releaseBall ();
			}
		}
	}
}