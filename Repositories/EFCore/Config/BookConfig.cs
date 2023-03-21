using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repositories.EFCore.Config;
public class BookConfig : IEntityTypeConfiguration<Book> {
    public void Configure(EntityTypeBuilder<Book> builder) {
        builder.HasData(
            new Book { Id = Guid.NewGuid(), CategoryId = CategoryConfig.COMPUTER_SCIENCE_ID, Title = "Karagöz ve Hacivat", Price = 75 },
            new Book { Id = Guid.NewGuid(), CategoryId = CategoryConfig.NETWORK_ID, Title = "Mesnevi", Price = 175 },
            new Book { Id = Guid.NewGuid(), CategoryId = CategoryConfig.DATABASE_MANAGEMENT_SYSTEMS_ID, Title = "Devlet", Price = 375 }
        );
    }
}