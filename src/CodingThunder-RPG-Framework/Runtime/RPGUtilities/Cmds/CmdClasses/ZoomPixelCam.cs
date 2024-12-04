using CodingThunder.RPGUtilities.DataManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.U2D;
using CodingThunder.RPGUtilities.Utilities;

namespace CodingThunder.RPGUtilities.Cmds
{
    public class ZoomPixelCam : ICmd
    {
        public string ID { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
        public object ReturnValue { get; set; }
        public bool Suspended { get; set; }

        public int? X { get; set; }
        public int? Y { get; set; }
        public float? Dur { get; set; }

        public string Rate { get; set; }

        public IEnumerator ExecuteCmd(Action<ICmd> completionCallback)
        {
            if (X == null)
            {
                X = new RPGRef<int>() { ReferenceId = Parameters["X"] };
            }
            if (Y == null)
            {
                Y = new RPGRef<int>() { ReferenceId = Parameters["Y"] };
            }
            if (Dur == null)
            {
                Dur = new RPGRef<float>() { ReferenceId = Parameters["Dur"] };
            }
            if (Rate == null)
            {
                if (Parameters.ContainsKey("Rate"))
                {
                    Rate = Parameters["Rate"];
                }
                else
                {
                    Rate = "";
                }
            }

            var pixelCamera = Camera.main.GetComponent<PixelPerfectCamera>();
            var mainCamera = Camera.main;

            if (pixelCamera == null || mainCamera == null)
            {
                Debug.LogError("PixelPerfectCamera or Main Camera not found!");
                completionCallback.Invoke(this);
                yield break;
            }

            // Disable Pixel Perfect Camera
            pixelCamera.enabled = false;
            int ppu = pixelCamera.assetsPPU;

            // Calculate the initial and final orthographic sizes
            float initialOrthoSize = mainCamera.orthographicSize;
            float finalOrthoSize = (float)Y.Value / ppu / 2.0f; // Assuming vertical resolution determines orthographic size
            float timePast = 0f;

            // Lerp the orthographic size
            while (!Mathf.Approximately(mainCamera.orthographicSize, finalOrthoSize))
            {
                yield return null;

                if (Suspended)
                {
                    continue;
                }

                timePast += Time.deltaTime;

                float t = LerpDerp.Transform(timePast / Dur.Value, Rate);
                mainCamera.orthographicSize = Mathf.Lerp(initialOrthoSize, finalOrthoSize, t);
            }

            // Update the Pixel Perfect Camera reference resolution
            pixelCamera.refResolutionX = X.Value;
            pixelCamera.refResolutionY = Y.Value;

            // Re-enable Pixel Perfect Camera
            pixelCamera.enabled = true;

            completionCallback.Invoke(this);
        }
    }
}
