using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crtical_Path
{
    class Program
    {
        public static int Activity_Num =0;
        public static string Critical_Path = "";
        public static string Critical_Path_Str = "";
        public static Dictionary<Block, List<Block>> Grid = new Dictionary<Block, List<Block>>();
        public struct Block
        {
            public int ES, EF, LS, LF, Dur;
            public char Name;
            public bool Flg;
            public Block(int es, int ef, int ls, int lf, int dur, char name)
            {
                this.ES = es;
                this.EF = ef;
                this.LS = ls;
                this.LF = lf;
                this.Dur = dur;
                this.Name = name;
                this.Flg = false;
            }
        }
        public static void Calculate()
        {
            //Forword Path Calculations
           for (int count = 0; count < Grid.Count; count++)
            {
                var item = Grid.ElementAt(count);
                var xKey = item.Key;
                var xValueList = item.Value;
        
                int Max = 0;
                for (int i = 0; i < xValueList.Count(); i++)
                {
                    if (xValueList[i].Name == '-')
                    {
                        xKey.ES = 0;
                        xKey.EF = xKey.ES + xKey.Dur;
                    }
                    else if ((xValueList[i].Name != '-') && (xValueList.Count == 1))
                    {
                        xKey.ES = xValueList[0].EF;
                        xKey.EF = xKey.ES + xKey.Dur;
                    }
                    else
                    {
                        int var1 = 0, var2 = 0;
                        var1 = xValueList[i].EF;
                        if (i + 1 != xValueList.Count()-1)
                        {
                            var2 = xValueList[i + 1].EF;
                            Max = Math.Max(var1, var2);
                        }
                        else
                        {
                            var2 = xValueList[i + 1].EF;
                            Max = Math.Max(var1, var2);
                            xKey.ES = Max;
                            xKey.EF = xKey.ES + xKey.Dur;
                            break;
                        }
                    }
                }
                Grid.Remove(item.Key);
                Grid.Add(xKey, xValueList);
                Update();
            }

            //Backward Path Calculations
            bool flag = false, flag2=false;
            for (int cnt = Grid.Count-1; cnt >=0; cnt--)
            {
                var item = Grid.ElementAt(cnt);
                var xKey = item.Key;
                var xValueList_rev = item.Value;
                int cntr = 0;
                int[] arr = new int[30];
                int y = 0;
                for (int count = 0; count < Grid.Count; count++)
                {
                    var element = Grid.ElementAt(count);
                    var key = element.Key;
                    var val = element.Value;
                    for (int k = 0; k < val.Count(); k++)
                    {
                        if (xKey.Name == val[k].Name)
                            cntr++;
                    }
                }
                    if (flag == false)
                    {
                        xKey.LF = xKey.EF;
                        xKey.LS = xKey.LF - xKey.Dur;
                        for (int i = 0; i < xValueList_rev.Count(); i++)
                        {
                            Block Temp = new Block();
                            Temp = xValueList_rev[i];
                            Temp.LF = xKey.LS;
                            Temp.LS = Temp.LF - Temp.Dur;
                            xValueList_rev[i] = Temp;
                        }
                        flag = true;
                    }
                    else if (flag == true)
                    {
                        if (cntr >1)
                        {
                            for (int x = 0; x < Grid.Count; x++)
                            {
                                var element = Grid.ElementAt(x);
                                var keyx = element.Key;
                                var valx = element.Value;
                                for (int k = 0; k < valx.Count(); k++)
                                {
                                    if (xKey.Name == valx[k].Name)
                                    {
                                        arr[y] = keyx.LS;
                                        y++;
                                    }
                                }
                            }
                            int Min = GetMin(arr, y);
                            xKey.LF = Min;
                            xKey.LS = xKey.LF - xKey.Dur;
                            xKey.Flg = true;
                            flag2 = true;
                             for (int count = 0; count < Grid.Count; count++)
                        {
                            var elem = Grid.ElementAt(count);
                            var keyS = elem.Key;
                            var valS = elem.Value;
                            for(int z=0; z<valS.Count(); z++)
                            {
                                if (xKey.Name == valS[z].Name)
                                {
                                    Block Tempz = new Block();
                                    Tempz = valS[z];
                                    Tempz.Flg = true;
                                    valS[z] = Tempz;
                                }
                            }
                        }
                       }
                        else if ((xValueList_rev.Count == 1))
                        {
                            Block Temp = new Block();
                            Temp = xValueList_rev[0];
                            Temp.LF = xKey.LS;
                            Temp.LS = Temp.LF - Temp.Dur;
                            xValueList_rev[0] = Temp;
                            flag2 = false;
                        }

                        else if (xValueList_rev.Count > 1)
                        {
                            for (int j = 0; j < xValueList_rev.Count(); j++)
                            {
                                Block Temp = new Block();
                                Temp = xValueList_rev[j];
                                Temp.LF = xKey.LS;
                                Temp.LS = Temp.LF - Temp.Dur;
                                xValueList_rev[j] = Temp;
                            }
                            flag2 = false;
                        }
                    }
                Grid.Remove(item.Key);
                Grid.Add(xKey, xValueList_rev);
                if(flag2)
                {
                    flag2 = false;
                }
                else
                    Update_Rev();
            }
        }
        public static int GetMin(int [] arr, int y)
        {
            int Min = arr[0];
            Min = Math.Min(Min, arr[1]);
            for (int i = 1; i < y-1; i++)
            {
                if(i==0)
                    Min = Math.Min(Min, arr[i + 1]);
            }
                return Min;
        }
        public static void Update()
        {

            for (int count = 1; count < Grid.Count; count++)
            {
                var item = Grid.ElementAt(count);
                var xKey = item.Key;
                var xValueList = item.Value;
                for (int i = 0; i < xValueList.Count(); i++)
                {
                    Block Temp = new Block();
                    for (int j = 0; j < Grid.Count; j++)
                    {
                        var element = Grid.ElementAt(j);
                        var key = element.Key;
                        if (key.Name == xValueList[i].Name)
                        {
                            Temp = xValueList[i];
                            Temp.ES = key.ES;
                            Temp.EF = key.EF;
                            Temp.LS = key.LS;
                            Temp.LF = key.LF;
                            Temp.Name = key.Name;
                            Temp.Dur = key.Dur;
                            xValueList[i] = Temp;
                        }
                    }
                }
            }
        }
        public static void Update_Rev()
        {

            for (int count = 1; count < Grid.Count; count++)
            {
                var item = Grid.ElementAt(count);
                var xKey = item.Key;
                var xValueList = item.Value;
                for (int i = 0; i < xValueList.Count(); i++)
                {
                    Block Temp = new Block();
                    for (int j = 0; j < Grid.Count; j++)
                    {
                        var element = Grid.ElementAt(j);
                        var key = element.Key;
                        var val = element.Value;
                        if ((key.Name == xValueList[i].Name) && (xValueList[i].Flg==false))
                        {
                            Temp = xValueList[i];
                            Grid.Remove(element.Key);
                            Grid.Add(Temp, val);
                        }
                    }
                }
            }
        }
        public static void FindCriticalPath()
        {
            for (int count = 0; count < Grid.Count; count++)
            {
                var item = Grid.ElementAt(count);
                var xKey = item.Key;
                if (xKey.EF == xKey.LF)
                    Critical_Path = Critical_Path + xKey.Name + " - ";
            }
        }
        private static void Main(string[] args)
        {
            Console.WriteLine("Enter Number Of Activities: ");
            Activity_Num = Int32.Parse(Console.ReadLine());
            for (int i = 0; i < Activity_Num; i++)
            {
                Block KeyInput = new Block();
                Block B=new Block();
                List<Block> ValueInput = new List<Block>();

                Console.WriteLine("Enter Activity Character: ");
                char Activity = Console.ReadKey().KeyChar;
                Console.WriteLine();
                Console.WriteLine("Enter Activity Duration: ");
                int Duration = Int32.Parse(Console.ReadLine());
                Console.WriteLine("Enter Activity Dependencies: ");
                string Predecessor = Console.ReadLine();
                KeyInput.Name = Activity;
                KeyInput.Dur = Duration;
                if (Predecessor.Count() == 1)
                {
                    B.Name = Predecessor[0];
                    ValueInput.Add(B);
                }
                else
                {
                    for (int j = 0; j < Predecessor.Count(); j++)
                    {
                        if (j == 0)
                        {
                            B.Name = Predecessor[j]; //A,B,C
                            ValueInput.Add(B);
                        }
                        else
                        {
                            B.Name = Predecessor[j + 1];
                            ValueInput.Add(B);
                        }
                        if (j + 1 == (Predecessor.Count() - 1))
                            break;
                    }
                }
                Grid.Add(KeyInput, ValueInput);
            }
            Calculate();
            FindCriticalPath();
            Console.WriteLine("Critical Path Is : ");
            Console.WriteLine(Critical_Path);

        }
    }
}
