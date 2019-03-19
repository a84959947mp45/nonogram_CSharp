using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;


namespace nonogram
{
    class UnpaintPoint {
        public UnpaintPoint(int RowX,int ColX)
        {
            this.Row = RowX;
            this.Col = ColX;
        }
        public int Row { set; get; }
        public int Col { set; get; }
        public double Weight{ set; get; }
        public bool use = false;
    }
    class Point
    {
        public Point(int RowX, int ColX,char PointX)
        {
            this.Row = RowX;
            this.Col = ColX;
            this.Point1 = PointX;
        }
        public int Row { set; get; }
        public int Col { set; get; }
        public char Point1 { set; get; }
    }
    class Function
    {
        #region construction
        //建構子
        public Function()
        {
            InitialMatrix = new List<char> { 'u', 'u', 'u', 'u', 'u', 'u', 'u', 'u', 'u', 'u' ,
            'u', 'u', 'u', 'u', 'u', 'u', 'u', 'u', 'u', 'u',
            'u', 'u', 'u', 'u', 'u'};
            for (int i = 0; i < 25; i++)
            {
                InitialMatrix2.Add(new List<char>(InitialMatrix));
                for (int j = 0; j < 25; j++) {
                    List0[i,j] = new List<Point>();
                    List1[i, j] = new List<Point>();
                }
            }
        }
        #endregion

        #region delaration
        //給大家使用一維
        private List<char> InitialMatrix = new List<char>();
        //給大家使用二維
        private  List<List<char>> InitialMatrix2 = new List<List<char>>();
        //所有的東西放這裡
        public static List<List<char>> rowAll = new List<List<char>>();
        //整個尚未塗上點的地方
        private List<UnpaintPoint> G = new List<UnpaintPoint>() ;
        private List<Point>[,] List0 = new List<Point>[25, 25];
        private List<Point>[,] List1 = new List<Point>[25, 25];
        public List<UnpaintPoint> getG()
        {
            return G;
        }

        #endregion

