using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShopSystem.Core.Dtos.Program.Ivoice;
using ShopSystem.Core.Dtos.Program;
using ShopSystem.Core.Dtos;
using ShopSystem.Core.Models.Entites;
using ShopSystem.Core.Services.Programe;
using ShopSystem.Repository.Data.Identity;
using ShopSystem.Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Kernel.Pdf.Canvas;
using PdfDocument = iText.Kernel.Pdf.PdfDocument;
using PdfDocumentLayout = iText.Layout.Document;
using iText.Layout.Properties;
using System.Drawing.Printing;
using System.IO;
using iText.IO.Font;
using iText.Kernel.Font;
using ShopSystem.Core.Models.Identity;
using ShopSystem.Core.Dtos.Account;
using System.Reflection;



namespace ShopSystem.Repository.Reposatories.Programe
{
    public class OrderService : IOrderRepository
    {
        private readonly StoreContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderService> _logger;
        private readonly AppIdentityDbContext _appIdentityDbContext;


        public OrderService(StoreContext context, IMapper mapper, ILogger<OrderService> logger,
            AppIdentityDbContext appIdentityDbContext)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _appIdentityDbContext = appIdentityDbContext;

        }

        public async Task<OrderDTO> CreateOrderAsync(CreateOrderDTO createOrderDto, string userId)
        {
            // Begin a new transaction
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Verify the user exists in the database
                var user = await _appIdentityDbContext.Users
                                .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    throw new InvalidOperationException($"User with ID {userId} does not exist in the AppUser table.");
                }

                // Initialize a new order entity
                var order = new Order
                {
                    CustomerId = createOrderDto.CustomerId,
                    UserId = userId, 
                    OrderDate = createOrderDto.OrderDate,
                    Notes = createOrderDto.Notes,
                    OrderItems = new List<OrderItem>()
                };

                decimal totalAmount = 0;
                decimal totalDiscount = 0;

                // Loop through each order item to process
                foreach (var itemDto in createOrderDto.OrderItems)
                {
                    var product = await _context.Products.FindAsync(itemDto.ProductId);
                    if (product == null)
                    {
                        throw new InvalidOperationException($"Product with ID {itemDto.ProductId} does not exist.");
                    }

                    if (product.Quantity < itemDto.Quantity)
                    {
                        throw new InvalidOperationException($"Insufficient stock for product ID {itemDto.ProductId}. Available quantity: {product.Quantity}");
                    }

                    product.Quantity -= itemDto.Quantity;
                    product.IsStock = product.Quantity > 0;

                    decimal itemSubtotal = itemDto.Quantity * product.SellingPrice;
                    decimal itemDiscount = itemSubtotal * (itemDto.Discount / 100m);
                    decimal itemTotal = itemSubtotal - itemDiscount;

                    totalAmount += itemTotal;
                    totalDiscount += itemDiscount;

                    var orderItem = new OrderItem
                    {
                        ProductId = itemDto.ProductId,
                        Quantity = itemDto.Quantity,
                        Discount = itemDto.Discount
                        
                    };

                    order.OrderItems.Add(orderItem);
                }

