using ByteBank.Forum.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.AspNet.Identity.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using ByteBank.Forum.App_Start.Identity;

//atributo OwinStartup do assembly, que definirá o tipo da classe de inicialização do OWIN
//definimos para o OWIN que Startup é a classe de inicialização
[assembly: OwinStartup(typeof(ByteBank.Forum.Startup))]


/*
 *1 O que é o Owin:
Vimos o papel do Owin na comunicação entre aplicação e servidor. Além disso, aprendemos como o Owin é muito mais leve em comparação a biblioteca System.Web.

  2  Estender o Owin:
Aprendemos sobre a modularização do Owin e instalamos o pacote de extensão do Owin Microsoft.Owin.Host.SystemWeb.

3 Como configurar o Owin:

Para configurar o Owin, criamos o arquivo Startup.cs e o atributo de assembly OwinStartup usando a classe de inicialização com o método Configuration.

4 Como usar o contexto Owin:

Vimos que é possível usar o padrão de alocação de serviço com o Owin e recuperar instâncias à partir do método Get e GetUserManager.
 */
namespace ByteBank.Forum
{
    public class Startup
    {
        //Passaremos o Pipeline de construção dessa aplicação — objeto que implementa a interface IAppBuilder
        public void Configuration(IAppBuilder builder)
        {
            //Contexto do Owin
            //creaando o DBConext
            builder.CreatePerOwinContext<DbContext>(() => new IdentityDbContext<UsuarioAplicacao>("DefaultConnection"));

            builder.CreatePerOwinContext<IUserStore<UsuarioAplicacao>>((opcoes, contextOwin) =>
            {
                /*
                 * Mas, o que é um contexto do OWIN (contextOwin)?
                 * Contexto do OWIN: acontece sempre que é feita uma requisição à aplicação. Seja o Get de uma página, ou o Post de um formulário,
                 * assim se cria um contexto do OWIN.
                 */
                var dbContexto = contextOwin.Get<DbContext>(); //Microsoft.AspNet.Identity.Owin Get
                return new UserStore<UsuarioAplicacao>(dbContexto);
            });

            builder.CreatePerOwinContext<UserManager<UsuarioAplicacao>>((opcoes, contextOwin) =>
            {
                var userStore = contextOwin.Get<IUserStore<UsuarioAplicacao>>();
                var userManager = new UserManager<UsuarioAplicacao>(userStore);

                var userValidator = new UserValidator<UsuarioAplicacao>(userManager);
                //configurar para nao permidar mais emails duplicados
                userValidator.RequireUniqueEmail = true;

                //property iniatialize. Quando voce tem chaves na construcao de seus ojectos vc esta usando o property initialize.
                userManager.PasswordValidator = new SenhaValidador()
                {
                    TamanhoRequiredo = 6,
                    ObrigatorioCaracteresEspeciais = true,
                    ObrigatorioDigitos = true,
                    ObrigatorioLowerCase = true,
                    ObrigatorioUpperCase = true
                };

                userManager.UserValidator = userValidator;

                //pligando o servico de email no identity
                userManager.EmailService = new EmailServico();
                
                /*
                 * user token provider
                 * DataProtectionProvider, que é um provedor de objetos protetores de dados. O utilizaremos para construir UserTokenProvider
                 */
                var dataProtectionProvider = opcoes.DataProtectionProvider;
                var dataProcttionPrividerCreated = dataProtectionProvider.Create("ByteBank.Forum");

                userManager.UserTokenProvider = new DataProtectorTokenProvider<UsuarioAplicacao>(dataProcttionPrividerCreated);


                return userManager;

            });
        }
    }
}