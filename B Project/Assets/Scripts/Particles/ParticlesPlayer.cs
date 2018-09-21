using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesPlayer : MonoBehaviour {

	
	public ParticleSystem particleSystem;
	public bool destroyOnComplete = true;
	ParticleSystem.MainModule mainModule;
	float startTime = 0f;
	float endTime = 0f;
	float destroyTime = 0f;

	public void ShowParticles(int particleCount = 25) {
		particleSystem.Emit(particleCount);
	}

	public void StartEmissionLoop() {
		mainModule.loop = true;
		particleSystem.Play();
	}

	public void StopEmissionLoop() {
		mainModule.loop = false;
		particleSystem.Stop();
	}

	void Start() {
		mainModule = particleSystem.main;
		startTime = Time.realtimeSinceStartup;
		endTime = startTime + mainModule.duration;
		destroyTime = endTime + (mainModule.startLifetime.constant * mainModule.startLifetimeMultiplier);
	}

	void Update() {
		float currentTime = Time.realtimeSinceStartup;
		if(currentTime > endTime && currentTime < destroyTime) {
			particleSystem.Stop();
		} else if(currentTime > destroyTime) {
			Destroy(gameObject);
		}
	}
}
