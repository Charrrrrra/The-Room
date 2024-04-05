using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

public class Damagable : MonoBehaviour {
    public float maxHp = 30.0f;
    public float hp;
    public bool healOnDeath;
    public UnityEvent onDeath;
    public UnityEvent onGetDamage;
    [SerializeField]
    protected GameObject[] hitEffects;
    [SerializeField]
    protected GameObject[] deathEffects;
    [SerializeField]
    protected Renderer[] flashRenders;

    protected virtual void Awake() {
        Reset();
    }

    void Update() {
        
    }

    public virtual void HandleDamage(DamageInfo info) {
        Debug.Log(1);
        float lastHp = hp;
        hp = Mathf.Max(hp - info.damage, 0);
        OnGetDamage();
        if (lastHp > 0 && hp <= 0) {
            OnDeath(info.attacker);
        } else {
            
        }
    }

    public virtual void OnDeath(Transform attacker) {
        if (deathEffects != null) {
            foreach (GameObject effect in deathEffects) {
                SpawnMgr.SpawnGameObject(effect, transform.GetComponentInChildren<Collider>().bounds.center, transform.rotation);
            }
        }
        onDeath?.Invoke();
        if (healOnDeath) {
            hp = maxHp;
        }
    }

    public virtual void Reset() {
        hp = maxHp;
    }

    public virtual void OnGetDamage() {
        if (hitEffects != null) {
            foreach (GameObject effect in hitEffects) {
                SpawnMgr.SpawnGameObject(effect, transform.GetComponentInChildren<Collider>().bounds.center, Quaternion.identity);
            }
        }
    
        onGetDamage?.Invoke();
        if (flashRenders == null) {
            return;
        }
        for (int i = 0; i < flashRenders.Length; i++) {
            MeshFlasher.GetInstance().DoFlash(flashRenders[i], 0.2f);
        }
        foreach (Renderer render in flashRenders) {
            MeshFlasher.GetInstance().DoFlash(render, 0.2f);
        }
    }

}

#if UNITY_EDITOR
[CustomEditor(typeof(Damagable))]
public class DamagableDebugEditor : Editor {

    public override void OnInspectorGUI() {
        
        DrawDefaultInspector();

        Damagable damagable = (Damagable)target;
        if (EditorApplication.isPlaying) {
            if (GUILayout.Button("Death")) {
                damagable.OnDeath(null);
            }
            if (GUILayout.Button("Damage 1")) {
                damagable.HandleDamage(new DamageInfo(Vector3.zero, null, 1, ""));
            }
        }

    }

}
#endif
