using API.Repositories.Contracts;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CoreController<TRepository, TPk, TEntity, TDTO, TInsertRequest, TUpdateRequest> : ControllerBase
    where TRepository : IBaseRepository<TPk, TEntity>
    where TEntity : class, IEntity<TPk>
{
    protected readonly TRepository _repository;
    protected readonly IMapper _mapper;

    public CoreController(TRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAsync()
    {
        var entities = await _repository.FindAll();

        return Ok(_mapper.Map<IEnumerable<TDTO>>(entities));
    }

    [HttpGet("{pk}")]
    public async Task<IActionResult> GetAsync(TPk pk)
    {
        var entity = await _repository.FindOneByPk(pk);

        return Ok(_mapper.Map<TDTO>(entity));
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync(TInsertRequest request)
    {
        Console.WriteLine(request);
        var insertedEntity = await _repository.InsertOne(_mapper.Map<TEntity>(request));

        return Created("", _mapper.Map<TDTO>(insertedEntity));
    }

    [HttpPut("{pk}")]
    public async Task<IActionResult> PutAsync(TPk pk, TUpdateRequest request)
    {
        var updatedEntity = await _repository.UpdateOneByPk(pk, _mapper.Map<TEntity>(request));

        return Ok(_mapper.Map<TDTO>(updatedEntity));
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAsync(TPk pk)
    {
        var deletedEntity = await _repository.DeleteOneByPk(pk);

        return Ok(_mapper.Map<TDTO>(deletedEntity));
    }
}