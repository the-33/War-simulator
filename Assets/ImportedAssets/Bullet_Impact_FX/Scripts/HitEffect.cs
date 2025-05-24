using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;

public class HitEffect : MonoBehaviour 
{
	public AudioClip[] m_HitSounds;
	public float m_HitEffectLifeTime;
	[HideInInspector]
	public Vector3 m_Rot;

	public List<Material> m_Materials = new();
	// Use this for initialization
	void Start () 
	{
		GetComponent<AudioSource>().PlayOneShot(m_HitSounds[Random.Range(0,m_HitSounds.Length)]);
		if (m_Materials.Count > 0) gameObject.GetComponent<DecalProjector>().material = m_Materials[Random.Range(0, m_Materials.Count)];

		Destroy(gameObject,m_HitEffectLifeTime);
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
}
