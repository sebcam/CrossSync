using System.Collections.Generic;
using System.Linq;
using CrossSync.Entity.Abstractions.Entities;
using CrossSync.Infrastructure.Server;
using Microsoft.AspNetCore.Mvc;

namespace CrossSync.AspNetCore.Api
{
  [Route("api/tombstone")]
  public class TombstoneController : Controller
  {
    private readonly IServerContext context;

    public TombstoneController(IServerContext context)
    {
      this.context = context;
    }
    
    //TODO : Add datetime filter
    [HttpGet("{entityType}")]
    public IEnumerable<DeletedEntity> Get(string entityType)
    {
      return context.DeletedEntities.Where(f => f.DataType == entityType).AsEnumerable();
    }
  }
}
