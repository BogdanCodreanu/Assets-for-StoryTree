namespace Game.SceneObjects {
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Sirenix.OdinInspector;

    [RequireComponent(typeof(Light))]
    public class LightFlicker : MonoBehaviour {

        private new Light light;
        private Vector3 initialPosition;

        [SerializeField]
        private float overallLength = 1f;

        [PropertyTooltip("Multiplying each component of the vector 3 Time.time * thisValue")]
        [SerializeField, MinMaxSlider(0, 100)]
        private Vector2[] randomTimes = new Vector2[3];


        private float[] randomGeneratedTimes = new float[3];

        [SerializeField, MinValue(0)]
        private float lightIntensityX, lightIntensityY, intensityChangeSpeed;


        private void Awake() {
            light = GetComponent<Light>();
            initialPosition = transform.position;
            GenerateRandomTimes();
        }

        private void Update() {
            MovePosition();
            LightIntensity();
        }

        private void MovePosition() {
            transform.position = initialPosition + new Vector3(
                Mathf.Sin(Time.time * randomGeneratedTimes[0]),
                Mathf.Cos(Time.time * randomGeneratedTimes[1]),
                Mathf.Sin(Time.time * randomGeneratedTimes[2]))
                * overallLength;
        }

        [Button]
        private void GenerateRandomTimes() {
            for (int i = 0; i < 3; i++) {
                randomGeneratedTimes[i] =
                    Random.Range(randomTimes[i].x, randomTimes[i].y);
            }
        }

        private void LightIntensity() {
            light.intensity = Mathf.Lerp(lightIntensityX, lightIntensityY,
                Mathf.Cos(Time.time * intensityChangeSpeed));
        }
    }
}