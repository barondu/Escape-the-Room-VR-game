using UnityEngine;

namespace ThunderWire.Helper.Input
{
    public enum Axis { Forward, Backward, Left, Right }

    /// <summary>
    /// Provides additional methods for Input
    /// </summary>
    public static class InputHelper
    {

        /// <summary>
        /// Linearly interpolates Keyboard Button
        /// </summary>
        public static float GetKeyAxis(Axis axis, float from, bool lerpIn, float speed)
        {
            switch (axis)
            {
                case Axis.Forward:
                    if (lerpIn)
                    {
                        if (from < 0.9f)
                        {
                            return from += Time.deltaTime * speed;
                        }
                        else
                        {
                            return 1;
                        }
                    }
                    else
                    {
                        if (from > 0.1f)
                        {
                            return from -= Time.deltaTime * speed;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                case Axis.Backward:
                    if (lerpIn)
                    {
                        if (from > -0.9f)
                        {
                            return from -= Time.deltaTime * speed;
                        }
                        else
                        {
                            return -1;
                        }
                    }
                    else
                    {
                        if (from < -0.1f)
                        {
                            return from += Time.deltaTime * speed;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                case Axis.Left:
                    if (lerpIn)
                    {
                        if (from > -0.9f)
                        {
                            return from -= Time.deltaTime * speed;
                        }
                        else
                        {
                            return -1;
                        }
                    }
                    else
                    {
                        if (from < -0.1f)
                        {
                            return from += Time.deltaTime * speed;
                        }
                        else
                        {
                            return 0;
                        }
                    }
                case Axis.Right:
                    if (lerpIn)
                    {
                        if (from < 0.9f)
                        {
                            return from += Time.deltaTime * speed;
                        }
                        else
                        {
                            return 1;
                        }
                    }
                    else
                    {
                        if (from > 0.1f)
                        {
                            return from -= Time.deltaTime * speed;
                        }
                        else
                        {
                            return 0;
                        }
                    }
            }

            return 0;
        }
    }
}
