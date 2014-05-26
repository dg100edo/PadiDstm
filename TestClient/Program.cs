using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TestClient
{
    class Program
    {
        static void Main(string[] args){
            PADI_DSTM.Init();

            PADI_DSTM.CreatePadInt(1);
            PADI_DSTM.CreatePadInt(2);
            
            PadInt padInt1 = PADI_DSTM.AccessPadInt(1);
            PadInt padInt2 = PADI_DSTM.AccessPadInt(2);
            
            padInt1.Write(10);
            padInt2.Write(99);
            
            int sum = padInt1.Read() + padInt2.Read();
            
            Console.WriteLine(sum);
        }
    }
}
