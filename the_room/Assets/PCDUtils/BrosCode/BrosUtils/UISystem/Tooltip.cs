using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Bros.UI2D {
    public class Tooltip : MonoBehaviour {
        public SpriteRenderer boxRender;
        public TextMeshPro text;

        public Vector3 leftTop;
        public Vector3 leftBottom;
        public Vector3 rightTop;
        public Vector3 rightBottom;

        private void Awake() {
            RefreshGFX();
        }

        private void Update() {
            RefreshGFX();
        }

        public void SetupText(string str) {
            text.text = "<wave>" + str + "</wave>";
            RefreshGFX();
        }

        public void SetCornerToPos(CornerOption option, Vector3 pos) {
            RefreshGFX();
            Vector3 cornerCurPos = GetCornerPos(option);
            Vector3 diff = pos - cornerCurPos;

            transform.position += diff;
        }

        private Vector3 GetCornerPos(CornerOption option) {
            switch (option) {
                case CornerOption.LeftTop: return leftTop;
                case CornerOption.LeftBottom: return leftBottom;
                case CornerOption.RightTop: return rightTop;
                case CornerOption.RightBottom: return rightBottom;
                default: return Vector3.zero;
            }
        }

        private void RefreshGFX() {
            float blankSize = (float)text.fontSize / 144f;
            float textWidth = Mathf.Min(text.preferredWidth, text.rectTransform.rect.width) * text.transform.localScale.x;
            float textHeight = text.preferredHeight * text.transform.localScale.y;
            boxRender.transform.localScale = new Vector3(textWidth + blankSize, textHeight + blankSize, 1f);

            float horizontalHalf = boxRender.transform.lossyScale.x / 2f;
            float verticalHalf = boxRender.transform.lossyScale.y / 2f;

            leftTop = transform.position + new Vector3(-horizontalHalf, verticalHalf, 0f);
            leftBottom = transform.position + new Vector3(-horizontalHalf, -verticalHalf, 0f);
            rightTop = transform.position + new Vector3(horizontalHalf, verticalHalf, 0f);
            rightBottom = transform.position + new Vector3(horizontalHalf, -verticalHalf, 0f);
        }

        public enum CornerOption {
            LeftTop,
            LeftBottom,
            RightTop,
            RightBottom,
        };
    }
}