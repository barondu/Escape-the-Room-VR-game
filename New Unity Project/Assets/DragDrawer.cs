//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: Drives a linear mapping based on position between 2 positions
//
//=============================================================================

using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using Valve.VR;




namespace Valve.VR.InteractionSystem
{
    //-------------------------------------------------------------------------
    [RequireComponent(typeof(Interactable))]
    public class DragDrawer : MonoBehaviour
    {
        public Transform movingPart;

        public Vector3 localMoveDistance = new Vector3(0, 0, 0);

        [Range(0, 1)]
        public float engageAtPercent = 0.95f;

        [Range(0, 1)]
        public float disengageAtPercent = 0.9f;

        public HandEvent onButtonDown;
        public HandEvent onButtonUp;
        public HandEvent onButtonIsPressed;

        public bool engaged = false;
        public bool buttonDown = false;
        public bool buttonUp = false;

        private Vector3 startPosition;
        private Vector3 endPosition;

        private Vector3 handEnteredPosition;

        private bool hovering;

        private Hand lastHoveredHand;
        protected Interactable interactable;

        private bool isattatch = false;

        protected Hand.AttachmentFlags attachmentFlags = Hand.AttachmentFlags.DetachFromOtherHand;


        protected virtual void Awake()
        {
            interactable = GetComponent<Interactable>();
        }

        private void Start()
        {
            if (movingPart == null && this.transform.childCount > 0)
                movingPart = this.transform.GetChild(0);

            startPosition = movingPart.localPosition;
            endPosition = startPosition + localMoveDistance;
            handEnteredPosition = endPosition;
        }

        private void HandHoverUpdate(Hand hand)
        {

            GrabTypes startingGrabType = hand.GetGrabStarting();

            if (interactable.attachedToHand == null && startingGrabType != GrabTypes.None)
            {
                //if (movingPart.localPosition.x <= -1.29)
                //{
                //    movingPart.localPosition = new Vector3(-1.2f, 0.000f, 1.616f);
                //    print("local position drag: " + movingPart.localPosition.x);
                    // movingPart.Translate(-1.29f, 0.000f, 1.616f);
                //}
                hand.AttachObject(gameObject, startingGrabType, attachmentFlags);
                
            }


        }

        protected virtual void HandAttachedUpdate(Hand hand)
        {
            hovering = true;
            lastHoveredHand = hand;

            bool wasEngaged = engaged;

            float currentDistance = Vector3.Distance(movingPart.parent.InverseTransformPoint(hand.transform.position), endPosition);
            float enteredDistance = Vector3.Distance(handEnteredPosition, endPosition);

            if (currentDistance > enteredDistance)
            {
                enteredDistance = currentDistance;
                //handEnteredPosition = movingPart.parent.InverseTransformPoint(hand.transform.position);
                handEnteredPosition = movingPart.localPosition;
            }

            float distanceDifference = enteredDistance - currentDistance;

            float lerp = Mathf.InverseLerp(0, localMoveDistance.magnitude, distanceDifference);

            if (lerp > engageAtPercent)
                engaged = true;
            else if (lerp < disengageAtPercent)
                engaged = false;
            print("end position: " + endPosition.x);
            //print("localmovedis: " + localMoveDistance.x);
            print("local position: " + movingPart.localPosition.x);
            if (movingPart.localPosition.x <= endPosition.x)
            {
                print("dsgsgdraesgraehaerhreg\rwhrwhrehraehr");
                hand.DetachObject(gameObject);
                movingPart.localPosition = new Vector3(endPosition.x + 0.2f, endPosition.y, endPosition.z);
               // movingPart.Translate(-1.29f, 0.000f, 1.616f);
            }
            else
            {
                movingPart.localPosition = Vector3.Lerp(startPosition, endPosition, lerp);
            }

            
            //print("end position: " + endPosition.x);

            //InvokeEvents(wasEngaged, engaged);

            //print(currentDistance);

            if (hand.IsGrabEnding(this.gameObject))
            {
                hand.DetachObject(gameObject);
            }
        }

        //private void LateUpdate()
        //{
         //   if (hovering == false)
         //   {
                //movingPart.localPosition = startPosition;
                // handEnteredPosition = endPosition;

                //InvokeEvents(engaged, false);
         //       engaged = false;
         //   }

         //   hovering = false;
     //   }

        /*
        private void InvokeEvents(bool wasEngaged, bool isEngaged)
        {
            buttonDown = wasEngaged == false && isEngaged == true;
            buttonUp = wasEngaged == true && isEngaged == false;

            if (buttonDown && onButtonDown != null)
                onButtonDown.Invoke(lastHoveredHand);
            if (buttonUp && onButtonUp != null)
                onButtonUp.Invoke(lastHoveredHand);
            if (isEngaged && onButtonIsPressed != null)
                onButtonIsPressed.Invoke(lastHoveredHand);
        }
    
    */
    }
}
