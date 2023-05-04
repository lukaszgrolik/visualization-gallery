using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Main
{
    public sealed class CellularAutomataAlgo_TestAutomaton : CellularAutomataAlgo
    {
        [SerializeField] private int seed = 1;
        [SerializeField] private Vector2Int cellMaxAge = new Vector2Int(5, 50);
        [SerializeField][Min(1)] private int enableCellMinNeighbors = 4;
        [SerializeField][Range(0, 1)] private float newCellChance = .75f;
        [SerializeField] private Vector2Int newCellAmount = new Vector2Int(1, 5);
        [SerializeField] private Vector2 cellMaxEnergy = new Vector2(.75f, 1.5f);
        [SerializeField][Min(0)] private float energyChangeRate = .05f;
        [SerializeField] private Vector2 cellInitialEnergyRatio = new Vector2(.1f, .25f);
        [SerializeField][Min(1)] private int energyPenaltyMinNeighbors = 6;
        [SerializeField][Min(0)] private float cellAgeGrowth = .1f;

        public override Core.CA.CellsGrid CreateCellsGrid(Vector2Int gridSize)
        {
            return new Core.Automata.TestAutomaton.TestAutomaton(
                gridSize: gridSize,
                seed: seed,
                cellMaxAge: cellMaxAge,
                enableCellMinNeighbors: enableCellMinNeighbors,
                newCellChance: newCellChance,
                newCellAmount: newCellAmount,
                cellMaxEnergy: cellMaxEnergy,
                energyChangeRate: energyChangeRate,
                cellInitialEnergyRatio: cellInitialEnergyRatio,
                energyPenaltyMinNeighbors: energyPenaltyMinNeighbors
            );
        }

        public override CellItem OnCellItemSpawned(GameObject cellItemObj, Core.CA.Cell cell, CellularAutomata cellularAutomata)
        {
            var cellItemScript = cellItemObj.AddComponent<CellItem_TestAutomaton>();
            cellItemScript.Setup(
                // cellItem: cell.Item as Core.Automata.TestAutomaton.CellItem,
                cellItem: cell.Item as Core.Automata.TestAutomaton.CellItem,
                maxSize: cellularAutomata.CellItemSize,
                ageGrowth: cellAgeGrowth
            );

            return cellItemScript;
        }
    }
}
