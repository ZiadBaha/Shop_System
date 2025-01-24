using AutoMapper;
using ShopSystem.Core.Dtos.Account;
using ShopSystem.Core.Dtos.Program;
using ShopSystem.Core.Models.Entites;
using ShopSystem.Core.Models.Identity;

namespace Shop_System.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            CreateMap<Category, CategoryDTO>().ReverseMap();

            CreateMap<Customer, CustomerDTO>().ReverseMap();
            CreateMap<Customer, CreateCustomerDTO>().ReverseMap();
            CreateMap<Customer, UpdateCustomerDTO>().ReverseMap();
            CreateMap<Customer, GetCustomerDTO>().ReverseMap()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));

            CreateMap<Expense, ExpenseDTO>().ReverseMap();

           
            CreateMap<Merchant, MerchantDTO>()
            .ForMember(dest => dest.TotalPurchaseAmount, opt => opt.MapFrom(src => src.TotalPurchaseAmount))
            .ForMember(dest => dest.TotalOutstandingBalance, opt => opt.MapFrom(src => src.TotalOutstandingBalance));

            CreateMap<Merchant, CreateMerchantDTO>().ReverseMap();

            CreateMap<Merchant, UpdateMerchantDTO>().ReverseMap();


            CreateMap<Payment, PaymentDTO>().ReverseMap();
            CreateMap<Payment, GetPaymentDTO>().ReverseMap();


            //CreateMap<ProductDTO, Product>()
            //.ForMember(dest => dest.Id, opt => opt.Ignore()); 
            CreateMap<Product, ProductDTO>().ReverseMap();
            CreateMap<Product, GetProductsDTO>();


            CreateMap<Purchase, PurchaseDTO>().ReverseMap();
            CreateMap<PurchaseItem, PurchaseItemDTO>().ReverseMap();
            CreateMap<CreatePurchaseDTO, Purchase>();
            CreateMap<GetPurchaseDTO, Purchase>()
                .ForMember(dest => dest.Merchant, opt => opt.MapFrom(src => src.Merchant));
            CreateMap<CreatePurchaseItemDTO, PurchaseItem>();


            CreateMap<Order, OrderDTO>()
                .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems));
            CreateMap<Order, GetOrderDTO>()
                        .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => src.Customer));

            CreateMap<OrderItem, OrderItemDTO>()
                .ForMember(dest => dest.SubTotal, opt => opt.MapFrom(src => src.Quantity * src.Product.SellingPrice));
            CreateMap<OrderItem, GetOrderItemDTO>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.SellingPrice, opt => opt.MapFrom(src => src.Product.SellingPrice));
            CreateMap<CreateOrderDTO, Order>()
                .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems));
            CreateMap<OrderItemDTO, OrderItem>();



            CreateMap<AppUser, UserDto>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName ?? "Unknown First Name"))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName ?? "Unknown Last Name"))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email ?? "Unknown Email"))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber ?? "Unknown Phone"));

        }

    }
}
