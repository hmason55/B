using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesPlayer : MonoBehaviour {

	public ParticleSystem particleSystem;

	public void ShowParticles(int particleCount = 25) {
		particleSystem.Emit(particleCount);
	}

	public void StartEmissionLoop() {
		ParticleSystem.MainModule mainModule = particleSystem.main;
		mainModule.loop = true;
		particleSystem.Play();
	}

	public void StopEmissionLoop() {
		ParticleSystem.MainModule mainModule = particleSystem.main;
		mainModule.loop = false;
		particleSystem.Stop();
	}
}
