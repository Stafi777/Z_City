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
            var stream = new StreamReader(Resources.input, Encoding.GetEncoding(1251));
            var inputParams = SplitText(stream.ReadToEnd());

            var graph = new Graph
            {
                RootsCount = int.Parse(inputParams[0]),
                EdgesCount = int.Parse(inputParams[1])
            };
            for(var i = 1; i < 25; i++)
            {
                graph.RootsArray.Add(new Roots
                {
                    RootName = inputParams[3 * i],
                    RootWeight = int.Parse(inputParams[3 * i + 1])
                });
            }
        }

        public List<string> SplitText(string s)
        {
            // разделяющие знаки
            char[] separateSymbols = { ' ', ',', '.', '\r', '\n'};

            // результирующий список
            var splitList = s.Split(separateSymbols).ToList();

            return splitList;
        }
    }
}
