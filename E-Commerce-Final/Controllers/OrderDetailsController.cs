using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using E_Commerce_Final.Data;
using E_Commerce_Final.Models;

namespace E_Commerce_Final.Controllers
{
    [Route("api/orderdetails")]
    [ApiController]
    public class OrderDetailsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrderDetailsController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ GET: api/orderdetails (Get all OrderDetails)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDetail>>> GetOrderDetails()
        {
            return await _context.OrderDetails.Include(od => od.Order).ToListAsync();
        }

        // ✅ GET: api/orderdetails/{id} (Get single OrderDetail by ID)
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDetail>> GetOrderDetail(Guid id)
        {
            var orderDetail = await _context.OrderDetails.Include(od => od.Order)
                                                         .FirstOrDefaultAsync(od => od.OrderDetailId == id);

            if (orderDetail == null)
            {
                return NotFound("Order detail not found.");
            }

            return orderDetail;
        }

        // ✅ GET: api/orderdetails/order/{orderId} (Get all OrderDetails for a specific Order)
        [HttpGet("order/{orderId}")]
        public async Task<ActionResult<IEnumerable<OrderDetail>>> GetOrderDetailsByOrder(Guid orderId)
        {
            var orderExists = await _context.Orders.AnyAsync(o => o.OrderId == orderId);
            if (!orderExists)
            {
                return NotFound("Order not found.");
            }

            var orderDetails = await _context.OrderDetails.Where(od => od.OrderId == orderId)
                                                          .Include(od => od.Order)
                                                          .ToListAsync();
            return orderDetails;
        }

        // ✅ POST: api/orderdetails (Create OrderDetail)
        [HttpPost]
        public async Task<ActionResult<OrderDetail>> CreateOrderDetail(OrderDetail orderDetail)
        {
            // Validate if the Order exists
            var orderExists = await _context.Orders.AnyAsync(o => o.OrderId == orderDetail.OrderId);
            if (!orderExists)
            {
                return BadRequest("Invalid OrderId. The associated order does not exist.");
            }

            // Ensure a new OrderDetailId is assigned
            orderDetail.OrderDetailId = Guid.NewGuid();

            _context.OrderDetails.Add(orderDetail);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrderDetail), new { id = orderDetail.OrderDetailId }, orderDetail);
        }

        // ✅ PUT: api/orderdetails/{id} (Update OrderDetail)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrderDetail(Guid id, OrderDetail orderDetail)
        {
            if (id != orderDetail.OrderDetailId)
            {
                return BadRequest("OrderDetail ID mismatch.");
            }

            // Check if the OrderDetail exists
            var existingOrderDetail = await _context.OrderDetails.FindAsync(id);
            if (existingOrderDetail == null)
            {
                return NotFound("Order detail not found.");
            }

            // Update properties
            existingOrderDetail.Item = orderDetail.Item;
            existingOrderDetail.Price = orderDetail.Price;
            existingOrderDetail.Qty = orderDetail.Qty;
            existingOrderDetail.Amount = orderDetail.Amount;
            existingOrderDetail.OrderId = orderDetail.OrderId;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.OrderDetails.Any(e => e.OrderDetailId == id))
                {
                    return NotFound("Order detail not found.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // ✅ DELETE: api/orderdetails/{id} (Delete OrderDetail)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderDetail(Guid id)
        {
            var orderDetail = await _context.OrderDetails.FindAsync(id);
            if (orderDetail == null)
            {
                return NotFound("Order detail not found.");
            }

            _context.OrderDetails.Remove(orderDetail);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
