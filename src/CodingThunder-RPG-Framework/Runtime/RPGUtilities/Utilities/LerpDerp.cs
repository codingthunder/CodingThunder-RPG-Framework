using System;
using System.Collections.Generic;

namespace CodingThunder.RPGUtilities.Utilities
{
    /// <summary>
    /// Use this to transform a number between 0 and 1 (inclusive) via a mathematical function. 0 = 0, 1 = 1, everything in between is up in the air.
    /// </summary>
    public static class LerpDerp
    {
        private static readonly Dictionary<string, Func<float, float>> ops = new()
        {
            {"SQUEEZE", Squeeze },
            {"SQUARE", Square },
            { "CUBE", Cube },
            {"SQRT", Sqrt },
            {"CBRT", Cbrt },
            {"COS", Cos },
            {"LINE", Linear }
        };

        public static float Transform(float num, string op)
        {
            if (ops.ContainsKey(op.ToUpper()))
            {
                return ops[op.ToUpper()](num);
            }
            return Linear(num);
        }

        private static float Linear(float num) { return num; }

        private static float Squeeze(float num)
        {
            var op1 = 2 * num + 1;
            var cuberoot = Math.Pow(op1, 1.0 / 3.0);
            return (float) ((cuberoot + 1.0) / 2.0);
        }

        private static float Square(float num)
        {
            return num * num;
        }

        private static float Cube(float num)
        {
            return num * num * num;
        }

        private static float Sqrt(float num)
        {
            return (float)Math.Sqrt(num);
        }

        private static float Cbrt(float num)
        {
            return (float)Math.Cbrt(num);
        }

        private static float Cos(float num)
        {
            return (float)((1 + Math.Cos(Math.PI * num + Math.PI))/2.0);
        }
    }
}