using System;
using System.Collections.Generic;
using System.Text;

namespace Parallel_Contract_Prototype
{


    class Parallel_Tx
    {
        public class Storage_Fixer
        {

            public bool InitStorage(System.Collections.Concurrent.ConcurrentDictionary<string, System.Numerics.BigInteger> storage)
            {
                this.storage = storage;
                if (storage.ContainsKey(key) == false)
                    return false;

                return true;
            }
            public System.Collections.Concurrent.ConcurrentDictionary<string, System.Numerics.BigInteger> storage;
            public string key;
            public System.Numerics.BigInteger Get()
            {
                return storage[key];
            }
            public void Fix(System.Numerics.BigInteger value)
            {
                storage[key] = value;
            }
        }
        public class Storage_CreateOrAdder
        {
            public bool InitStorage(System.Collections.Concurrent.ConcurrentDictionary<string, System.Numerics.BigInteger> storage)
            {
                this.storage = storage;
                if (storage.ContainsKey(key) == false)
                    storage[key] = 0;
                return true;
            }
            public System.Collections.Concurrent.ConcurrentDictionary<string, System.Numerics.BigInteger> storage;
            public string key;
            public void Add(System.Numerics.BigInteger value)
            {
                lock (storage)
                {
                    storage[key] = storage[key] + value;
                }
            }
        }
        public class Transaction
        {

            public Transaction(string from, string to, System.Numerics.BigInteger value)
            {
                this.from_fixer = new Storage_Fixer()
                {
                    key = from
                };

                this.to_adder = new Storage_CreateOrAdder()
                {
                    key = to
                };
                this.value = value;
            }
            public Storage_Fixer from_fixer;
            public Storage_CreateOrAdder to_adder;
            public System.Numerics.BigInteger value;
        }

        System.Collections.Concurrent.ConcurrentDictionary<string, System.Numerics.BigInteger> storage
           = new System.Collections.Concurrent.ConcurrentDictionary<string, System.Numerics.BigInteger>();


        public void InitChain()
        {
            storage["alice"] = 50000;
        }
        List<Transaction> parallel_tx
            = new List<Transaction>();
        Dictionary<string, Storage_Fixer> checkFixer
             = new Dictionary<string, Storage_Fixer>();
        public void SendRaw(Transaction tx)
        {

            if (!tx.from_fixer.InitStorage(this.storage))
                return;
            if (!tx.to_adder.InitStorage(this.storage))
                return;

            //can run tx paraller
            if (checkFixer.ContainsKey(tx.from_fixer.key) == false)
            {
                checkFixer[tx.from_fixer.key] = tx.from_fixer;
                parallel_tx.Add(tx);
            }
            else
            {
                WaitAllTx();
            }
        }
        public void WaitAllTx()
        {
            checkFixer.Clear();
            int count = parallel_tx.Count;
            int succCount = 0;
            foreach (var _tx in parallel_tx)
            {
                System.Threading.ThreadPool.QueueUserWorkItem((s) =>
                {
                    var b = NativeContract_Transfer(_tx.from_fixer, _tx.to_adder, _tx.value);
                    System.Threading.Interlocked.Decrement(ref count);                        //count--;

                    if (b)
                        System.Threading.Interlocked.Increment(ref succCount);//succCount++;
                });
            }
            while (count != 0)
            {
                System.Threading.Thread.Sleep(1);
            }
            if(parallel_tx.Count > 1)
            {
                Console.WriteLine("process tx with " + parallel_tx.Count + " thread.");
            }
            parallel_tx.Clear();
        }
        public System.Numerics.BigInteger NativeContract_BalanceOf(string addr)
        {
            if (storage.ContainsKey(addr) == false)
                return 0;
            return storage[addr];
        }
        bool NativeContract_Transfer(Storage_Fixer from, Storage_CreateOrAdder to, System.Numerics.BigInteger value)
        {
            if (value <= 0)
                return false;

            var srcvalue = from.Get();
            if (srcvalue <= value)
                return false;

            from.Fix(srcvalue - value);
            to.Add(value);
            return true;
        }
    }
}
