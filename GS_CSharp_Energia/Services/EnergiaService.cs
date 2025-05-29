using System;
using System.Collections.Generic;
using System.IO;
using EnergiaMain.Models;

namespace EnergiaMain.Services
{
    public class EnergiaService
    {
        private List<EnergiaModel> events = new List<EnergiaModel>();
        private List<string> logs = new List<string>();

        public void RegisterFailure(EnergiaModel failureEvent)
        {
            try
            {
                events.Add(failureEvent);
                Log($"Novo evento registrado: ID {failureEvent.Id} em {failureEvent.Location}");

                if (failureEvent.ImpactLevel >= ImpactLevel.Critical)
                    GenerateHighImpactAlert(failureEvent);
            }
            catch (Exception ex)
            {
                Log($"Erro ao registrar falha: {ex.Message}", true);
                throw;
            }
        }

        public void ResolveFailure(int id, DateTime resolutionTime)
        {
            var failure = GetActiveFailures().Find(f => f.Id == id);
            if (failure == null)
                throw new ArgumentException("Falha não encontrada ou já resolvida.");

            failure.MarkAsResolved(resolutionTime);
            Log($"Falha ID {failure.Id} resolvida em {resolutionTime:dd/MM/yyyy HH:mm}");
        }

        public List<EnergiaModel> GetActiveFailures() => events.FindAll(e => !e.IsResolved);

        public void GenerateImpactReport(DateTime startDate, DateTime endDate, string filePath)
        {
            try
            {
                var failures = events.FindAll(e =>
                    e.StartTime.Date >= startDate.Date &&
                    (e.EndTime?.Date ?? DateTime.Now.Date) <= endDate.Date);

                File.WriteAllText(filePath, GenerateReportContent(failures, startDate, endDate));
                Log($"Relatório gerado em: {filePath}");
            }
            catch (Exception ex)
            {
                Log($"Erro ao gerar relatório: {ex.Message}", true);
                throw;
            }
        }

        private string GenerateReportContent(List<EnergiaModel> failures, DateTime start, DateTime end)
        {
            var report = new System.Text.StringBuilder();
            report.AppendLine("RELATÓRIO DE FALHAS DE ENERGIA");
            report.AppendLine($"Período: {start:dd/MM/yyyy} até {end:dd/MM/yyyy}");
            report.AppendLine($"Total de falhas: {failures.Count}");

            foreach (var f in failures)
            {
                report.AppendLine($"\nID: {f.Id}");
                report.AppendLine($"Data: {f.StartTime:dd/MM/yyyy HH:mm}");
                report.AppendLine($"Gravidade: {f.ImpactLevel} ({(int)f.ImpactLevel})");
                report.AppendLine($"Local: {f.Location}");
                report.AppendLine($"Status: {(f.IsResolved ? "Resolvida" : "Ativa")}");
                report.AppendLine($"Sistemas afetados: {string.Join(", ", f.AffectedSystems)}");
            }

            return report.ToString();
        }

        private void GenerateHighImpactAlert(EnergiaModel failureEvent)
        {
            string alert = $"ALERTA: Falha crítica em {failureEvent.Location}";
            Log(alert);
            Console.WriteLine($"\n[ALERTA] {alert}");
        }

        private void Log(string message, bool isError = false)
        {
            string logEntry = $"[{DateTime.Now}] {(isError ? "ERRO" : "INFO")}: {message}";
            logs.Add(logEntry);
            Console.WriteLine(logEntry);
        }

        public List<string> GetLogs() => new List<string>(logs);
    }
}
