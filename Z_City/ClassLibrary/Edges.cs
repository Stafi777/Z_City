namespace ClassLibrary
{
    public class Edges
    {
        // Начало ребра
        public Roots StartRoot { get; set; }

        // Конец ребра
        public Roots EndRoot { get; set; }

        // Вес ребра
        public int EdgeWeight { get; set; }

        // Ориентированность
        public bool Orientation { get; set; }
    }
}
