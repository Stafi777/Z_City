using System.Collections.Generic;

namespace ClassLibrary
{
    public class Graph
    {
        public Graph()
        {
            RootsArray = new List<Roots>();
            EdgesArray = new List<Edges>();
        }

        // Количество вершин
        public int RootsCount
        {
            get { return RootsArray.Count; }
        }

        // Количество ребер
        public int EdgesCount
        {
            get { return EdgesArray.Count; }
        }

        // Массив вершин
        public List<Roots> RootsArray { get; set; }

        // Массив ребер
        public List<Edges> EdgesArray { get; set; }
    }
}
