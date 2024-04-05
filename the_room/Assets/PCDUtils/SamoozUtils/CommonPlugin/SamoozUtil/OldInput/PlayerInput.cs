 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : SingletonMono<PlayerInput>
{
    public Vector2 dMouse;
    public Vector2 mousePosition;
    public Vector3 mouseGroundPoint;
    public Vector3 inputAxis;
    public float scroll;
    public BaseKey mouseLeft;
    public BaseKey mouseRight;
    public BaseKey mouseMid;
    public BaseKey keyF;
    public BaseKey keyV;
    public BaseKey keyTab;
    public BaseKey keyLAlt;
    public BaseKey keySpace;
    public BaseKey leftShift;
    private static List<BaseKey> keyList = new List<BaseKey>();

    void Awake() {
        mouseLeft = new BaseKey(KeyCode.Mouse0);
        mouseRight = new BaseKey(KeyCode.Mouse1);
        mouseMid = new BaseKey(KeyCode.Mouse2);
        keyTab = new BaseKey(KeyCode.Tab);
        keyF = new BaseKey(KeyCode.F);
        keyV = new BaseKey(KeyCode.V);
        keyLAlt = new BaseKey(KeyCode.LeftAlt);
        keySpace = new BaseKey(KeyCode.Space);
        leftShift = new BaseKey(KeyCode.LeftShift);
    }

    void Update() {
        inputAxis = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        scroll = Input.mouseScrollDelta.y;
        BaseKey.UpdateAllKey();
        dMouse = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        mousePosition = Input.mousePosition;
        mouseGroundPoint = GetMouseGroundPoint();
    }

    private Vector3 GetMouseGroundPoint() {
        Ray mouseRay = Camera.main.ScreenPointToRay(mousePosition);
        float angleToDown = Vector3.Angle(mouseRay.direction, Vector3.down);
        float groundDis = mouseRay.origin.y / Mathf.Cos(angleToDown * Mathf.Deg2Rad);
        return mouseRay.GetPoint(groundDis);
    }

    [System.Serializable]
    public class BaseKey {
        public BaseKey(KeyCode keycode) {
            this.keycode = keycode;
            keyList.Add(this);
        }
        private KeyCode keycode;
        public float longPressTick;
        public bool keyDown;
        public bool press;
        public bool longPress;
        public bool holding;
        public bool release;
        private bool needTick;

        private void Update() {
            press = false;
            longPress = false;
            keyDown = Input.GetKeyDown(keycode);
            holding = Input.GetKey(keycode);
            release = Input.GetKeyUp(keycode);
            needTick = keyDown ? true : release ? false : needTick;

            if (keyDown) {
                ResetTick();
            }

            if (needTick || release) {    
                bool tickFinish = Tick();
                
                if (tickFinish && !release) {
                    longPress = true;
                    needTick = false;
                    // Debug.Log("longPress");
                    return;
                }

                if (tickFinish || !release) {
                    return;
                }

                press = true;
                // Debug.Log("press");

            }
        
        }

        public static void UpdateAllKey() {
            foreach (BaseKey key in keyList)
            {
                key.Update();
            }
        }

        void ResetTick() {
            longPressTick = 0.16f;
        }

        bool Tick() {
            if (longPressTick <= 0) {
                return true;
            }
            longPressTick -= Time.deltaTime;
            return false;
        }
    }

}
