using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ParticlesController : MonoBehaviour {

	public Button particleButton;
	public Button toggleParticlesButton;
	bool particlesPlaying;

	// Use this for initialization
	void Start () {
		//particleButton.onClick.AddListener(()=>OverlayParticles.ShowParticles(10, particleButton.transform.position));
		toggleParticlesButton.onClick.AddListener(ToggleParticles);
	}

	void ToggleParticles() {
		if(particlesPlaying) {
			OverlayParticles.StopParticlesLoop();
			particlesPlaying = false;
			particleButton.enabled = true;
		} else {
			OverlayParticles.ShowParticlesLoop();
			particlesPlaying = true;
		}
	}

	void Update() {
		if(particlesPlaying) {
			
			//OverlayParticles.Move(Input.mousePosition);
		}
	}
}
