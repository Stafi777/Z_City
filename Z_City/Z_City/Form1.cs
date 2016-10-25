using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ClassLibrary;
using Z_City.Properties;

namespace Z_City
{
    public partial class Form1 : Form
    {
        private readonly Graph _g;

        // матрица смежности
        private int[,] _c;

        // матрица расстояний
        private int[,] _d;

        // лямбда
        private int l;

        // изменение лямбды
        private int dl;

        public Form1()
        {
            InitializeComponent();

            // чтение input файла
            var inputParams = SplitText(Resources.input);
            var rootsCount = int.Parse(inputParams[0]);
            var edgesCount = int.Parse(inputParams[1]);
            _g = new Graph();
            l = 0;

            for (var i = 1; i <= rootsCount; i++)
            {
                _g.RootsArray.Add(new Roots
                {
                    RootName = inputParams[2 * i],
                    RootWeight = int.Parse(inputParams[2 * i + 1])
                });
            }
            InitConnectionMatrix();

            for (var i = 0; i < edgesCount; i++)
            {
                var ind = (rootsCount + 1) * 2 + i * 4;
                var edge = new Edges
                {
                    StartRoot = _g.RootsArray.FirstOrDefault(r => r.RootName == inputParams[ind]),
                    EndRoot = _g.RootsArray.FirstOrDefault(r => r.RootName == inputParams[ind + 1]),
                    EdgeWeight = int.Parse(inputParams[ind + 2]),
                    Orientation = inputParams[ind + 3] == "1"
                };

                _g.EdgesArray.Add(edge);

                var startInd = _g.RootsArray.FindIndex(r => r.RootName == inputParams[ind]);
                var endInd = _g.RootsArray.FindIndex(r => r.RootName == inputParams[ind + 1]);
                _c[startInd, endInd] = edge.EdgeWeight;
                if (!edge.Orientation)
                {
                    _c[endInd, startInd] = edge.EdgeWeight;
                }

                if (edge.EdgeWeight > l)
                {
                    l = edge.EdgeWeight;
                }
            }
            dl = l / 5;

            CalculateDistance();
        }

        private List<string> SplitText(string s)
        {
            // разделяющие знаки
            char[] separateSymbols = { ' ', ',', '.', '\r', '\n'};

            // результирующий список
            var splitList = s.Split(separateSymbols, StringSplitOptions.RemoveEmptyEntries).ToList();

            return splitList;
        }

        private void InitConnectionMatrix()
        {
            _c = new int[_g.RootsCount, _g.RootsCount];
            for (var i = 0; i < _g.RootsCount; i++)
            {
                for (var j = 0; j < _g.RootsCount; j++)
                {
                    _c[i, j] = 0;
                }
            }
        }

        private void InitDistanceMatrix()
        {
            _d = new int[_g.RootsCount, _g.RootsCount];
            for (var i = 0; i < _g.RootsCount; i++)
            {
                for (var j = 0; j < _g.RootsCount; j++)
                {
                    _d[i, j] = _c[i, j] != 0 ? _c[i, j] : 100000;
                }
            }
        }

        private void CalculateDistance()
        {
            InitDistanceMatrix();

            for (var k = 0; k < _g.RootsCount; k++)
            {
                for (var i = 0; i < _g.RootsCount; i++)
                {
                    for (var j = 0; j < _g.RootsCount; j++)
                    {
                        _d[i, j] = Math.Min(_d[i, j], _d[i, k] + _d[k, j]);
                    }
                }
            }
        }

        private void FindRoots()
        {
            // массив лучших смещений для ребер
            var points = new int[_g.EdgesCount];
            // массив достигаемых вершин
            int[][] edgesRoots;

            do
            {
                edgesRoots = new int[_g.EdgesCount][];

                // проходим по всем ребрам
                for (var i = 0; i < _g.EdgesCount; i++)
                {
                    edgesRoots[i] = new int[_g.RootsCount];

                    // продвигаясь по каждому ребру
                    for (var j = 1; j < _g.EdgesArray[i].EdgeWeight; j++)
                    {
                        var roots = new int[_g.RootsCount];
                        // находим все доступные вершины
                        for (var k = 0; k < _g.RootsCount; k++)
                        {
                            roots[k] = RootIsAccessible(_g.EdgesArray[i], _g.RootsArray[k], j) ? 1 : 0;
                        }

                        // если доступных вершин стало больше - обновляем значения
                        if (edgesRoots[i].Sum(e => e) < roots.Sum(e => e))
                        {
                            points[i] = j;
                            edgesRoots[i] = roots;
                        }
                    }
                }

                l += dl;
            }
            while (!IsCovered(edgesRoots));

            FindMinCover(edgesRoots, points);
        }

        private bool RootIsAccessible(Edges edge, Roots end, int dif)
        {
            var firstStartInd = _g.RootsArray.IndexOf(edge.StartRoot);
            var secondStartInd = _g.RootsArray.IndexOf(edge.EndRoot);
            var endInd = _g.RootsArray.IndexOf(end);
            var dFirst = _d[firstStartInd, endInd] + dif;
            var dSecond = _d[secondStartInd, endInd] + (edge.EdgeWeight - dif);
            var d = Math.Min(dFirst, dSecond);

            return d * end.RootWeight <= l;
        }

        private bool IsCovered(int[][] roots)
        {
            for (var i = 0; i < _g.RootsCount; i++)
            {
                var covered = false;
                for (var j = 0; j < _g.EdgesCount; j++)
                {
                    if (roots[j][i] == 1)
                    {
                        covered = true;
                    }
                }

                if (!covered) return false;
            }

            return true;
        }

        private void FindMinCover(int[][] roots, int[] points)
        {
            // столбцы, которые покрывает одна строка
            var cores = new List<int>();
            for (var i = 0; i < _g.RootsCount; i++)
            {
                var sum = 0;
                for (var j = 0; j < _g.EdgesCount; j++)
                {
                    sum += roots[j][i];
                }

                if (sum == 1)
                {
                    cores.Add(i);
                }
            }
        }

        private void button_Calc_Click(object sender, EventArgs e)
        {
            FindRoots();
        }
    }
}
