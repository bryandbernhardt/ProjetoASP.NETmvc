using Microsoft.AspNetCore.Mvc;
using SalesWebMvc.Models;
using SalesWebMvc.Models.ViewModels;
using SalesWebMvc.Services;
using SalesWebMvc.Services.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SalesWebMvc.Controllers
{
    public class SellersController : Controller
    {
        //declarar dependência para o SellerService
        private readonly SellerService _sellerService;
        private readonly DepartmentService _departmentService;

        //contrutor para injetar a dependência
        public SellersController(SellerService sellerService, DepartmentService departmentService)
        {
            _sellerService = sellerService;
            _departmentService = departmentService;
        }

        public IActionResult Index()
        {
            //chamar operação FindAll
            var list = _sellerService.FindAll();

            //passar lista como argumento para o método view, para gerar um IActionResult contendo a lista
            return View(list);
        }

        public IActionResult Create()
        {
            // chama nosso método FindAll do serviço para trazer os departamentos
            var departments = _departmentService.FindAll();
            // inicia noss ViewModel já contendo esses departamentos
            var viewModel = new SellerFormViewModel { Departments = departments, Seller = new Seller() };
            // tela de cadatro inicia já recebendo o objeto com os departamentos
            return View(viewModel);
        }

        //indica que a ação será uma ação de post
        [HttpPost]
        //previde que a app sofra ataques CSRF
        [ValidateAntiForgeryToken]
        public IActionResult Create(Seller seller)
        {
            //chamar método do service que add no bd
            _sellerService.Insert(seller);
            //redireciona para a página principal exibindo o dado adicionado
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete (int? id)
        {
            if(id == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id not provided (is null)" });
            }

            // usamos o id.Value para pegar o valor caso exista (pois pode ser null)
            var obj = _sellerService.FindById(id.Value);
            if (obj == null){
                return RedirectToAction(nameof(Error), new { message = "Id not found" }); ;
            }

            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            _sellerService.Remove(id);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id not provided (is null)" });
            }

            // usamos o id.Value para pegar o valor caso exista (pois pode ser null)
            var obj = _sellerService.FindById(id.Value);
            if (obj == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id not found" });
            }

            return View(obj);
        }

        public IActionResult Edit(int? id)
        {
            if(id == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id not provided (is null)" });
            }

            var obj = _sellerService.FindById(id.Value);
            if (obj == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Id not found" });
            }

            List<Department> departments = _departmentService.FindAll();
            SellerFormViewModel viewModel = new SellerFormViewModel { Seller = obj, Departments = departments };
            return View(viewModel);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Seller seller)
        {
            if (id != seller.Id)
            {
                return RedirectToAction(nameof(Error), new { message = "Id mismatch" });
            }
            try
            {
                _sellerService.Update(seller);
                return RedirectToAction(nameof(Index));
            }
            catch (ApplicationException e)
            {
                return RedirectToAction(nameof(Error), new { message = e.Message });
            }
        }

        public IActionResult Error(string message)
        {
            var viewModel = new ErrorViewModel
            {
                Message = message,
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };
            return View(viewModel);
        }

    }
}
