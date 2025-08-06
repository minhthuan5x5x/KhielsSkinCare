using KhielsSkincare.Controllers;
using KhielsSkincare.Models.ViewModels;
using KhielsSkincare.Models;
using KhielsSkincare.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace KhielsSkincare.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles ="Admin")]

    public class ProductController : Controller
    {
        private readonly KhielsContext _khielsContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(KhielsContext khielsContext, IWebHostEnvironment webHostEnvironment)
        {
            _khielsContext = khielsContext;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {           
            return View();
        }
        public async Task<IActionResult> ProductList()
        {
            var products = await _khielsContext.Products
                .OrderByDescending(p => p.ProductId)
                .Include(p => p.Categories)
                .Include(p => p.ProductVariants)
                .ToListAsync();

            return View(products);
        }

        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_khielsContext.Categories, "CategoryId", "CategoryId");
            return View(new ProductViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Tạo sản phẩm mới
                var product = model.Product;

                // Lưu hình ảnh
                if (model.ImageUpload != null && model.ImageUpload.Count > 0)
                {
                    for (int i = 0; i < model.ImageUpload.Count; i++)
                    {
                        var file = model.ImageUpload[i];
                        if (file != null && file.Length > 0)
                        {
                            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                            var path = Path.Combine(_webHostEnvironment.WebRootPath, "images/product", fileName);
                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            // Cập nhật đường dẫn hình ảnh vào cơ sở dữ liệu
                            switch (i)
                            {
                                case 0:
                                    product.ImageUrl = $"{fileName}"; // Hình ảnh chính
                                    break;
                                case 1:
                                    product.Image1 = $"{fileName}"; // Hình ảnh thứ nhất
                                    break;
                                case 2:
                                    product.Image2 = $"{fileName}"; // Hình ảnh thứ hai
                                    break;
                                case 3:
                                    product.Image3 = $"{fileName}"; // Hình ảnh thứ ba
                                    break;
                                case 4:
                                    product.Image4 = $"{fileName}"; // Hình ảnh thứ tư
                                    break;
                                case 5:
                                    product.Image5 = $"{fileName}"; // Hình ảnh thứ năm
                                    break;
                                default:
                                    break; // Không xử lý thêm nếu có nhiều hơn 5 hình ảnh
                            }
                        }
                    }
                }

                // Lưu sản phẩm vào cơ sở dữ liệu
                _khielsContext.Products.Add(product);
                await _khielsContext.SaveChangesAsync();

                // Lưu các biến thể
                foreach (var variant in model.ProductVariants)
                {
                    Console.WriteLine($"Variant Size: {variant.Size}, Price: {variant.Price}");
                    variant.ProductId = product.ProductId; // Gán ProductId cho mỗi biến thể
                    _khielsContext.ProductVariants.Add(variant);
                }

                await _khielsContext.SaveChangesAsync();
                Console.WriteLine($"Product ID: {product.ProductId}");
                return RedirectToAction(nameof(ProductList));
            }

            ViewBag.Categories = new SelectList(_khielsContext.Categories, "CategoryId", "CategoryId", model.Product.CategoryId);
            return View(model);
        }
        
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _khielsContext.Products
        .Include(p => p.ProductVariants) // Bao gồm các biến thể
        .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            var model = new ProductViewModel
            {
                Product = product,
                ProductVariants = product.ProductVariants.ToList(), // Gán danh sách biến thể
                ImageUpload = new List<IFormFile>() // Khởi tạo danh sách hình ảnh
            };

            ViewBag.Categories = new SelectList(_khielsContext.Categories, "CategoryId", "CategoryId", product.CategoryId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                var product = await _khielsContext.Products.FindAsync(model.Product.ProductId);
                if (product == null)
                {
                    return NotFound();
                }

                // Cập nhật thông tin sản phẩm
                product.ProductName = model.Product.ProductName;
                product.Brand = model.Product.Brand;
                product.Description = model.Product.Description;
                product.CategoryId = model.Product.CategoryId;

                // Lưu hình ảnh mới nếu có
                if (model.ImageUpload != null && model.ImageUpload.Count > 0)
                {
                    for (int i = 0; i < model.ImageUpload.Count; i++)
                    {
                        var file = model.ImageUpload[i];
                        if (file != null && file.Length > 0)
                        {
                            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                            var path = Path.Combine(_webHostEnvironment.WebRootPath, "images/product", fileName);
                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            // Cập nhật đường dẫn hình ảnh vào cơ sở dữ liệu
                            switch (i)
                            {
                                case 0:
                                    product.ImageUrl = $"{fileName}"; // Hình ảnh chính
                                    break;
                                case 1:
                                    product.Image1 = $"{fileName}"; // Hình ảnh thứ nhất
                                    break;
                                case 2:
                                    product.Image2 = $"{fileName}"; // Hình ảnh thứ hai
                                    break;
                                case 3:
                                    product.Image3 = $"{fileName}"; // Hình ảnh thứ ba
                                    break;
                                case 4:
                                    product.Image4 = $"{fileName}"; // Hình ảnh thứ tư
                                    break;
                                case 5:
                                    product.Image5 = $"{fileName}"; // Hình ảnh thứ năm
                                    break;
                                default:
                                    break; // Không xử lý thêm nếu có nhiều hơn 5 hình ảnh
                            }
                        }
                    }
                }
                // Lấy danh sách biến thể hiện tại trong cơ sở dữ liệu
                var existingVariants = _khielsContext.ProductVariants
                    .Where(v => v.ProductId == product.ProductId).ToList();

                // Cập nhật hoặc thêm mới các biến thể
                foreach (var variant in model.ProductVariants)
                {
                    var existingVariant = existingVariants
                        .FirstOrDefault(v => v.ProductVariantId == variant.ProductVariantId);

                    if (existingVariant != null)
                    {
                        // Cập nhật biến thể hiện có
                        existingVariant.Size = variant.Size;
                        existingVariant.Price = variant.Price;
                        _khielsContext.ProductVariants.Update(existingVariant);
                    }
                    else
                    {
                        // Thêm biến thể mới nếu chưa tồn tại
                        variant.ProductId = product.ProductId;
                        _khielsContext.ProductVariants.Add(variant);
                    }
                }

                // Xóa các biến thể không còn trong danh sách cập nhật
                var variantIds = model.ProductVariants.Select(v => v.ProductVariantId).ToList();
                var variantsToRemove = existingVariants
                    .Where(v => !variantIds.Contains(v.ProductVariantId)).ToList();
                _khielsContext.ProductVariants.RemoveRange(variantsToRemove);

                // Lưu thay đổi vào cơ sở dữ liệu
                await _khielsContext.SaveChangesAsync();
                return RedirectToAction(nameof(ProductList));

            }
            ViewBag.Categories = new SelectList(_khielsContext.Categories, "CategoryId", "CategoryId", model.Product.CategoryId);
            return View(model); // Trả lại model để hiển thị trong view
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _khielsContext.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            var orderDetails = _khielsContext.OrderDetails.Where(od => od.ProductId == id).ToList();
            if (orderDetails.Any())
            {
                _khielsContext.OrderDetails.RemoveRange(orderDetails);
            }

            _khielsContext.Products.Remove(product);
            await _khielsContext.SaveChangesAsync();
            return RedirectToAction(nameof(ProductList));
        }
        private bool ProductExists(int id)
        {
            return _khielsContext.Products.Any(e => e.ProductId == id);
        }
        public IActionResult Stock()
        {
            var productVariants = _khielsContext.ProductVariants.ToList(); // Lấy tất cả các biến thể sản phẩm
            return View(productVariants); // Truyền dữ liệu vào view
        }
        public IActionResult StockByProductId(int productId)
        {
            var productVariants = _khielsContext.ProductVariants
                .Where(pv => pv.ProductId == productId)
                .ToList();

            return View(productVariants); // Trả về view với danh sách ProductVariant
        }
        [Route("AddQuantity")]
        [HttpGet]
        public async Task<IActionResult> AddQuantity(int Id)
        {
            var productByQuantity = await _khielsContext.ProductQuantities.Where(pq => pq.ProductVariantId == Id).ToListAsync();
            ViewBag.ProductByQuantity = productByQuantity;
            ViewBag.Id = Id;
            return View();
        }
        [Route("StoreProductQuantity")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult StoreProductQuantity(ProductQuantity productQuantity)
        {
            var product = _khielsContext.ProductVariants.Find(productQuantity.ProductVariantId);

            if(product == null)
            {
                return NotFound();
            }

            product.Quantity += productQuantity.Quantity;

            productQuantity.ProductVariantId = productQuantity.ProductVariantId;
            productQuantity.DateCreate = DateTime.Now;

            _khielsContext.Add(productQuantity);
            _khielsContext.SaveChanges();
            TempData["success"] = "Them thanh cong";
            return RedirectToAction("AddQuantity", "Product", new { id = productQuantity.ProductVariantId });
        }
    }
}
