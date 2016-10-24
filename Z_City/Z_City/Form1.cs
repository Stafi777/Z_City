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
        private int[,] _c;
        private int[,] _d;
        private int l;
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
    }
}
