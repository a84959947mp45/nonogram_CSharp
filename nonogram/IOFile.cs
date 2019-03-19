using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace nonogram
{
    class IOFile
    {
        StreamReader sr = new StreamReader("input.txt");
      
        public bool read(ref string num,ref List<List<int>> row,ref List<List<int>> col) {
           
            try
            {
                    String line = sr.ReadLine();

                int oneNum=0;
                if (line[0] == '$')
                    {
                        num = line.Substring(1);
                        for (int i = 0; i < 25; i++)
                        {
                            string re = sr.ReadLine();
                            string[] tokens = re.Split();
                            List<int> reVector = new List<int>();
                            
                            foreach (string s in tokens)
                            {
                                if (Int32.TryParse(s, out oneNum))
                                {
                                    reVector.Add(oneNum);
                                }
                            }
                            row.Add(reVector);
                        }
                        for (int i = 0; i < 25; i++)
                        {
                            string re = sr.ReadLine();
                            string[] tokens = re.Split();
                            List<int> reVector = new List<int>();

                            foreach (string s in tokens)
                            {

                                if (Int32.TryParse(s, out oneNum))
                                {
                                    reVector.Add(oneNum);
                                }

                            }
                           col.Add(reVector);
                        }
                    }
                
                    
                return true;
                
            }
            catch (Exception e)
            {

                Console.WriteLine("已無資料讀取");
                
                
                return false;
            }
           

        }
        public bool Write(string num)
        {
            using (StreamWriter sw = new StreamWriter("output"+num+".txt"))
            {
                try
                {

                    for (int i = 0; i < 25; i++)
                    {
                        for (int j = 0; j < 25; j++)
                        {
                            sw.Write(" " + Function.rowAll[i][j]);

                        }
                        sw.WriteLine();

                    }
                    sw.Close();
                    return true;

                }
                catch (Exception e)
                {

                    Console.WriteLine("資料不符合格式");


                    return false;
                }

            }
        }
    }
}
