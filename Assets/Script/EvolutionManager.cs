using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class EvolutionManager : MonoBehaviour
{
    public static EvolutionManager singleton;
    public static float maxGeneValue = 4.0f;
    public static float minGeneValue = 0.1f;
    public static float mutationRate = 0.2f; 
    public float highScore = 0.0f;
    public GameObject HighestMarker;
    public GameObject prefabFlyer;
    int numberOfTopFlyersToBreed = 4; //breed the top 4 flyers
    public int numberPerGeneration;
    public int numberOfDead = 0;
    public List<Flyer> currentGeneration = new List<Flyer>();
    float spawnNextGenerationTimer;
    int currentGenerationNumber = 0;

    //UI
    public Text uitext;

    //file
    StreamWriter statisticsFile;
        
    // Start is called before the first frame update
    void Start()
    {
        singleton = this;
        numberPerGeneration = numberOfTopFlyersToBreed * (numberOfTopFlyersToBreed - 1) + numberOfTopFlyersToBreed;

        SpawnInitialGeneration();

        statisticsFile = new StreamWriter("statistics.txt", true);
        
    }

    private void OnApplicationQuit()
    {
        statisticsFile.WriteLine("=====================================================");
        statisticsFile.WriteLine("+++++++++++++++++++++++++++++++++++++++++++++++++++++");
        statisticsFile.WriteLine("=====================================================");
        statisticsFile.WriteLine("OVERALL HIGH SCORE: " + highScore);
        statisticsFile.Close();
    }

    // Update is called once per frame
    void Update()
    {
        if(numberOfDead == numberPerGeneration) //everyone dead
        {
            spawnNextGenerationTimer -= Time.deltaTime;
            if (spawnNextGenerationTimer <= 0f) { 
                FinalizeCurrentGeneration();
                spawnNextGenerationTimer = 3.0f;
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
        statisticsFile.WriteLine("========================================");
        statisticsFile.WriteLine("GENERATION #" + currentGenerationNumber + " STATISTICS");
        statisticsFile.WriteLine("========================================");
        statisticsFile.WriteLine("Rating,Chromosome,MaxHeight,WillReproduce?");

        currentGeneration.Sort(new FlyerSorter()); //sort by max height achieved
        float currentGenerationMaxHeight = 0.0f;

        // Get high score stastics, and gather the top flyers - they will survive to the next generation and interbreed
        int idx = 0;
        float thisGenHighScore = 0;
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
            if(flyer.maxHeight > thisGenHighScore)
            {
                thisGenHighScore = flyer.maxHeight;
            }

            string reproducesString = "";
            if(idx < numberOfTopFlyersToBreed)
            {
                Debug.Log(flyer.maxHeight);
                topFlyersChromosomes.Add(flyer.chromosome);
                reproducesString = "TOP FLYER - REPRODUCES";
            }
            idx++;

            string chromosomeString = "";
            foreach(float f in flyer.chromosome)
            {
                chromosomeString += (f + ",");
            }
            statisticsFile.WriteLine(idx + "," + chromosomeString + flyer.maxHeight + "," + reproducesString);
        }

        statisticsFile.WriteLine("THIS GENERATION'S HIGH SCORE: " + thisGenHighScore);

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
                    //(float[] offspring1, float[] offspring2) = SinglePointCrossover(topFlyersChromosomes[i],topFlyersChromosomes[j]);
                    //(float[] offspring1, float[] offspring2) = TwoPointCrossover(topFlyersChromosomes[i], topFlyersChromosomes[j]);
                    (float[] offspring1, float[] offspring2) = UniformCrossover(topFlyersChromosomes[i],topFlyersChromosomes[j]);

                    //mutations
                    for (int k = 0; k < 7; k++)
                    {
                        //offspring 1
                        int random = UnityEngine.Random.Range(1, 101); //between 1 and 10
                        if (random <= (100 * mutationRate))
                        {
                            //Gene was selected for mutation!
                            offspring1[k] = UnityEngine.Random.Range(EvolutionManager.minGeneValue, EvolutionManager.maxGeneValue);
                        }

                        //offspring 2
                        random = UnityEngine.Random.Range(1, 101); //between 1 and 10
                        if (random <= (100 * mutationRate))
                        {
                            //Gene was selected for mutation!
                            offspring2[k] = UnityEngine.Random.Range(EvolutionManager.minGeneValue, EvolutionManager.maxGeneValue);
                        }
                    }

                    nextGenerationChromosomes.Add(offspring1);
                    nextGenerationChromosomes.Add(offspring2);
                }
            }
            nextGenerationChromosomes.Add(topFlyersChromosomes[i]);
        }

        //SPAWN NEXT GENERATION
        //spawn everyone in the next generation using their chromosomes
        this.currentGenerationNumber++;
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

            flyerObject.transform.position = new Vector3(GetSpawnXPosition(idx), flyer.body.transform.localScale.y/2f, 0);
            idx++;
        }

        numberOfDead = 0;

        updateUIText();
    }

    (float[], float[]) SinglePointCrossover(float[] parentAchromosome, float[] parentBchromosome)
    {
        //randomly select a point on the chromosome to be the crossover point
        float[] offspring1 = new float[7];
        float[] offspring2 = new float[7];
        int point = UnityEngine.Random.Range(1,7);
        for (int i = 0; i < 7; i++)
        {
            if (i < point)
            {
               offspring1[i] = parentAchromosome[i];
               offspring2[i] = parentBchromosome[i]; 
            }
            if (i >= point)
            {
              offspring1[i] = parentBchromosome[i];
              offspring2[i] = parentAchromosome[i];  
            }
        }
        
        return (offspring1, offspring2);
    }

    (float[], float[]) TwoPointCrossover(float[] parentAchromosome, float[] parentBchromosome)
    {
        //randomly select two points and crossover the genome at that point 
        float[] offspring1 = new float[7];
        float[] offspring2 = new float[7];
        int point1 = UnityEngine.Random.Range(1, 7);
        int point2 = UnityEngine.Random.Range(1, 7);

        //make sure that point1 and point2 are not the same 
        while (point1 == point2)
        {
            point2 = UnityEngine.Random.Range(1, 7);
        }

        if(point1 > point2)
        {
            //point2 must always be greater than point1, so swap them
            int temp = point2;
            point2 = point1;
            point1 = temp;
        }

        for (int i = 0; i < 7; i++)
        {
            if (i < point1 || i >= point2)
            {
                offspring1[i] = parentAchromosome[i];
                offspring2[i] = parentBchromosome[i];
            }else
            {
                offspring1[i] = parentBchromosome[i];
                offspring2[i] = parentAchromosome[i];
            }
        }
        
        return (offspring1, offspring2);
    }


    (float[], float[]) UniformCrossover(float[] parentAchromosome, float[] parentBchromosome)
    {
        //select each part of the chromosome from either parent with equal probability 
        float[] offspring1 = new float[7];
        float[] offspring2 = new float[7];
        
        //create offspring 1
        for (int i = 0; i < 7; i++)
        {
            int parent = UnityEngine.Random.Range(0, 2); //which parent to grab from?
            if (parent == 0)
            {
                offspring1[i] = parentAchromosome[i];
              
            } 
            if (parent == 1)
            {
                offspring1[i] = parentBchromosome[i];
            } 
        }

        //create offspring 2
        for (int i = 0; i < 7; i++)
        {
            int parent = UnityEngine.Random.Range(0, 2); //which parent to grab from?
            if (parent == 0)
            {
                offspring2[i] = parentAchromosome[i];
              
            } 
            if (parent == 1)
            {
                offspring2[i] = parentBchromosome[i];
            }
        }

        return (offspring1, offspring2);
    }


    //Spawn the initial generation
    void SpawnInitialGeneration()
    {
        this.currentGenerationNumber = 1;
        numberOfDead = 0;
        spawnNextGenerationTimer = 3.0f;
        for (int i=0; i < numberPerGeneration; i++)
        {
            GameObject flyerObject = Instantiate(prefabFlyer);
            Flyer flyer = flyerObject.GetComponent<Flyer>();
            currentGeneration.Add(flyer);

            flyer.init(UnityEngine.Random.Range(EvolutionManager.minGeneValue, EvolutionManager.maxGeneValue),
                UnityEngine.Random.Range(EvolutionManager.minGeneValue, EvolutionManager.maxGeneValue), 
                UnityEngine.Random.Range(EvolutionManager.minGeneValue, EvolutionManager.maxGeneValue), 
                UnityEngine.Random.Range(EvolutionManager.minGeneValue, EvolutionManager.maxGeneValue), 
                UnityEngine.Random.Range(EvolutionManager.minGeneValue, EvolutionManager.maxGeneValue), 
                UnityEngine.Random.Range(EvolutionManager.minGeneValue, EvolutionManager.maxGeneValue), 
                UnityEngine.Random.Range(EvolutionManager.minGeneValue, EvolutionManager.maxGeneValue));

            flyerObject.transform.position = new Vector3(GetSpawnXPosition(i), flyer.body.transform.localScale.y/2f, 0);
     
        }

        updateUIText();
    }

    void updateUIText()
    {
        uitext.text = "Generation: #" + this.currentGenerationNumber
           + "\n" + "Current Best: " + this.highScore + " meters" ;
    }

    float GetSpawnXPosition(int idx)
    {
        return (idx * 2 - numberPerGeneration);
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
