﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace ProductService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        //这显然是为了demo，这样放到内存中不能“集群”
        private static List<Product> products = new List<Product>();
        static ProductController()
        {
            products.Add(new Product { Id = 1, Name = "T430笔记本", Price = 8888, Description = "CPU i7标压版，1T硬盘" });
            products.Add(new Product { Id = 2, Name = "华为Mate10", Price = 3888, Description = "大猩猩屏幕，多点触摸" });
            products.Add(new Product { Id = 3, Name = "天梭手表", Price = 9888, Description = "瑞士经典款，可好了" });
        }


        [HttpGet]
        public IEnumerable<Product> Get()
        {
            return products;
            string name = this.User.Identity.Name;//读取的就是"Name"这个特殊的Claims的值
            string userId = this.User.FindFirst("UserId").Value;
            string realName = this.User.FindFirst("RealName").Value;
            string email = this.User.FindFirst("Email").Value;
            Console.WriteLine($"name={name},userId={userId},realName={realName},email={email}");

            System.Console.WriteLine("Get请求过来了" + DateTime.Now);
            return products;
        }

        [HttpGet("{id}")]
        public Product Get(int id)
        {
            var product = products.SingleOrDefault(p => p.Id == id);
            if (product == null)
            {
                Response.StatusCode = 404;
            }
            return product;
        }

        [HttpPost]
        public void Add(Product model)
        {
            if (products.Any(p => p.Id == model.Id))
            {
                Response.StatusCode = (int)HttpStatusCode.Conflict;//通过状态码而非响应体报错，是restful风格
                return;
            }
            products.Add(model);
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            var product = products.SingleOrDefault(p => p.Id == id);
            if (product != null)
            {
                products.Remove(product);
            }
        }
    }
}
