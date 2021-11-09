using Newtonsoft.Json;
using RoboCopyPgesc.Configuracoes;
using RoboCopyPgesc.Model;
using RoboCopyPgesc.Properties;
using RoboCopyPgesc.Utils;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace RoboCopyPgesc
{
    static class Program
    {
        /// <summary>
        /// Ponto de entrada principal para o aplicativo.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new RoboCopyPgescApplicationContexto());
        }

        public class RoboCopyPgescApplicationContexto : ApplicationContext
        {
            private NotifyIcon trayIcon;
            private FormConfiguracoes _formConfiguracoes;

            private Configuracao _configuracao;
            private bool _emExecucao = false;
            private Timer _timer;

            public RoboCopyPgescApplicationContexto()
            {
                CarregarConfiguracoes();
                MontarMenu();
                ConfigurarServico();
            }

            private void ConfigurarServico()
            {
                _timer = new Timer();
                _timer.Interval = (_configuracao.IntevaloMinutos * 1000) * 60;
                _timer.Tick += OnExecutarServico;
                _timer.Enabled = true;
            }

            private void OnExecutarServico(object sender, EventArgs e)
            {
                if (!_emExecucao)
                    return;

                Servico();
            }

            private void Servico()
            {
                try
                {
                    _configuracao.DiretoriosOrigem.ForEach(origem => {
                        var destino = _configuracao.DiretoriosDestino[_configuracao.DiretoriosOrigem.IndexOf(origem)];
                        Common.ExecutarCMD($@"/c robocopy {origem} {_configuracao.Prefixo}* {destino} /R:0 /W:0 /NP /LOG:{destino}\MovArq.log", false);
                    });

                    trayIcon.ContextMenuStrip.Items[2].Text = "Última Exec: " + DateTime.Now.TimeOfDay.ToString().Substring(0, 8);
                }
                catch
                {
                    trayIcon.ContextMenuStrip.Items[2].Text = "Erro na execução: " + DateTime.Now.TimeOfDay.ToString().Substring(0, 8);
                }
            }

            private void CarregarConfiguracoes()
            {
                if (!File.Exists(Constantes.ArquivoConfiguracao))
                {
                    AbrirConfiguracoes(null, null);
                    return;
                }

                _configuracao = Configuracao.GetInstance();

                using (StreamReader streamReader = new StreamReader(Constantes.ArquivoConfiguracao))
                {
                    string json = streamReader.ReadToEnd();
                    Configuracao objeto = JsonConvert.DeserializeObject<Configuracao>(json);

                    _configuracao.Prefixo = objeto.Prefixo;
                    _configuracao.IntevaloMinutos = objeto.IntevaloMinutos;
                    objeto.DiretoriosOrigem.ForEach(d => _configuracao.DiretoriosOrigem.Add(d));
                    objeto.DiretoriosDestino.ForEach(d => _configuracao.DiretoriosDestino.Add(d));
                };
            }

            private ToolStripMenuItem CriarMenu(string texto, EventHandler evento)
            {
                return new ToolStripMenuItem(texto, null, evento);
            }

            private ToolStripSeparator CriarSeparador()
            {
                return new ToolStripSeparator();
            }

            private void MontarMenu()
            {
                trayIcon = new NotifyIcon()
                {
                    Icon = Resources.TrayIcon,
                    Text = "MovArq",
                    Visible = true,
                    ContextMenuStrip = new ContextMenuStrip()
                };

                var toolStripItemCollection = new ToolStripItem[]
                {
                    CriarMenu("MovArq 1.0", null),
                    CriarSeparador(),

                    CriarMenu("Última Exec: ", null),
                    CriarSeparador(),

                    CriarMenu("Iniciar serviço", IniciarServico),
                    CriarMenu("Parar serviço", PararServico),
                    CriarMenu("Executar agora", ExecucaoUnicaDoServico),
                    CriarSeparador(),

                    CriarMenu("Configurações...", AbrirConfiguracoes),
                    CriarSeparador(),

                    CriarMenu("Fechar", Fechar)
                };

                trayIcon.ContextMenuStrip.Items.AddRange(toolStripItemCollection);
                trayIcon.ContextMenuStrip.Items[0].Font = new Font(trayIcon.ContextMenuStrip.Font, FontStyle.Bold);
            }

            private void IniciarServico(object sender, EventArgs e)
            {
                _emExecucao = true;
            }

            private void PararServico(object sender, EventArgs e)
            {
                _emExecucao = false;
            }

            private void ExecucaoUnicaDoServico(object sender, EventArgs e)
            {
                if (_emExecucao)
                {
                    Common.ExibirAviso("Serviço em execução, pare o seviço antes de executar manualmente.");
                    return;
                }

                Servico();
            }

            private void AbrirConfiguracoes(object sender, EventArgs e)
            {
                if (_formConfiguracoes != null)
                    _formConfiguracoes.BringToFront();
                else
                {
                    _formConfiguracoes = new FormConfiguracoes();
                    _formConfiguracoes.ShowDialog();
                    _formConfiguracoes = null;
                }

                CarregarConfiguracoes();
            }

            private void Fechar(object sender, EventArgs e)
            {
                trayIcon.Visible = false;
                Application.Exit();
            }
        }
    }
}
