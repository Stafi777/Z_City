using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClassLibrary;
using Z_City.Properties;

namespace Z_City
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            // чтение input файла
            //var stream = new StreamReader(Resources.input, Encoding.GetEncoding(1251));
            var inputParams = SplitText(Resources.input);
            var rootsCount = int.Parse(inputParams[0]);
            var edgesCount = int.Parse(inputParams[1]);
            var graph = new Graph();

            for(var i = 1; i <= rootsCount; i++)
            {
                graph.RootsArray.Add(new Roots
                {
                    RootName = inputParams[2 * i],
                    RootWeight = int.Parse(inputParams[2 * i + 1])
                });
            }

            for (var i = 0; i < edgesCount; i++)
            {
                var ind = (rootsCount + 1) * 2 + i * 4;
                graph.EdgesArray.Add(new Edges
                {
                    StartRoot = graph.RootsArray.FirstOrDefault(r => r.RootName == inputParams[ind]),
                    EndRoot = graph.RootsArray.FirstOrDefault(r => r.RootName == inputParams[ind + 1]),
                    EdgeWeight = int.Parse(inputParams[ind + 2]),
                    Orientation = inputParams[ind + 3] == "1"
                });
            }
        }

        public List<string> SplitText(string s)
        {
            // разделяющие знаки
            char[] separateSymbols = { ' ', ',', '.', '\r', '\n'};

            // результирующий список
            var splitList = s.Split(separateSymbols, StringSplitOptions.RemoveEmptyEntries).ToList();

            return splitList;
        }
    }
}
