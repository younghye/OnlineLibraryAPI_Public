using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OlineLibraryAPI.Dao;
using OlineLibraryAPI.Dao.Factory;
using OlineLibraryAPI.Models.Item_;

namespace OlineLibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly IItemDAO dao = DAOFactory.GetDAOFactory(DAOFactory.DAOFactoryType.SQL_SERVER).GetItemDao();

        [Authorize]
        [HttpGet("SearchItemCopy")]
        public async Task<ActionResult<ItemCopy>> SearchByBarcode(long barcode)
        {
            ItemCopy? iCopy = await dao.FindByBarcode(barcode);
            return iCopy == null ? NotFound() : Ok(iCopy);
        }

        [Authorize]
        [HttpGet("SearchItem")]
        public async Task<ActionResult<Item>> SearchByItemID(int id)
        {
            Item? item = await dao.FindByItemID(id);
            return item == null ? NotFound() : Ok(item);
        }

        [Authorize]
        [HttpGet("SelectItem")]
        public async Task<ActionResult<List<Item>>> SelectItem(long? barcode, int? typeID,
            string? title, string? producer, DateTime? publishDateFrom, DateTime? publishDateTo)
        {
            List<Item> items = await dao.SelectItem(barcode, typeID, title, producer, publishDateFrom, publishDateTo);
            return items == null || items.Count == 0 ? NotFound() : Ok(items);
        }

        [Authorize]
        [HttpPost("AddItem")]
        public async Task<IActionResult> InsertItem(Item item)
        {
            int newID = await dao.InsertItem(item);
            if (newID == -1)
                return Conflict("The Item is Already Exist");
            else if (newID > 0)
                return Ok(newID);
            else
                return Conflict("No Record Added");
        }

        [Authorize]
        [HttpPost("AddItemCopy")]
        public async Task<IActionResult> InsertItemCopy(ItemCopy itemCopy)
        {
            ItemCopy result = await dao.InsertItemCopy(itemCopy);
            return result == null ? Conflict("No Record Added") : Ok(result);
        }

        [Authorize]
        [HttpPut("DeleteItem")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            int result = await dao.DeleteItem(id);
            if (result == 0)
                return NotFound("No Record Deleted");
            if (result == -1)
                return Conflict("Cannot delete because the item is checking out");
            return Ok();
        }

        [Authorize]
        [HttpPut("DeleteItemCopy")]
        public async Task<IActionResult> DeleteItemCopy(long barcode)
        {
            int result = await dao.DeleteItemCopy(barcode);
            return result == 0 ? NotFound("Cannot delete because the item is checking out or already deleted") : Ok();
        }

        [Authorize]
        [HttpPut("UpdateItem")]
        public async Task<IActionResult> UpdateItem(Item item)
        {
            int result = await dao.UpdateItem(item);
            return result == 0 ? NotFound("No Record Updated") : Ok();
        }

        [Authorize]
        [HttpPut("UpdateItemCopy")]
        public async Task<IActionResult> UpdateItemCopy(ItemCopy itemCopy)
        {
            int result = await dao.UpdateItemCopy(itemCopy);
            return result == 0 ? NotFound("No Record Updated") : Ok();
        }
    }
}
