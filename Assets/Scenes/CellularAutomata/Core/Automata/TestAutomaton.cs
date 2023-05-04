using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Automata.TestAutomaton
{
    public class CellItem : CA.CellItem
    {
        public readonly int maxAge;
        private int age; public int Age => age;

        public float Progress => (float)age / maxAge;

        public readonly float maxEnergy;
        public readonly float energyChangeRate;
        private float energy; public float Energy => energy;

        public float EnergyProgress => energy / maxEnergy;

        public event System.Action<CellItem> aged;
        public event System.Action<CellItem> energyChanged;

        public CellItem(
            int birthIteration,
            int maxAge,
            int age,
            float maxEnergy,
            float energyChangeRate,
            float energy
        ) : base(birthIteration)
        {
            this.maxAge = maxAge;
            this.age = age;
            this.maxEnergy = maxEnergy;
            this.energyChangeRate = energyChangeRate;
            this.energy = energy;
        }

        public void IncrementAge()
        {
            age += 1;

            aged?.Invoke(this);
        }

        public void DecreaseEnergy()
        {
            energy -= energyChangeRate;

            if (energy < 0) energy = 0;

            energyChanged?.Invoke(this);
        }

        public void IncreaseEnergy()
        {
            energy += energyChangeRate;

            if (energy > maxEnergy) energy = maxEnergy;

            energyChanged?.Invoke(this);
        }
    }

    // interface ICellItem
    // {
    //     int Age { get; }
    //     void IncrementAge();
    // }

    // class BetterRandom
    // {
    //     private System.Random prng;

    //     public BetterRandom(int seed)
    //     {
    //         prng = new System.Random(seed);
    //     }

    //     public float value => (float)prng.NextDouble();

    //     public int Range(int min, int max)
    //     {
    //         return prng.Next(min, max);
    //     }

    //     public float Range(float min, float max)
    //     {
    //         return min + this.value * (max - min);
    //     }
    // }

    // namespace System {
    static class RandomExtensions
    {
        public static float Next(this System.Random random, float min, float max)
        {
            return min + (float)random.NextDouble() * (max - min);
        }
    }
    // }

    public class TestAutomaton : CA.CellsGrid
    {
        private Vector2Int cellMaxAge;
        private int seed;
        private int enableCellMinNeighbors;
        private float newCellChance;
        private Vector2Int newCellAmount;
        private Vector2 cellMaxEnergy;
        private float energyChangeRate;
        private Vector2 cellInitialEnergyRatio;
        private int energyPenaltyMinNeighbors;

        private System.Random prng;

        public TestAutomaton(
            Vector2Int gridSize,
            int seed,
            Vector2Int cellMaxAge,
            int enableCellMinNeighbors,
            float newCellChance,
            Vector2Int newCellAmount,
            Vector2 cellMaxEnergy,
            float energyChangeRate,
            Vector2 cellInitialEnergyRatio,
            int energyPenaltyMinNeighbors
        ) : base(
            gridSize
        )
        {
            this.cellMaxAge = cellMaxAge;
            this.seed = seed;
            this.enableCellMinNeighbors = enableCellMinNeighbors;
            this.newCellChance = newCellChance;
            this.newCellAmount = newCellAmount;
            this.cellMaxEnergy = cellMaxEnergy;
            this.energyChangeRate = energyChangeRate;
            this.cellInitialEnergyRatio = cellInitialEnergyRatio;
            this.energyPenaltyMinNeighbors = energyPenaltyMinNeighbors;

            prng = new System.Random(seed);
        }

        protected override CA.CellItem CreateCellItem(int iteration)
        {
            var maxEnergy = prng.Next(cellMaxEnergy.x, cellMaxEnergy.y);
            var energyRatio = prng.Next(cellInitialEnergyRatio.x, cellInitialEnergyRatio.y);
            var energy = maxEnergy * energyRatio;
            var item = new CellItem(
                birthIteration: iteration,
                maxAge: prng.Next(cellMaxAge.x, cellMaxAge.y),
                age: 0,
                maxEnergy: maxEnergy,
                energyChangeRate: energyChangeRate,
                energy: energy
            );

            return item;
        }

        protected override void OnInitialIteration()
        {
            var cell = currentGridState.GetCellByPosition(gridSize.x / 2, gridSize.y / 2);

            SpawnCellItem(cell);
        }

        protected override void OnTick()
        {
            for (int i = 0; i < currentGridState.freeCells.Count; i++)
            {
                var cell = currentGridState.freeCells[i];
                // if 4 neighbours, enable cell

                var nbCells = currentGridState.EnabledNeighborCells(cell);
                if (nbCells.Count >= enableCellMinNeighbors)
                {
                    SpawnCellItem(cell);
                }
            }

            if (prng.NextDouble() <= newCellChance)
            {
                var count = prng.Next(newCellAmount.x, newCellAmount.y);
                var cells = currentGridState.freeCells.RandomMany(count, prng);

                for (int i = 0; i < cells.Count; i++)
                {
                    SpawnCellItem(cells[i]);
                }
            }

            for (int i = 0; i < currentGridState.enabledCells.Count; i++)
            {
                var cell = currentGridState.enabledCells[i];
                var cellItem = cell.Item as CellItem;
                cellItem.IncrementAge();

                if (cellItem.Age > cellItem.maxAge)
                {
                    DestroyCellItem(cell);

                    continue;
                }

                var nbCells = currentGridState.EnabledNeighborCells(cell);
                // var maxNbCount = 8;
                if (nbCells.Count >= energyPenaltyMinNeighbors)
                {
                    // DestroyCellItem(cell);
                    // var x = nbCells.Count / maxNbCount;
                    // Random.Range();
                    cellItem.DecreaseEnergy();
                }
                else
                {
                    cellItem.IncreaseEnergy();
                }

                if (cellItem.Energy == 0)
                {
                    DestroyCellItem(cell);

                    continue;
                }
            }
        }
    }
}