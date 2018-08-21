using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Sample.TodoList.Entities.Shared.Configurations
{
  public class TodoListConfiguration : IEntityTypeConfiguration<TodoList>
  {
    public void Configure(EntityTypeBuilder<TodoList> builder)
    {
      builder.HasKey(f => f.Id);
      builder.HasMany(f => f.Items).WithOne().OnDelete(DeleteBehavior.Cascade);
    }
  }
}
