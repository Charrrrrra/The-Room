using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class StateMachine<T> {
        public T curState;
        public T defaultState;
        public Action onReturnFromSubSM;
        private ActionState<T> curStateAction;
        private Dictionary<T, ActionState<T>> stateDic;
        private bool isHandleBySubSM;

        public StateMachine(T defaultState) {
            stateDic = new Dictionary<T, ActionState<T>>();
            CreateAllState((T[])Enum.GetValues(typeof(T)));
            this.defaultState = defaultState;
            onReturnFromSubSM = () => isHandleBySubSM = false;
        }

        public void CreateAllState(T[] states) {
            foreach (T stateName in states) { CreateState(stateName); }
        }

        public void CreateState(T state) {
            if (stateDic.ContainsKey(state)) { return; }
            stateDic.Add(state, new ActionState<T>(state));
        }

        public ActionState<T> GetState(T state) {
            if (!stateDic.ContainsKey(state)) { return null; }
            return stateDic[state];
        }

        public void GotoState(T state, bool allowGotoSameState = true) {
            if (!stateDic.ContainsKey(state)) { return; }
            if (curStateAction != null) {
                curStateAction.onExit?.Invoke();
            }
            if (!allowGotoSameState && state.Equals(curState)) {
                return;
            }
            curStateAction = GetState(state);
            curState = curStateAction.state;
            curStateAction.onEnter?.Invoke();
            onReturnFromSubSM.Invoke();
        }

        public void Init() {
            GotoState(defaultState);
        }

        public void UpdateStateAction() {
            if (CheckUpdateStateCond()) {
                curStateAction.onUpdate?.Invoke();
            }
        }

        /// <summary>
        /// handle update by a subSM, waiting subSM return to continue cur update
        /// </summary>
        /// <typeparam name="SubState"></typeparam>
        /// <param name="subSM"></param>
        public void GotoSubSM<SubState>(SubStateMachine<T, SubState> subSM, SubState targetState) {
            if (subSM == null)
                return;
            isHandleBySubSM = true;
            subSM.OnSubStateEnter(targetState);
        }

        protected virtual bool CheckUpdateStateCond() {
            return !isHandleBySubSM;
        }

    }

    public class ActionState<T> {
        public ActionState(T stateName) {
            this.state = stateName;
        }
        public T state;
        public System.Action onEnter = () => {};
        public System.Action onUpdate = () => {};
        public System.Action onExit = () => {};

    // void CreateSM() {
    //     sm = new StateMachine<State>(State.Idle);

    //     sm.GetState(State.Idle).Bind(
    //         () => {},
    //         () => {},
    //         () => {}
    //     );

    //     sm.Init();

    // }
        public void Bind(System.Action onEnter = null, System.Action onUpdate = null, System.Action onExit = null) {
            this.onEnter = onEnter;
            this.onUpdate = onUpdate;
            this.onExit = onExit;
        }

    }

public class SubStateMachine<ParentState, T> : StateMachine<T> {
    public StateMachine<ParentState> parentSM;
    public bool isActiveSM;
    public SubStateMachine(StateMachine<ParentState> parentSM, T defaultState) : base(defaultState) {
        this.parentSM = parentSM;
    }

    public void ReturnToParentSM() {
        parentSM.onReturnFromSubSM();
        isActiveSM = false;
        GotoState(defaultState);
    }

    public void OnSubStateEnter(T targetState) {
        isActiveSM = true;
        Init();
        GotoState(targetState);
    }

    // only update after parentSM GotoSubSM
    protected override bool CheckUpdateStateCond() {
        return isActiveSM;
    }

}
