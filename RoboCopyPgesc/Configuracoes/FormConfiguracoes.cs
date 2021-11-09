using RoboCopyPgesc.Model;
using RoboCopyPgesc.Properties;
using RoboCopyPgesc.Utils;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace RoboCopyPgesc.Configuracoes
{
    public partial class FormConfiguracoes : Form
    {
        public FormConfiguracoes()
        {
            InitializeComponent();
            Icon = Resources.TrayIcon;
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            if (!ValidarConfiguracoes(true))
                return;

            SalvarConfiguracoes();
            Close();
        }

        private bool ValidarConfiguracoes(bool exibirAviso)
        {
            bool configuracoesValidas = true;

            if (string.IsNullOrEmpty(txtOrigens.Text))
                configuracoesValidas = false;

            if (string.IsNullOrEmpty(txtDestinos.Text))
                configuracoesValidas = false;

            if (string.IsNullOrEmpty(txtPrefixo.Text))
                configuracoesValidas = false;

            if (!configuracoesValidas && exibirAviso)
                Common.ExibirAviso("Preencha todas as configurações.");

            return configuracoesValidas;
        }

        private void SalvarConfiguracoes()
        {
            var configuracao = Configuracao.GetInstance();
            configuracao.DiretoriosOrigem.Clear();
            foreach (string origem in txtOrigens.Lines.ToList())
                if (origem.Trim() != string.Empty)
                    configuracao.DiretoriosOrigem.Add(origem.Trim());

            configuracao.DiretoriosDestino.Clear();
            foreach (string destino in txtDestinos.Lines.ToList())
                if (destino.Trim() != string.Empty)
                    configuracao.DiretoriosDestino.Add(destino.Trim());

            configuracao.Prefixo = txtPrefixo.Text.Trim();
            configuracao.IntevaloMinutos = (int)udIntervalo.Value;

            string json = JsonConvert.SerializeObject(configuracao, Formatting.Indented);
            File.WriteAllText(Constantes.ArquivoConfiguracao, json);
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormConfiguracoes_Load(object sender, EventArgs e)
        {
            CarregarConfiguracoes();
        }

        private void CarregarConfiguracoes()
        {
            var configuracao = Configuracao.GetInstance();
            foreach (string origem in configuracao.DiretoriosOrigem)
            {
                txtOrigens.AppendText(origem);
                txtOrigens.AppendText(Environment.NewLine);
            }
            foreach (string destino in configuracao.DiretoriosDestino)
            {
                txtDestinos.AppendText(destino);
                txtDestinos.AppendText(Environment.NewLine);
            }
            txtPrefixo.Text = configuracao.Prefixo;
            udIntervalo.Value = configuracao.IntevaloMinutos >= 1 ? configuracao.IntevaloMinutos : 1;
        }
    }
}
