
using UnityEngine;
using System.Collections;
using Leap;
using Leap.Unity;

namespace TossCraft {

	public class Throw : Gesture {
		
		public GameObject prefabBall;
		public float forceToAdd = 100;
		public Vector delta = new Vector (0, .1f, 0);

		void Start () {
			currentType = GestureManager.GestureTypes.Throw;
			specificEvent = throwBall;
		}
	
		void Update () {}

		/// <summary>
		/// Checks whether the position of the palm and its movement are pointing 
		/// towards the same direction. If this is the case, the gesture is recognized
		/// as a throwing gesture.
		/// </summary>
		/// <returns><c>true</c>, if condition gesture was checked, <c>false</c> otherwise.</returns>
		protected override bool checkConditionGesture () {
			Hand hand = GetCurrentHand ();
			if (hand != null) {
				if (isPalmNormalSameDirectionWith (hand, UnityVectorExtension.ToVector3 (hand.PalmVelocity))
					&& !isStationary (hand)) {
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Performs a throw by changing the gameobject's position and velocity depending on the
		/// position and velocity of the player's palm.
		/// </summary>
		void throwBall () {
			Hand hand = GetCurrentHand ();
			if (hand != null) {
				GameObject go = GameObject.Instantiate (prefabBall);
				go.transform.position = UnityVectorExtension.ToVector3 (hand.PalmPosition);
				setupGravity (go);
				addForce (go, UnityVectorExtension.ToVector3 ((hand.PalmVelocity) * forceToAdd));
			}
		}
			
		void setupGravity (GameObject go) {
			go.GetComponent<Rigidbody> ().useGravity = true;
		}


		void addForce (GameObject go, Vector3 force) {
			go.GetComponent<Rigidbody> ().AddForce (force);
		}
	}

}