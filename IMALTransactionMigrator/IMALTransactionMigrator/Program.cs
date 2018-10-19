using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMALTransactionMigrator
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                DoJob.doJobs();
            }
        }
    }
}
