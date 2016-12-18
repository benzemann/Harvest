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
        // Number of current alive ants of this type
     //   [HideInInspector]
        public int alive;
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
        BreedAnts();
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
                // Breed the current ant type
                isBreeding = false;
                antTypes[currentBreedingIndx].alive++;
                return;
            }
            
        } else
        {
            // Find a free anttype if any
            var freeAntType = antTypes.Where(ant => ant.alive < ant.free && ant.alive < ant.max).OrderBy(ant => ant.breedingWeight).Select(ant => ant.antType).ToArray();
            if (freeAntType.Any())
            {
                // Breed a free ant with the highest weight
                for (int i = 0; i < antTypes.Count(); i++)
                {
                    if (freeAntType[0] == antTypes[i].antType)
                    {
                        // start breeding 
                        currentBreedingIndx = i;
                        startBreedingTime = Time.time;
                        isBreeding = true;
                        return;
                    }
                }
            }
            // Get the total number of alive ants of all types
            var numberOfAnts = 0;
            foreach (var antType in antTypes)
                numberOfAnts += antType.alive;
            // Get a sorted list based on weight to target weight
            var nextAnt = antTypes.Where(ant => ant.alive < ant.max).OrderBy(ant => (ant.breedingWeight / numberOfAnts) - (ant.alive / numberOfAnts)).ToArray();
            if (nextAnt.Any())
            {
                if (nextAnt.First().costs <= resources)
                {
                    for (int i = 0; i < antTypes.Count(); i++)
                    {
                        if (nextAnt.First().antType == antTypes[i].antType)
                        {
                            currentBreedingIndx = i;
                            startBreedingTime = Time.time;
                            isBreeding = true;
                            resources -= nextAnt.First().costs;
                            return;
                        }
                    }
                }
            }
        }
    }
}