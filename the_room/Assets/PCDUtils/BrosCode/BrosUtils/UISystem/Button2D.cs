using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bros.UI2D;
using TMPro;

/* 该组件不考虑重叠两个按钮的情况，该情况不预期发生 */
namespace Bros.UI2D {
    public class Button2D : BrosUI2D {
        public TextMeshPro text;
        public Sprite mouseOverSprite;
        public Sprite mouseHoldSprite;
        public Color mouseOverColor;
        public Color mouseHoldColor;
        public bool hasMouseOverColor = false;
        public bool hasMouseHoldColor = false;

        private Sprite defaultSprite;
        private Color defaultColor;
        
        override protected void Awake() {
            base.Awake();
            defaultSprite = render.sprite;
            defaultColor = render.color;

            if (mouseOverSprite != null || hasMouseOverColor) {
                onMouseEnter += RefreshSpriteAndColor;
                onMouseExit += RefreshSpriteAndColor;
            }
            if (mouseHoldSprite != null || hasMouseHoldColor) {
                onMouseDown += RefreshSpriteAndColor;
                onMouseUp += RefreshSpriteAndColor;
            }
        }

        override protected void Update() {
            base.Update();
            text.sortingLayerID = render.sortingLayerID;
            text.sortingOrder = render.sortingOrder + 1;
        }

        private void RefreshSpriteAndColor() {
            Sprite sprite = defaultSprite;
            Color color = defaultColor;

            bool setMouseOver = isMouseOver && (mouseOverSprite != null || hasMouseOverColor);
            bool setMouseHold = isMouseHold && (mouseHoldSprite != null || hasMouseHoldColor);
            /* for sprite */
            if (setMouseHold && mouseHoldSprite != null)
                sprite = mouseHoldSprite;
            else if (setMouseOver && mouseOverSprite != null)
                sprite = mouseOverSprite;
            /* for color */
            if (setMouseHold && hasMouseHoldColor)
                color = mouseHoldColor;
            else if (setMouseOver && hasMouseOverColor)
                color = mouseOverColor;

            render.sprite = sprite;
            render.color = color;
        }
    }
}
