using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MatModifier : MonoBehaviour
{
	public Material mat;
	public bool changeWhenStart;
	private List<MeshRenderer> renders;
	private List<List<Material>> origMats;
	private List<List<Material>> otherMats;

	private void Awake() {
		origMats = new List<List<Material>>();
		otherMats = new List<List<Material>>();
		renders = GetComponents<MeshRenderer>().ToList();
		GetComponentsInChildren<MeshRenderer>().ToList().ForEach((render) => {
			renders.Add(render);
		});
		renders.ForEach((render) => {
			origMats.Add(render.materials.ToList());
		});
		if (mat != null)
			SetupOtherMaterial(mat);
	}

	private void Start() {
		if (changeWhenStart)
			ChangeIntoOtherMaterials();
	}

	public void SetupOtherMaterial(Material material) {
		otherMats.Clear();
		for (int i = 0; i < origMats.Count; i++) {
			List<Material> materials = new List<Material>();
			origMats[i].ForEach((mat) => {
				materials.Add(material);
			});
			otherMats.Add(materials);
		}
	}

	public void ChangeIntoOtherMaterials() {
		for (int i = 0; i < renders.Count; i++) {
			renders[i].SetMaterials(otherMats[i]); 
		}
	}

	public void ChangeIntoOriginalMaterials() {
		for (int i = 0; i < renders.Count; i++) {
			renders[i].SetMaterials(origMats[i]);
		}
	}
}
