using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repositories.EFCore.Config;
public class CategoryConfig : IEntityTypeConfiguration<Category> {
    public static Guid COMPUTER_SCIENCE_ID = Guid.NewGuid();
    public static Guid NETWORK_ID = Guid.NewGuid();
    public static Guid DATABASE_MANAGEMENT_SYSTEMS_ID = Guid.NewGuid();

    public void Configure(EntityTypeBuilder<Category> builder) {
        builder.HasKey(c => c.Id); // PK
        builder.Property(c => c.Name).IsRequired();

        builder.HasData(
            new() {
                Id = COMPUTER_SCIENCE_ID,
                Name = "Computer Science"
            },
            new() {
                Id = NETWORK_ID,
                Name = "Network"
            },
            new() {
                Id = DATABASE_MANAGEMENT_SYSTEMS_ID,
                Name = "Database Management Systems"
            }
        );
    }
}