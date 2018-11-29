
using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace Valve.VR.InteractionSystem
{
    public class FlashlightContorl : Throwable
    {
        [Tooltip("The local point which acts as a positional and rotational offset to use while held with a grip type grab")]
        public Transform gripOffset;

        [Tooltip("The local point which acts as a positional and rotational offset to use while held with a pinch type grab")]
        public Transform pinchOffset;

        protected override void HandHoverUpdate(Hand hand)
        {
            GameObject flashlight = GameObject.Find("Light_flashlight");
            GrabTypes startingGrabType = hand.GetGrabStarting();

            if (startingGrabType != GrabTypes.None)
            {
                if (startingGrabType == GrabTypes.Pinch)
                {
                    flashlight.GetComponent<Light>().enabled = true;
                    hand.AttachObject(gameObject, startingGrabType, attachmentFlags, pinchOffset);
                }
                //else if (startingGrabType == GrabTypes.Grip)
                //{
                 //   hand.AttachObject(gameObject, startingGrabType, attachmentFlags, gripOffset);
                //}
               // else
                //{
                //    hand.AttachObject(gameObject, startingGrabType, attachmentFlags, attachmentOffset);
               // }
                hand.HideGrabHint();

            }
        }

        protected override void OnDetachedFromHand(Hand hand)
        {
            GameObject flashlight = GameObject.Find("Light_flashlight");
            flashlight.GetComponent<Light>().enabled = false;
            attached = false;

            onDetachFromHand.Invoke();

            hand.HoverUnlock(null);

            rigidbody.interpolation = hadInterpolation;

            Vector3 velocity;
            Vector3 angularVelocity;

            GetReleaseVelocities(hand, out velocity, out angularVelocity);

            rigidbody.velocity = velocity;
            rigidbody.angularVelocity = angularVelocity;
        }

    }
}