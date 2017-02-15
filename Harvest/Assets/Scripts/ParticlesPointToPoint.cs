using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesPointToPoint : MonoBehaviour {
    [SerializeField]
    private float speed;
    [SerializeField]
    private Transform endPosition;

    private ParticleSystem particleSystem;
    private ParticleSystem.Particle[] particles;

    // Use this for initialization
    void Start () {
        particleSystem = GetComponent<ParticleSystem>();
        particles = new ParticleSystem.Particle[particleSystem.main.maxParticles];
    }
	
	// Update is called once per frame
	void LateUpdate () {
        
        var count = particleSystem.GetParticles(particles);

        for(int i = 0; i < count; i++)
        {
            if (Vector3.Distance(particles[i].position, endPosition.position) < 0.05f)
            {
                particles[i].remainingLifetime = -1;
                continue;
            }
                
            var dir = endPosition.position - particles[i].position;
          
            particles[i].position += dir.normalized * speed * Time.deltaTime;
        }

        particleSystem.SetParticles(particles, count);
    }
}
