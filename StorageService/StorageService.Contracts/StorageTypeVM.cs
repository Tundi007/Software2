namespace StorageService.Contracts
{
    public class StorageTypeVM
    {
        public int StorageTypeID { get; set; }

        public string Title { get; set; }
        public int Capacity { get; set; }
        public int Price { get; set; }
        public int Month { get; set; }
    }
}