        #region propogate
        //一開始進入
        private bool Fix(int i, int j, List<char> rowValue, List<int> useKey, ref List<char> paint)
        {
            List<char> re0 = new List<char>(paint);
            List<char> re1 = new List<char>(paint);
            
            if (i <= 0 && j == 0)
            {

                return true;
            }
            else if ((i == 0 && j >= 1) || (i < 0))
            {
                return false;
            }
            else if (i + 1 == j)
            {
                return false;
            }
            else if (i >= 0 && j == 0)
            {
                List<char> XX = new List<char>(paint);
                //Console.WriteLine("沒事");
                for (int x = 0; x < i; x++)
                {
                    if (XX[x] == '1')
                    {
                        return false;
                    }
                    else if (XX[x] == 'u' || XX[x] == '0')
                    {
                        XX[x] = '0';
                    }
                }

                paint = XX;
                return true;
            }
            else
            {
                bool one = Fix1(i, j, rowValue, useKey, ref re1);
                bool zero = Fix0(i, j, rowValue, useKey, ref re0);

                if (one && zero)
                {

                    paint = match(re0, re1, i);


                    return true;
                }
                else if (zero)
                {

                    paint =re0;

                    return true;
                }
                else if (one)
                {

                    paint = re1;

                    return true;
                }
                else
                {

                    return false;
                }

            }
        }
        private bool Fix0(int i, int j, List<char> rowValue, List<int> useKey, ref List<char> paint)
        {
            
            List<char> re = new List<char>(rowValue);


            int x = i - 1;
            if (x < 0)
            {
                return false;
            }


            //檢查數字是否為0
            if (re[x] == 'u' || re[x] == '0')
            {
                re[x] = '0';

            }
            else
            {
                return false;
            }

            bool reX = Fix(i - 1, j, re, useKey, ref paint);
            if (reX)
            {

                paint = overrideMatrix(paint, re, i);

            }

            return reX;

        }
        private bool Fix1(int i, int j, List<char> rowValue, List<int> useKey, ref List<char> paint)
        {
           
            List<char> re = new List<char>(rowValue);
            int x = i - useKey[j - 1];

            if (x < 0)
            {
                return false;
            }
            //檢查數字
            for (; x <= i - 1; x++)
            {
                if (re[x] == 'u' || re[x] == '1')
                {
                    re[x] = '1';
                }
                else if (re[x] == '0')
                {
                    return false;
                }
            }

            //檢查最前面數字是否為0 (但假如已經是最前面就不用補0)
            if (i - useKey[j - 1] == 0)
            {

                bool reX = Fix(i - useKey[j - 1] - 1, j - 1, re, useKey, ref paint);
                if (reX)
                {
                    paint = overrideMatrix(paint, re, i);
                  
                }


                return reX;
            }
            //檢查是不是可以補0隔開
            else if (re[i - useKey[j - 1] - 1] == 'u' || re[i - useKey[j - 1] - 1] == '0')
            {
                re[i - useKey[j - 1] - 1] = '0';
                //填入的位數是第一位
                if (i - useKey[j - 1] - 1 == 0)
                {

                    bool reX = Fix(0, j - 1, re, useKey, ref paint);
                    if (reX)
                    {
                        paint = overrideMatrix(paint, re, i);
                        
                    }

                    return reX;
                }
                else
                {

                    bool reX = Fix(i - useKey[j - 1] - 1, j - 1, re, useKey, ref paint);
                    if (reX)
                    {
                        paint = overrideMatrix(paint, re, i);
                        
                    }

                    return reX;
                }
            }
            //不能補0
            else
            {
                return false;
            }
        }
        //Propagate
        public void Propagate(int mode)
        {
            //模式，第一次做跟第二次做不一樣
            if (mode == 1)
            {
                rowAll = new List<List<char>>(Copy(InitialMatrix2));
               
            }
            for (int i = 0; i < 25; i++)
            {
                List<char> re = new List<char>(rowAll[i]);
                bool xx = Fix(25, Program.row[i].Count, rowAll[i], Program.row[i], ref re);
                if (xx == true)
                {
                   rowAll[i] = new List<char>(re);

                }

            }
            //記錄橫的結果
            List<List<char>> record = new List<List<char>>(Copy(rowAll));
         

            //轉置
            ChangeMatrix(ref rowAll);
            //作全部直的25
            for (int i = 0; i < 25; i++)
            {
                List<char> re = new List<char>(rowAll[i]);
                bool xx = Fix(25, Program.col[i].Count, rowAll[i], Program.col[i], ref re);
                if (xx == true)
                {
                    rowAll[i] = new List<char>(re);

                }
                
            }
            //再轉回來
            ChangeMatrix(ref rowAll);
            
            //比對橫的改變
            List<int> changeItems = CompareMatrix(record, rowAll);


            //橫的有改
            if (changeItems.Count != 0)
            {
                while (true)
                {
                    //轉成直的
                    ChangeMatrix(ref rowAll);
                    //紀錄直的
                    record =new List<List<char>>( Copy(rowAll));


                    //再轉回來做橫改變的fix
                    ChangeMatrix(ref rowAll);
                    
                    //做橫的fix
                    if (changeItems.Count > 0)
                    {

                        for (int i = 0; i < changeItems.Count; i++)
                        {
                            List<char> re = new List<char>(rowAll[changeItems[i]]);
                            bool xx = Fix(25, Program.row[changeItems[i]].Count, rowAll[changeItems[i]], Program.row[changeItems[i]], ref re);
                            if (xx == true)
                            {
                                rowAll[changeItems[i]] = new List<char>(re);

                            }
                          
                        }
                    }
                    else
                    {

                        break;
                    }
                    //轉成直的
                    ChangeMatrix(ref rowAll);

                    //直的跟直的比
                    changeItems = CompareMatrix(record, rowAll);

                    //在轉回來橫的
                    ChangeMatrix(ref rowAll);
                    //紀錄橫
                    record = new List<List<char>>(Copy(rowAll));
                    //在轉回來直的
                    ChangeMatrix(ref rowAll);
                    //直改變的數量
                    if (changeItems.Count > 0)
                    {
                        //做直的
                        for (int i = 0; i < changeItems.Count; i++)
                        {
                            List<char> re = new List<char>(rowAll[changeItems[i]]);
                            bool xx = Fix(25, Program.col[changeItems[i]].Count, rowAll[changeItems[i]], Program.col[changeItems[i]], ref re);
                            if (xx == true)
                            {
                                rowAll[changeItems[i]] = new List<char>(re);

                            }
                            
                        }
                    }
                    else
                    {

                        ChangeMatrix(ref Function.rowAll);
                        break;
                    }
                    //轉回來橫的

                    ChangeMatrix(ref Function.rowAll);


                    //比對橫的改變
                    changeItems = CompareMatrix(record, Function.rowAll);
                   
                }
            }
           
        }
        //Propagate自訂
        private List<List<char>> PropagateOwn(List<List<char>>Own)
        {
            List<List<char>> reOwn = new List<List<char>>(Copy(Own));
                for (int i = 0; i < 25; i++)
                {
                    List<char> re = new List<char>(reOwn[i]);
                    bool xx = Fix(25, Program.row[i].Count, reOwn[i], Program.row[i], ref re);
                    if (xx == true)
                    {
                    reOwn[i] = new List<char>(re);

                    }

                }
            
            //記錄橫的結果
            List<List<char>> record = new List<List<char>>(Copy(reOwn));


            //轉置
            reOwn=OwnChangeMatrix(reOwn);
            //作全部直的25
            for (int i = 0; i < 25; i++)
            {
                List<char> re = new List<char>(reOwn[i]);
                bool xx = Fix(25, Program.col[i].Count, reOwn[i], Program.col[i], ref re);
                if (xx == true)
                {
                    reOwn[i] = new List<char>(re);

                }

            }
            //再轉回來
            reOwn = OwnChangeMatrix(reOwn);
            //比對橫的改變
            List<int> changeItems = CompareMatrix(record, reOwn);


            //橫的有改
            if (changeItems.Count != 0)
            {
                while (true)
                {
                    //轉成直的
                    reOwn = OwnChangeMatrix(reOwn);
                    //紀錄直的
                    record = new List<List<char>>(Copy(reOwn));


                    //再轉回來做橫改變的fix
                    reOwn = OwnChangeMatrix(reOwn);

                    //做橫的fix
                    if (changeItems.Count > 0)
                    {

                        for (int i = 0; i < changeItems.Count; i++)
                        {
                            List<char> re = new List<char>(reOwn[changeItems[i]]);
                            bool xx = Fix(25, Program.row[changeItems[i]].Count, reOwn[changeItems[i]], Program.row[changeItems[i]], ref re);
                            if (xx == true)
                            {
                                reOwn[changeItems[i]] = new List<char>(re);

                            }

                        }
                    }
                    else
                    {

                        break;
                    }
                    //轉成直的
                    reOwn = OwnChangeMatrix(reOwn);

                    //直的跟直的比
                    changeItems = CompareMatrix(record, reOwn);

                    //在轉回來橫的
                    reOwn = OwnChangeMatrix(reOwn);
                    //紀錄橫
                    record = new List<List<char>>(Copy(reOwn));
                    //在轉回來直的
                    reOwn = OwnChangeMatrix(reOwn);
                    //直改變的數量
                    if (changeItems.Count > 0)
                    {
                        //做直的
                        for (int i = 0; i < changeItems.Count; i++)
                        {
                            List<char> re = new List<char>(reOwn[changeItems[i]]);
                            bool xx = Fix(25, Program.col[changeItems[i]].Count, reOwn[changeItems[i]], Program.col[changeItems[i]], ref re);
                            if (xx == true)
                            {
                                reOwn[changeItems[i]] = new List<char>(re);

                            }

                        }
                    }
                    else
                    {

                        reOwn = OwnChangeMatrix(reOwn);
                        break;
                    }
                    //轉回來橫的

                    reOwn = OwnChangeMatrix(reOwn);


                    //比對橫的改變
                    changeItems = CompareMatrix(record, reOwn);

                }
            }
            return reOwn;
        }
        #endregion

