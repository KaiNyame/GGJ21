using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour {
    public LayerMask sheep;
    public float transitionSpeed;
    public ParticleSystem fog;
    public ParticleSystem sparkles;
    public ParticleSystem circle;
    public Renderer portal;
    public Light lighting;
    public bool complete;

    public Color[] mainTint;
    public Color[] fogTint;
    public Color[] portalTint;

    public int mode;
    private static readonly int MainColorID = Shader.PropertyToID("Color_1d355fc4f9974701893a5a97f3d4acc2");
    private static readonly int AccentColorID = Shader.PropertyToID("Color_d17c3061e6f148d19484893ff2558e72");

    private void OnTriggerEnter(Collider other) {
        if ((sheep.value & (1 << other.gameObject.layer)) == 0) return;
        complete = true;
        mode = 1;
    }
    
    private void OnTriggerExit(Collider other) {
        if ((sheep.value & (1 << other.gameObject.layer)) == 0) return;
        complete = false;
        mode = 0;
    }

    private void LerpParticle(ParticleSystem p, Color[] colors, float delta) {
        var main = p.main;
        main.startColor = Color.Lerp(main.startColor.color, colors[mode], delta);
    }

    public void Start() {
        lighting.color = mainTint[mode];

        LerpParticle(sparkles, mainTint, 1);
        LerpParticle(circle, mainTint, 1);
        LerpParticle(fog, fogTint, 1);
        
        portal.material.SetColor(MainColorID, mainTint[mode]);
        portal.material.SetColor(AccentColorID, portalTint[mode]);
    }

    public void Update() {
        var delta = Time.deltaTime * transitionSpeed;
        lighting.color = Color.Lerp(lighting.color, mainTint[mode], delta);

        LerpParticle(sparkles, mainTint, delta);
        LerpParticle(circle, mainTint, delta);
        LerpParticle(fog, fogTint, delta);
        
        portal.material.SetColor(MainColorID, Color.Lerp(portal.material.GetColor(MainColorID),mainTint[mode], delta));
        portal.material.SetColor(AccentColorID, Color.Lerp(portal.material.GetColor(AccentColorID), portalTint[mode], delta));
    }
}
