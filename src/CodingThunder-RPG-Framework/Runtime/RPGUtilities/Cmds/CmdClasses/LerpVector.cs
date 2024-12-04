using CodingThunder.RPGUtilities.DataManagement;
using CodingThunder.RPGUtilities.Utilities;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace CodingThunder.RPGUtilities.Cmds
{
    /// <summary>
    /// X, Y, Z, W, Dur, work as RPGRefs.
    /// Only vars you plan to change are necessary. The rest can be ignored.
    /// Should work for Vector2, Vector3, and Vector4.
    /// Rate is just a string, not an RPGRef. Only a certain number of possible options. See LerpDerp.
    /// </summary>
    public class LerpVector : SetVar, ICmd
    {
        public override IEnumerator ExecuteCmd(Action<ICmd> OnFinishCallback)
        {
            while (Suspended)
            {
                yield return null;
            }

            var targetString = Parameters["Target"];
            float dur = new RPGRef<float>() { ReferenceId = Parameters["Dur"] };
            float? x = null;
            float? y = null;
            float? z = null;
            float? w = null;

            if (Parameters.ContainsKey("X"))
            {
               x = new RPGRef<float>() { ReferenceId = Parameters["X"] };
            }

            if (Parameters.ContainsKey("Y"))
            {
                y = new RPGRef<float>() { ReferenceId = Parameters["Y"] };
            }

            if (Parameters.ContainsKey("Z"))
            {
                z = new RPGRef<float>() { ReferenceId = Parameters["Z"] };
            }

            if (Parameters.ContainsKey("W"))
            {
                w = new RPGRef<float>() { ReferenceId = Parameters["W"] };
            }


            string rate = "";
            //Rate is not an RPGRef.
            if (Parameters.ContainsKey("Rate"))
            {
                rate = Parameters["Rate"];
            }

            ISetVarMetadata setVarMetadata = null;

            try
            {
                setVarMetadata = BuildVarMetadata(targetString);
            }
            catch (ArgumentException)
            {
                Debug.LogError($"Failed to find target variable's parent in {targetString}.");
                OnFinishCallback.Invoke(this);
                yield break;
            }
            catch (MemberAccessException)
            {
                Debug.LogError($"Found parent, but couldn't find field or property from {targetString}");
                OnFinishCallback.Invoke(this);
                yield break;
            }

            object val = setVarMetadata.GetValue();

            Type vectorType = val.GetType();

            char vectorTypeNum = vectorType.Name.Last();



            Vector4 targetVector4 = Vector4.zero;

            Vector4 valAsVector4 = Vector4.zero;
            Vector3 valAsVector3 = Vector3.zero;
            Vector2 valAsVector2 = Vector2.zero;

            switch (vectorTypeNum)
            {
                case '4':
                    valAsVector4 = (Vector4)val;
                    targetVector4.w = w == null ? valAsVector4.w : w.Value;
                    targetVector4.z = z == null ? valAsVector4.z : z.Value;
                    targetVector4.y = y == null ? valAsVector4.y : y.Value;
                    targetVector4.x = x == null ? valAsVector4.x : x.Value;
                    break;
                case '3':
                    valAsVector3 = (Vector3)val;
                    targetVector4.z = z == null ? valAsVector3.z : z.Value;
                    targetVector4.y = y == null ? valAsVector3.y : y.Value;
                    targetVector4.x = x == null ? valAsVector3.x : x.Value;
                    break;
                case '2':
                    valAsVector4 = (Vector2)val;
                    targetVector4.y = y == null ? valAsVector2.y : y.Value;
                    targetVector4.x = x == null ? valAsVector2.x : x.Value;
                    break;
                default:
                    Debug.LogError("Vector format not supported: " + vectorType.Name);
                    OnFinishCallback.Invoke(this);
                    yield break;
            }



            if (dur == 0f)
            {
                switch (vectorTypeNum)
                {
                    case '2':
                        setVarMetadata.SetValue((Vector2)targetVector4);
                        break;
                    case '3':
                        setVarMetadata.SetValue((Vector3)targetVector4);
                        break;
                    case '4':
                        setVarMetadata.SetValue(targetVector4);
                        break;
                }
                OnFinishCallback.Invoke(this);
                yield break;
            }

            float timePast = 0f;
            while (timePast <= dur)
            {

                switch (vectorTypeNum)
                {
                    case '2':
                        Vector2 vector2Lerp = Vector2.Lerp(valAsVector2, (Vector2) targetVector4, LerpDerp.Transform(timePast / dur, rate));
                        setVarMetadata.SetValue(vector2Lerp);
                        break;
                    case '3':
                        Vector3 vector3Lerp = Vector3.Lerp(valAsVector3, (Vector3)targetVector4, LerpDerp.Transform(timePast / dur, rate));
                        setVarMetadata.SetValue(vector3Lerp);
                        break;
                    case '4':
                        Vector4 vector4Lerp = Vector4.Lerp(valAsVector4, targetVector4, LerpDerp.Transform(timePast / dur, rate));
                        setVarMetadata.SetValue(vector4Lerp);
                        break;
                }

                //Vector2 vector2Lerp = Vector2.Lerp(initialVector, finalVector, LerpDerp.Transform(timePast / dur, rate));
                ////float lerpValue = Mathf.Lerp(initialValue, value, timePast / dur);
                ////initialVector.a = lerpValue;
                //setVarMetadata.SetValue(vector2Lerp);

                yield return null;
                if (Suspended)
                {
                    continue;
                }
                timePast += Time.deltaTime;
            }

            OnFinishCallback.Invoke(this);
            yield break;
        }
    }
}
