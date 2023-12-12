using MathNet.Numerics.Distributions;
using MathNet.Numerics.Random;
using System.Linq;

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

            int numSimulations = 1000000;
            int attackerDiceCount = 4;
            int defenderDiceCount = 3;
            int attackerArmies = 10;
            int defenderArmies = 8;

            //int[] probabilityDistribution = new int[attackerArmies];
            Dictionary<int, double> probabilityDistribution = new Dictionary<int, double>();
            for (int i = -8; i < 11; i++)
            {
                probabilityDistribution.Add(i, 0);
            }
            int wins = 0;

            for (int i = 0; i < numSimulations; i++)
            {
                int currentAttackerArmies = attackerArmies;
                int currentDefenderArmies = defenderArmies;

                while (currentAttackerArmies > 0 && currentDefenderArmies > 0)
                {
                    int[] attackerRolls = RollDice(attackerDiceCount);
                    int[] defenderRolls = RollDice(defenderDiceCount);

                    attackerRolls = attackerRolls.OrderByDescending(x => x).ToArray();
                    defenderRolls = defenderRolls.OrderByDescending(x => x).ToArray();

                    for (int j = 0; j < Math.Min(attackerDiceCount, defenderDiceCount); j++)
                    {
                        if (currentDefenderArmies == 0 || currentAttackerArmies == 0) break;

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
            foreach(var pair in probabilityDistribution)
            {
                probabilityDistribution[pair.Key] = pair.Value / numSimulations;
            }
            Console.WriteLine("Probability Distribution Table:");
            Console.WriteLine("X\tProbability");
            int a = 0;
            foreach (var pair in probabilityDistribution.OrderBy(x => x.Key))
            {
       
                Console.WriteLine($"{pair.Key}\t{pair.Value:P}"); //might work
                a++;
            }

            Console.WriteLine("\nProbability Distribution Histogram:");
            DrawHistogram(probabilityDistribution, defenderArmies, numSimulations);

            double expectedValue = CalculateExpectedValue(probabilityDistribution);
            Console.WriteLine($"\nE(X): {expectedValue}");

            double winProbability = (double)wins / numSimulations;
            Console.WriteLine($"Probability of Attacker Winning: {winProbability:P}");

            // Comparing with the conventional Risk result
            double conventionalRiskWinProbability = .5833; // 72%
            Console.WriteLine($"Conventional Risk Attacker Win Probability: {conventionalRiskWinProbability:P}");

            double standardDeviation = CalculateStandardDeviation(probabilityDistribution, expectedValue, expectedValue);
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

        static double CalculateExpectedValue(Dictionary<int, double> probabilityDistribution)
        {
            double expectedValue = 0;
            foreach(var pair in probabilityDistribution)
            {
                expectedValue += pair.Key * pair.Value;
            }
            return expectedValue;
        }

        static double CalculateStandardDeviation(Dictionary<int, double> probabilityDistribution, double expectedValue, double mean)
        {
          

            //(key - mean)^2 * value(output)  sum all of them then sqrt it 
            double sum = 0;

            foreach (var pair in probabilityDistribution.OrderBy(x=>x.Key))
            {
                double deviation = Math.Pow((pair.Key - mean), 2) * pair.Value;
                sum += deviation;   
                
              //  i++;
            }
            return Math.Sqrt(sum);
        }

        static void DrawHistogram(Dictionary<int, double> probabilityDistribution, int defenderArmies, int numSimulations)
        {
            foreach(var pair in probabilityDistribution.OrderBy(x => x.Key))
            {
                int xValue = pair.Key - Math.Abs(defenderArmies);
                Console.Write($"{xValue,3}: |");

                int barLength = (int)(pair.Value * 100.0 / numSimulations);
                for (int j = 0; j < barLength; j++)
                {
                    Console.Write("#");
                }

                Console.WriteLine();
            }
        }


    }
}
