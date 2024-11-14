using Account.Core.Dtos;
using Account.Core.Dtos.Program;
using Account.Core.Models.Entites;
using AutoMapper;
using System.Globalization;

namespace Account.Apis.Helpers
{
    // AutoMapper configuration
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            CreateMap<Category, CategoryDTO>().ReverseMap();

            CreateMap<Customer, CustomerDTO>().ReverseMap();
            CreateMap<Customer,CreateCustomerDTO>().ReverseMap();
            CreateMap<Customer, UpdateCustomerDTO>().ReverseMap();

            CreateMap<Expense, ExpenseDTO>().ReverseMap();

            //CreateMap<Merchant, MerchantDTO>().ReverseMap();
            //CreateMap<Merchant,CreateMerchantDTO>().ReverseMap();
            CreateMap<Merchant, MerchantDTO>()
            .ForMember(dest => dest.TotalPurchaseAmount, opt => opt.MapFrom(src => src.TotalPurchaseAmount))
            .ForMember(dest => dest.TotalOutstandingBalance, opt => opt.MapFrom(src => src.TotalOutstandingBalance));

            CreateMap<Merchant, CreateMerchantDTO>().ReverseMap();



            //CreateMap<Order, OrderDTO>().ReverseMap();

            CreateMap<Payment, PaymentDTO>().ReverseMap();
            //CreateMap<Product, ProductDTO>().ReverseMap();
            CreateMap<ProductDTO, Product>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()); // Ignore Id during mapping
            CreateMap<Product, ProductDTO>(); // Map from Product to ProductDTO


            CreateMap<Purchase, PurchaseDTO>().ReverseMap();

            //CreateMap<OrderItem, OrderItemDTO>().ReverseMap();
            CreateMap<PurchaseItem, PurchaseItemDTO>().ReverseMap();




            // Map from Order entity to OrderDTO
            CreateMap<Order, OrderDTO>().ReverseMap();           
            // Map from CreateOrderDTO to Order entity
            CreateMap<CreateOrderDTO, Order>().ReverseMap();              
            // Map from OrderItem entity to OrderItemDTO
            CreateMap<OrderItem, OrderItemDTO>().ReverseMap();            
            // Map from CreateOrderItemDTO to OrderItem entity
            CreateMap<CreateOrderItemDTO, OrderItem>().ReverseMap();
         



        }

    }
}
