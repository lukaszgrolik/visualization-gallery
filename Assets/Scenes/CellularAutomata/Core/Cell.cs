namespace Core.CA
{
    public class Cell
    {
        public readonly int x;
        public readonly int y;

        private CellItem item; public CellItem Item => item;

        public Cell(int x, int y, CellItem item)
        {
            this.x = x;
            this.y = y;
            this.item = item;
        }

        public void SetItem(CellItem item)
        {
            this.item = item;
        }
    }
}