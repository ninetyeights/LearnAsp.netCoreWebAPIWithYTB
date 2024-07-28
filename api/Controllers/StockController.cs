// Ignore Spelling: Dto

using api.Data;
using api.Dtos.Stock;
using api.Helpers;
using api.Interfaces;
using api.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace api.Controllers
{
    [Route("api/stock")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly IStockRepository _stockRepo;

        public StockController(ApplicationDBContext context, IStockRepository stockRepo)
        {
            _stockRepo = stockRepo;
            _context = context;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] QueryObject query)
        {
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);

            //var stocks = await _context.Stocks.ToListAsync();

            var stocks = await _stockRepo.GetAll(query);

            var stockDto = stocks.Select(s => s.ToStockDto()).ToList();

            return Ok(stockDto);
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);

            //var stock = await _context.Stocks.FindAsync(id);
            var stock = await _stockRepo.GetByIdAsync(id);

            if (stock == null)
            {
                return NotFound();
            }

            return Ok(stock.ToStockDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateStockRequestDto stockDto)
        {
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);

            var stockModel = stockDto.ToStockFromCreateDTO();

            await _stockRepo.CreateAsync(stockModel);

            //await _context.Stocks.AddAsync(stockModel);
            //await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = stockModel.Id }, stockModel.ToStockDto());
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateStockRequestDto updateDto)
        {
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);

            //var stockModel = await _context.Stocks.FirstOrDefaultAsync(x => x.Id == id);

            var stockModel = await _stockRepo.UpdateAsync(id, updateDto);

            if (stockModel == null)
            {
                return NotFound();
            }

            //stockModel.Symbol = updateDto.Symbol;
            //stockModel.CompanyName = updateDto.CompanyName;
            //stockModel.Purchase = updateDto.Purchase;
            //stockModel.LastDiv = updateDto.LastDiv;
            //stockModel.Industry = updateDto.Industry;
            //stockModel.MarketCap = updateDto.MarketCap;

            //await _context.SaveChangesAsync();

            return Ok(stockModel.ToStockDto());
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);

            //var stockModel = await _context.Stocks.FirstOrDefaultAsync(x => x.Id == id);

            var stockModel = await _stockRepo.DeleteAsync(id);

            if (stockModel == null)
            {
                return NotFound();
            }

            //_context.Stocks.Remove(stockModel);

            //await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
