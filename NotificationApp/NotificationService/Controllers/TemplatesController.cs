using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NotificationService.Data;
using NotificationService.Models;
using TemplateService.Dtos;

namespace NotificationService.Controllers;

[ApiController]
[Route("api/v1/templates")]
public class TemplatesController (ITemplateRepo _templateRepo, IMapper _mapper, ILogger<Template> _logger): ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<ReadTemplateDto>>> GetAll()
    {
        var result = await _templateRepo.GetAll();
        
        return Ok(_mapper.Map<ReadTemplateDto>(result));
    }
    
    [HttpGet("{id}", Name = "[controller]/GetById")]
    public async Task<ActionResult<ReadTemplateDto>> GetById(Guid id)
    {
        var result = await _templateRepo.GetByIdAsync(id);
        
        return Ok(_mapper.Map<ReadTemplateDto>(result));
    }
    
    [HttpPost]
    public async Task<ActionResult<ReadTemplateDto>> Create(CreateTemplateDto template)
    {
        try
        {
            var tempplateModel = _mapper.Map<Template>(template);
            var result = await _templateRepo.AddAsync(tempplateModel);
            await _templateRepo.SaveChangesAsync();
            
            var route = this.ControllerContext.ActionDescriptor?.ControllerName + "/GetById";
            return CreatedAtRoute(route, new { result.Id }, _mapper.Map<ReadTemplateDto>(result));
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return Problem();
        }
        
    }
    
}