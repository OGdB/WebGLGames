using System;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField]
    private ParallaxLayer[] parallaxLayers;
    [SerializeField]
    private float smoothing = 1f;

    [SerializeField]
    private Transform cam;
    private Vector3 previousCamPos;
    private float loopThreshold = 960;

    private void Start() => previousCamPos = cam.position;


    private void LateUpdate()
    {
        for (int i = 0; i < parallaxLayers.Length; i++)
        {
            ParallaxLayer layer = parallaxLayers[i];
            // Move the layer in the direction the player camera moves in.
            // Get the values
            float parallaxX = (previousCamPos.x - cam.position.x) * layer.speed;
            float targetPosX = layer.transform.anchoredPosition.x + parallaxX;
            Vector3 targetPos = new(targetPosX, layer.transform.anchoredPosition.y);

            // Apply with lerp.
            layer.transform.anchoredPosition = Vector3.Lerp(layer.transform.anchoredPosition, targetPos, smoothing * Time.deltaTime);

            // Looping logic
            // If loopthreshold is passed, loop positions.
            if (Mathf.Abs(layer.transform.anchoredPosition.x) > loopThreshold)
            {
                // Set the x position of the layer to 
                Vector3 loopPos = new(loopThreshold * -Mathf.Sign(layer.transform.anchoredPosition.x), layer.transform.anchoredPosition.y);
                layer.transform.anchoredPosition = loopPos;
            }
        }

        previousCamPos = cam.position;
    }

    [Serializable]
    public class ParallaxLayer
    {
        public RectTransform transform;
        public float speed;
    }
}
