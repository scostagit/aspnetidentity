using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace ByteBank.Forum.App_Start.Identity
{
    /*
    Para criarmos o validador de senhas SenhaValidador, foi necessário implementar qual interface IIdentityValidator
    Implementamos esta interface e seu método assíncrono ValidateAsync!
    */
    public class SenhaValidador : IIdentityValidator<string>
    {
        public int TamanhoRequiredo { get; set; }
        public bool ObrigatorioCaracteresEspeciais { get; set; }
        public bool ObrigatorioLowerCase { get; set; }
        public bool ObrigatorioUpperCase { get; set; }
        public bool ObrigatorioDigitos { get; set; }

        public async Task<IdentityResult> ValidateAsync(string item)
        {
            var erros = new List<string>();

            if (this.ObrigatorioCaracteresEspeciais && !this.VerificaCararecteresEspeciais(item))
                erros.Add("A Senha deve caratestres especiais!");


            if (this.ObrigatorioLowerCase && !this.VerificarLowerCase(item))
                erros.Add("A Senha deve caratestres Lowercase!");


            if (this.ObrigatorioUpperCase && !this.VerificarUperCase(item))
                erros.Add("A Senha deve caratestres Upercase!");

            if (this.ObrigatorioDigitos && !this.VerificaDigito(item))
                erros.Add("A Senha deve caratestres Digiitos!");

            if (!this.VerificaTamanhoRequerido(item))
                erros.Add(string.Format("A Senha deve {0} caracteres", this.TamanhoRequiredo));


            if (erros.Any())
                return IdentityResult.Failed(erros.ToArray());
            else
                return IdentityResult.Success;
        }


        //Esse codigo vai ser substituido pelo operator null propagator.
        //if (senha == null)
        //    return false;
        private bool VerificaTamanhoRequerido(string senha) =>
            senha?.Length >= TamanhoRequiredo; //? check if is null.

        private bool VerificaCararecteresEspeciais(string senha) =>
            Regex.IsMatch(senha, @"[~`!@#$%^&*()+=|\\{}':;.,<>/?[\]""_-]");

        private bool VerificarLowerCase(string senha) =>
              senha.Any(char.IsLower);

        private bool VerificarUperCase(string senha) =>
            senha.Any(char.IsUpper);

        private bool VerificaDigito(string senha) =>
            senha.Any(char.IsDigit);
    }
}