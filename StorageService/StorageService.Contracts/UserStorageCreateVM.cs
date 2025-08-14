using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageService.Contracts
{
    public class UserStorageCreateVM
    {
        public int StorageTypeID { get; set; }
        public int UserID { get; set; }
    }
}