        #region ToolMethon
        private List<char> overrideMatrix(List<char> A, List<char> B, int count)
        {
            List<char> matchUse = new List<char>(B);
            for (int i = 0; i < count; i++)
            {
                if ((A[i] == '1' && B[i] == 'u') || (A[i] == 'u' && B[i] == '1'))
                {
                    matchUse[i] = '1';
                }
                else if ((A[i] == '0' && B[i] == 'u') || (A[i] == 'u' && B[i] == '0'))
                {
                    matchUse[i] = '0';
                }


            }
            return matchUse;
        }
        //衝突的時候做Match
        private List<char> match(List<char> A, List<char> B, int count)
        {
            List<char> matchUse = new List<char>(A);
            for (int i = 0; i < count; i++)
            {
                if (A[i] == '1' && B[i] == '1')
                {
                    matchUse[i] = '1';
                }
                else if (A[i] == '0' && B[i] == '0')
                {
                    matchUse[i] = '0';
                }
                else
                {
                    matchUse[i] = 'u';
                }
            }

            return matchUse;
        }
        //複製二元LIST
        private List<List<char>> Copy(List<List<char>> copy)
        {

            List<List<char>> output = new List<List<char>>();
            for (int i = 0; i < copy.Count; i++)
            {
                output.Add(new List<char>(copy[i]));
            }


            return output;
        }
        //置換陣列
        private void ChangeMatrix(ref List<List<char>> rowAll)
        {
            /*  for(int i = 0; i < 25; i++)
              {
                  for(int j = 0; j < 25; j++)
                  {
                      Console.Write(InitialMatrix2[i][j]);
                  }
                  Console.WriteLine();
              }
              Console.WriteLine();*/
            List<List<char>> temp = new List<List<char>>(Copy(InitialMatrix2));

            for (int i = 0; i < 25; i++)
            {
                for (int j = 0; j < 25; j++)
                {

                    temp[j][i] = rowAll[i][j];
                }
            }


            rowAll = temp;
        }
        //轉置自訂陣列
        private List<List<char>> OwnChangeMatrix(List<List<char>> Own)
        {
            List<List<char>> temp = new List<List<char>>(Copy(InitialMatrix2));

            for (int i = 0; i < 25; i++)
            {
                for (int j = 0; j < 25; j++)
                {

                    temp[j][i] = Own[i][j];
                }
            }


            return temp;
        }
        //比對前後
        private List<int> CompareMatrix(List<List<char>> font, List<List<char>> after)
        {

            List<int> output = new List<int>();

            for (int i = 0; i < font.Count; i++)
            {
                for (int j = 0; j < font.Count; j++)
                {
                    if (font[i][j] != after[i][j])
                    {
                        output.Add(i);
                        break;
                    }
                }

            }
            return output;
        }
        //比對前後
        private List<Point> ComparePoint(List<List<char>> font, List<List<char>> after)
        {

            List<Point> output = new List<Point>();

            for (int i = 0; i < font.Count; i++)
            {
                for (int j = 0; j < font.Count; j++)
                {
                    if (font[i][j] != after[i][j])
                    {
                        output.Add(new Point(i, j, after[i][j]));
                        break;
                    }
                }

            }
            return output;
        }
        //全部沒有塗顏色的點
        public List<UnpaintPoint> AllUnpaintPoint()
        {
            List<UnpaintPoint> reG = new List<UnpaintPoint>();
            for (int i = 0; i < 25; i++)
            {
                for (int j = 0; j < 25; j++)
                {
                    if (rowAll[i][j] == 'u')
                    {
                        UnpaintPoint re = new UnpaintPoint(i , j);
                      
                        reG.Add(re);
                    }
                }
            }
            return reG;
        }
        //自訂盤面不塗顏色的地方
        public List<UnpaintPoint> AllUnpaintPoint(List<List<char>>Own)
        {
            List<UnpaintPoint> reG = new List<UnpaintPoint>();
            for (int i = 0; i < 25; i++)
            {
                for (int j = 0; j < 25; j++)
                {
                    if (Own[i][j] == 'u')
                    {
                        UnpaintPoint re = new UnpaintPoint(i,j);
                       
                        reG.Add(re);
                    }
                }
            }
            return reG;
        }
        //給出相反數
        private char Inverse(char c)
        {
            if (c == '0')
            {
                return '1';
            }
            else if (c == '1')
            {
                return '0';
            }
            else
            {
                return 'X';
            }

        }
        //比對兩邊Matrix
        private List<List<char>> PropagateMatrix(UnpaintPoint point)
        {

            List<List<char>> put0 = new List<List<char>>(Copy(rowAll));
            List<List<char>> put1 = new List<List<char>>(Copy(rowAll));
            List<List<char>> output = new List<List<char>>();

            put0[point.Row][point.Col] = '0';
            put1[point.Row][point.Col] = '1';

            put0 = PropagateOwn(put0);
            put1 = PropagateOwn(put1);

            for (int i = 0; i < 25; i++)
            {
                List<char> re = match(put0[i], put1[i], 25);
                output.Add(re);
            }

            return output;
        }
        #endregion

