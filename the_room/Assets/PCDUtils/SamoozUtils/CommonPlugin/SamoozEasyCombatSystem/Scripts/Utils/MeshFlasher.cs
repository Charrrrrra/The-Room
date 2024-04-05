using System.Collections;
using UnityEngine;

public class MeshFlasher : SingletonMono<MeshFlasher> {
    [SerializeField]
    private Material flashMat;
    public void DoFlash(Renderer renderer, float duration) {
            Debug.Log(renderer.material.name == (flashMat.name + " (Instance)"));
        if (renderer.material.name == (flashMat.name + " (Instance)")) {
            return;
        }
        StartCoroutine(FlashTask(renderer, duration));
    }

    IEnumerator FlashTask(Renderer renderer, float duration) {

        Material prevMat = renderer.material;
        renderer.material = flashMat;

        yield return new WaitForSeconds(duration);

        Debug.Log("reset");
        renderer.material = prevMat;

    }

}
