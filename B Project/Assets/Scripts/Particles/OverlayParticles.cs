using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayParticles : MonoBehaviour {

	static ParticlesPlayer particlesPlayer;
	static ParticlesDisplay particlesDisplay;

	public static void Init() {
		if(particlesPlayer == null) {
			particlesPlayer = GameObject.FindObjectOfType<ParticlesPlayer>();
		}

		if(particlesDisplay == null) {
			particlesDisplay = GameObject.FindObjectOfType<ParticlesDisplay>();
		}
	}

	public static void ShowParticles(int particleCount) {
		Init();
		particlesDisplay.Reset();
		particlesPlayer.ShowParticles(particleCount);
	}

	public static void ShowParticles(int particleCount, Vector3 position) {
		Init();
		particlesDisplay.Move(position);
		particlesPlayer.ShowParticles(particleCount);
	}

	public static void ShowParticlesLoop() {
		Init();
		particlesDisplay.Reset();
		particlesPlayer.StartEmissionLoop();
	}

	public static void ShowParticlesLoop(Vector3 position) {
		Init();
		particlesDisplay.Move(position);
		particlesPlayer.StartEmissionLoop();
	}

	public static void StopParticlesLoop() {
		Init();
		particlesPlayer.StopEmissionLoop();
		particlesDisplay.Reset();
	}

	public static void Move(Vector3 position) {
		particlesDisplay.Move(position);
	}
}
