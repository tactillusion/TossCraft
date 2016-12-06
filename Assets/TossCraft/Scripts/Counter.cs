using UnityEngine;
using System.Collections;

namespace CustomUtils {
	
	public class Counter : MonoBehaviour {

		public enum CounterState {
			RUN,
			STOP,
			PAUSE
		}

		public CounterState CurrentState {
			get {
				return currentState;
			}
		}


		public delegate void EndTimer();

		public delegate void EndEverySeconds(int secs);

		public delegate void UpdatingPercentage(float percent);

		CounterState currentState = CounterState.STOP;
		float step;
		float maxTimer;
		float timer;
		float preTimer;

		EndTimer endTimerFunction;
		EndEverySeconds endEverySeconds;
		UpdatingPercentage updating;

		void Start() {}
			
		void FixedUpdate() {

			switch (CurrentState) {
			case CounterState.RUN:

				timer -= Time.fixedDeltaTime * step;

				if (updating != null) {
					updating(1 - timer * 1.0f / maxTimer);
				}

				if (Mathf.Abs(timer - preTimer) >= 1) {
					preTimer = timer;
					if (endEverySeconds != null) {
						endEverySeconds(Mathf.RoundToInt(preTimer));
					}
				}

				if (timer < 0) {
					timer = maxTimer;
					currentState = CounterState.STOP;

					if (endTimerFunction != null) {
						endTimerFunction();
					}
				}
				break;
			case CounterState.PAUSE:
				break;
			case CounterState.STOP:
				preTimer = timer = maxTimer;
				break;
			}
		}

		public void StartTimer(float _maxTimer, EndTimer endFunc) {

			step = 1;
			timer = maxTimer = _maxTimer;
			endTimerFunction = endFunc;
			endEverySeconds = null;
			updating = null;
			currentState = CounterState.RUN;

		}

		public void StartTimerUpdateSeconds(float _maxTimer, EndTimer endFunc, EndEverySeconds endSecs = null) {
			step = 1;
			timer = maxTimer = _maxTimer;
			endTimerFunction = endFunc;
			endEverySeconds = endSecs;
			updating = null;
			currentState = CounterState.RUN;

		}

		public void StartTimerUpdatePercentage(float _maxTimer, EndTimer endFunc, UpdatingPercentage updatingFunc = null) {

			step = 1;
			timer = maxTimer = _maxTimer;
			endTimerFunction = endFunc;
			endEverySeconds = null;
			updating = updatingFunc;
			currentState = CounterState.RUN;

		}

		public void StopTimer() {
			currentState = CounterState.STOP;
			endTimerFunction = null;
			endEverySeconds = null;
			//      Debug.Log ("stop timer ");

		}

		public void PauseTimer() {
			if (currentState == CounterState.RUN) {
				currentState = CounterState.PAUSE;
				//  Debug.Log("Pause");
			}
		}

		public void ContinueTimer() {
			if (currentState == CounterState.PAUSE) {
				currentState = CounterState.RUN;
				//  Debug.Log("Cont");

			}
		}
	}
}