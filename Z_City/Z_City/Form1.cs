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

namespace Z_City
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            // чтение iput файла
            string InputFile = "";
            StreamReader Stream1 = new StreamReader(@"C:\Users\Vladimir\Desktop\Город Z\ZCity\Z_City\Z_City\Z_City\data\input.txt", Encoding.GetEncoding(1251));
            InputFile = Stream1.ReadToEnd();

            List<string> InputParams = new List<string>();
            InputParams = SplitText(InputFile);

            var graph = new Graph
            {
                RootsCount = Convert.ToInt32(InputParams[0]),
                EdgesCount = Convert.ToInt32(InputParams[1])
            };
            for(int i = 1; i < 25; i++)
            {
                graph.RootsArray[i - 1].RootName = Convert.ToChar(InputParams[3 * i]);
                graph.RootsArray[i - 1].RootWeight = Convert.ToChar(InputParams[3 * i + 1]);
            }
            
        }

        public List<string> SplitText(string s)
        {
            // разделяющие знаки
            char[] SeparateSymbols = { ' ', ',', '.', '\r', '\n'};

            // результирующий список
            List<string> SplitList = new List<string>();

            SplitList = s.Split(SeparateSymbols).ToList<string>();

            return SplitList;
        }
    }
}
