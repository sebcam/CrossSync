using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CrossSync.Entity.Abstractions.Entities;
using CrossSync.Infrastructure.Server;
using Microsoft.AspNetCore.Mvc;

namespace CrossSync.AspNetCore.Api
{
  [Route("api/ee")]
  public class TombstoneController : Controller
  {
    private readonly IServerContext context;

    public TombstoneController(IServerContext context)
    {
      this.context = context;
    }

    [HttpGet("{entityType}")]
    public IEnumerable<DeletedEntity> Get(string entityType)
    {
      return context.DeletedEntities.Where(f => f.DataType == entityType).AsEnumerable();
    }
  }
}
