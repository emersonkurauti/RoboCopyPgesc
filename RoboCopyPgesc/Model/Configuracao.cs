using System.Collections.Generic;

namespace RoboCopyPgesc.Model
{
    public class Configuracao
    {
        private static Configuracao _instance;

        public List<string> DiretoriosOrigem { get; set; }
        public List<string> DiretoriosDestino { get; set; }
        public int IntevaloMinutos { get; set; }
        public string Prefixo { get; set; }

        public Configuracao()
        {
            DiretoriosOrigem = new List<string>();
            DiretoriosDestino = new List<string>();
        }

        public static Configuracao GetInstance()
        {
            if (_instance == null)
                _instance = new Configuracao();

            return _instance;
        }
    }
}
