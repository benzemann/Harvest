using UnityEngine;
using System.Collections;
using System;
using System.Linq;

public class HiveNew : MonoBehaviour {

    #region private
    [Header("Spawning stats")]
    [SerializeField]
    [Tooltip("A delay between when ants is allowed to exit the hive (in seconds)")]
    private float spawningDelay;
    [Serializable]
    private struct AntType
    {
        [Tooltip("Prefab of ant type here.")]
        public AntNew antType;
        [Tooltip("Maximum number of this ant type alive at one time.")]
        public int max;
        [Tooltip("The amount of resources it costs to breed this kind of ant")]
        public int costs;
        [Tooltip("The time it takes to breed this kind of ant")]
        public float breedingTime;
        [Tooltip("How often this kind of ant is breed.")]
        public int breedingWeight;
        [Tooltip("The number of free ants of this type.")]
        public int free;
    }
    [SerializeField]
    [Tooltip("A list of the different ant types this hive can spawn")]
    private AntType[] antTypes;
    private int resources;
    private bool isBreeding = false;
    private int currentBreedingIndx;
    private float startBreedingTime;
    #endregion
    #region public

    #endregion

    void Start()
    {
        
    }

    void Update()
    {

    }

    /// <summary>
    /// Tries to breed an ant based on their breedingWeight and the resources of the hive.
    /// </summary>
    void BreedAnts()
    {
        if (isBreeding)
        {
            // Check if breeding time has finished
            if(Time.time - startBreedingTime >= antTypes[currentBreedingIndx].breedingTime)
            {
                // Breed the currentAnt type
            }
        }
        // Find a 
        var sortedAntTypes = antTypes.OrderBy(ant => ant.breedingWeight).ToArray();
        
    }
}