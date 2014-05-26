using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PADI_DSTM;
using padi_dstm_exceptions;

namespace TestClient
{
    class TestClient
    {
        

        static void Main(string[] args){
            
            PadiDstm.Init();
            PadInt padInt1 = PadiDstm.CreatePadInt(1);
            if (padInt1 == null)
            {
                Console.WriteLine("ID (1) of PadInt already exist, NOOB! we gonna access it");
                padInt1 = PadiDstm.AccessPadInt(2);
            } 

            PadInt padInt2 = PadiDstm.CreatePadInt(2);
            if (padInt2 == null)
            {
                Console.WriteLine("ID (2) of PadInt already exist, NOOB! we gonna access it");
                padInt2 = PadiDstm.AccessPadInt(2);
            }


            try
            {
                PadiDstm.TxBegin();
            //    Console.WriteLine("PadInt 1: " + padInt1.Read());
            //    Console.WriteLine("PadInt 2: " + padInt2.Read());
                padInt1.Write(10);
                PadInt padInt3 = PadiDstm.CreatePadInt(3);
                if (padInt3 == null) padInt3 = PadiDstm.AccessPadInt(3);
                padInt2.Write(20 + padInt1.Read());
                padInt3.Write(100);
                Console.WriteLine("PadInt 1: " + padInt1.Read());
                Console.WriteLine("PadInt 2: " + padInt2.Read());
                PadiDstm.TxCommit();
            }
            catch (TxException e) { Console.WriteLine(e.Message); }

            try
            {
                PadiDstm.TxBegin();
                padInt1.Write(50);
                padInt2.Write(100);
                PadInt padInt3 = PadiDstm.CreatePadInt(3);
                if (padInt3 == null) padInt3 = PadiDstm.AccessPadInt(3);
                padInt3.Write(padInt3.Read() * padInt3.Read());
                Console.WriteLine("PadInt 1: " + padInt1.Read());
                Console.WriteLine("PadInt 2: " + padInt2.Read());
                Console.WriteLine("PadInt 3: " + padInt3.Read());
                PadiDstm.TxAbort();
                }
            catch (TxException e) { Console.WriteLine(e.Message); }
            try
            {
                PadiDstm.TxBegin();
                PadInt padInt3 = PadiDstm.CreatePadInt(3);
                if (padInt3 == null) padInt3 = PadiDstm.AccessPadInt(3);
                Console.WriteLine("PadInt 1: " + padInt1.Read());
                Console.WriteLine("PadInt 2: " + padInt2.Read());
                Console.WriteLine("PadInt 3: " + padInt3.Read());
                PadiDstm.TxCommit();
            }
            catch (TxException e) { Console.WriteLine(e.Message);}

            Console.ReadKey();
        }
    }
}
