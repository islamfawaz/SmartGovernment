using E_Government.Application.DTO.Bills;
using E_Government.Application.DTO.License;
using E_Government.Domain.DTO;
using E_Government.Domain.Entities.Bills;
using E_Government.Domain.Entities.Liscenses;
using Mapster;

namespace E_Government.Domain.Helpers
{
    public static class MapsterConfig
    {
        public static void ConfigureMappings()
        {
            // Bill to BillRequestDto
            TypeAdapterConfig<Bill, BillRequestDto>.NewConfig()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.BillNumber, src => src.BillNumber)
                .Map(dest => dest.IssueDate, src => src.IssueDate)
                .Map(dest => dest.DueDate, src => src.DueDate)
                .Map(dest => dest.Amount, src => src.Amount)
                .Map(dest => dest.Status, src => src.Status.ToDtoStatusString())
                .Map(dest => dest.PreviousReading, src => src.PreviousReading)
                .Map(dest => dest.CurrentReading, src => src.CurrentReading)
                .Map(dest => dest.UnitPrice, src => src.UnitPrice)
                .Map(dest => dest.Type, src => src.Type)
                .Map(dest => dest.PdfUrl, src => src.PdfUrl)
                .Map(dest => dest.PaymentId, src => src.PaymentId ?? src.StripePaymentId)
                .Map(dest => dest.UserNID, src => src.UseNID)
           //     .Map(dest => dest.CustomerName, src => src.Customer.Name)
                .Map(dest => dest.CustomerAddress, src => src.User.Address)
                .Map(dest => dest.MeterId, src => src.MeterId)
                .Map(dest => dest.MeterNumber, src => src.Meter.MeterNumber);

            // BillRequestDto to Bill
            TypeAdapterConfig<BillRequestDto, Bill>.NewConfig()
                .Map(dest => dest.BillNumber, src => src.BillNumber)
                .Map(dest => dest.IssueDate, src => src.IssueDate)
                .Map(dest => dest.DueDate, src => src.DueDate)
                .Map(dest => dest.Amount, src => src.Amount)
                .Map(dest => dest.Status, src => src.Status.ToEntityStatus())
                .Map(dest => dest.PreviousReading, src => src.PreviousReading)
                .Map(dest => dest.CurrentReading, src => src.CurrentReading)
                .Map(dest => dest.UnitPrice, src => src.UnitPrice)
                .Map(dest => dest.Type, src => src.Type)
                .Map(dest => dest.PdfUrl, src => src.PdfUrl)
                .Map(dest => dest.PaymentId, src => src.PaymentId)
                .Map(dest => dest.UseNID, src => src.UserNID)
                .Map(dest => dest.MeterId, src => src.MeterId);

           
        }
    }
}