        #region FP
        public void FP2()
        {
            //初始化G
            G = new List<UnpaintPoint>(AllUnpaintPoint());
            while (true)
            {
                //判斷是否可以填入
               Propagate(2);
               
                //更新未填入點
                G = new List<UnpaintPoint>(AllUnpaintPoint());
                //全部都填
                if (G.Count == 0)
                {
                    return;
                }
                //一個一個點找
                int i = 0;
                for (; i < G.Count; i++)
                {
                    if (rowAll[G[i].Row][G[i].Col] != 'u')
                    {
                        continue;
                    }
                    bool re = Probe(G[i]);


                    //被填入成功，就跳出
                    if (re)
                    {
                       /* Console.Write("成功填入的點 座標為:");
                        Console.WriteLine(G[i].Row+1 + " " + (G[i].Col+1) + " 填入的點為" + rowAll[G[i].Row][G[i].Col]+" 再次執行Propagate");*/
                        break;
                    }
                }
                //木有可以填入了，就結束
                if (i == G.Count)
                {
                    return;
                }
            }
        }
        //傳點進入看能否填入
        private bool Probe(UnpaintPoint point)
        {
            bool Probe0 = ProbeG(point, '0',rowAll);
            bool Probe1 = ProbeG(point, '1',rowAll);
            
            //沒辦法找到解
            if (!Probe0 && !Probe1)
            {
                return false;
            }
            else if (Probe0 && Probe1)
            {

                rowAll = PropagateMatrix(point);
               /* Console.Write("成功填入的點 座標為:");
                Console.WriteLine(point.Row + 1 + " " + (point.Col + 1) + " 填入的點為" + rowAll[point.Row][point.Col] + " 兩個都能填的情況");*/
                return false;
            }
            else if (Probe1)
            {
                List<int> re = new List<int>();
                for (int i = 0; i < List1[point.Row, point.Col].Count; i++) {
                    if (rowAll[ List1[point.Row, point.Col ][i].Row][ List1[point.Row, point.Col ][i].Col] == 'u') {
                        rowAll[List1[point.Row, point.Col][i].Row][List1[point.Row, point.Col][i].Col] = List1[point.Row, point.Col][i].Point1;
                        re.Add(i);
                    }
                }
                for (int i = 0; i < re.Count; i++) {
                    List1[point.Row, point.Col].RemoveAt(i);
                }
                rowAll[point.Row][point.Col] = '1';

                return true;
            }
            else if (Probe0)
            {
                List<int> re = new List<int>();
                for (int i = 0; i < List0[point.Row, point.Col].Count; i++)
                {
                    if (rowAll[List0[point.Row, point.Col][i].Row][List0[point.Row, point.Col][i].Col] == 'u')
                    {
                        rowAll[List0[point.Row, point.Col][i].Row][List0[point.Row, point.Col][i].Col] = List0[point.Row, point.Col][i].Point1;
                    }

                }
                for (int i = 0; i < re.Count; i++)
                {
                    List0[point.Row, point.Col].RemoveAt(i);
                }
                rowAll[point.Row][point.Col] = '0';
                return true;
            }
            else
            {
                return false;
            }
        }
        //判斷能否填入點
        private bool ProbeG(UnpaintPoint point, char num,List<List<char>>all)
        {
            //假設棋盤
            List<List<char>> reAll =new List<List<char>>(Copy(all));
            //填入num的部分
            reAll[point.Row][point.Col] = num;
            List<char> re = new List<char>(reAll[point.Row]);
           
            bool paintRow = Fix(25, Program.row[point.Row].Count, reAll[point.Row], Program.row[point.Row], ref re);
            if (!paintRow)
            {
                return false;
            }
         
            //轉置
            reAll = OwnChangeMatrix(reAll);

            re= new List<char>(reAll[point.Col]);
            
            bool paintCol = Fix(25, Program.col[point.Col].Count, reAll[point.Col], Program.col[point.Col], ref re);

            //再轉回
            reAll = OwnChangeMatrix(reAll);
            if (!paintCol)
            {
                return false;
            }
            else
            {
                reAll=PropagateOwn(reAll);
                List<Point>reList=ComparePoint(all, reAll);
                for (int i = 0; i < reList.Count; i++) {
                    if (reList[i].Row != point.Row && reList[i].Col != point.Col) {
                        if (reList[i].Point1 == '0')
                        {
                            List1[reList[i].Row, reList[i].Col].Add(new Point(point.Row, point.Col, Inverse(num)));
                        }
                        else if (reList[i].Point1 == '1')
                        {
                            List0[reList[i].Row, reList[i].Col].Add(new Point(point.Row, point.Col, Inverse(num)));
                        }
                    }
                    
                }
                return true;
            }
        }
        #endregion

