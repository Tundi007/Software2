using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageService.Contracts
{
    public class StorageTypeVMCreate
    {
        public string Title { get; set; }
        public int Capacity { get; set; }
        public int Price { get; set; }
        public int Month { get; set; }
    }
}
