using ByteBank.Forum.Models;
using ByteBank.Forum.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ByteBank.Forum.Controllers
{
    public class ContaController : Controller
    {
        //Com o back field criado, adicionaremos a propriedade UserManager, na qual implementaremos os métodos get e set, logo abaixo:
        private UserManager<UsuarioAplicacao> _userManager;

        public UserManager<UsuarioAplicacao> UserManager
        {
            get
            {
                if (_userManager == null)
                {
                    var contextOwin = HttpContext.GetOwinContext();
                    _userManager = contextOwin.GetUserManager<UserManager<UsuarioAplicacao>>();
                }
                return _userManager;
            }
            set
            {
                _userManager = value;
            }
        }

        // GET: Conta
        public ActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Registrar(ContaRegistrarViewModel modelo)
        {
            if (ModelState.IsValid)
            {

                var novoUsuario = new UsuarioAplicacao();
                novoUsuario.Email = modelo.Email;
                novoUsuario.UserName = modelo.UserName;
                novoUsuario.NomeCompleto = modelo.NomeCompleto;


                var usuario = this.UserManager.FindByEmail(modelo.Email);
                var usuarioJaExiste = usuario != null;

                if (usuarioJaExiste)
                    return RedirectToAction("Index", "Home");

                var identityResult = await this.UserManager.CreateAsync(novoUsuario, modelo.Senha);

                if (identityResult.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    AdicionaErros(identityResult);
                }


            }
            //alguma coisa aconteceu de errado.
            return View(modelo);
        }

        private void AdicionaErros(IdentityResult identityResult)
        {
            foreach (var erro in identityResult.Errors)
            {
                ModelState.AddModelError("", erro);
            }
        }

        [HttpPost]
        public async Task<ActionResult> RegistrarSemOwin(ContaRegistrarViewModel modelo)
        {
            if (ModelState.IsValid)
            {
                var dbContext = new IdentityDbContext<UsuarioAplicacao>("DefaultConnection");

                //para desaclopar o Identity do banco de dadaos vammos uam classe the storage
                var userStorage = new UserStore<UsuarioAplicacao>(dbContext);

                //para de fato usar o identity sem depender da tecnolgia de banco de dataos
                //temos que usar o userManager do pacote Identity
                /*
                 * Constataremos que essa interface está no namespace do Identity, somente no Core dele. Sendo assim, vamos utilizá-la em ContaController. 
                 * Sabendo que UserStore não é suficiente para separar o banco de dados e o Identity, precisamos de um objeto que gerencie todos os elementos.
                 * Portanto, abaixo de userStore, criaremos a variável userManager (termo em inglês para "gerenciamento"). Esse também é o nome da classe que 
                 * instanciaremos. O Visual Studio não oferecerá a opção de autocompletar porque UserManager está em um namespace diferente.

                    estamos desacoplando a dependência da tecnologia do banco de dados. Aqui sim, começamos a trabalhar com o Identity, de fato.
                 */
                var userManager = new UserManager<UsuarioAplicacao>(userStorage);

                var novoUsuario = new UsuarioAplicacao();
                novoUsuario.Email = modelo.Email;
                novoUsuario.UserName = modelo.UserName;
                novoUsuario.NomeCompleto = modelo.NomeCompleto;

                //nao vamos mais preciar usar a tecnoligoa estpeciaficas do dbcontext.
                //dbContext.Users.Add(novoUsuario);
                //dbContext.SaveChanges();
                //podemos incluir o usuario
                await userManager.CreateAsync(novoUsuario, modelo.Senha);
                return RedirectToAction("Index", "Home");
            }
            //alguma coisa aconteceu de errado.
            return View(modelo);
        }
    }
}

/*
 * Aprendemos sobre o relacionamento entre as principais classes do AspNet Identity para o gerenciamento de usuários.

Utilizando o EntityFramework e as implementações oferecidas pelos pacotes Microsoft.AspNet.Identity.Core e Microsoft.AspNet.Identity.EntityFramework,
qual é o caminho completo entre a classe UserManager<TUser> e o banco de dados?
 * 
 * 
 * 
 * UserManager<TUser>, UserStore<TUser> e IdentityDbContext<TUser>.

 
 Para o acesso aos dados, a UserManager<TUser> se relaciona apenas com uma classe que implementa a
interface IUserStore<TUser>, com o pacote Microsoft.AspNet.Identity.EntityFramework podemos fazer uso da 
UserStore<TUser> com o DbContext já implementado com IdentityDbContext<TUser>.
 */
