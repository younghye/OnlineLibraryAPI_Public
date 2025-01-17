using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OlineLibraryAPI.Dao;
using OlineLibraryAPI.Dao.Factory;
using OlineLibraryAPI.Models.Item_.Attribute;
using Type = OlineLibraryAPI.Models.Item_.Attribute.Type;

namespace OlineLibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MasterController : ControllerBase
    {
        private readonly IMasterDAO dao = DAOFactory.GetDAOFactory(DAOFactory.DAOFactoryType.SQL_SERVER).GetMasterDao();

        [Authorize]
        [HttpGet("GetCategory")]
        public async Task<ActionResult<List<Category>>> GetCategory()
        {
            List<Category> categories = await dao.GetCategory();
            return categories == null || categories.Count() == 0 ? NotFound() : Ok(categories);
        }

        [Authorize]
        [HttpGet("GetGenre")]
        public async Task<ActionResult<List<Genre>>> GetGenre()
        {
            List<Genre> genres = await dao.GetGenre();
            return genres == null || genres.Count() == 0 ? NotFound() : Ok(genres);
        }

        [Authorize]
        [HttpGet("GetType")]
        public async Task<ActionResult<List<Type>>> GetTypes()
        {
            List<Type> types = await dao.GetTypes();
            return types == null || types.Count() == 0 ? NotFound() : Ok(types);
        }
    }
}
