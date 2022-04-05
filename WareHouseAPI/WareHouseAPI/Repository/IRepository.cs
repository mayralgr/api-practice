using WareHouseAPI.Models;

namespace WareHouseAPI.Repository
{
    public interface IRepository
    {
        Task<IEnumerable<WarehouseEntry>> GetProductRecords();
        Task<IEnumerable<ProductRecord>> GetCapacityRecords(Predicate<ProductRecord> filter, bool bol);
        Task<IEnumerable<WareHouseRecord>> GetWareHouseQty(Predicate<WareHouseRecord> filter, bool bol);
        Task SetProductCapacity(int productId, int capacity);
        Task ReceiveProduct(int productId, int capacity);

    }
}
