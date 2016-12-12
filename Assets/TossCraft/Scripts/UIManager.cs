using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using CustomUtils;
using System;

namespace TossCraft
{
	[RequireComponent (typeof(Counter))]
	public class UIManager : MonoBehaviour
	{
		public Text CurrentGestureText;
		public Text TryCountText;
		public Transform ListGesturePivot;
		public GameObject prefabSliderUI;

		GameManager gameManager;
		Counter countDownSlider;

		int tryCount = 0;

		Dictionary<GestureManager.GestureTypes, Slider> listSliders;

		GameManager.EndEvent endEventCountDown;

		void Start ()
		{
			countDownSlider = GetComponent<Counter> ();
		}
	
		void Update ()
		{

		}
			
		public bool IsReady ()
		{
			return countDownSlider.CurrentState == Counter.CounterState.STOP;
		}

		public void RegisterEventEndCountDown (GameManager.EndEvent end)
		{
			endEventCountDown = end;
		}

		public void InitUI (GameManager manager)
		{
			gameManager = manager;

			Dictionary<GestureManager.GestureTypes, object> listActiveGestures = gameManager.GetCurrentActiveGestures ();
			listSliders = new Dictionary<GestureManager.GestureTypes, Slider> ();
			foreach (KeyValuePair<GestureManager.GestureTypes, object> gesture in listActiveGestures) {
				GameObject go = GameObject.Instantiate (prefabSliderUI);
				go.transform.SetParent (ListGesturePivot);
				go.transform.localScale = Vector3.one;
				go.transform.localPosition = Vector3.zero;
				go.name = gesture.Key.ToString ();
				go.GetComponentInChildren<Text> ().text = go.name;

				listSliders.Add (gesture.Key, go.GetComponentInChildren<Slider> ());
			}
		}

		public void UpdateTimerLoadingGesture (GestureManager.GestureTypes type, float percent)
		{
			Slider currentSlider = GetSliderBasedType (type);
			currentSlider.image.color = Color.green;
			currentSlider.value = percent;
		}

		public void UpdateSliderBlockingGesture (GestureManager.GestureTypes type, float timer)
		{
			Slider currentSlider = GetSliderBasedType (type);

			countDownSlider.StartTimerUpdatePercentage (timer, () => {
				currentSlider.value = 0;
				currentSlider.image.color = Color.green;

				if (endEventCountDown != null)
					endEventCountDown (type);
			
			}, (float percent) => {
				currentSlider.image.color = Color.red;
				currentSlider.value = Mathf.Clamp01 (1 - percent);
			});

			CurrentGestureText.text = type.ToString ();
			tryCount++;
			TryCountText.text = "Tries: " + tryCount;
			Debug.Log ("Change Gesture: " + type.ToString ());
		}
			
		Slider GetSliderBasedType (GestureManager.GestureTypes type)
		{
			if (listSliders.ContainsKey (type)) {
				return listSliders [type];
			}
			return listSliders [0];
		}
	}
}