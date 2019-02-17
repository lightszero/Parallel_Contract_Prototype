using System;
using System.Collections.Generic;
using System.Text;

namespace Parallel_Contract_Prototype
{
    public class Single_Thread_TX
    {
        public class Transaction
        {
            public Transaction(string from, string to, System.Numerics.BigInteger value)
            {
                this.from = from;
                this.to = to;
                this.value = value;
            }
            public string from;
            public string to;
            public System.Numerics.BigInteger value;
        }

        System.Collections.Concurrent.ConcurrentDictionary<string, System.Numerics.BigInteger> storage
           = new System.Collections.Concurrent.ConcurrentDictionary<string, System.Numerics.BigInteger>();


        public void InitChain()
        {
            storage["alice"] = 50000;
        }
        List<Transaction> single_tx
   = new List<Transaction>();

        public void SendRaw(Transaction tx)
        {
            if (single_tx.Count < 500)
            {
                single_tx.Add(tx);
            }
            else
            {
                WaitAllTx();
            }
        }
        public void WaitAllTx()
        {
            foreach(var tx in single_tx)
            {
                NativeContract_Transfer(tx.from, tx.to, tx.value);

            }
            single_tx.Clear();
        }
        public System.Numerics.BigInteger NativeContract_BalanceOf(string addr)
        {
            if (storage.ContainsKey(addr) == false)
                return 0;
            return storage[addr];
        }
        bool NativeContract_Transfer(string from, string to, System.Numerics.BigInteger value)
        {
            if (value <= 0)
                return false;

            if (storage.ContainsKey(from) == false) return false;
            var srcvalue = storage[from];
            if (srcvalue <= value)
                return false;

            if (storage.ContainsKey(to) == false)
                storage[to] = 0;

            //storage.put
            storage[from] = srcvalue - value;

            //storage.get -> num.add -> storage.put
            storage[to] = storage[to] + value;

            return true;
        }
    }
}
