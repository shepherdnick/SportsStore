using Microsoft.AspNetCore.Mvc;
using Moq;
using SportsStore.Controllers;
using SportsStore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SportsStore.Tests
{
    public class OrderControllerTests
    {
        [Fact]
        public void Cannot_Checkout_Empty_Cart()
        {
            //Arrange - create a mock repository
            Mock<IOrderRepository> mock = new Mock<IOrderRepository>();
            //Arrange - create an empty cart
            Cart cart = new Cart();
            //Arrange - create the order
            Order order = new Order();
            //Arrange - create an instance of the controller
            OrderController target = new OrderController(mock.Object, cart);

            //Act
            ViewResult result = target.Checkout(order) as ViewResult;

            //Assert - check that the order hasn't been stored
            mock.Verify(m => m.SaveOrder(It.IsAny<Order>()), Times.Never);
            //Assert - check that the method is returning the default view
            Assert.True(string.IsNullOrEmpty(result.ViewName));
            //Assert - check that I am passing an invalid model to the view
            Assert.False(result.ViewData.ModelState.IsValid);
        }

        [Fact]
        public void Cannot_Checkout_Invalid_ShippingDetails()
        {
            //Arrange - create a mock repository
            Mock<IOrderRepository> mock = new Mock<IOrderRepository>();
            //Arrange - create an empty cart
            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);
            //Arrange - create an instance of the controller
            OrderController target = new OrderController(mock.Object, cart);
            target.ModelState.AddModelError("error", "error");

            //Act
            ViewResult result = target.Checkout(new Order()) as ViewResult;

            //Assert - check that the order hasn't been stored
            mock.Verify(m => m.SaveOrder(It.IsAny<Order>()), Times.Never);
            //Assert - check that the method is returning the default view
            Assert.True(string.IsNullOrEmpty(result.ViewName));
            //Assert - check that I am passing an invalid model to the view
            Assert.False(result.ViewData.ModelState.IsValid);
        }

        [Fact]
        public void Can_Checkout_And_Submit_Order()
        {
            //Arrange - create a mock repository
            Mock<IOrderRepository> mock = new Mock<IOrderRepository>();
            //Arrange - create an empty cart
            Cart cart = new Cart();
            cart.AddItem(new Product(), 1);
            //Arrange - create an instance of the controller
            OrderController target = new OrderController(mock.Object, cart);

            //Act
            RedirectToActionResult result = target.Checkout(new Order()) as RedirectToActionResult;

            //Assert - check that the order hasn't been stored
            mock.Verify(m => m.SaveOrder(It.IsAny<Order>()), Times.Once);
            //Assert - check that the method is returning the default view
            Assert.Equal("Completed", result.ActionName);
        }
    }
}
