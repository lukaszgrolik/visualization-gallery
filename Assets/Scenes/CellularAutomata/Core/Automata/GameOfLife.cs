using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Automata.GameOfLife
{
    public class CellItem : CA.CellItem
    {
        public CellItem(int birthIteration) : base(birthIteration)
        {

        }
    }

    // interface ICellItem
    // {
    //     int Age { get; }
    //     void IncrementAge();
    // }

    public class GameOfLife : CA.CellsGrid
    {
        private int seed;
        private float chanceForInitialSpawn;

        private System.Random prng;

        public GameOfLife(
            Vector2Int gridSize,
            int seed,
            float chanceForInitialSpawn
        ) : base(
            gridSize
        )
        {
            this.seed = seed;
            this.chanceForInitialSpawn = chanceForInitialSpawn;

            prng = new System.Random(seed);
        }

        protected override CA.CellItem CreateCellItem(int iteration)
        {
            var item = new CellItem(
                birthIteration: iteration
            );

            return item;
        }

        protected override void OnInitialIteration()
        {
            // var cell = GetCellByPosition(gridSize.x / 2, gridSize.y / 2);

            // SpawnCellItem(cell);

            for (int y = 0; y < gridSize.y; y++)
            {
                for (int x = 0; x < gridSize.x; x++)
                {
                    // if (Random.value <= chanceForInitialSpawn)
                    if (prng.NextDouble() <= chanceForInitialSpawn)
                    {
                        var cell = currentGridState.GetCellByPosition(x, y);
                        SpawnCellItem(cell);
                    }
                }
            }
        }

        protected override void OnTick()
        {
            for (int i = 0; i < previousGridState.enabledCells.Count; i++)
            {
                var cell = previousGridState.enabledCells[i];
                // var cellItem = cell.Item as CellItem;

                var nbCells = previousGridState.EnabledNeighborCells(cell);
                if (nbCells.Count < 2 || nbCells.Count > 3)
                {
                    DestroyCellItem(cell);
                }
            }

            for (int i = 0; i < previousGridState.freeCells.Count; i++)
            {
                var cell = previousGridState.freeCells[i];
                // var cellItem = cell.Item as CellItem;

                var nbCells = previousGridState.EnabledNeighborCells(cell);
                if (nbCells.Count == 3)
                {
                    SpawnCellItem(cell);
                }
            }
        }
    }
}