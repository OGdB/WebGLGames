using System;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    public ParallaxLayer[] parallaxLayers;
    public float smoothing = 1f;

    private float loopThreshold = 960;

    private Transform cam;
    private Vector3 previousCamPos;

    private void Awake() => cam = Camera.main.transform;

    private void Start()
    {
        previousCamPos = cam.position;

        // Calculate the width between the left and right borders at the desired depth
        float height = 2f * Camera.main.orthographicSize;
        loopThreshold = height * Camera.main.aspect / 2f;
    }


    private void LateUpdate()
    {
        for (int i = 0; i < parallaxLayers.Length; i++)
        {
            ParallaxLayer layer = parallaxLayers[i];

            float parallaxX = (previousCamPos.x - cam.position.x) * layer.speed;

            float targetPosX = layer.transform.position.x + parallaxX;

            Vector3 targetPos = new(targetPosX, layer.transform.position.y, layer.transform.position.z);

            layer.transform.position = Vector3.Lerp(layer.transform.position, targetPos, smoothing * Time.deltaTime);

            // Looping logic
            // If loopthreshold is passed, loop positions.
            if (Mathf.Abs(layer.transform.position.x) > loopThreshold)
            {
                layer.transform.position = new(loopThreshold * -Mathf.Sign(layer.transform.position.x), layer.transform.position.y, layer.transform.position.z);
            }
        }

        previousCamPos = cam.position;
    }

    [Serializable]
    public class ParallaxLayer
    {
        public Transform transform;
        public float speed;
    }
}
