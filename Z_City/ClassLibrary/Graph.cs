using System.Collections.Generic;

namespace ClassLibrary
{
    public class Graph
    {
        // Количество вершин
        public int RootsCount { get; set; }

        // Количество ребер
        public int EdgesCount { get; set; }

        // Массив вершин
        public List<Roots> RootsArray { get; set; }

        // Массив ребер
        public List<Edges> EdgesArray { get; set; }
    }
}
