using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CoreSync.Entity.Server.Exceptions;
using CoreSync.Entity;
using CoreSync.Entity.Abstractions;
using CoreSync.Entity.Abstractions.Services;
using CoreSync.Entity.Abstractions.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

namespace CoreSync.AspNetCore.Api
{
  public class SyncController<T> : Controller where T : class, IIdentifiable, IVersionableEntity, new()
  {
    private readonly IUnitOfWork unitOfWork;
    private readonly ISyncRepository<T> repository;

    public SyncController(IUnitOfWork unitOfWork, ISyncRepository<T> repository)
    {
      this.unitOfWork = unitOfWork;
      this.repository = repository;
    }

    [HttpGet]
    public virtual async Task<IQueryable<T>> Get(DateTimeOffset from)
    {
      var toto = Includes(await repository.GetAllAsync(f => f.UpdatedAt > from.UtcDateTime)).ToList();
      return toto.AsQueryable();
    }

    [HttpGet("{id}")]
    public virtual async Task<T> Get(Guid id)
    {
      return await Includes(await repository.GetAllAsync()).FirstOrDefaultAsync(f => f.Id == id);
    }

    [HttpPost]
    public async virtual Task<IActionResult> Post([FromBody]T value)
    {
      var added = await repository.AddAsync(value);

      await unitOfWork.CommitAsync();

      return CreatedAtRoute(routeValues: new { id = added.Id }, value: added);
    }

    [HttpPut("{id}")]
    public virtual async Task<IActionResult> Put(Guid id, [FromBody]T value)
    {
      if (!ModelState.IsValid)
        return BadRequest(ModelState);
      try
      {
        var result = await repository.UpdateAsync(id, value);

        await unitOfWork.CommitAsync();
        return Accepted(result);
      }
      catch (SyncConflictVersionException<T> e)
      {
        if (Request.Headers.TryGetValue("ForceConflictUpdate", out StringValues values) && values.Any() && values.First() == bool.TrueString)
        {
          //TODO : Change with logger
          Trace.WriteLine($"Conflict handled with ForceConflictUpdate");
          e.Value.Version = e.Existing.Version;
          var result = await repository.UpdateAsync(id, value);

          await unitOfWork.CommitAsync();
          return Accepted(result);
        }
        return StatusCode(StatusCodes.Status409Conflict, e.Existing);
      }
    }

    [HttpDelete("{id}")]
    public virtual async Task<IActionResult> Delete(Guid id)
    {
      var entity = await repository.GetAsync(id);
      if (entity != null)
      {
        await repository.DeleteAsync(await repository.GetAsync(id));
        await unitOfWork.CommitAsync();
      }

      return Ok();
    }

    protected virtual IQueryable<T> Includes(IQueryable<T> query)
    {
      return query;
    }
  }
}
