using E_Government.Domain.Entities.OTP;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Infrastructure.Persistence._Data.Configuration
{
    internal class OtpCodeConfigurations : IEntityTypeConfiguration<OtpCode>
    {
        public void Configure(EntityTypeBuilder<OtpCode> builder)
        {
            builder.Property(e => e.Code).IsRequired().HasMaxLength(10);
            builder.Property(e => e.Email).IsRequired().HasMaxLength(256);
            builder.Property(e => e.Purpose).IsRequired().HasMaxLength(50);
            builder.HasIndex(e => new { e.Email, e.Purpose, e.IsUsed });
            builder.HasIndex(e => e.ExpiresAt);
        }
    }
}
