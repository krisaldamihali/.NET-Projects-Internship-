using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductManagementApp.Data;
using ProductManagementApp.Models;
using ProductManagementApp.ViewModels;

namespace ProductManagementApp.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly string _imagesFolderPath;
        private const int PageSize = 3; // Adjust page size as needed
        public ProductsController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            // Set the folder path where images will be stored
            _imagesFolderPath = Path.Combine(_hostEnvironment.WebRootPath, "Product-Images");
            // Ensure the images folder exists
            if (!Directory.Exists(_imagesFolderPath))
            {
                Directory.CreateDirectory(_imagesFolderPath);
            }
        }
        // GET: Products/Index
        public async Task<IActionResult> Index(string? searchName, string? searchBrand, decimal? minPrice, decimal? maxPrice, int page = 1)
        {
            // Use AsNoTracking() for read-only queries to improve performance
            var query = _context.Products.AsNoTracking().AsQueryable();
            // Apply filters if provided.
            if (!string.IsNullOrEmpty(searchName))
            {
                query = query.Where(p => p.Name.Contains(searchName));
            }
            if (!string.IsNullOrEmpty(searchBrand))
            {
                query = query.Where(p => p.Brand.Contains(searchBrand));
            }
            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }
            // Count the total records for pagination.
            var totalRecords = await query.CountAsync();
            // Calculate total pages.
            var totalPages = (int)Math.Ceiling(totalRecords / (double)PageSize);
            // Get the current page records.
            var products = await query
            .OrderBy(p => p.Name) // Adjust sorting as needed
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();
            // Map products to the display view model.
            var productDisplayList = products.Select(p => new ProductDisplayViewModel
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Description = p.Description,
                Brand = p.Brand,
                Price = p.Price,
                Discount = p.Discount,
                MainImageFileName = p.MainImageFileName
            }).ToList();
            // Create our view model for the list.
            var vm = new ProductListViewModel
            {
                Products = productDisplayList,
                SearchName = searchName,
                SearchBrand = searchBrand,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                CurrentPage = page,
                TotalPages = totalPages
            };
            return View(vm);
        }
        // GET: Products/Details/5
        public async Task<IActionResult> Details(int id)
        {
            // Use AsNoTracking() for a read-only query including related images.
            var product = await _context.Products.AsNoTracking()
            .Include(p => p.RelatedImages)
            .FirstOrDefaultAsync(p => p.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }
            // Map to the display view model.
            var vm = new ProductDisplayViewModel
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                Brand = product.Brand,
                Price = product.Price,
                Discount = product.Discount,
                MainImageFileName = product.MainImageFileName,
                RelatedImages = product.RelatedImages
            .Select(img => new ProductRelatedImageDisplayViewModel
            {
                ImageId = img.ImageId,
                ImageFileName = img.ImageFileName
            })
            .ToList()
            };
            return View(vm);
        }
        // GET: Products/Create
        public IActionResult Create()
        {
            // Simply display the create view.
            return View();
        }
        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProductViewModel vm)
        {
            // If the submitted model is invalid, re-display the form.
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            // Create a new Product entity from the view model data.
            var product = new Product
            {
                Name = vm.Name,
                Description = vm.Description,
                Brand = vm.Brand,
                Price = vm.Price,
                Discount = vm.Discount
            };
            // Process Main Image Upload using Buffering Approach
            if (vm.MainImageFile != null && vm.MainImageFile.Length > 0)
            {
                // Buffering Approach Explanation:
                // The entire uploaded file is read into memory using a MemoryStream.
                // Once fully buffered, the file content is converted into a byte array,
                // which is then used to store the file on disk and, optionally, in the database.
                // This approach is simple but may consume significant memory for large files.
                using var memoryStream = new MemoryStream();
                await vm.MainImageFile.CopyToAsync(memoryStream); // Read file into memory
                var fileBytes = memoryStream.ToArray(); // Convert the buffered stream to a byte array
                                                        // Optionally store the file bytes in the database (not recommended in production)
                product.MainImageData = fileBytes;
                // Generate a unique file name to avoid collisions and assign it to the product.
                var uniqueFileName = GenerateUniqueFileName(vm.MainImageFile.FileName);
                product.MainImageFileName = uniqueFileName;
                // Write the buffered file bytes to the specified location on disk.
                var filePath = Path.Combine(_imagesFolderPath, uniqueFileName);
                await System.IO.File.WriteAllBytesAsync(filePath, fileBytes);
            }
            // Save the product to the database to obtain its ProductId.
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            // Process Related Images Upload using Buffering Approach
            if (vm.RelatedImageFiles != null && vm.RelatedImageFiles.Any())
            {
                foreach (var file in vm.RelatedImageFiles)
                {
                    if (file.Length > 0)
                    {
                        // Buffer the file into memory.
                        using var memoryStream = new MemoryStream();
                        await file.CopyToAsync(memoryStream); // Load file into memory
                        var fileBytes = memoryStream.ToArray(); // Convert buffered data to byte array
                                                                // Create a new ProductImage entity and assign the binary data.
                        var productImage = new ProductImage
                        {
                            ProductId = product.ProductId,
                            ImageData = fileBytes
                        };
                        // Generate a unique file name for the related image.
                        var uniqueFileName = GenerateUniqueFileName(file.FileName);
                        productImage.ImageFileName = uniqueFileName;
                        // Write the buffered image bytes to disk.
                        var relatedFilePath = Path.Combine(_imagesFolderPath, uniqueFileName);
                        await System.IO.File.WriteAllBytesAsync(relatedFilePath, fileBytes);
                        // Add the related image entity to the context.
                        _context.ProductImages.Add(productImage);
                    }
                }
                // Save all the related images to the database.
                await _context.SaveChangesAsync();
            }
            // Redirect to the Index view after successfully creating the product.
            return RedirectToAction(nameof(Index));
        }
        // GET: Products/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            // Use AsNoTracking() for the GET request to simply fetch data.
            var product = await _context.Products.AsNoTracking()
            .Include(p => p.RelatedImages)
            .FirstOrDefaultAsync(p => p.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }
            // Map the product to the update view model.
            var vm = new UpdateProductViewModel
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                Brand = product.Brand,
                Price = product.Price,
                Discount = product.Discount,
                ExistingMainImageFileName = product.MainImageFileName,
                ExistingRelatedImages = product.RelatedImages.ToList()
            };
            return View(vm);
        }
        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateProductViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            // Retrieve the product with related images from the database (tracking enabled for update).
            var product = await _context.Products
            .Include(p => p.RelatedImages)
            .FirstOrDefaultAsync(p => p.ProductId == vm.ProductId);
            if (product == null)
            {
                return NotFound();
            }
            // Update product details.
            product.Name = vm.Name;
            product.Description = vm.Description;
            product.Brand = vm.Brand;
            product.Price = vm.Price;
            product.Discount = vm.Discount;
            // Update Main Image using Streaming Approach
            if (vm.MainImageFile != null && vm.MainImageFile.Length > 0)
            {
                // Delete the old main image file from the file system, if it exists.
                if (!string.IsNullOrEmpty(product.MainImageFileName))
                {
                    var oldPath = Path.Combine(_imagesFolderPath, product.MainImageFileName);
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }
                // Generate a unique file name for the new main image.
                var uniqueFileName = GenerateUniqueFileName(vm.MainImageFile.FileName);
                product.MainImageFileName = uniqueFileName;
                var filePath = Path.Combine(_imagesFolderPath, uniqueFileName);
                // Streaming Approach Explanation:
                // Instead of reading the entire file into memory (buffering) before saving,
                // we open a file stream to the target file and copy the uploaded file's stream directly to it.
                // This minimizes memory usage, especially for large files.
                // Create a FileStream to write to the file.
                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    // - filePath: Tells the system where to save the file
                    // - FileMode.Create: Ensures a new file is created (or an existing one is overwritten)
                    // - FileAccess.Write: Opens the file for writing only. Allows data to be written to the file
                    // - FileShare.None: Ensures exclusive access to the file while open. That means no other process can access the file during this write operation
                    await vm.MainImageFile.CopyToAsync(fileStream);
                }
                // Optionally, if you need to store the file bytes in the database (not recommended for production),
                // you could read the file from disk here. For example:
                product.MainImageData = System.IO.File.ReadAllBytes(filePath);
            }
            // Process Additional Related Images using Streaming Approach
            if (vm.RelatedImageFiles != null && vm.RelatedImageFiles.Any())
            {
                foreach (var file in vm.RelatedImageFiles)
                {
                    if (file.Length > 0)
                    {
                        // Generate a unique file name for each related image.
                        var uniqueFileName = GenerateUniqueFileName(file.FileName);
                        // Create a new ProductImage entity.
                        var productImage = new ProductImage
                        {
                            ProductId = product.ProductId,
                            ImageFileName = uniqueFileName
                        };
                        var filePath = Path.Combine(_imagesFolderPath, uniqueFileName);
                        // Streaming Approach Explanation:
                        // We open a file stream for the destination file and copy the uploaded file's stream
                        // directly into it without first loading the entire file into memory.
                        using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            await file.CopyToAsync(fileStream);
                        }
                        // Optionally, you can store binary data if necessary:
                        productImage.ImageData = System.IO.File.ReadAllBytes(filePath);
                        // Add the new related image entity to the context.
                        _context.ProductImages.Add(productImage);
                    }
                }
            }
            // Update the product in the database and save changes.
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        // GET: Products/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            // Use AsNoTracking() for the deletion confirmation view.
            var product = await _context.Products.AsNoTracking()
            .FirstOrDefaultAsync(p => p.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }
            // Map product details to the delete view model.
            var vm = new DeleteProductViewModel
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Brand = product.Brand,
                Price = product.Price,
                MainImageFileName = product.MainImageFileName,
                Description = product.Description
            };
            return View(vm);
        }
        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Retrieve the product with its related images (tracking is required for deletion).
            var product = await _context.Products
            .Include(p => p.RelatedImages)
            .FirstOrDefaultAsync(p => p.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }
            // Delete the main image file from the file system if it exists.
            if (!string.IsNullOrEmpty(product.MainImageFileName))
            {
                var filePath = Path.Combine(_imagesFolderPath, product.MainImageFileName);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
            // Delete all related image files from the file system.
            foreach (var img in product.RelatedImages)
            {
                var path = Path.Combine(_imagesFolderPath, img.ImageFileName);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
            }
            // Remove the product images and the product record from the database.
            _context.ProductImages.RemoveRange(product.RelatedImages);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        // GET: Products/DeleteImage/5
        public async Task<IActionResult> DeleteImage(int imageId)
        {
            // Retrieve the related image.
            var image = await _context.ProductImages.FindAsync(imageId);
            if (image == null)
            {
                return NotFound();
            }
            // Remove the image file from the file system.
            var imagePath = Path.Combine(_imagesFolderPath, image.ImageFileName);
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
            // Remove the image record from the database.
            _context.ProductImages.Remove(image);
            await _context.SaveChangesAsync();
            // Redirect back to the edit view for the product.
            return RedirectToAction(nameof(Edit), new { id = image.ProductId });
        }
        // Helper method to generate a unique file name based on the original file name.
        private string GenerateUniqueFileName(string originalFileName)
        {
            var extension = Path.GetExtension(originalFileName);
            return Guid.NewGuid().ToString() + extension;
        }
    }
}