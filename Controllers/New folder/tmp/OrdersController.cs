using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppZeroAPI.Interfaces;
using AppZeroAPI.Repository;
using AppZeroAPI.Shared;
using AppZeroAPI.Entities;
using AppZeroAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ShopAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService orderService;

        public OrdersController(IOrderService orderService)
        {
            this.orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyOrders()
        {
            var orderId = await orderService.GetMyOrders();
            return Ok(orderId);
        }

        //[HttpPost]
        //public async Task<IActionResult> PlaceOrder(OrderRequest orderRequest)
        //{
        //    var orderId = await orderService.PlaceOrder(orderRequest);
        //    return Ok(orderId);
        //}

        //[HttpPost]
        //[Route("payment")]
        //public async Task<IActionResult> PayTheOrder(OrderPaymentRequest orderPaymentRequest)
        //{
        //    var orderId = await orderService.PayTheOrder(orderPaymentRequest);
        //    return Ok(orderId);
        //}

        [HttpGet]
        public async Task<IActionResult> GetOrderDetails()
        {
            var order  = await orderService.GetOrderDetails();
            return Ok();
        }

    }
}