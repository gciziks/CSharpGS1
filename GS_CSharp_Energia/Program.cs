using System;
using System.Collections.Generic;
using EnergiaMain.Models;
using EnergiaMain.Services;

namespace EnergiaMain
{
    class Program
    {
        private static AuthenticationService authService = new AuthenticationService();
        private static EnergiaService powerService = new EnergiaService();
        private static List<string> availableSystems = new List<string>
        {
            "Gerador de contingência", "Alarmes de fumaça", "Alimentação elétrica", "Internet", "Servidor do Banco de Dados","Tensão elétrica"
        };

        static void Main(string[] args)
        {
            WriteColored("Sistema de Monitoramento de Falhas em Sistemas", ConsoleColor.Cyan);

            while (true)
            {
                if (!authService.IsLoggedIn)
                    ShowLoginMenu();
                else
                    ShowMainMenu();
            }
        }

        static void WriteColored(string message, ConsoleColor color)
        {
            var original = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = original;
        }

        static void ShowLoginMenu()
        {
            Console.WriteLine("\n1. Entrar\n2. Cadastrar\n3. Sair");
            Console.WriteLine("Opção:");
            switch (Console.ReadLine())
            {
                case "1": PerformLogin(); break;
                case "2": PerformRegistration(); break;
                case "3": Environment.Exit(0); break;
                default: WriteColored("\nOpção inválida!", ConsoleColor.Red); break;
            }
        }

        static void PerformLogin()
        {
            Console.Write("Nome de usuário: ");
            var username = Console.ReadLine();
            Console.Write("Senha: ");
            var password = Console.ReadLine();

            if (authService.Login(username, password))
            {
                WriteColored($"\nBem-vindo(a), {authService.CurrentUser.Name}!", ConsoleColor.Green);
            }
            else
            {
                WriteColored("\nCredenciais inválidas!", ConsoleColor.Red);
            }
        }

        static void PerformRegistration()
        {
            Console.Write("Novo nome de usuário: ");
            var username = Console.ReadLine();
            Console.Write("Senha: ");
            var password = Console.ReadLine();
            Console.Write("Nome completo: ");
            var name = Console.ReadLine();

            if (authService.RegisterUser(username, password, name))
            {
                WriteColored("\nCadastro realizado com sucesso!", ConsoleColor.Green);
            }
            else
            {
                WriteColored("\nNome de usuário já existe!", ConsoleColor.Yellow);
            }
        }

        static void ShowMainMenu()
        {
            Console.WriteLine(
                $"\nUsuário: {authService.CurrentUser.Name}\n" +
                "1. Reportar falha\n2. Falhas ativas\n" +
                "3. Resolver falha\n4. Gerar relatório\n" +
                "5. Ver logs\n6. Sair");

            try
            {
                switch (Console.ReadLine())
                {
                    case "1": ReportFailure(); break;
                    case "2": ListActiveFailures(); break;
                    case "3": ResolveFailure(); break;
                    case "4": GenerateReport(); break;
                    case "5": ViewLogs(); break;
                    case "6": authService.Logout(); break;
                    default: WriteColored("Opção inválida!", ConsoleColor.Red); break;
                }
            }
            catch (Exception ex)
            {
                WriteColored($"Erro: {ex.Message}", ConsoleColor.Red);
            }
        }

