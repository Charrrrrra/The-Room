using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

namespace Bros.Utils {
    public static class UtilClass {
        static public Vector3 GetMouseWorldPostitonRaw(Camera cam = null) {
            if (cam == null) cam = Camera.main;
            return cam.ScreenToWorldPoint(Input.mousePosition);
        }

        static public Vector3 GetMouseWorldPosition2D(Camera cam = null) {
            Vector3 mousePos = GetMouseWorldPostitonRaw(cam);
            mousePos.z = 0f;
            return mousePos;
        }

        static public TextMeshPro CreateWorldTextPopup(string text, int fontSize, Color color, Vector3 localPos,
            Transform parent = null, TMP_SpriteAsset spriteAsset = null) {
            float popupTime = 1f;
            float popupDist = 0.3f * fontSize;
            return CreateWorldTextPopup(text, fontSize, color, localPos, parent, spriteAsset, popupTime, popupDist);
        }

        static public TextMeshPro CreateWorldTextPopup(string text, int fontSize, Color color, Vector3 localPos,
            Transform parent, TMP_SpriteAsset spriteAsset, float popupTime, float popupDist) {
            GameObject meshObject = new GameObject("WorldTextPopup", typeof(TextMeshPro));
            meshObject.transform.SetParent(parent);

            TextMeshPro textMesh = meshObject.GetComponent<TextMeshPro>();
            textMesh.text = text;
            textMesh.color = color;
            textMesh.fontSize = fontSize;
            textMesh.spriteAsset = spriteAsset;
            textMesh.transform.localPosition = localPos;
            textMesh.alignment = TextAlignmentOptions.Center;
            textMesh.enableWordWrapping = false;

            float popupSpeed = popupDist / popupTime;
            FuncUpdater.Create(delegate () {
                popupTime -= Time.deltaTime;
                if (popupTime < 0) {
                    GameObject.Destroy(textMesh.gameObject);
                    return true;
                }
                textMesh.transform.localPosition += new Vector3(0f, popupSpeed * Time.deltaTime, 0f);
                return false;
            });

            return textMesh;
        }

        static public List<T> MakeList2<T>(T a, T b) {
            var res = new List<T>();
            res.Add(a); res.Add(b);
            return res;
        }

        static public List<T> MakeList3<T>(T a, T b, T c) {
            var res = new List<T>();
            res.Add(a); res.Add(b); res.Add(c);
            return res;
        }

        static public List<T> AppendList<T>(List<T> first, List<T> second) {
            second.ForEach((item) => { first.Add(item); });
            return first;
        }

        /* Function Helpers */

        /// <summary>
        /// 根据评分函数找到最优对象，如果评分为 NegativeInfinity 则跳过
        /// </summary>
        static public T FindBest<T>(List<T> list, Func<T, float> socreFunc) {
            T best = default(T);
            float bestScore = float.NegativeInfinity;
            foreach (var item in list) {
                float score = socreFunc(item);
                if (score > float.NegativeInfinity && score > bestScore) {
                    bestScore = score;
                    best = item;
                }
			}
            return best;
		}

        static public T FindBest<T>(T[] list, Func<T, float> socreFunc) {
            T best = default(T);
            float bestScore = float.NegativeInfinity;
            foreach (var item in list) {
                float socre = socreFunc(item);
                if (socre > bestScore) {
                    bestScore = socre;
                    best = item;
                }
            }
            return best;
        }
    }
}
