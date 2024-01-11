using GeekShopping.ProductAPI.Data.ValueObjects;
using GeekShopping.ProductAPI.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Collections;

namespace GeekShopping.ProductAPI.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ProductController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

    private readonly ILogger<ProductVO> _logger;
    private IProductRepository _productRepository;

    public ProductController(ILogger<ProductVO> logger, IProductRepository productRepository)
    {
        _logger = logger;
        _productRepository = productRepository ?? throw new
            ArgumentNullException(nameof(productRepository));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductVO>>> FindAll()
    {
        _logger.LogInformation("Teststs");
        _logger.LogWarning("Pesquisando produtos");
        var product = await _productRepository.FindAll();

        return Ok(product);
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<ProductVO>> FindById(long id)
    {
        _logger.LogWarning("Pesquisando produto");
        var product = await _productRepository.FindById(id);

        if (product == null) return NotFound();

        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<ProductVO>> Create([FromBody] ProductVO vo)
    {
        _logger.LogWarning("Criando produto");
        if (vo == null)
            return BadRequest("Requisição vazia");

        var product = await _productRepository.Create(vo);
        return Ok(product);
    }

    [HttpPut]
    public async Task<ActionResult<ProductVO>> Update([FromBody] ProductVO vo)
    {
        _logger.LogWarning("Atualizando produto");
        if (vo == null)
            return BadRequest("Requisição vazia");

        var product = await _productRepository.Update(vo);
        return Ok(product);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(long id)
    {
        var status = await _productRepository.DeleteById(id);

        if (!status) return BadRequest("Produto não existe");

        return Ok("Deletado com sucesso!");
    }

}

