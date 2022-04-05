using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Net;
using WareHouseAPI.Models;
using WareHouseAPI.Repository;

namespace WareHouseAPI.Controllers
{
    [ApiController]
    [Route("[product]")]
    public class WarehouseController : ControllerBase
    {
        IRepository _repository;
        public WarehouseController(IRepository repository)
        {
            _repository = repository;
        }
        
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<WarehouseEntry>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetProducts()
        {
            var result = await _repository.GetProductRecords();
            return Ok(result);
        }

        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> SetProductCapacity(int productId, int capacity)
        {
            try
            {
                if (capacity <= 0)
                {
                    return BadRequest("NotPositiveQuantityMessage");
                }
                // current capacity
                var capacityRecord = await _repository.GetCapacityRecords(record =>
                {
                    return record.ProductId == productId;
                }, true);
                // 
                if (capacityRecord.FirstOrDefault().Capacity > capacity)
                {
                    return BadRequest("QuantityTooHigh");
                }
                var result = _repository.SetProductCapacity(productId, capacity);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest("Product does not exists"+ e.ToString());
            }
        }
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ReceiveProduct(int productId, int qty)
        {
            try
            {
                if (qty <= 0)
                {
                    return BadRequest("NotPositiveQuantityMessage");
                }
                // current capacity
                var capacityRecord = await _repository.GetCapacityRecords(record =>
                {
                    return record.ProductId == productId;
                }, true);
                // current qty
                var qtyRecord = await _repository.GetWareHouseQty(record =>
                {
                    return record.ProductId == productId;
                }, true);
                // validation
                if (qtyRecord.FirstOrDefault().Qty + qty > capacityRecord.FirstOrDefault().Capacity)
                {
                    return BadRequest("QuantityTooHigh");
                }
                var finalQty = qtyRecord.FirstOrDefault().Qty + qty;
                var result = _repository.ReceiveProduct(productId, finalQty);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest("Product does not exists" + e.ToString());
            }

        }
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DispatchProduct(int productId, int qty)
        {
            try
            {
                // current capacity
                var capacityRecord = await _repository.GetCapacityRecords(record =>
                {
                    return record.ProductId == productId;
                }, true);

                if (qty > capacityRecord.FirstOrDefault().Capacity)
                {
                    return BadRequest("QuantityTooHigh");
                }
                // current qty
                var qtyRecord = await _repository.GetWareHouseQty(record =>
                {
                    return record.ProductId == productId;
                }, true);
                // validation
                if (qtyRecord.FirstOrDefault().Qty > qty)
                {
                    return BadRequest("QuantityTooHigh");
                }
                var finalQty = qtyRecord.FirstOrDefault().Qty - qty;
                await _repository.ReceiveProduct(productId, finalQty);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest("Product does not exists" + e.ToString());
            }

        }
    }
}