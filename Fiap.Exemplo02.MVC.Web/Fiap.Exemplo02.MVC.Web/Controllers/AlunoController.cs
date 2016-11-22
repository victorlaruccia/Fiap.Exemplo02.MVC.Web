using Fiap.Exemplo02.MVC.Web.Models;
using Fiap.Exemplo02.MVC.Web.UnitsOfWork;
using Fiap.Exemplo02.MVC.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Fiap.Exemplo02.MVC.Web.Controllers
{
    public class AlunoController : Controller
    {
        #region FIELD

        //private PortalContext _context = new PortalContext();
        private UnitOfWork _unit = new UnitOfWork();

        #endregion

        #region GET

        [HttpGet]
        public ActionResult Cadastrar(string msg)
        {
            var viewModel = new AlunoViewModel()
            {
                ListaGrupo = ListarGrupos(),
                Mensagem = msg,
                DataNascimento = DateTime.Now
            };
            return View(viewModel);
        }

        [HttpGet]
        public ActionResult Buscar(string nomeBusca, int? idBusca)
        {
            var lista = _unit.AlunoRepository.BuscarPor(a =>
                a.Nome.Contains(nomeBusca) && (a.GrupoId == idBusca || idBusca == null));

            var viewModel = new AlunoViewModel()
            {
                ListaGrupo = ListarGrupos(),
                Alunos = lista
            };

            return View("Listar", viewModel);
        }

        [HttpGet]
        public ActionResult Listar()
        {
            var viewModel = new AlunoViewModel()
            {
                ListaGrupo = ListarGrupos(),
                Alunos = _unit.AlunoRepository.Listar()
            };
            return View(viewModel);
        }

        [HttpGet]
        public ActionResult Editar(int id)
        {
            //buscar o objeto (aluno) no banco
            var aluno = _unit.AlunoRepository.BuscarPorId(id);
            var viewModel = new AlunoViewModel()
            {
                ListaGrupo = ListarGrupos(),
                Nome = aluno.Nome,
                Bolsa = aluno.Bolsa,
                Desconto = aluno.Desconto,
                Id = aluno.Id,
                GrupoId = aluno.GrupoId,
                DataNascimento = aluno.DataNascimento,
            };
            return View(viewModel);
        }

        #endregion

        #region POST

        [HttpPost]
        public ActionResult Cadastrar(AlunoViewModel alunoViewModel)
        {
            if (ModelState.IsValid)
            {
                var aluno = new Aluno()
                {
                    Nome = alunoViewModel.Nome,
                    DataNascimento = alunoViewModel.DataNascimento,
                    Bolsa = alunoViewModel.Bolsa,
                    Desconto = alunoViewModel.Desconto,
                    GrupoId = alunoViewModel.GrupoId
                };
                _unit.AlunoRepository.Cadastrar(aluno);
                _unit.Salvar();
                return RedirectToAction("Cadastrar", new { msg = "Aluno Cadastrado" });
            }
            else
            {
                alunoViewModel.ListaGrupo = ListarGrupos();
                return View(alunoViewModel);
            }
        }

        [HttpPost]
        public ActionResult Editar(Aluno aluno)
        {
            _unit.AlunoRepository.Alterar(aluno);
            _unit.Salvar();
            TempData["msg"] = "Aluno atualizado";
            return RedirectToAction("Listar");
        }

        [HttpPost]
        public ActionResult Excluir(int alunoId)
        {
            _unit.AlunoRepository.Remover(alunoId);
            _unit.Salvar();
            TempData["msg"] = "Aluno excluido";
            return RedirectToAction("Listar");
        }

        #endregion

        #region PRIVATE

        private void CarregarComboGrupos()
        {
            //enviar para a tela os grupos para o "select"
            ViewBag.grupos = new SelectList(_unit.GrupoRepository.Listar(), "Id", "Nome");
        }

        private SelectList ListarGrupos()
        {
            //Buscar todos os grupos cadastrados
            var lista = _unit.GrupoRepository.Listar();
            return new SelectList(lista, "Id", "Nome");
        }

        #endregion

        #region DISPOSE

        protected override void Dispose(bool disposing)
        {
            _unit.Dispose();
            base.Dispose(disposing);
        }

        #endregion
    }
}