using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Timers;
using System.Diagnostics;
namespace nonogram
{
    class Program
    {
        //設定線索
        public static List<List<int>> row = new List<List<int>>();
        public static List<List<int>> col = new List<List<int>>();
        
        static void Main(string[] args)
        {
            IOFile input = new IOFile();
            
            //假如有讀到一筆資料就做沒有就結束
            while (true)
            {
                string number="";
                row.Clear();
                col.Clear();
                Stopwatch sw = new Stopwatch();
                if (input.read(ref number, ref row, ref col))
                {
                    Console.WriteLine("讀到第"+number+"筆的資料");


                    Function.rowAll.Clear();
                    Function execute = new Function();

                    sw.Reset();
                    sw.Start();

                    execute.Propagate(1);
                    Console.WriteLine("完成第一次Propagate");

                    execute.FP2();
                    Console.WriteLine("完成FP2");
                /*    for (int i = 0; i < 25; i++)
                    {
                        for (int j = 0; j < 25; j++)
                        {
                            Console.Write(Function.rowAll[i][j]+" ");
                        }
                        Console.WriteLine();
                    }
                    Console.WriteLine();*/
                    List<List<char>> output0 = Function.rowAll.Select(X => X.ToList()).ToList();
                    List<List<char>> output1 =new List<List<char>>();

                    List<UnpaintPoint>xx = execute.MinLoged(Function.rowAll);
                    for (int i = 0; i < xx.Count; i++) {

                        Console.WriteLine(xx[i].Row+" "+xx[i].Col+" "+xx[i].Weight );
                    }

                    execute.backtrack(xx,Function.rowAll,ref output0,ref output1,0);
                    Function.rowAll = output1;
                    Console.WriteLine("backtracking完成");
                    sw.Stop();
                    for (int i = 0; i < 25; i++)
                    {
                        for (int j = 0; j < 25; j++)
                        {
                            Console.Write(Function.rowAll[i][j] + " ");
                        }
                        Console.WriteLine();
                    }
                    Console.WriteLine(); 
                     /*Console.Write("解出來的點");
                     Console.WriteLine(625 - execute.AllUnpaintPoint(output1).Count);*/
                     Console.Write("總共花的時間:");
                    Console.WriteLine((double)sw.ElapsedMilliseconds/1000.00+"秒="+ (double)sw.ElapsedMilliseconds /60000.00+"分");
                    Console.WriteLine("------已完成------------");
                    input.Write(number);
                    //Console.ReadKey();


                    }
                    else
                    {
                        Console.WriteLine("操作結束");
                        Console.ReadKey();
                        break;
                    };
                }

            }

        }
    }

