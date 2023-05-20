using UnityEngine;

namespace JZ.Common
{
    /// <summary>
    /// Some helpful math functions
    /// </summary>
    public static class JZMath
    {
        /// <summary>
        /// Does a quadratic interpolation of three points
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="t"></param>
        /// <returns>The Interpolated position</returns>
        public static Vector3 QuadInterp(Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            Vector3 ab = Vector3.Lerp(p1, p2, t);
            Vector3 bc = Vector3.Lerp(p2, p3, t);
            return Vector3.Lerp(ab, bc, t);
        }

        /// <summary>
        /// Does a cubic interpolation of four points
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vector3 CubicInterp(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, float t)
        {
            Vector3 abc = QuadInterp(p1, p2, p3, t);
            Vector3 bcd = QuadInterp(p2, p3, p4, t);
            return Vector3.Lerp(abc, bcd, t);
        }
    }

}