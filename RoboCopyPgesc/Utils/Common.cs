using System.Diagnostics;
using System.Windows.Forms;

namespace RoboCopyPgesc.Utils
{
    public static class Common
    {
        public static void ExibirAviso(string mensagem) =>
            MessageBox.Show(mensagem, "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        public static void ExibirErro(string mensagem) =>
            MessageBox.Show(mensagem, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);

        public static void ExibirInformacao(string mensagem) =>
            MessageBox.Show(mensagem, "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

        public static void ExecutarCMD(string argumentos, bool esconderJanela)
        {
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.Arguments = argumentos;
            cmd.StartInfo.CreateNoWindow = esconderJanela;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();
        }
    }
}
