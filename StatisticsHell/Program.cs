using MathNet.Numerics.Distributions;
using MathNet.Numerics.Random;
namespace StatisticsHell
{
    internal class Program
    {
        static Random random = new Random();

        static void Main(string[] args)
        {
            //Console.WriteLine("Hello, World!");
            //var gamma = new Gamma(2.0, 1.5);

            //// distribution properties
            //double mean = gamma.Mean;
            //double variance = gamma.Variance;
            //double entropy = gamma.Entropy;

            //// distribution functions
            //double a = gamma.Density(2.3); // PDF
            //double b = gamma.DensityLn(2.3); // ln(PDF)
            //double c = gamma.CumulativeDistribution(0.7); // CDF

            //// non-uniform number sampling
            //double randomSample = gamma.Sample();

            //using System;

            int numSimulations = 10;
            int attackerDiceCount = 4;
            int defenderDiceCount = 3;
            int attackerArmies = 10;
            int defenderArmies = 8;

            //int[] probabilityDistribution = new int[attackerArmies];
            Dictionary<int, int> probabilityDistribution = new Dictionary<int, int>();
            int wins = 0;

            for (int i = 0; i < numSimulations; i++)
            {
                int currentAttackerArmies = attackerArmies;
                int currentDefenderArmies = defenderArmies;

                while (currentAttackerArmies > 0 && currentDefenderArmies > 0)
                {
                    int[] attackerRolls = RollDice(attackerDiceCount);
                    int[] defenderRolls = RollDice(defenderDiceCount);

                    Array.Sort(attackerRolls, (a, b) => b.CompareTo(a));
                    Array.Sort(defenderRolls, (a, b) => b.CompareTo(a));

                    for (int j = 0; j < Math.Min(attackerDiceCount, defenderDiceCount); j++)
                    {
                        if (attackerRolls[j] > defenderRolls[j])
                        {
                            currentDefenderArmies--;
                        }
                        else
                        {
                            currentAttackerArmies--;
                        }
                    }
                }

                int x = currentAttackerArmies - currentDefenderArmies;
                probabilityDistribution[x]++;

                if (currentDefenderArmies == 0)
                {
                    wins++;
                }
            }

            Console.WriteLine("Probability Distribution Table:");
            Console.WriteLine("X\tProbability");
            int i = 0;
            foreach (var pair in probabilityDistribution)
            {
                double probability = (double)pair.Value / numSimulations;
                Console.WriteLine($"{i - Math.Abs(defenderArmies)}\t{probability:P}"); //might work
                i++;
            }

            Console.WriteLine("\nProbability Distribution Histogram:");
            DrawHistogram(probabilityDistribution, defenderArmies, numSimulations);

            double expectedValue = CalculateExpectedValue(probabilityDistribution);
            Console.WriteLine($"\nE(X): {expectedValue}");

            double winProbability = (double)wins / numSimulations;
            Console.WriteLine($"Probability of Attacker Winning: {winProbability:P}");

            // Comparing with the conventional Risk result
            double conventionalRiskWinProbability = 0.72; // 72%
            Console.WriteLine($"Conventional Risk Attacker Win Probability: {conventionalRiskWinProbability:P}");

            double standardDeviation = CalculateStandardDeviation(probabilityDistribution, expectedValue);
            Console.WriteLine($"Standard Deviation of X: {standardDeviation}");
        }


        static int[] RollDice(int count)
        {
            int[] rolls = new int[count];
            for (int i = 0; i < count; i++)
            {
                rolls[i] = random.Next(1, 7);
            }
            return rolls;
        }

        static double CalculateExpectedValue(Dictionary<int, int> probabilityDistribution)
        {
            double expectedValue = 0;
            for (int i = 0; i < probabilityDistribution.Count; i++)
            {
                expectedValue += i * probabilityDistribution.ElementAt(i).Value;
            }
            return expectedValue / probabilityDistribution.Count;
        }

        static double CalculateStandardDeviation(Dictionary<int, int> probabilityDistribution, double expectedValue)
        {
            double mean = 0;
            foreach(var p in probabilityDistribution)
            {
                mean += p.Key * p.Value;
                

            }

            //(key - mean)^2 * value(output)  sum all of them then sqrt it 
            mean = (probabilityDistribution.ElementAt(0).Value + probabilityDistribution.ElementAt(1).Value + probabilityDistribution.ElementAt(2).Value + probabilityDistribution.ElementAt(3))/ probabilityDistribution.Count;
            double sum = 0;

            foreach (var pair in probabilityDistribution)
            {
                double deviation =pair.Value- expectedValue;
                sum += pair.Value * deviation * deviation;   
                
                i++;
            }
            return Math.Sqrt(sum / probabilityDistribution.Count);
        }

        static void DrawHistogram(Dictionary<int, int> probabilityDistribution, int defenderArmies, int numSimulations)
        {
            for (int i = 0; i < probabilityDistribution.Count; i++)
            {
                int xValue = i - Math.Abs(defenderArmies);
                Console.Write($"{xValue,3}: |");

                int barLength = (int)(probabilityDistribution[i] * 100.0 / numSimulations);
                for (int j = 0; j < barLength; j++)
                {
                    Console.Write("#");
                }

                Console.WriteLine();
            }
        }


    }
}
