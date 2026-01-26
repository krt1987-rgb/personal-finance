using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinance.API.Extensions;
using PersonalFinance.Application.DTOs;
using PersonalFinance.Application.Interfaces;

namespace PersonalFinance.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StockAnalysisController : ControllerBase
{
    private readonly IStockAnalysisService _stockAnalysisService;
    private readonly ILogger<StockAnalysisController> _logger;

    public StockAnalysisController(
        IStockAnalysisService stockAnalysisService,
        ILogger<StockAnalysisController> logger)
    {
        _stockAnalysisService = stockAnalysisService;
        _logger = logger;
    }

    /// <summary>
    /// Create a new AI-powered stock analysis
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<StockAnalysisDto>> CreateAnalysis(CreateStockAnalysisRequestDto request)
    {
        try
        {
            var userId = User.GetUserId();
            var analysis = await _stockAnalysisService.CreateAnalysisAsync(userId, request);
            return CreatedAtAction(nameof(GetAnalysis), new { id = analysis.Id }, analysis);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid stock analysis request: {Message}", ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating stock analysis for {Symbol}", request.Symbol);
            return StatusCode(500, "An error occurred while creating the stock analysis");
        }
    }

    /// <summary>
    /// Get a specific stock analysis by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<StockAnalysisDto>> GetAnalysis(Guid id)
    {
        try
        {
            var analysis = await _stockAnalysisService.GetAnalysisAsync(id);
            if (analysis == null)
            {
                return NotFound();
            }
            return Ok(analysis);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving stock analysis {AnalysisId}", id);
            return StatusCode(500, "An error occurred while retrieving the stock analysis");
        }
    }

    /// <summary>
    /// Get all analyses for a specific stock symbol
    /// </summary>
    [HttpGet("symbol/{symbol}")]
    public async Task<ActionResult<IEnumerable<StockAnalysisDto>>> GetAnalysesBySymbol(string symbol)
    {
        try
        {
            var userId = User.GetUserId();
            var analyses = await _stockAnalysisService.GetAnalysesBySymbolAsync(userId, symbol);
            return Ok(analyses);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving analyses for symbol {Symbol}", symbol);
            return StatusCode(500, "An error occurred while retrieving stock analyses");
        }
    }

    /// <summary>
    /// Get all stock analyses for the current user
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<StockAnalysisDto>>> GetUserAnalyses([FromQuery] int limit = 50)
    {
        try
        {
            var userId = User.GetUserId();
            var analyses = await _stockAnalysisService.GetUserAnalysesAsync(userId, limit);
            return Ok(analyses);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user analyses");
            return StatusCode(500, "An error occurred while retrieving stock analyses");
        }
    }

    /// <summary>
    /// Ask AI a specific question about a stock (interactive research)
    /// </summary>
    [HttpPost("research")]
    public async Task<ActionResult<StockResearchResponseDto>> ResearchStock(StockResearchRequestDto request)
    {
        try
        {
            var userId = User.GetUserId();
            var response = await _stockAnalysisService.ResearchStockAsync(userId, request);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid stock research request: {Message}", ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error researching stock {Symbol}", request.Symbol);
            return StatusCode(500, "An error occurred while researching the stock");
        }
    }

    /// <summary>
    /// Get quick AI overview for a stock (uses cache when available)
    /// </summary>
    [HttpGet("quick-overview/{symbol}")]
    public async Task<ActionResult<StockAnalysisDto>> GetQuickOverview(string symbol)
    {
        try
        {
            var userId = User.GetUserId();
            var request = new CreateStockAnalysisRequestDto
            {
                Symbol = symbol,
                AnalysisType = Domain.Enums.StockAnalysisType.QuickOverview,
                UseCache = true
            };
            var analysis = await _stockAnalysisService.CreateAnalysisAsync(userId, request);
            return Ok(analysis);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid quick overview request: {Message}", ex.Message);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting quick overview for {Symbol}", symbol);
            return StatusCode(500, "An error occurred while getting the quick overview");
        }
    }
}