        #region weight
        //計算p比重
        public List<UnpaintPoint> MinLoged(List<List<char>>reAll) {

            List<UnpaintPoint> reWeight = AllUnpaintPoint(reAll);
            List<UnpaintPoint> Weight = new List<UnpaintPoint>();
            List<int> re = new List<int>();

            if (reWeight.Count != 0) {
                Weight.Add(reWeight[0]);
               Weight[0].Weight=Vmin(reWeight[0],reAll) + Math.Abs(Vlog(reWeight[0], '0',reAll) - Vlog(reWeight[0], '1',reAll));
            }

            for (int i = 1; i < reWeight.Count; i++) {
               
                reWeight[i].Weight = Vmin(reWeight[i],reAll) + Math.Abs(Vlog(reWeight[i], '0',reAll) - Vlog(reWeight[i], '1',reAll));
                reWeight[i].use = false;
                int j;
                for ( j = 0; j < Weight.Count; j++)
                {
                    if (reWeight[i].Weight <= Weight[j].Weight)
                    {
                        Weight.Insert(j, reWeight[i]);
                        break;
                    }
                }
                if (j == Weight.Count)
                {
                    Weight.Add(reWeight[i]);
                }
                     
            }
            
            return Weight;

        }
        private double Vmin(UnpaintPoint p, List<List<char>> reAll) {

            double r0 = m(p, '0',reAll);
            double r1 = m(p, '1',reAll);


            if (r0 > r1) {
                return r1;
            }
            else
            {
                return r0;

            }

        }
        private double Vlog(UnpaintPoint point ,char num, List<List<char>> reAll) {

            return  Math.Log(1 + m(point, num,reAll)) / Math.Log(10)+1;
        }
        private double m(UnpaintPoint point, char num, List<List<char>> reAll) {


            List<List<char>> re = new List<List<char>>(Copy(reAll));

            re[point.Row][point.Col] = num;

            re = PropagateOwn(re);

            List<Point> reP=ComparePoint(re,reAll);

            return Convert.ToDouble(reP.Count);

        }
        #endregion