        static void ReportFailure()
        {
            Console.WriteLine("\nSistemas Disponíveis:");
            for (int i = 0; i < availableSystems.Count; i++)
                Console.WriteLine($"{i + 1}. {availableSystems[i]}");

            Console.Write("\nSistemas afetados (números separados por vírgula): ");
            var systems = Console.ReadLine().Split(',');
            var affected = new List<string>();

            foreach (var s in systems)
                if (int.TryParse(s.Trim(), out int idx) && idx > 0 && idx <= availableSystems.Count)
                    affected.Add(availableSystems[idx - 1]);

            if (affected.Count == 0)
            {
                WriteColored("Nenhum sistema válido selecionado", ConsoleColor.Red);
                return;
            }

            Console.Write("Data/Hora (dd/MM/yyyy HH:mm): ");
            DateTime failureTime;
            while (!DateTime.TryParse(Console.ReadLine(), out failureTime))
            {
                WriteColored("Formato inválido! Use dd/MM/yyyy HH:mm", ConsoleColor.Yellow);
                Console.Write("Data/Hora: ");
            }

            Console.WriteLine("\nNível de Severidade:");
            Console.WriteLine("1. Baixo");
            Console.WriteLine("2. Médio");
            Console.WriteLine("3. Alto");
            Console.WriteLine("4. Crítico");
            Console.Write("Escolha: ");

            ImpactLevel severity;
            while (!Enum.TryParse(Console.ReadLine(), out severity) ||
                   !Enum.IsDefined(typeof(ImpactLevel), severity))
            {
                WriteColored("Opção inválida! Digite 1, 2, 3 ou 4", ConsoleColor.Yellow);
                Console.Write("Escolha: ");
            }

            Console.Write("Localização: ");
            var location = Console.ReadLine();
            Console.Write("Descrição: ");
            var description = Console.ReadLine();

            var failure = new EnergiaModel(
                failureTime,
                location,
                description,
                severity,
                authService.CurrentUser.Name,
                affected);

            powerService.RegisterFailure(failure);
            WriteColored("Falha registrada com sucesso!", ConsoleColor.Green);
        }

        static void ListActiveFailures()
        {
            var failures = powerService.GetActiveFailures();
            if (failures.Count == 0)
            {
                WriteColored("\nNenhuma falha ativa encontrada.", ConsoleColor.Yellow);
                return;
            }

            WriteColored("\nFalhas Ativas:", ConsoleColor.Cyan);
            failures.ForEach(f =>
                Console.WriteLine($"ID: {f.Id} - {f.Location}"));
        }

        static void ResolveFailure()
        {
            Console.Write("ID da falha: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                WriteColored("Formato de ID inválido.", ConsoleColor.Red);
                return;
            }

            Console.Write("Data/Hora da resolução (dd/MM/yyyy HH:mm): ");
            if (!DateTime.TryParse(Console.ReadLine(), out var dt))
            {
                WriteColored("Formato de data/hora inválido.", ConsoleColor.Red);
                return;
            }

            try
            {
                powerService.ResolveFailure(id, dt);
                WriteColored("Falha marcada como resolvida!", ConsoleColor.Green);
            }
            catch (Exception ex)
            {
                WriteColored($"Erro: {ex.Message}", ConsoleColor.Red);
            }
        }

        static void GenerateReport()
        {
            Console.Write("Data de início (dd/MM/yyyy): ");
            if (!DateTime.TryParse(Console.ReadLine(), out var start))
            {
                WriteColored("Formato da data de início inválido.", ConsoleColor.Red);
                return;
            }

            Console.Write("Data de término (dd/MM/yyyy): ");
            if (!DateTime.TryParse(Console.ReadLine(), out var end))
            {
                WriteColored("Formato da data de término inválido.", ConsoleColor.Red);
                return;
            }
            Console.WriteLine("Os arquivos serão criados no caminho padrão GS_CSharp_Energia\\GS_CSharp_Energia\\bin\\Debug\\net8.0");
            Console.Write("Caminho do arquivo de saída (adicione .txt no final): ");
            var path = Console.ReadLine();

            powerService.GenerateImpactReport(start, end, path);
            WriteColored("Relatório gerado com sucesso!", ConsoleColor.Green);
        }

        static void ViewLogs()
        {
            var logs = powerService.GetLogs();
            if (logs.Count == 0)
            {
                WriteColored("Nenhum log disponível.", ConsoleColor.Yellow);
                return;
            }

            WriteColored("Registros de Eventos:", ConsoleColor.Cyan);

            foreach (var log in logs)
            {
                Console.WriteLine(log);
            }
        }

    }
}
