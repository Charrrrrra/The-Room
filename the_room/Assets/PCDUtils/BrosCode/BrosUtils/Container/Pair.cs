using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bros.Container {
    [System.Serializable]
    public class Pair<T1, T2> {
        public T1 first;
        public T2 second;

        public Pair(T1 first, T2 second) {
            this.first = first;
            this.second = second;
        }

        static public Pair<T1, T2> MakePair(T1 first, T2 second) {
            return new Pair<T1, T2>(first, second);
        }

        public void Unpack(out T1 fst, out T2 sec) {
            fst = first;
            sec = second;
        }
    }
}


