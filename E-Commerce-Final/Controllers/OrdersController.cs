using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using E_Commerce_Final.Data;
using E_Commerce_Final.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Commerce_Final.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrdersController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ GET: api/orders (Retrieve all Orders with OrderDetails)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            return await _context.Orders.Include(o => o.OrderDetails).ToListAsync();
        }

        // ✅ GET: api/orders/user/{userId} (Retrieve Orders by UserId)
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrdersByUser(string userId)
        {
            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderDetails)
                .ToListAsync();

            if (orders == null || !orders.Any())
            {
                return NotFound("No orders found for this user.");
            }

            return orders;
        }

        // ✅ GET: api/orders/{id} (Retrieve Single Order with OrderDetails)
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(Guid id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        // ✅ POST: api/orders (Create Order with OrderDetails)
        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder([FromBody] Order order)
        {
            if (order == null || string.IsNullOrEmpty(order.UserId))
            {
                return BadRequest("Invalid order data. UserId is required.");
            }

            // Ensure OrderDetails are properly added
            if (order.OrderDetails != null && order.OrderDetails.Any())
            {
                foreach (var detail in order.OrderDetails)
                {
                    detail.OrderDetailId = Guid.NewGuid(); // Ensure each OrderDetail has a unique ID
                }
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrder), new { id = order.OrderId }, order);
        }

        // ✅ PUT: api/orders/{id} (Update Order and OrderDetails)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(Guid id, [FromBody] Order order)
        {
            if (id != order.OrderId)
            {
                return BadRequest("Order ID mismatch.");
            }

            var existingOrder = await _context.Orders.Include(o => o.OrderDetails).FirstOrDefaultAsync(o => o.OrderId == id);

            if (existingOrder == null)
            {
                return NotFound();
            }

            // Update order properties
            existingOrder.InvoiceNumber = order.InvoiceNumber;
            existingOrder.IssueDate = order.IssueDate;
            existingOrder.Total = order.Total;
            existingOrder.Discount = order.Discount;
            existingOrder.GrandTotal = order.GrandTotal;
            existingOrder.UserId = order.UserId; // Update UserId

            // Update OrderDetails
            if (order.OrderDetails != null && order.OrderDetails.Any())
            {
                _context.OrderDetails.RemoveRange(existingOrder.OrderDetails);

                foreach (var detail in order.OrderDetails)
                {
                    detail.OrderDetailId = Guid.NewGuid();
                    detail.OrderId = id;
                }

                existingOrder.OrderDetails = order.OrderDetails;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Orders.Any(e => e.OrderId == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // ✅ DELETE: api/orders/{id} (Delete Order and OrderDetails)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(Guid id)
        {
            var order = await _context.Orders.Include(o => o.OrderDetails).FirstOrDefaultAsync(o => o.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
