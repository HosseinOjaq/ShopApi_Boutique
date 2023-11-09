using Common;
using Data.Contracts;
using Entities;
using Entities.DTOs.Order;
using Entities.DTOs.Product;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class OrderRepository : Repository<Order>, IOrderRepository, IScopedDependency
    {
        private readonly IUserRepository userRepository;
        private readonly IProductRepository productRepository;
        private readonly IOrderDetailRepository orderDetailRepository;
        private readonly IHostingEnvironment env;

        public OrderRepository(ApplicationDbContext dbContext, IUserRepository userRepository, IProductRepository productRepository, IOrderDetailRepository orderDetailRepository, IHostingEnvironment env) : base(dbContext)
        {
            this.userRepository = userRepository;
            this.productRepository = productRepository;
            this.orderDetailRepository = orderDetailRepository;
            this.env = env;
        }

        public async Task<Order> AddOrderAsync(int userId, int productId, int count, CancellationToken cancellationToken)
        {
            var order = await Table.FirstOrDefaultAsync(x => x.UserId == userId && !x.IsFinaly, cancellationToken);
            var product = await productRepository.GetByIdAsync(cancellationToken, productId);
            var productPrice = product.Price - (product.Price * product.Discount / 100);
            if (order is null)
            {
                order = new Order
                {
                    UserId = userId,
                    IsFinaly = false,
                    CreateDate = DateTime.Now,
                    OrderSum = productPrice * count,
                    OrderDetails = new List<OrderDetail>
                    {
                        new OrderDetail
                        {
                            ProductId = productId,
                            Count = count,
                            Price = productPrice
                        },
                    }
                };
                await AddAsync(order, cancellationToken);
            }
            else
            {
                var detail = await orderDetailRepository.TableNoTracking
                    .FirstOrDefaultAsync(d => d.OrderId == order.Id && d.ProductId == productId, cancellationToken);
                if (detail is not null)
                {
                    detail.Count += count;
                    if (detail.Count > product.Count || product.Count is 0)
                        throw new Exception($"عدم موجودی محصول، حداکثر می توانید {product.Count} عدد به سبد خرید خود اضافه نمایید.");
                    await orderDetailRepository.UpdateAsync(detail, cancellationToken);
                }
                else
                {
                    detail = new OrderDetail
                    {
                        OrderId = order.Id,
                        Count = count,
                        ProductId = productId,
                        Price = productPrice,
                    };
                    order.OrderSum += productPrice * detail.Count;
                    await UpdateAsync(order, cancellationToken);
                    await orderDetailRepository.AddAsync(detail, cancellationToken);
                }
            }
            return order;
        }
        public async Task<Order> EditOrderBayid(int orderId, int orderDetailId, int count, CancellationToken cancellationToken)
        {
            var order = await base.TableNoTracking.SingleOrDefaultAsync(u => u.Id == orderId, cancellationToken);
            var orderDtails = await orderDetailRepository.Entities.Where(a => a.OrderId == orderId).ToListAsync();
            var orderDetail = orderDtails.SingleOrDefault(x => x.Id == orderDetailId);
            orderDetail.Count = count;
            var productPrice = orderDetail.Price * count;
            var sumPrice = orderDtails.Where(x => x.Id != orderDetailId).Sum(x => x.Price * x.Count);
            order.OrderSum = sumPrice + productPrice;
            await UpdateAsync(order, cancellationToken);
            await orderDetailRepository.UpdateAsync(orderDetail, cancellationToken);
            return order;
        }
        public async Task<bool> Delete(int orderId, CancellationToken cancellationToken)
        {
            var order = await base.TableNoTracking.SingleOrDefaultAsync(a => a.Id == orderId, cancellationToken);
            if (order is null)
            {
                throw new Exception("فاکتور یافت نشد .");
            }
            await DeleteAsync(order, cancellationToken);
            return true;
        }

        public async Task<List<UserOrderDto>> GetUserOrders(bool? IsFinaly, int userId, CancellationToken cancellationToken)
        {
            var orders = base.TableNoTracking
                .Where(x => x.UserId == userId);
            if (IsFinaly is not null)
            {
                orders = orders.Where(x => x.IsFinaly == IsFinaly);
            }
            orders = orders.Include(a => a.OrderDetails)
                .ThenInclude(p => p.Product)
                .ThenInclude(p => p.ProductFiles);
            return await orders.Select(order => new UserOrderDto
            {
                OrderId = order.Id,
                OrderSum = order.OrderSum,
                OrderCreatedDate = order.CreateDate,
                IsFinally = order.IsFinaly,
                Products = order.OrderDetails.Select(OrderDetail => new OrderProduct
                {
                    ProductId = OrderDetail.Product.Id,
                    OrderDetailId = OrderDetail.Id,
                    Price = order.OrderDetails.FirstOrDefault(x => x.ProductId == OrderDetail.Product.Id).Price,
                    Count = order.OrderDetails.FirstOrDefault(x => x.ProductId == OrderDetail.Product.Id).Count,
                    Title = OrderDetail.Product.Title,
                    ProductFiles = GetProductFileFullPath(OrderDetail.Product.ProductFiles.FirstOrDefault(x => x.MainPicture).FileName, env.WebRootPath)
                }).ToList()
            }).ToListAsync(cancellationToken);
        }
        private static string GetProductFileFullPath(string fileName, string contentRootPath)
                   => Path.Combine(contentRootPath, "Uploads", "Products", fileName);
    }
}