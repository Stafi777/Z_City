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

        // полученные значения точек
        private List<Edges> _y;

        // лямбда
        private int _l;

        // изменение лямбды
        private readonly int _dl;

        public Form1()
        {
            InitializeComponent();

            // чтение input файла
            var inputParams = SplitText(Resources.input);
            var rootsCount = int.Parse(inputParams[0]);
            var edgesCount = int.Parse(inputParams[1]);
            _g = new Graph();
            _l = 0;

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

                if (edge.EdgeWeight > _l)
                {
                    _l = edge.EdgeWeight;
                }
            }
            _dl = _l / 5;

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

                _l += _dl;
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

            return d * end.RootWeight <= _l;
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
            _y = new List<Edges>();

            // если есть строка, покрывающая все вершины
            for (var i = 0; i < _g.EdgesCount; i++)
            {
                var sum = 0;
                for (var j = 0; j < _g.RootsCount; j++)
                {
                    sum += roots[j][i];
                }

                if (sum == _g.RootsCount)
                {
                    _y.Add(new Edges
                    {
                        StartRoot = _g.EdgesArray[i].StartRoot,
                        EndRoot = _g.EdgesArray[i].EndRoot,
                        EdgeWeight = points[i]
                    });

                    return;
                }
            }

            // столбцы, которые покрывает одна строка
            for (var i = 0; i < _g.RootsCount; i++)
            {
                var sum = 0;
                var last = 0;
                for (var j = 0; j < _g.EdgesCount; j++)
                {
                    sum += roots[j][i];
                    if (roots[j][i] == 1)
                    {
                        last = j;
                    }
                }

                if (sum == 1)
                {
                    _y.Add(new Edges
                    {
                        StartRoot = _g.EdgesArray[last].StartRoot,
                        EndRoot = _g.EdgesArray[last].EndRoot,
                        EdgeWeight = points[last]
                    });
                }
            }

            // чистим строки, которые можно поглотить
            for (var i = 0; i < _g.EdgesCount; i++)
            {
                for (var j = 0; j < _g.EdgesCount; j++)
                {
                    if (i == j) continue;

                    if (Contains(roots[i], roots[j]) && roots[j].Any(r => r > 0))
                    {
                        Clear(roots[j]);
                    }
                }
            }

            var s = roots.Count(root => root.Any(r => r > 0));
        }

        // true, если массив а содержит массив b
        private bool Contains(int[] a, int[] b)
        {
            for (var i = 0; i < a.Length; i++)
            {
                if (b[i] > a[i]) return false;
            }

            return true;
        }

        private void Clear(int[] arr)
        {
            for (var i = 0; i < arr.Length; i++)
            {
                arr[i] = 0;
            }
        }

        private void button_Calc_Click(object sender, EventArgs e)
        {
            FindRoots();
        }
    }
}
