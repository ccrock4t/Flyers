using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvolutionManager : MonoBehaviour
{
    public static EvolutionManager singleton;
    public float highScore = 0.0f;
    public GameObject HighestMarker;
    public GameObject prefabFlyer;
    int numberOfTopFlyersToBreed = 4; //breed the top 4 flyers
    int numberPerGeneration;
    int numberOfDead = 0;
    List<Flyer> currentGeneration = new List<Flyer>();
    float spawnNextGenerationTimer;


    // Start is called before the first frame update
    void Start()
    {
        singleton = this;
        numberPerGeneration = numberOfTopFlyersToBreed * (numberOfTopFlyersToBreed - 1);

        SpawnInitialGeneration();
    }

    // Update is called once per frame
    void Update()
    {
        if(numberOfDead == numberPerGeneration) //everyone dead
        {
            spawnNextGenerationTimer -= Time.deltaTime;
            if (spawnNextGenerationTimer <= 0f) { 
                FinalizeCurrentGeneration();
            }
        }
    }

    public void FlyerDied()
    {
        numberOfDead++;
    }

    /**
     *  Gather the statistics from this generation
     *  and interbreed the best performers
     */
    void FinalizeCurrentGeneration()
    {
        currentGeneration.Sort(new FlyerSorter()); //sort by max height achieved
        float currentGenerationMaxHeight = 0.0f;

        // Set high score stastics, and gather the top flyers - they will survive to the next generation and interbreed
        int idx = 0;
        List<float[]> topFlyersChromosomes = new List<float[]>();
        foreach (Flyer flyer in currentGeneration)
        {
            if(flyer.maxHeight > currentGenerationMaxHeight)
            {
                currentGenerationMaxHeight = flyer.maxHeight;
            }
            if(flyer.maxHeight > highScore)
            {
                highScore = flyer.maxHeight;
                HighestMarker.transform.position = new Vector3(0, highScore, 0); //set the red bar if high score is exceeded
            }

            if(idx < numberOfTopFlyersToBreed)
            {
                Debug.Log(flyer.maxHeight);
                topFlyersChromosomes.Add(flyer.chromosome);
            }
            idx++;
        }

        //DESTROY the current generation
        foreach (Flyer flyer in currentGeneration)
        {
            Destroy(flyer.gameObject);
        }
        currentGeneration.Clear();

        // Breed the Top Flyers to get offspring chromosomes
        // also insert Top Flyer chromosomes into List
        List<float[]> nextGenerationChromosomes = new List<float[]>();
        for (int i=0; i < topFlyersChromosomes.Count; i++)
        {
            for (int j = i+1; j < topFlyersChromosomes.Count; j++)
            {
                if(i != j) //breed them if they are different Flyers
                {
                    (float[] offspring1, float[] offspring2) = SinglePointCrossover(topFlyersChromosomes[i],topFlyersChromosomes[j]);
                    //(float[] offspring1, float[] offspring2) = DoublePointCrossover();
                    //(float[] offspring1, float[] offspring2) = UniformCrossover();
                   // nextGenerationChromosomes.Add(offspring1);
                    //nextGenerationChromosomes.Add(offspring2);
                }
            }
            nextGenerationChromosomes.Add(topFlyersChromosomes[i]);
        }

        //then spawn everyone in the next generation using their chromosomes
        idx = 0;
        foreach(float[] chromosome in nextGenerationChromosomes)
        {
            GameObject flyerObject = Instantiate(prefabFlyer);
            Flyer flyer = flyerObject.GetComponent<Flyer>();
            currentGeneration.Add(flyer);
            flyer.init(chromosome[(int)Flyer.Gene.LeftWingLength],
                chromosome[(int)Flyer.Gene.RightWingLength],
                chromosome[(int)Flyer.Gene.LeftWingWidth],
                chromosome[(int)Flyer.Gene.BodyDiameter],
                chromosome[(int)Flyer.Gene.RightWingWidth],
                chromosome[(int)Flyer.Gene.LeftWingThickness],
                chromosome[(int)Flyer.Gene.RightWingThickness]);

            flyerObject.transform.position = new Vector3(idx*4-numberPerGeneration*2, flyer.GetBodyDiameter(), 0);
            idx++;
        }

        numberOfDead = 0;
    }

    (float[], float[]) SinglePointCrossover(float[] parentAchromosome, float[] parentBchromosome)
    {
        float[] offspring1 = new float[7];
        float[] offspring2 = new float[7];
        //Todo - Checkpoint 3
        return (offspring1, offspring2);
    }

    (float[], float[]) TwoPointCrossover(float[] parentAchromosome, float[] parentBchromosome)
    {
        float[] offspring1 = new float[7];
        float[] offspring2 = new float[7];
        //Todo - Checkpoint 3
        return (offspring1, offspring2);
    }


    (float[], float[]) UniformCrossover(float[] parentAchromosome, float[] parentBchromosome)
    {
        float[] offspring1 = new float[7];
        float[] offspring2 = new float[7];
        //Todo - Checkpoint 3
        return (offspring1, offspring2);
    }


    //Spawn the initial generation
    void SpawnInitialGeneration()
    {
        numberOfDead = 0;
        spawnNextGenerationTimer = 1.0f;
        for (int i=0; i < numberPerGeneration; i++)
        {
            GameObject flyerObject = Instantiate(prefabFlyer);
            Flyer flyer = flyerObject.GetComponent<Flyer>();
            currentGeneration.Add(flyer);

            float minGeneValue = 0.0f;
            float maxGeneValue = 5.0f;
            flyer.init(UnityEngine.Random.Range(minGeneValue, maxGeneValue),
                UnityEngine.Random.Range(minGeneValue, maxGeneValue), 
                UnityEngine.Random.Range(minGeneValue, maxGeneValue), 
                UnityEngine.Random.Range(minGeneValue, maxGeneValue), 
                UnityEngine.Random.Range(minGeneValue, maxGeneValue), 
                UnityEngine.Random.Range(minGeneValue, maxGeneValue), 
                UnityEngine.Random.Range(minGeneValue, maxGeneValue));

            flyerObject.transform.position = new Vector3(i * 4 - numberPerGeneration * 2, flyer.GetBodyDiameter(), 0);
     
        }
    }

    public class FlyerSorter : IComparer<Flyer>
    {

        public int Compare(Flyer x, Flyer y)
        {
            if (x.maxHeight <= y.maxHeight)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }
    }

}
