using System;

namespace Parallel_Contract_Prototype
{

    class Program
    {
        static string[] test_Address = new string[] { "alice", "bob0", "bob1", "bob2", "bob3", "bob4", "bob5", "bob6", "bob7" };

        static void Main(string[] args)
        {
            Console.WriteLine("type 1 to make a single thread contract test.");
            Console.WriteLine("type 2 to make a Parallel contract test.");
            while (true)
            {
                var line = Console.ReadLine();
                if (line == "exit")
                {
                    break;
                }
                if (line == "1")
                {
                    Test_Single();
                }
                if (line == "2")
                {
                    Test_Parallel();
                }
            }
        }
        static void Test_Single()
        {
            Single_Thread_TX chain = new Single_Thread_TX();
            //give alice some money for test. 
            chain.InitChain();

            var r = new Random();
            //alice->bobs 1000 times
            for (var i = 0; i < 1000; i++)
            {
                var bobCount = test_Address.Length - 1;
                var bob = test_Address[r.Next(bobCount) + 1];
                var alice = test_Address[0];
                chain.SendRaw(new Single_Thread_TX.Transaction(alice, bob, 100 + r.Next(10000)));
            }



            //show money
            Console.WriteLine("==== show money ====");
            foreach (var add in test_Address)
            {
                Console.WriteLine(add + " = " + chain.NativeContract_BalanceOf(add));
            }


            //随机互转1000次

            for (var i = 0; i < 1000; i++)
            {
                var ran1 = r.Next(test_Address.Length);

                var ran2 = r.Next(test_Address.Length);
                while (ran2 == ran1)
                {
                    ran2 = r.Next(test_Address.Length);
                }
                var from = test_Address[ran1];
                var to = test_Address[ran2];
                chain.SendRaw(new Single_Thread_TX.Transaction(from, to, 5));

            }
            chain.WaitAllTx();



            //show money
            Console.WriteLine("==== show money 2 ====");
            foreach (var add in test_Address)
            {
                Console.WriteLine(add + " = " + chain.NativeContract_BalanceOf(add));
            }

        }

        static void Test_Parallel()
        {
            Parallel_Tx chain = new Parallel_Tx();
            //give alice some money for test. 
            chain.InitChain();

            var r = new Random();
            //alice->bobs 1000 times
            for (var i = 0; i < 1000; i++)
            {
                var bobCount = test_Address.Length - 1;
                var bob = test_Address[r.Next(bobCount) + 1];
                var alice = test_Address[0];
                chain.SendRaw(new Parallel_Tx.Transaction(alice, bob, 100 + r.Next(10000)));
            }



            //show money
            Console.WriteLine("==== show money ====");
            foreach (var add in test_Address)
            {
                Console.WriteLine(add + " = " + chain.NativeContract_BalanceOf(add));
            }


            //随机互转1000次
            for (var i = 0; i < 1000; i++)
            {
                var ran1 = r.Next(test_Address.Length);

                var ran2 = r.Next(test_Address.Length);
                while (ran2 == ran1)
                {
                    ran2 = r.Next(test_Address.Length);
                }
                var from = test_Address[ran1];
                var to = test_Address[ran2];
                chain.SendRaw(new Parallel_Tx.Transaction(from, to, 5));

            }
            chain.WaitAllTx();



            //show money
            Console.WriteLine("==== show money 2 ====");
            foreach (var add in test_Address)
            {
                Console.WriteLine(add + " = " + chain.NativeContract_BalanceOf(add));
            }

        }
    }
}