        #region backtraking

        //最後填入backtrack
        public bool backtrack( List<UnpaintPoint>G,List<List<char>>All,ref List<List<char>> Paint,ref List<List<char>>FinalOutput,int num)
        {
            
            List<List<char>> re0 = new List<List<char>>(Copy(Paint)) ;
            List<List<char>> re1 = new List<List<char>>(Copy(Paint)) ;
          
            List<UnpaintPoint> reG = new List < UnpaintPoint >( G);

            while (true) {
                if ( num == reG.Count||(All[reG[num].Row][reG[num].Col]=='u'))
                {
                    break;
                }
                else {
                    num++;
                }
            }


            if (reG.Count == 0||num==reG.Count)
            {
                FinalOutput =All;
               
                return true;
            }
            else 
            {
           
                bool zero = backtrack0(G,reG[num],All,ref re0,ref  FinalOutput,num);
                if (zero)
                {
                    Paint = re0;
                    return true;
                }
                else
                {
                     bool one = backtrack1(G,reG[num], All, re1,ref FinalOutput,num);
                    if (one)
                    {
                        Paint = re1;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
               
                
                
            }
            
        }
        public bool backtrack0(List<UnpaintPoint> G, UnpaintPoint P, List<List<char>> All,ref List<List<char>> Paint, ref List<List<char>> FinalOutput,int num)
        {
            List<List<char>> re =new List<List<char>>(All);
            if (ProbeG(P, '0', re))
            {
                for (int i = 0; i < List0[P.Row, P.Col].Count; i++)
                {
                    if (rowAll[List0[P.Row, P.Col][i].Row][List0[P.Row, P.Col][i].Col] == 'u')
                    {
                        re[List0[P.Row, P.Col][i].Row][List0[P.Row, P.Col][i].Col] = List0[P.Row, P.Col][i].Point1;
                    }

                }
                re[P.Row][P.Col] = '0';
                re = PropagateOwn(re);

               
                bool reX = backtrack(G, re,ref Paint,ref  FinalOutput,num+1);
                if (reX)
                {
                  
                    Paint = re;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }



            

        }
        public bool backtrack1(List<UnpaintPoint> G, UnpaintPoint P, List<List<char>> All, List<List<char>> Paint,ref List<List<char>> FinalOutput,int num)
        {
            List<List<char>> re =new List<List<char>>(All); 
            if (ProbeG(P, '1', re))
            {
                for (int i = 0; i < List1[P.Row, P.Col].Count; i++)
                {
                    if (rowAll[List1[P.Row, P.Col][i].Row][List1[P.Row, P.Col][i].Col] == 'u')
                    {
                        re[List1[P.Row, P.Col][i].Row][List1[P.Row, P.Col][i].Col] = List1[P.Row, P.Col][i].Point1;
                    }

                }
                re[P.Row][P.Col] = '1';
                re = PropagateOwn(re);


                bool reX = backtrack(G, re, ref Paint,ref FinalOutput,num+1);
                if (reX)
                {
                
                    Paint = re;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }

        }
        #endregion


    }
}
