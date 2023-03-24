using System;
using UnityEngine;

namespace Matryoshka.Utils
{
    public class Utils : System.Object
    {

        public static Vector2 NormalizeToOne(Vector2 vector)
        {
        
            float x = Math.Abs(vector.x);
            float y = Math.Abs(vector.y);
            float max = x;
            if(y > max)
            {
                max = y;
            }
            return new Vector2(vector.x / max, vector.y / max);
        }

        public static Vector2 ConvertToOne(Vector2 vector)
        {
            return new Vector2(ConvertFloatToOne(vector.x), ConvertFloatToOne(vector.y));
        }

        public static Vector2 ConvertToEight(Vector2 vector)
        {
            //Vector2 v = NormalizeToOne(vector);
            return new Vector2(MathF.Round(vector.x, 0), MathF.Round(vector.y, 0));
        }

        public static float ConvertFloatToOne(float value)
        {
            if(value > 0f)
            {
                return 1f;
            }
            else if(value < 0f)
            {
                return -1f;
            }
            return 0f;
        }

        public static Quaternion CalculateRotationFromVector(Vector2 vector, float rotationOffset=90f)
        {
            return Quaternion.Euler(0f, 0f, (Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg - rotationOffset));
        }

        public static Vector3 VectorDifference(Vector3 vector1, Vector3 vector2)
        {
            return new Vector3(vector1.x - vector2.x, vector1.y - vector2.y, vector1.z - vector2.z);
        }

        public static Vector2 NudgeVector2(Vector2 vector)
        {
            float xShift = (UnityEngine.Random.value - 0.5f) / 3f;
            float yShift = (UnityEngine.Random.value - 0.5f) / 3f;
            Vector2 newVector = NormalizeToOne(new Vector2(vector.x + xShift, vector.y + yShift));
            return newVector;
        }

        public static Vector2 GetClawSwipeOffset(Vector2 direction)
        {
            if (direction == Vector2.down)
            {
                return new Vector2(0f, -2f);
            }
            else if (direction == Vector2.left)
            {
                return new Vector2(-4f, .75f);
            }
            else if (direction == Vector2.up)
            {
                return new Vector2(0f, 8f); // might be wrong as we don't use this direction
            }
            else if (direction == Vector2.right)
            {
                return new Vector2(4f, .75f);
            }
            return new Vector2(0f, 0f);
        }

        public static float VectorDistance(Vector3 vector1, Vector3 vector2)
        {
            var diff = VectorDifference(vector1, vector2);
            return (float)Math.Sqrt(diff.x*diff.x + diff.y*diff.y + diff.z*diff.z);
        }

    }
}
