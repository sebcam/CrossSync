﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrossSync.AspNetCore.Api;
using CrossSync.Entity.Abstractions.Services;
using CrossSync.Entity.Abstractions.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sample.TodoList.Entities.Shared;

namespace Sample.TodoList.API.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class TodoListController : SyncController<TodoList.Entities.Shared.TodoList>
  {
    public TodoListController(IUnitOfWork unitOfWork, ISyncRepository<Entities.Shared.TodoList> repository) : base(unitOfWork, repository)
    {
    }
  }
}