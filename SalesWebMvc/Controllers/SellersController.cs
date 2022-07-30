using Microsoft.AspNetCore.Mvc;
using SalesWebMvc.Models;
using SalesWebMvc.Models.ViewModels;
using SalesWebMvc.Services;
using System;
using System.Collections.Generic;
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
            List<Models.Seller> list = _sellerService.FindAll();

            //passar lista como argumento para o método view, para gerar um IActionResult contendo a lista
            return View(list);
        }

        public IActionResult Create()
        {
            // chama nosso método FindAll do serviço para trazer os departamentos
            var departments = _departmentService.FindAll();
            // inicia noss ViewModel já contendo esses departamentos
            var viewModel = new SellerFormViewModel { Departments = departments };
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
    }
}
