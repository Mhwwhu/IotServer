//using System;
//using System.ComponentModel;
//using System.Data.SQLite;

//class Program
//{
//    //预定义模块
//    const int messageSize = 12;
//    const int Mode = 10;

//    const double wRate = 0.01;
//    const int wData = 300;
//    const int HRMINLIMIT = 35;
//    const int HRMAXLIMIT = 120;

    

//    const string databasePath = @"D:\sqlite\broker.sqlite";
//    const string connectionString = $"Data Source={databasePath};Version=3;";
//    const string query = "SELECT message FROM messages where timestamp like '2024-07-17%'";

//    static void Main(string[] args)
//    {
      
        

//        //读取数据库模块
//        int[] resultArray = new int[HRMAXLIMIT];
//        int size = 0;
//        int _wData=wData;
//        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
//        {
//            connection.Open();

//            using (SQLiteCommand command = new SQLiteCommand(query, connection))
//            {
//                using (SQLiteDataReader reader = command.ExecuteReader())
//                {
//                    while (reader.Read())
//                    {
//                        byte[] data = (byte[])reader["message"];

//                        if (data.Length >= messageSize)
//                        {
 
//                            if (data[Mode]!=0 &&(data[Mode]<=HRMINLIMIT|| data[Mode] >= HRMAXLIMIT)&&_wData>0)
//                            {
//                                _wData--;
//                                resultArray[data[Mode]]++;
//                                size++;
//                            }
//                            else if(data[Mode] <= HRMINLIMIT || data[Mode] >= HRMAXLIMIT)
//                            {
//                                ;
//                            }
//                            else
//                            {
//                                resultArray[data[Mode]]++;
//                                size++;
//                            }
//                        }
//                    }
//                }
//            }

//            connection.Close();
//        }

       

//        //计算模块
//        int HRmin = 0, HRmax = 0;
//        int _min = 0,_max = 0;
//        double HRave = 0;

//        for (int i=1; i < resultArray.Length; i++)
//        {
//             HRave += (double)(i * resultArray[i]) / size;
//        }

//        bool Flag = true;
//        for(int j=1; j<resultArray.Length; j++) {
//            if (resultArray[j] > 0 && Flag) { HRmin = j;Flag = false; };
//            if (resultArray[j] > 0 && resultArray[j]>size*wRate) { _min = j; break; }
//        }
//        Flag= true;
//        for (int j = resultArray.Length-1; j>0; j--)
//        {
//            if (resultArray[j] > 0 && Flag) { HRmax = j; Flag = false; };
//            if (resultArray[j] > 0 && resultArray[j] > size * wRate) { _max = j; break; }
//        }

//       // for(int j = 1;j<resultArray.Length; j++) { if (resultArray[j] > _max && resultArray[j] < _max)  }
//        for (int j = 0; j < resultArray.Length; j++) { Console.WriteLine("{0}::{1}",j,resultArray[j]); } //调试用，输出分布图

//        Console.WriteLine("HRaverage:{0}",(int)HRave);
//        Console.WriteLine("HRrange:{0}-{1}",_min,_max);
//        Console.WriteLine("HRMax:{0}",HRmax);
//        Console.WriteLine("HRMin:{0}",HRmin);

//    }
//}