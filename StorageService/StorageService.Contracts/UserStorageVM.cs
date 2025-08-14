using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageService.Contracts
{
    public class UserStorageVM
    {
        public StorageTypeVM StorageType { get; set; }
        public int UserID { get; set; }


        public int TotalPrice { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