                order.TotalAmount = totalAmount;
                order.TotalDiscount = totalDiscount;

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return _mapper.Map<OrderDTO>(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while creating the order for user ID {userId}: {ex.Message}");
                await transaction.RollbackAsync();
                throw;
            }
        }




        public async Task<GetOrderDTO> GetOrderByIdAsync(int id)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.Customer) 
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (order == null)
                {
                    throw new KeyNotFoundException($"Order with ID {id} not found.");
                }

                var user = await _appIdentityDbContext.Users
                    .FirstOrDefaultAsync(u => u.Id == order.UserId);

                var orderDTO = new GetOrderDTO
                {
                    Id = order.Id,
                    OrderDate = order.OrderDate,
                    Notes = order.Notes,
                    TotalAmount = order.TotalAmount,
                    TotalDiscount = order.TotalDiscount,
                    FinalAmount = order.TotalAmount - order.TotalDiscount, 
                    Customer = new GetCustomerDTO
                    {
                        Id = order.Customer.Id,
                        Name = order.Customer.Name ?? "Unknown Customer",
                        Phone = order.Customer.Phone ?? "Unknown Phone"
                    },
                
                    User = user != null ? new UserDto
                    {
                        id = user.Id,
                        FirstName = user.FirstName ?? "Unknown First Name",
                        LastName = user.LastName ?? "Unknown Last Name",
                        Email = user.Email ?? "Unknown Email",
                        PhoneNumber = user.PhoneNumber ?? "Unknown Phone",
                    } : null,

                    OrderItems = order.OrderItems.Select(oi => new GetOrderItemDTO
                    {
                        ProductId = oi.Product?.Id ?? 0,
                        ProductName = oi.Product?.Name ?? "Unknown Product",
                        SellingPrice = oi.Product?.SellingPrice ?? 0,
                        Quantity = oi.Quantity,
                        Discount = oi.Discount,
                        Subtotal = oi.Quantity * (oi.Product?.SellingPrice ?? 0) * (1 - oi.Discount / 100)
                    }).ToList()
                };

                return orderDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching details for Order ID {id}: {ex.Message}");
                throw;
            }
        }

       
        public async Task<PagedResult<GetOrderDTO>> GetAllOrdersAsync(PaginationParameters paginationParameters, QueryOptions queryOptions)
        {
            try
            {
                var query = _context.Orders
                    .Include(o => o.Customer) 
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                    .AsQueryable();

                // Apply searching
                if (!string.IsNullOrEmpty(queryOptions.Search))
                {
                    query = query.Where(o => o.Customer.Name.Contains(queryOptions.Search) ||
                                             o.OrderItems.Any(oi => oi.Product.Name.Contains(queryOptions.Search)));
                }

                // Apply filtering by MinAmount and MaxAmount if available
                if (queryOptions.MinAmount.HasValue)
                {
                    query = query.Where(o => o.TotalAmount >= queryOptions.MinAmount.Value);
                }
                if (queryOptions.MaxAmount.HasValue)
                {
                    query = query.Where(o => o.TotalAmount <= queryOptions.MaxAmount.Value);
                }

                // Apply sorting if the specified SortField exists on the Order entity
                if (!string.IsNullOrEmpty(queryOptions.SortField))
                {
                    var propertyInfo = typeof(Order).GetProperty(queryOptions.SortField);
                    if (propertyInfo != null)
                    {
                        query = queryOptions.SortDescending
                            ? query.OrderByDescending(e => EF.Property<object>(e, queryOptions.SortField))
                            : query.OrderBy(e => EF.Property<object>(e, queryOptions.SortField));
                    }
                    else
                    {
                        _logger.LogWarning($"Sort field '{queryOptions.SortField}' does not exist on Order entity.");
                    }
                }

                // Get total count for pagination
                var totalItems = await query.CountAsync();

                // Apply pagination
                var orders = await query
                    .Skip((paginationParameters.PageNumber - 1) * paginationParameters.PageSize)
                    .Take(paginationParameters.PageSize)
                    .ToListAsync();

                // Fetching User details for each order
                var userIds = orders.Select(o => o.UserId).Distinct().ToList();
                var users = await _appIdentityDbContext.Users
                    .Where(u => userIds.Contains(u.Id))
                    .ToDictionaryAsync(u => u.Id, u => u);

                // Map to OrderDTO
                var orderDtos = orders.Select(order => new GetOrderDTO
                {
                    Id = order.Id,
                    OrderDate = order.OrderDate,
                    Notes = order.Notes,
                    TotalAmount = order.TotalAmount,
                    TotalDiscount = order.TotalDiscount,
                    FinalAmount = order.TotalAmount ,
                    Customer = new GetCustomerDTO
                    {
                        Id = order.Customer.Id,
                        Name = order.Customer.Name ?? "Unknown Customer",
                        Phone = order.Customer.Phone ?? "Unknown Phone"
                    },
                    UserId = order.UserId, 
                    User = users.ContainsKey(order.UserId) ? new UserDto
                    {
                        id = users[order.UserId].Id,
                        FirstName = users[order.UserId].FirstName ?? "Unknown First Name",
                        LastName = users[order.UserId].LastName ?? "Unknown Last Name",
                        Email = users[order.UserId].Email ?? "Unknown Email",
                        PhoneNumber = users[order.UserId].PhoneNumber ?? "Unknown Phone",
                    } : null,
                    OrderItems = order.OrderItems.Select(oi => new GetOrderItemDTO
                    {
                        ProductId = oi.Product?.Id ?? 0,
                        ProductName = oi.Product?.Name ?? "Unknown Product",
                        SellingPrice = oi.Product?.SellingPrice ?? 0,
                        Quantity = oi.Quantity,
                        Discount = oi.Discount,
                        Subtotal = oi.Quantity * (oi.Product?.SellingPrice ?? 0) * (1 - oi.Discount / 100)
                    }).ToList()
                }).ToList();

                // Return the paged result
                return new PagedResult<GetOrderDTO>
                {
                    Items = orderDtos,
                    TotalCount = totalItems,
                    PageNumber = paginationParameters.PageNumber,
                    PageSize = paginationParameters.PageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all orders.");
                throw;
            }
        }


        public async Task<PagedResult<GetOrderDTO>> GetCustomerOrdersAsync(int customerId, PaginationParameters paginationParameters)
        {
            try
            {
                // Query to fetch orders for the specific customer
                var query = _context.Orders
                    .Where(o => o.CustomerId == customerId)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                    .Include(o => o.Customer)
                    .AsQueryable();

                // Get total count for pagination
                var totalItems = await query.CountAsync();

                // Apply pagination
                var orders = await query
                    .Skip((paginationParameters.PageNumber - 1) * paginationParameters.PageSize)
                    .Take(paginationParameters.PageSize)
                    .ToListAsync();

                // Map the orders to DTOs
                var orderDtos = orders.Select(order => new GetOrderDTO
                {
                    Id = order.Id,
                    OrderDate = order.OrderDate,
                    Notes = order.Notes,
                    TotalAmount = order.TotalAmount,
                    TotalDiscount = order.TotalDiscount,
                    FinalAmount = order.TotalAmount ,
                    Customer = new GetCustomerDTO
                    {
                        Id = order.Customer.Id,
                        Name = order.Customer.Name ?? "Unknown Customer",
                        Phone = order.Customer.Phone ?? "Unknown Phone"
                    },
                    OrderItems = order.OrderItems.Select(oi => new GetOrderItemDTO
                    {
                        ProductId = oi.Product?.Id ?? 0,
                        ProductName = oi.Product?.Name ?? "Unknown Product",
                        SellingPrice = oi.Product?.SellingPrice ?? 0,
                        Quantity = oi.Quantity,
                        Discount = oi.Discount,
                        Subtotal = oi.Quantity * (oi.Product?.SellingPrice ?? 0) * (1 - oi.Discount / 100)
                    }).ToList()
                }).ToList();

                // Return the paged result
                return new PagedResult<GetOrderDTO>
                {
                    Items = orderDtos,
                    TotalCount = totalItems,
                    PageNumber = paginationParameters.PageNumber,
                    PageSize = paginationParameters.PageSize
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while fetching orders for Customer ID {customerId}: {ex.Message}");
                throw;
            }
        }

        public async Task<decimal> SumOfAmountOfAllOrdersAsync()
        {
            try
            {
                // Get the total sum of all order amounts
                var totalAmount = await _context.Orders
                    .SumAsync(o => o.TotalAmount);

                return totalAmount;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error calculating the total amount of all orders: {ex.Message}");
                throw;
            }
        }


        //public async Task<OrderDTO> UpdateOrderAsync(int id, OrderDTO orderDto)
        //{
        //    var order = await _context.Orders
        //        .Include(o => o.OrderItems)
        //        .FirstOrDefaultAsync(o => o.Id == id);

        //    if (order == null)
        //    {
        //        throw new KeyNotFoundException($"Order with ID {id} not found.");
        //    }

        //    // Update order details
        //    order.OrderDate = orderDto.OrderDate;
        //    order.CustomerId = orderDto.CustomerId;
        //    // Other updates...

        //    await _context.SaveChangesAsync();
        //    return _mapper.Map<OrderDTO>(order);
        //}


        public async Task<OrderDTO> UpdateOrderAsync(int orderId, CreateOrderDTO createOrderDTO, string userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Validate user
                var user = await _appIdentityDbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                {
                    throw new InvalidOperationException($"User with ID {userId} does not exist.");
                }

                // Fetch the existing order
                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .FirstOrDefaultAsync(o => o.Id == orderId);

                if (order == null)
                {
                    throw new KeyNotFoundException($"Order with ID {orderId} not found.");
                }

                // Update order details
                order.CustomerId = createOrderDTO.CustomerId;
                order.Notes = createOrderDTO.Notes;
                order.UserId = userId;

                // Handle order items
                decimal totalAmount = 0;
                decimal totalDiscount = 0;

                // Track existing items for potential removal
                var existingItemIds = order.OrderItems.Select(oi => oi.ProductId).ToHashSet();

                foreach (var itemDto in createOrderDTO.OrderItems)
                {
                    var product = await _context.Products.FindAsync(itemDto.ProductId);
                    if (product == null)
                    {
                        throw new InvalidOperationException($"Product with ID {itemDto.ProductId} does not exist.");
                    }

                    // Update stock quantities if necessary
                    var existingOrderItem = order.OrderItems.FirstOrDefault(oi => oi.ProductId == itemDto.ProductId);
                    if (existingOrderItem != null)
                    {
                        var quantityDifference = itemDto.Quantity - existingOrderItem.Quantity;
                        if (product.Quantity < quantityDifference)
                        {
                            throw new InvalidOperationException($"Insufficient stock for product ID {itemDto.ProductId}.");
                        }
                        product.Quantity -= quantityDifference;
                        existingOrderItem.Quantity = itemDto.Quantity;
                        existingOrderItem.Discount = itemDto.Discount;
                    }
                    else
                    {
                        if (product.Quantity < itemDto.Quantity)
                        {
                            throw new InvalidOperationException($"Insufficient stock for product ID {itemDto.ProductId}.");
                        }
                        product.Quantity -= itemDto.Quantity;

                        var newOrderItem = new OrderItem
                        {
                            ProductId = itemDto.ProductId,
                            Quantity = itemDto.Quantity,
                            Discount = itemDto.Discount
                        };
                        order.OrderItems.Add(newOrderItem);
                    }

                    // Calculate item totals
                    decimal itemSubtotal = itemDto.Quantity * product.SellingPrice;
                    decimal itemDiscount = itemSubtotal * (itemDto.Discount / 100m);
                    totalAmount += itemSubtotal - itemDiscount;
                    totalDiscount += itemDiscount;

                    // Remove from existingItemIds set
                    existingItemIds.Remove(itemDto.ProductId);
                }

                // Remove items no longer in the updated list
                foreach (var productId in existingItemIds)
                {
                    var itemToRemove = order.OrderItems.First(oi => oi.ProductId == productId);
                    var product = await _context.Products.FindAsync(itemToRemove.ProductId);
                    product.Quantity += itemToRemove.Quantity;
                    order.OrderItems.Remove(itemToRemove);
                }

                // Update order totals
                order.TotalAmount = totalAmount;
                order.TotalDiscount = totalDiscount;

                // Save changes and commit transaction
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Map and return updated order
                return _mapper.Map<OrderDTO>(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating order ID {orderId} by user ID {userId}: {ex.Message}");
                await transaction.RollbackAsync();
                throw;
            }
        }


        public async Task<int> DeleteMultipleOrdersAsync(IEnumerable<int> ids)
        {
            var orders = await _context.Orders.Where(o => ids.Contains(o.Id)).ToListAsync();
            _context.Orders.RemoveRange(orders);
            return await _context.SaveChangesAsync();
        }

        public async Task<decimal> CalculateTotalOrderValueAsync(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                throw new KeyNotFoundException($"Order with ID {id} not found.");
            }

            return order.OrderItems.Sum(oi => oi.Quantity * oi.Product.SellingPrice * (1 - oi.Discount / 100));
        }

        public void GenerateInvoicePdf(InvoiceDTO invoice, string filePath)
        {
            try
            {
                // Define custom page size (e.g., receipt width of 80mm)
                var pageSize = new iText.Kernel.Geom.PageSize(226, 600); // Adjust height as needed
                using (var writer = new PdfWriter(filePath))
                using (var pdf = new PdfDocument(writer))
                {
                    pdf.SetDefaultPageSize(pageSize);
                    var document = new iText.Layout.Document(pdf);
                    document.SetMargins(10, 10, 10, 10);

                    // Add store name or header
                    document.Add(new Paragraph("Store")
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                        .SetFontSize(16)
                        .SetBold());

                    document.Add(new Paragraph("Egypt Beni-Suef Nasser \nPhone: 01022673000 \n _____________________________________")
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                        .SetFontSize(10));

                    // Add invoice details
                    document.Add(new Paragraph($"Invoice ID: {invoice.OrderId}")
                        .SetFontSize(10));
                    document.Add(new Paragraph($"Order Date: {invoice.OrderDate:yyyy-MM-dd}")
                        .SetFontSize(10));
                    document.Add(new Paragraph($"Customer: {invoice.CustomerName}")
                        .SetFontSize(10));
                    document.Add(new Paragraph($"Processed By: {invoice.UserName}")
                        .SetFontSize(10));

                    // Add line separator
                    document.Add(new iText.Layout.Element.LineSeparator(new iText.Kernel.Pdf.Canvas.Draw.SolidLine()));

                    // Add table for invoice items
                    var table = new Table(new float[] { 3, 1, 1, 1, 1 });
                    table.SetWidth(UnitValue.CreatePercentValue(100));
                    table.AddHeaderCell("Product");
                    table.AddHeaderCell("Qty");
                    table.AddHeaderCell("Price");
                    table.AddHeaderCell("Disc");
                    table.AddHeaderCell("Subtotal");

                    foreach (var item in invoice.Items)
                    {
                        table.AddCell(new Paragraph(item.ProductName).SetFontSize(10));
                        table.AddCell(new Paragraph(item.Quantity.ToString()).SetFontSize(10));
                        table.AddCell(new Paragraph(item.UnitPrice.ToString("C")).SetFontSize(10));
                        table.AddCell(new Paragraph(item.Discount.ToString("C")).SetFontSize(10));
                        table.AddCell(new Paragraph(item.SubTotal.ToString("C")).SetFontSize(10));
                    }
                    document.Add(table);

                    // Add totals section
                    document.Add(new Paragraph($"Total Amount: {invoice.TotalAmount:C}")
                        .SetFontSize(10)
                        .SetBold());
                    document.Add(new Paragraph($"Total Discount: {invoice.TotalDiscount:C}")
                        .SetFontSize(10));
                    document.Add(new Paragraph($"Final Amount: {invoice.FinalAmount:C}")
                        .SetFontSize(12)
                        .SetBold());

                    // Add thank you note
                    document.Add(new Paragraph("Thank you for your business!")
                        .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                        .SetFontSize(10)
                        .SetItalic());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error generating PDF: {ex.Message}");
                throw;
            }
        }

        public async Task<InvoiceDTO> GenerateInvoiceAsync(int orderId)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                    .FirstOrDefaultAsync(o => o.Id == orderId);


                if (order == null)
                {
                    throw new KeyNotFoundException($"Order with ID {orderId} not found.");
                }

                var user = await _appIdentityDbContext.Users
                    .FirstOrDefaultAsync(u => u.Id == order.UserId);

                //var customer = await _context.Customers
                //    .FirstOrDefaultAsync(z=>z.Name == order.Customer.Name);

                var invoice = new InvoiceDTO
                {
                    OrderId = order.Id,
                    OrderDate = order.OrderDate,
                    CustomerName = order.Customer.Name ?? "UnKnown Customer",

                    UserName = user?.UserName ?? "Unknown User", 
                    TotalAmount = order.TotalAmount,
                    TotalDiscount = order.TotalDiscount,
                    FinalAmount = order.TotalAmount - order.TotalDiscount,
                    Items = order.OrderItems.Select(oi => new InvoiceItemDTO
                    {
                        ProductName = oi.Product?.Name ?? "Unknown Product",
                        Quantity = oi.Quantity,
                        UnitPrice = oi.Product?.SellingPrice ?? 0,
                        Discount = oi.Discount,
                        SubTotal = oi.Quantity * (oi.Product?.SellingPrice ?? 0) * (1 - oi.Discount / 100)
                    }).ToList()
                };

                return invoice;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching invoice details for Order ID {orderId}: {ex.Message}");
                throw;
            }
        }

        public async Task PrintInvoiceAsync(int orderId, string printerName)
        {
            try
            {
                string targetDirectory = @"D:\System"; // Change as per your requirement
                string filePath = Path.Combine(targetDirectory, $"Invoice_{orderId}.pdf");

                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException("Invoice file does not exist.", filePath);
                }

                // Create and configure the PrintDocument object
                using (PrintDocument printDocument = new PrintDocument())
                {
                    printDocument.PrinterSettings.PrinterName = printerName;

                    printDocument.PrintPage += (sender, args) =>
                    {
                        // Load and print the file
                        using (var image = System.Drawing.Image.FromFile(filePath))
                        {
                            args.Graphics.DrawImage(image, args.PageBounds);
                        }
                    };

                    // Print the document
                    printDocument.Print();
                }

                _logger.LogInformation($"Order {orderId} invoice printed successfully on {printerName}.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while printing invoice for Order ID {orderId}: {ex.Message}");
                throw;
            }
        }

     



    }
}
