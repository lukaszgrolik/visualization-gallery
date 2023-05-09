using System.Collections.Generic;
using UnityEngine;

namespace Core.CA
{
    public class GridState
    {
        public Vector2Int gridSize;
        public int iteration;

        public readonly List<Cell> cells;
        public readonly List<Cell> enabledCells;
        public readonly List<Cell> freeCells;

        private readonly Dictionary<Cell, List<Cell>> neighborCells = new Dictionary<Cell, List<Cell>>();

        public GridState(
            Vector2Int gridSize,
            int iteration = 0,
            List<Cell> cells = null,
            List<Cell> enabledCells = null,
            List<Cell> freeCells = null,
            Dictionary<Cell, List<Cell>> neighborCells = null
        )
        {
            this.gridSize = gridSize;
            this.iteration = iteration;
            this.cells = cells ?? new List<Cell>();
            this.enabledCells = enabledCells ?? new List<Cell>();
            this.freeCells = freeCells ?? new List<Cell>();
            this.neighborCells = neighborCells ?? new Dictionary<Cell, List<Cell>>();
        }

        public GridState Clone(int iteration)
        {
            var grid = new GridState(
                gridSize: gridSize,
                iteration: iteration,
                cells: new List<Cell>(cells),
                enabledCells: new List<Cell>(enabledCells),
                freeCells: new List<Cell>(freeCells),
                neighborCells: new Dictionary<Cell, List<Cell>>(neighborCells)
            );

            return grid;
        }

        public void FillGrid()
        {
            if (cells.Count != 0) throw new System.Exception("cells list is not empty");

            for (int y = 0; y < gridSize.y; y++)
            {
                for (int x = 0; x < gridSize.x; x++)
                {
                    var cell = new Cell(
                        x: x,
                        y: y,
                        item: null
                    );

                    cells.Add(cell);
                    freeCells.Add(cell);
                }
            }

            foreach (var cell in cells)
            {
                neighborCells.Add(cell, NeighborCells(cell));
            }
        }

        public Cell GetCellByPosition(int x, int y)
        {
            // Debug.Log($"{cells.Count} | {x},{y} | {y * gridSize.x + x} | ");

            return cells[y * gridSize.x + x];
        }

        List<Cell> NeighborCells(Cell cell)
        {
            var positions = new List<Vector2Int>(){
                // top
                new Vector2Int(-1, 1),
                new Vector2Int(0, 1),
                new Vector2Int(1, 1),
                // right center
                new Vector2Int(1, 0),
                // bottom
                new Vector2Int(1, -1),
                new Vector2Int(0, -1),
                new Vector2Int(-1, -1),
                // left center
                new Vector2Int(-1, 0)
            };

            var resultCells = new List<Cell>();

            for (int i = 0; i < positions.Count; i++)
            {
                var pos = positions[i];

                // filter out unexisting cells outside the edges of the grid
                if (
                    (cell.x == 0 && pos.x == -1) ||
                    (cell.x == gridSize.x - 1 && pos.x == 1) ||
                    (cell.y == 0 && pos.y == -1) ||
                    (cell.y == gridSize.y - 1 && pos.y == 1)
                )
                {
                    continue;
                }

                var nbCell = GetCellByPosition(cell.x + pos.x, cell.y + pos.y);

                resultCells.Add(nbCell);
            }

            return resultCells;
        }

        List<T> Intersection<T>(List<T> listA, List<T> listB)
        {
            var resultList = new List<T>();

            for (int i = 0; i < listA.Count; i++)
            {
                if (listB.Contains(listA[i]))
                {
                    resultList.Add(listA[i]);
                }
            }

            return resultList;
        }

        public List<Cell> EnabledNeighborCells(Cell cell)
        {
            return Intersection(neighborCells[cell], enabledCells);
        }
    }
}