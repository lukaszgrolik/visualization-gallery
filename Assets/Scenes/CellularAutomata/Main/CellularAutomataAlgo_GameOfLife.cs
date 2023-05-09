using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main
{
    public sealed class CellularAutomataAlgo_GameOfLife : CellularAutomataAlgo
    {
        [SerializeField] private int seed = 1;
        [SerializeField][Range(0f, 1f)] private float chanceForInitialSpawn = .1f;
        [SerializeField][Min(0)] private float growthSpeed = 0;
        [SerializeField][Min(1)] private int colorsCount = 12;
        [SerializeField][Min(1)] private int colorGenerationSize = 1;

        public override Core.CA.CellsGrid CreateCellsGrid(Vector2Int gridSize)
        {
            return new Core.Automata.GameOfLife.GameOfLife(
                gridSize: gridSize,
                seed: seed,
                chanceForInitialSpawn: chanceForInitialSpawn
            );
        }

        public override CellItem OnCellItemSpawned(GameObject cellItemObj, Core.CA.Cell cell, CellularAutomata cellularAutomata)
        {
            var cellItemScript = cellItemObj.AddComponent<CellItem_GameOfLife>();
            cellItemScript.Setup(
                cellItem: cell.Item as Core.Automata.GameOfLife.CellItem,
                cellItemSize: cellularAutomata.CellItemSize,
                growthSpeed: growthSpeed,
                colorsCount: colorsCount,
                colorGenerationSize: colorGenerationSize
            );

            return cellItemScript;
        }
    }
}
