using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bros.Utils;

namespace Bros.UI2D {
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class BrosUI2D : MonoBehaviour {
        public Collider2D collider2d;
        public SpriteRenderer render;

        public bool isMouseOver = false;
        public bool isMouseHold = false;

        public Action onMouseEnter, onMouseStay, onMouseExit;
        public Action onMouseDown, onMouseHold, onMouseUp, onMouseClick;
        public FuncHook<bool> activeCheckFunc; /* use this to disable interact in some cases */

        protected virtual void Awake() {
            collider2d = GetComponent<BoxCollider2D>();
            render = GetComponent<SpriteRenderer>();
            if (activeCheckFunc != null)
                activeCheckFunc.AttachObject(this);
        }

        private int mouseClickStage = 0; // null, move-in click-down click-up
        protected virtual void Update() {
            Vector3 mousePos = Bros.Utils.UtilClass.GetMouseWorldPosition2D();

            if (activeCheckFunc != null && !activeCheckFunc.InvokeHook(this)) return;

            /* setup state */
            isMouseOver = collider2d.OverlapPoint(mousePos);
            isMouseHold = isMouseOver && Input.GetMouseButton(0);

            /* handle mouse input */
            if (isMouseOver) {
                onMouseStay?.Invoke();
                if (mouseClickStage == 0) {
                    onMouseEnter?.Invoke();
                    mouseClickStage = 1;
                }

                if (Input.GetMouseButtonDown(0)) {
                    onMouseDown?.Invoke();
                    if (mouseClickStage == 1)
                        mouseClickStage = 2;
                }

                if (Input.GetMouseButton(0)) {
                    onMouseHold?.Invoke();
                }

                if (Input.GetMouseButtonUp(0)) {
                    onMouseUp?.Invoke();
                    if (mouseClickStage == 2) {
                        onMouseClick?.Invoke();
                        mouseClickStage = 1;
                    }
                }
            } else {
                if (mouseClickStage != 0) {
                    mouseClickStage = 0;
                    onMouseExit?.Invoke();
                }
            }
        }
    }
}
